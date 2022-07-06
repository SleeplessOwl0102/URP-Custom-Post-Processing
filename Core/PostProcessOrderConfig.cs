using System;
using System.Collections.Generic;
using UnityEngine;

namespace SleeplessOwl.URPPostProcessing
{
    [CreateAssetMenu(menuName = "Sleepless Owl/PostProcess Order Config")]
    public class PostProcessOrderConfig : ScriptableObject
    {
        public List<string> afterSkybox = new List<string>();
        public List<string> beforePostProcess = new List<string>();
        public List<string> afterPostProcess = new List<string>();

        public List<string> GetVolumeList(InjectionPoint point)
        {
            switch (point)
            {
                case InjectionPoint.AfterOpaqueAndSky:
                    return afterSkybox;
                case InjectionPoint.BeforePostProcess:
                    return beforePostProcess;
                case InjectionPoint.AfterPostProcess:
                    return afterPostProcess;
            }
            return null;
        }

#if UNITY_EDITOR
        public Action OnDataChange;

        private void OnValidate()
        {
            OnDataChange?.Invoke();
        }
#endif
    }


}

#if UNITY_EDITOR
namespace SleeplessOwl.URPPostProcessing.CEditor
{
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine.Rendering;

    using static UnityEditor.GenericMenu;
    using static UnityEditorInternal.ReorderableList;

    [CustomEditor(typeof(PostProcessOrderConfig))]
    [CanEditMultipleObjects]
    public class PostProcessOrderConfigEditor : Editor
    {
        private ReorderableList beforePost;
        private ReorderableList afterPost;
        private ReorderableList afterSkybox;

        private PostProcessOrderConfig instance;

        public override void OnInspectorGUI()
        {
            //base.DrawDefaultInspector();
            serializedObject.Update();
            {
                afterSkybox.DoLayoutList();
                EditorGUILayout.Space();
                beforePost.DoLayoutList();
                EditorGUILayout.Space();
                afterPost.DoLayoutList();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            instance = (PostProcessOrderConfig)target;

            var afterSkyboxProp = serializedObject.FindProperty("afterSkybox");
            afterSkybox = Util_ReoederbleList.CreateAutoLayout(afterSkyboxProp, "After Skybox");
            afterSkybox.drawElementCallback = DrawElement(afterSkybox);
            afterSkybox.onAddDropdownCallback = DrawDropdownMenu(InjectionPoint.AfterOpaqueAndSky);

            var beforePostProp = serializedObject.FindProperty("beforePostProcess");
            beforePost = Util_ReoederbleList.CreateAutoLayout(beforePostProp, "Before Post-Process");
            beforePost.drawElementCallback = DrawElement(beforePost);
            beforePost.onAddDropdownCallback = DrawDropdownMenu(InjectionPoint.BeforePostProcess);

            var afterPostProp = serializedObject.FindProperty("afterPostProcess");
            afterPost = Util_ReoederbleList.CreateAutoLayout(afterPostProp, "After Post-Process");
            afterPost.drawElementCallback = DrawElement(afterPost);
            afterPost.onAddDropdownCallback = DrawDropdownMenu(InjectionPoint.AfterPostProcess);
        }

        private AddDropdownCallbackDelegate DrawDropdownMenu(InjectionPoint injectionPoint)
        {
            return (buttonRect, list) =>
            {
                var menu = new GenericMenu();

                foreach (var item in VolumeManager.instance.baseComponentTypeArray)
                {
                    var comp = VolumeManager.instance.stack.GetComponent(item) as PostProcessVolumeComponent;

                    if (comp == null)
                        continue;

                    if (comp.InjectionPoint != injectionPoint)
                        continue;

                    menu.AddItem(new GUIContent(comp.GetType().ToString()), false, tryAddVolumeComp(comp, injectionPoint));
                }

                menu.ShowAsContext();
            };

            MenuFunction tryAddVolumeComp(object userData, InjectionPoint customInjectionPoint)
            {
                return () =>
                {
                    var data = userData as PostProcessVolumeComponent;
                    var typeName = data.GetType().ToString();
                    var list = instance.GetVolumeList(customInjectionPoint);
                    if (list.Contains(typeName) == false)
                    {
                        list.Add(typeName);
                        instance.OnDataChange?.Invoke();
                        EditorUtility.SetDirty(instance);
                    }
                };
            }
        }

        private ElementCallbackDelegate DrawElement(ReorderableList list)
        {
            return (rect, index, isActive, isFocused) =>
            {
                var prop = list.serializedProperty;
                var item = prop.GetArrayElementAtIndex(index);
                rect.height = EditorGUI.GetPropertyHeight(item);
                EditorGUI.LabelField(rect, item.stringValue);
            };
        }
    }

}
#endif