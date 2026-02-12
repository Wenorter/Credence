// Assets/Editor/RoomDictionaryDrawer.cs
//
// RoomDictionaryDrawer
//
// What this script does:
// - Draws RoomTransformDictionary like a neat dictionary in the Inspector:
//   RoomId -> Transform.
// - Uses a ReorderableList so adding/removing entries is painless.
// - Warns if there are duplicate RoomId keys (because duplicates are almost always a wiring mistake).
//
// Notes:
// - This file must be inside an 'Editor' folder.
// - The actual data is stored by RoomTransformDictionary (serialized list). This only affects UI.

using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(RoomDictionaryAttribute))]
public sealed class RoomDictionaryDrawer : PropertyDrawer
{
    private readonly Dictionary<string, ReorderableList> _lists = new Dictionary<string, ReorderableList>();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var entriesProp = property.FindPropertyRelative("entries");
        if (entriesProp == null || !entriesProp.isArray)
            return EditorGUIUtility.singleLineHeight;

        var list = GetOrCreateList(property, entriesProp, label);

        var extra = HasDuplicateKeys(entriesProp) ? (EditorGUIUtility.singleLineHeight * 2.2f) : 0f;
        return list.GetHeight() + extra;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var entriesProp = property.FindPropertyRelative("entries");
        if (entriesProp == null || !entriesProp.isArray)
        {
            EditorGUI.LabelField(position, label.text, "Use [RoomDictionary] with RoomTransformDictionary.");
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        if (HasDuplicateKeys(entriesProp))
        {
            var helpRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight * 2.2f);
            EditorGUI.HelpBox(
                helpRect,
                "Duplicate RoomId keys detected. Only the last entry per key will be used at runtime.",
                MessageType.Warning);

            position.y += helpRect.height + EditorGUIUtility.standardVerticalSpacing;
            position.height -= helpRect.height + EditorGUIUtility.standardVerticalSpacing;
        }

        var list = GetOrCreateList(property, entriesProp, label);
        list.DoList(position);

        EditorGUI.EndProperty();
    }

    private ReorderableList GetOrCreateList(SerializedProperty dictProp, SerializedProperty entriesProp, GUIContent label)
    {
        var cacheKey = $"{dictProp.serializedObject.targetObject.GetInstanceID()}:{dictProp.propertyPath}";
        if (_lists.TryGetValue(cacheKey, out var cached) && cached.serializedProperty == entriesProp)
            return cached;

        var list = new ReorderableList(dictProp.serializedObject, entriesProp, true, true, true, true);

        list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, label);

        list.elementHeight = EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing * 3f;

        list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = entriesProp.GetArrayElementAtIndex(index);
            var keyProp = element.FindPropertyRelative("key");
            var valueProp = element.FindPropertyRelative("value");

            rect.y += 2f;

            // Key row
            var keyRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(keyRect, keyProp, new GUIContent("Room"));

            // Value row
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var valueRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(valueRect, valueProp, new GUIContent("Transform"));
        };

        list.onAddCallback = l =>
        {
            var newIndex = entriesProp.arraySize;
            entriesProp.arraySize++;

            var element = entriesProp.GetArrayElementAtIndex(newIndex);
            var keyProp = element.FindPropertyRelative("key");

            // Default to the first unused enum value if possible (best effort).
            if (keyProp != null)
                keyProp.enumValueIndex = FindFirstUnusedEnumIndex(entriesProp, keyProp);

            dictProp.serializedObject.ApplyModifiedProperties();
        };

        _lists[cacheKey] = list;
        return list;
    }

    private static bool HasDuplicateKeys(SerializedProperty entriesProp)
    {
        var used = new HashSet<int>();

        for (var i = 0; i < entriesProp.arraySize; i++)
        {
            var element = entriesProp.GetArrayElementAtIndex(i);
            var keyProp = element.FindPropertyRelative("key");
            if (keyProp == null)
                continue;

            if (!used.Add(keyProp.enumValueIndex))
                return true;
        }

        return false;
    }

    private static int FindFirstUnusedEnumIndex(SerializedProperty entriesProp, SerializedProperty keyProp)
    {
        var used = new HashSet<int>();

        for (var i = 0; i < entriesProp.arraySize; i++)
        {
            var element = entriesProp.GetArrayElementAtIndex(i);
            var k = element.FindPropertyRelative("key");
            if (k != null)
                used.Add(k.enumValueIndex);
        }

        var enumCount = keyProp != null ? keyProp.enumNames.Length : 0;
        for (var i = 0; i < enumCount; i++)
        {
            if (!used.Contains(i))
                return i;
        }

        return 0;
    }
}
