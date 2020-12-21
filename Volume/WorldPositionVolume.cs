using SleeplessOwl.URPPostProcessing;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SleeplessOwl.Volume
{
    [Serializable, VolumeComponentMenu("Custom Post-processing/ WorldScan")]
    public sealed class WorldPositionVolume : CustomVolumeComponent
    {
        public BoolParameter ColorAddMode = new BoolParameter(true);
        public FloatParameter GridSize = new FloatParameter(0f);
        public FloatParameter GridLineWidth = new FloatParameter(2);

        readonly int _UnitCubeGridCount = Shader.PropertyToID("_UnitCubeGridCount");
        readonly int _GridLineWidth = Shader.PropertyToID("_GridLineWidth");

        Material material;

        public override bool visibleInSceneView => true;
        public override InjectionPoint InjectionPoint => InjectionPoint.BeforePostProcess;
        public override bool IsActive()
        {
            return GridSize.value > 0 && GridLineWidth.value > 0;
        }

        public int AddModeID = Shader.PropertyToID("ADD_MODE");

        public override void Setup()
        {
            material = CoreUtils.CreateEngineMaterial("URP Custom PostEffect/World Position");
        }

        public override void Render(CommandBuffer cb, Camera camera, RenderTargetIdentifier source, RenderTargetIdentifier destination)
        {
            material.SetMatrix("_InverseView", camera.cameraToWorldMatrix);

            material.SetKeyWord("ADD_MODE", ColorAddMode.value);
            material.SetFloat(_UnitCubeGridCount, 1 / GridSize.value);
            material.SetFloat(_GridLineWidth, GridLineWidth.value);
            
            cb.SetSourceTexture(source);
            cb.DrawFullScreenTriangle(material, destination);
        }
    }
}