// Assets/Scripts/Vision/VisionTagMaskFeature.cs
//
// Unity 6 / URP Render Graph version.
//
// Fixes:
// - Proper occlusion (no "tint through hands") by attaching camera depth.
// - Avoids RenderGraph attachment size mismatch by forcing the mask RT to full-res when depth is attached.
//
// Notes:
// - If you enable Use Depth Occlusion, downsample is forced to 1 internally.
// - Mask texture format prefers R8 for a cheap single-channel mask.

using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public sealed class VisionTagMaskFeature : ScriptableRendererFeature
{
    [System.Serializable]
    private sealed class Settings
    {
        [Header("What to draw")]
        [Tooltip("Only objects on these layers will be drawn into the mask.")]
        public LayerMask layerMask = 0;

        [Tooltip("Material used to draw the mask (should output solid 1 in red).")]
        public Material overrideMaterial = null;

        [Header("When to run")]
        [Tooltip("Must run before the fullscreen pass that samples the mask.")]
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingOpaques;

        [Header("Occlusion")]
        [Tooltip("If true, the mask respects occluders using the camera depth texture. Prevents 'tint through hands'.")]
        public bool useDepthOcclusion = true;

        [Header("Texture")]
        [Tooltip("Global shader texture name that the fullscreen shader will sample.")]
        public string globalTextureName = "_VisionTagMaskTex";

        [Tooltip("1 = full res, 2 = half, 4 = quarter. If Use Depth Occlusion is enabled, this is forced to 1.")]
        [Range(1, 4)]
        public int downsample = 2;

        [Tooltip("If true, includes transparent objects too. Transparent occlusion depends on whether they write depth.")]
        public bool drawTransparents = true;
    }

    [SerializeField] private Settings settings = new();

    private VisionTagMaskPass _pass;

    public override void Create()
    {
        _pass = new VisionTagMaskPass(settings)
        {
            renderPassEvent = settings.passEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // If this is null, we don't enqueue the pass at all (so it won't show in Frame Debugger).
        if (settings.overrideMaterial == null)
            return;

        renderer.EnqueuePass(_pass);
    }

    private sealed class VisionTagMaskPass : ScriptableRenderPass
    {
        private static readonly ShaderTagId[] shaderTags =
        {
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
            new ShaderTagId("SRPDefaultUnlit"),
        };

        private readonly Settings _settings;
        private readonly ProfilingSampler _profiling = new("Vision Tag Mask Pass");
        private readonly int _globalId;

        public VisionTagMaskPass(Settings settings)
        {
            _settings = settings;
            _globalId = Shader.PropertyToID(_settings.globalTextureName);
        }

        private sealed class PassData
        {
            public RendererListHandle rendererList;
            public TextureHandle maskTex;
            public TextureHandle depthTex;
            public bool useDepth;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<UniversalCameraData>();
            var renderingData = frameData.Get<UniversalRenderingData>();
            var resourceData = frameData.Get<UniversalResourceData>();

            var depthTex = resourceData.activeDepthTexture;

            var targetDesc = cameraData.cameraTargetDescriptor;

            // RenderGraph requires matching attachment dimensions.
            // If we attach depth, we must be full-res to match camera depth.
            var ds = _settings.useDepthOcclusion ? 1 : Mathf.Max(1, _settings.downsample);

            var w = Mathf.Max(1, targetDesc.width / ds);
            var h = Mathf.Max(1, targetDesc.height / ds);

            var format = SystemInfo.IsFormatSupported(GraphicsFormat.R8_UNorm, FormatUsage.Render)
                ? GraphicsFormat.R8_UNorm
                : GraphicsFormat.R8G8B8A8_UNorm;

            var texDesc = new TextureDesc(w, h)
            {
                name = _settings.globalTextureName,
                colorFormat = format,
                clearBuffer = true,
                clearColor = Color.black,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                msaaSamples = MSAASamples.None,
                useMipMap = false,
                enableRandomWrite = false
            };

            var maskTex = renderGraph.CreateTexture(texDesc);

            DrawQueue(renderGraph, cameraData, renderingData, maskTex, depthTex, RenderQueueRange.opaque, cameraData.defaultOpaqueSortFlags);

            if (_settings.drawTransparents)
                DrawQueue(renderGraph, cameraData, renderingData, maskTex, depthTex, RenderQueueRange.transparent, SortingCriteria.CommonTransparent);
        }

        private void DrawQueue(
            RenderGraph renderGraph,
            UniversalCameraData cameraData,
            UniversalRenderingData renderingData,
            TextureHandle maskTex,
            TextureHandle depthTex,
            RenderQueueRange queue,
            SortingCriteria sorting)
        {
            var listDesc = new RendererListDesc(shaderTags, renderingData.cullResults, cameraData.camera)
            {
                renderQueueRange = queue,
                sortingCriteria = sorting,
                layerMask = _settings.layerMask,
                overrideMaterial = _settings.overrideMaterial,
                overrideMaterialPassIndex = 0
            };

            var rendererList = renderGraph.CreateRendererList(listDesc);

            var passName = (queue == RenderQueueRange.transparent)
                ? "Vision Tag Mask Pass (Transparents)"
                : "Vision Tag Mask Pass";

            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData, _profiling))
            {
                passData.rendererList = rendererList;
                passData.maskTex = maskTex;
                passData.depthTex = depthTex;
                passData.useDepth = _settings.useDepthOcclusion;

                builder.SetRenderAttachment(maskTex, 0);

                if (_settings.useDepthOcclusion)
                    builder.SetRenderAttachmentDepth(depthTex);

                builder.UseRendererList(rendererList);

                // Expose globally for the fullscreen shader.
                builder.SetGlobalTextureAfterPass(maskTex, _globalId);

                // Keep it from being optimized away while you iterate.
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
                {
                    ctx.cmd.DrawRendererList(data.rendererList);
                });
            }
        }
    }
}
