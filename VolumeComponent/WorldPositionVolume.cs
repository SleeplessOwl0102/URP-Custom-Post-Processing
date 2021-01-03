using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SleeplessOwl.URPPostProcessing
{
    [Serializable, VolumeComponentMenu("SleeplessOwl PostProcessing/World Position")]
    public sealed class WorldPositionVolume : PostProcessVolumeComponent
    {
        public BoolParameter ColorAddMode = new BoolParameter(true);
        public FloatParameter GridSize = new FloatParameter(0f);
        public FloatParameter GridLineWidth = new FloatParameter(2);

        Material material;

        public override bool visibleInSceneView => true;
        public override InjectionPoint InjectionPoint => InjectionPoint.BeforePostProcess;
        public override bool IsActive()
        {
            return GridSize.value > 0 && GridLineWidth.value > 0;
        }

        static class IDs
        {
            internal static readonly int _UnitCubeGridCount = Shader.PropertyToID("_UnitCubeGridCount");
            internal static readonly int _GridLineWidth = Shader.PropertyToID("_GridLineWidth");
            internal static readonly int _InverseView = Shader.PropertyToID("_InverseView");
            internal static readonly string ADD_MODE = "ADD_MODE";
        }

        public override void Initialize()
        {
            material = CoreUtils.CreateEngineMaterial("SleeplessOwl/Post-Processing/World Position");
        }

        public override void Render(CommandBuffer cb, Camera camera, RenderTargetIdentifier source, RenderTargetIdentifier destination)
        {
            material.SetMatrix(IDs._InverseView, camera.cameraToWorldMatrix);

            material.SetKeyWord(IDs.ADD_MODE, ColorAddMode.value);
            material.SetFloat(IDs._UnitCubeGridCount, 1 / GridSize.value);
            material.SetFloat(IDs._GridLineWidth, GridLineWidth.value);

            cb.SetPostProcessSourceTexture(source);
            cb.DrawFullScreenTriangle(material, destination);
        }
    }
}