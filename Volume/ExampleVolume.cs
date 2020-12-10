using System;
using UnityEngine;
using UnityEngine.Rendering;


namespace SleeplessOwl.URPPostProcessing
{
    [Serializable, VolumeComponentMenu("Owl/HalfToneOnlyPoint")]
    public class ExampleVolume : CustomVolumeComponent
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
            cb.SetSourceTexture(source);
            cb.DrawFullScreenTriangle(material, dest);
        }

        public override void Setup()
        {
            material = CoreUtils.CreateEngineMaterial("Owl Post-Processing/Example");
        }

    }
}