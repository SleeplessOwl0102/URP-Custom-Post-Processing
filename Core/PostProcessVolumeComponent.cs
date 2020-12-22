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

        public abstract bool IsActive();
        public virtual InjectionPoint InjectionPoint { get; } = InjectionPoint.BeforePostProcess;
        public virtual bool visibleInSceneView { get; } = false;

        internal bool isInitialized = false;

        public abstract void Setup();

        public abstract void Render(CommandBuffer cb, Camera camera, RenderTargetIdentifier source, RenderTargetIdentifier dest);

        internal void SetupIfNeed()
        {
            if (isInitialized == true)
                return;

            Setup();
            isInitialized = true;
        }

        internal virtual void CleanUp()
        {
            isInitialized = false;
        }
    }
}