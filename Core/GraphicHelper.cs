using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SleeplessOwl.URPPostProcessing
{
    public static class GraphicHelper
    {
        private static int _colorBufferId = -1;
        private static int _depthBufferId = -1;

        public static RenderTargetIdentifier UrpColorBufferId
        {
            get
            {
                if (_colorBufferId == -1)
                {
                    //_CameraColorTexture 是Lwrp的默認Color buffer
                    _colorBufferId = Shader.PropertyToID("_CameraColorTexture");
                }
                return _colorBufferId;
            }
        }

        public static RenderTargetIdentifier UrpDepthBufferId
        {
            get
            {
                if (_depthBufferId == -1)
                {
                    //_CameraColorTexture 默認Lwrp ColorBuffer含有Depth可直接作為DepthBuffer
                    _depthBufferId = Shader.PropertyToID("_CameraDepthTexture");
                }
                return _depthBufferId;
            }
        }

        private static Mesh fullscreenMesh = null;

        //use this method replace Blit() reduce unnecessary cost
        public static void DrawMeshFullScreen(this CommandBuffer cb, Camera camera, Material material, int shaderPass = 0, int subMeshIndex = 0)
        {
            cb.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cb.SetViewport(camera.pixelRect);
            cb.DrawMesh(FullscreenMesh, Matrix4x4.identity, material, subMeshIndex, shaderPass);
        }

        private static Mesh FullscreenMesh
        {
            get
            {
                if (fullscreenMesh != null)
                    return fullscreenMesh;

                float top = 1.0f;
                float bottom = 0.0f;

                fullscreenMesh = new Mesh { name = "Fullscreen Quad" };
                fullscreenMesh.SetVertices(new List<Vector3>
                {
                    new Vector3(-1.0f, -1.0f, 0.0f),
                    new Vector3(-1.0f,  1.0f, 0.0f),
                    new Vector3(1.0f, -1.0f, 0.0f),
                    new Vector3(1.0f, 1.0f, 0.0f)
                });

                fullscreenMesh.SetUVs(0, new List<Vector2>
                {
                    new Vector2(0.0f, bottom),
                    new Vector2(0.0f, top),
                    new Vector2(1.0f, bottom),
                    new Vector2(1.0f, top)
                });

                fullscreenMesh.SetIndices(new[] { 0, 1, 2, 2, 1, 3 }, MeshTopology.Triangles, 0, false);
                fullscreenMesh.UploadMeshData(true);
                return fullscreenMesh;
            }
        }
    }
}