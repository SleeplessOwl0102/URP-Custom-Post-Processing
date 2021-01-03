using UnityEngine;
using UnityEngine.Rendering;

namespace SleeplessOwl.URPPostProcessing
{
    public enum InjectionPoint
    {
        AfterOpaqueAndSky = 400,
        BeforePostProcess = 550,
        AfterPostProcess = 600,
    }

    public abstract class PostProcessVolumeComponent : VolumeComponent
    {
        protected PostProcessVolumeComponent()
        {
            string className = GetType().ToString();
            int dotIndex = className.LastIndexOf(".") + 1;
            displayName = className.Substring(dotIndex);
        }

        public virtual InjectionPoint InjectionPoint { get; } = InjectionPoint.BeforePostProcess;

        public virtual bool visibleInSceneView { get; } = true;

        public abstract bool IsActive();

        public abstract void Initialize();

        public abstract void Render(CommandBuffer cb, Camera camera, RenderTargetIdentifier source, RenderTargetIdentifier dest);

        internal bool isInitialized = false;

        internal void SetupIfNeed()
        {
            if (isInitialized == true)
                return;

            Initialize();
            isInitialized = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            isInitialized = false;
            CleanUp();
        }

        protected virtual void CleanUp()
        {

        }
    }
}