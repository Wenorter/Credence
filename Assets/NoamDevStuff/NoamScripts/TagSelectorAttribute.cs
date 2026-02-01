#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class TagSelectorAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(TagSelectorAttribute))]
public class TagSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use TagSelector with string.");
            return;
        }

        EditorGUI.BeginProperty(position, label, property);
        property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
        EditorGUI.EndProperty();
    }
}
#endif

