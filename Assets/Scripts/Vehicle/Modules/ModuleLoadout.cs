using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

// This was mostly written by chat gpt
// Sorry I have no idea how most of it works

[Serializable]
public class ModuleCapacityDictionary : SerializableDictionary<ModuleType, int>, ISerializationCallbackReceiver
{
    public new void OnBeforeSerialize() {
        keys.Clear();
        values.Clear();

        foreach (var kvp in this) {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public new void OnAfterDeserialize()
    {
        Clear();

        if (keys.Count != values.Count) {
            throw new Exception("The number of keys and values does not match.");
        }

        for (int i = 0; i < keys.Count; i++) {
            this[keys[i]] = values[i];
        }

        foreach (ModuleType type in Enum.GetValues(typeof(ModuleType))) {
            if (!ContainsKey(type)) {
                Add(type, 0);
            }
        }
    }
}

[CustomPropertyDrawer(typeof(ModuleCapacityDictionary))]
public class ModuleCapacityDictionaryDrawer : PropertyDrawer {
    private const int keyWidth = 100;
    private const int valueWidth = 50;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Begin property
        EditorGUI.BeginProperty(position, label, property);

        // Get dictionary
        var dictionary = fieldInfo.GetValue(property.serializedObject.targetObject) as IDictionary<ModuleType, int>;
        if (dictionary == null) {
            EditorGUI.LabelField(position, "Dictionary is null!");
        } else {
            // Draw foldout arrow
            var foldoutRect = new Rect(position.x, position.y, 16, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none);

            // Draw label
            var labelRect = new Rect(position.x + 16, position.y, position.width - 16, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            if (property.isExpanded) {
                // Indent
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 1;

                // Draw dictionary
                var keys = Enum.GetValues(typeof(ModuleType));
                foreach (var key in keys) {
                    var moduleType = (ModuleType)key;
                    var keyRect = new Rect(position.x + 16, position.y + EditorGUIUtility.singleLineHeight, keyWidth, EditorGUIUtility.singleLineHeight);
                    var valueRect = new Rect(position.x + keyWidth + 16, position.y + EditorGUIUtility.singleLineHeight, valueWidth, EditorGUIUtility.singleLineHeight);

                    EditorGUI.LabelField(keyRect, moduleType.ToString());
                    dictionary[moduleType] = EditorGUI.IntField(valueRect, dictionary[moduleType]);

                    position.y += EditorGUIUtility.singleLineHeight;
                }

                // Reset indent
                EditorGUI.indentLevel = indent;
            }
        }

        // End property
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var dictionary = fieldInfo.GetValue(property.serializedObject.targetObject) as IDictionary<ModuleType, int>;
        if (dictionary == null) {
            return EditorGUIUtility.singleLineHeight;
        } else if (property.isExpanded) {
            return EditorGUIUtility.singleLineHeight * (Enum.GetValues(typeof(ModuleType)).Length + 1);
        } else {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}