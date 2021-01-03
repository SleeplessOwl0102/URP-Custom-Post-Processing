using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SleeplessOwl.URPPostProcessing
{
    [Serializable, VolumeComponentMenu("SleeplessOwl PostProcessing/Cloud Shadow")]
    public sealed class CloudShadowVolume : PostProcessVolumeComponent
    {
        public FloatParameter ShadowStrength = new FloatParameter(0f);
        public FloatRangeParameter SmoothStepRange = new FloatRangeParameter(new Vector2(.45f, .55f), 0, 1);

        //public TextureParameter NoiseTexture1 = new TextureParameter(null);
        public Vector2Parameter NoiseScale1 = new Vector2Parameter(Vector2.one);
        public FloatParameter NoiseTime1= new FloatParameter(1);
        
        //public TextureParameter NoiseTexture2 = new TextureParameter(null);
        //public Vector2Parameter NoiseScale2 = new Vector2Parameter(Vector2.one);
        //public FloatParameter NoiseTime2 = new FloatParameter(1);

        public BoolParameter DebugMode = new BoolParameter(false);

        Material material;

        static class IDs
        {
            internal static readonly int _InverseView = Shader.PropertyToID("_InverseView");

            internal static readonly int _ShadowStrength = Shader.PropertyToID("_ShadowStrength");
            internal static readonly int _SmoothStepRange = Shader.PropertyToID("_SmoothStepRange");
            internal static readonly int _NoiseScale1 = Shader.PropertyToID("_NoiseScale1");
            internal static readonly int _NoiseTime1 = Shader.PropertyToID("_NoiseTime1");

            internal static readonly string DEBUG_MODE = "DEBUG_MODE";
        }

        public override bool visibleInSceneView => true;
        public override InjectionPoint InjectionPoint => InjectionPoint.AfterOpaqueAndSky;
        public override bool IsActive()
        {
            return ShadowStrength.value > 0;
        }

        public override void Initialize()
        {
            material = CoreUtils.CreateEngineMaterial("SleeplessOwl/Post-Processing/Cloud Shadow");
        }

        public override void Render(CommandBuffer cb, Camera camera, RenderTargetIdentifier source, RenderTargetIdentifier destination)
        {
            material.SetMatrix(IDs._InverseView, camera.cameraToWorldMatrix);


            material.SetFloat(IDs._ShadowStrength, ShadowStrength.value);
            material.SetVector(IDs._ShadowStrength, SmoothStepRange.value);

            //material.SetTexture("_NoiseTexture1", NoiseTexture1.value);
            material.SetVector(IDs._NoiseScale1, NoiseScale1.value);
            material.SetFloat(IDs._NoiseTime1, NoiseTime1.value);

            //material.SetTexture("_NoiseTexture2", NoiseTexture2.value);
            //material.SetVector("_NoiseScale2", NoiseScale2.value);
            //material.SetFloat("_NoiseTime2", NoiseTime2.value);

            material.SetKeyWord(IDs.DEBUG_MODE, DebugMode.value);

            cb.SetPostProcessSourceTexture(source);
            cb.DrawFullScreenTriangle(material, destination);
        }
    }
}