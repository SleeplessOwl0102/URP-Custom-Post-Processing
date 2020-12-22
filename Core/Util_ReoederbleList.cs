using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

namespace SleeplessOwl.URPPostProcessing
{
    public static class Util_ReoederbleList
    {
        public static ReorderableList CreateAutoLayout(SerializedProperty property, string headers)
        {
            var list = new ReorderableList(property.serializedObject, property, true, true, true, true);

            list.drawElementCallback = DrawDefaultElement(list);
            list.drawHeaderCallback = DrawHeader(headers);

            return list;
        }

        private static ElementCallbackDelegate DrawDefaultElement(ReorderableList list)
        {
            return (rect, index, isActive, isFocused) =>
            {
                var property = list.serializedProperty;
                for (var i = 0; i < property.arraySize; i++)
                {
                    rect.height = EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index));
                    EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(index), GUIContent.none);
                }
            };
        }

        private static HeaderCallbackDelegate DrawHeader(string header)
        {
            return (rect) =>
            {
                EditorGUI.LabelField(rect, header, EditorStyles.boldLabel);
            };
        }

    }
}