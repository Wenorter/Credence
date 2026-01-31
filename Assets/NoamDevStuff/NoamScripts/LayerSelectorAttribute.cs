#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class LayerSelectorAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(LayerSelectorAttribute))]
public class LayerSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Works with:
        // - int (recommended: stores layer index)
        // - string (optional: stores layer name)
        if (property.propertyType != SerializedPropertyType.Integer &&
            property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use LayerSelector with int or string.");
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            // Store layer index (0..31)
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
        else
        {
            // Store layer name (string)
            int currentLayer = LayerMask.NameToLayer(property.stringValue);
            if (currentLayer < 0) currentLayer = 0;

            int newLayer = EditorGUI.LayerField(position, label, currentLayer);
            property.stringValue = LayerMask.LayerToName(newLayer);
        }

        EditorGUI.EndProperty();
    }
}
#endif