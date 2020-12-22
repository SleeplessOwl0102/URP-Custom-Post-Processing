using System;
using UnityEngine;
using UnityEngine.Rendering;


namespace SleeplessOwl.URPPostProcessing
{
    [Serializable, VolumeComponentMenu("SleeplessOwl PostProcessing/Example Heart")]
    public class ExampleVolume : PostProcessVolumeComponent
    {
        public BoolParameter Enable = new BoolParameter(false);
        private Material material;

        public override InjectionPoint InjectionPoint { get; } = InjectionPoint.AfterPostProcess;

        public override bool IsActive()
        {
            return Enable.value;
        }

        public override void Render(CommandBuffer cb, Camera camera, RenderTargetIdentifier source, RenderTargetIdentifier dest)
        {
            cb.SetPostProcessSourceTexture(source);
            cb.DrawFullScreenTriangle(material, dest);
        }

        public override void Setup()
        {
            material = CoreUtils.CreateEngineMaterial("SleeplessOwl/Post-Process/Example");
        }

    }
}