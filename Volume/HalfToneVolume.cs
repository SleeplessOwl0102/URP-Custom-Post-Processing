using System;
using UnityEngine;
using UnityEngine.Rendering;
namespace SleeplessOwl.URPPostProcessing
{
    [Serializable, VolumeComponentMenu("SleeplessOwl PostProcessing/HalfTone")]
    public sealed class HalfToneVolume : PostProcessVolumeComponent
    {
        public BoolParameter _visibleInSceneview = new BoolParameter(true);
        public FloatParameter _Density = new FloatParameter(0);
        public FloatParameter _Radius = new FloatParameter(0.3f);
        public FloatParameter _SmoothEdge = new FloatParameter(0.2f);
        public FloatParameter _HalfToneFactor = new FloatParameter(0.5f);
        public FloatParameter _SourceFactor = new FloatParameter(0.5f);
        public FloatParameter _Lightness = new FloatParameter(1);
        public ColorParameter _PointColor = new ColorParameter(new Color(0, 0, 0, 1));
        public ColorParameter _ColorFactor = new ColorParameter(new Color(1, 1, 1, 1));

        Material material;

        public override InjectionPoint InjectionPoint => InjectionPoint.BeforePostProcess;
        public override bool visibleInSceneView => _visibleInSceneview.value;
        public override bool IsActive() => _Density.value != 0;

        public override void Setup()
        {
            material = CoreUtils.CreateEngineMaterial("URP Custom PostEffect/HalfTone");
        }

        public void UpdateParameter()
        {
            material.SetFloat("_Density", _Density.value);
            material.SetFloat("_Radius", _Radius.value);
            material.SetFloat("_SmoothEdge", _SmoothEdge.value);
            material.SetFloat("_HalfToneFactor", _HalfToneFactor.value);
            material.SetFloat("_SourceFactor", _SourceFactor.value);
            material.SetFloat("_Lightness", _Lightness.value);
            material.SetColor("_Color01", _PointColor.value);
            material.SetColor("_ColorFactor", _ColorFactor.value);
        }

        public override void Render(CommandBuffer cb, Camera camera, RenderTargetIdentifier source, RenderTargetIdentifier destination)
        {
            UpdateParameter();
            cb.SetSourceTexture(source);
            cb.DrawFullScreenTriangle(material, destination);
        }
    }

}