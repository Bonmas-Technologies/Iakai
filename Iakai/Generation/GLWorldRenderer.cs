using Iakai.Core;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Iakai.Generation
{
    internal class GLWorldRenderer
    {
        public int DistanceOfView 
        { 
            set { _presenter.DistanceOfView = value; }
            get { return _presenter.DistanceOfView; }
        }

        private int _vao;

        private ShaderProgram _worldRenderer;

        private WorldPresenter _presenter;

        private List<RenderData> data;

        public GLWorldRenderer()
        {
            data = new List<RenderData>();
            _presenter = new WorldPresenter();
            _worldRenderer = new ShaderProgram(@".\Resources\Shaders\vertex.vert", @".\Resources\Shaders\fragment.frag");
            Console.WriteLine(_worldRenderer.Log);
        }

        public void Init()
        {
            _vao = GL.GenVertexArray();

            GL.BindVertexArray(_vao);
            {

                var bufferData = WorldGenerator.GenerateIndices();
                var ebo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * bufferData.Length, bufferData, BufferUsageHint.StaticDraw);
            }
            GL.BindVertexArray(0);
        }

        public void Update(Vector2 cameraPosition)
        {
            MeshData[] meshes = LoadMeshes(_presenter.GetReachableChunks(cameraPosition));
           
            data.Clear();

            GL.BindVertexArray(_vao);
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].PackMesh();
                var mesh = meshes[i].GetPacked();

                var vbo = GL.GenBuffer();

                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, mesh.Length * 4, mesh, BufferUsageHint.StreamDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);

                data.Add(new RenderData() 
                {
                    data = meshes[i],
                    vbo = vbo
                });
            }
            GL.BindVertexArray(0);
        }

        private MeshData[] LoadMeshes(WorldChunkContainer[] chunks)
        {
            MeshData[] meshes = new MeshData[_presenter.CountOfDrawableMeshes];

            for (int i = 0; i < chunks.Length; i++)
                meshes[i] = chunks[i].GetMesh();

            return meshes;
        }

        public void Render(GLCamera camera)
        {
            GL.BindVertexArray(_vao);

            _worldRenderer.Use();

            for (int i = 0; i < data.Count; i++)
            {
                _worldRenderer.SetUniform("dirt", 0);
                _worldRenderer.SetUniform("wall", 1);
                _worldRenderer.SetUniform("model", data[i].data.offset);
                _worldRenderer.SetUniform("view", camera.GetViewMatrix());
                _worldRenderer.SetUniform("projection", camera.GetProjectionMatrix());
                _worldRenderer.SetUniform("lightDir", new Vector3(-1, -1f, -1f));
                _worldRenderer.SetUniform("texScale", 0.2f);
  
                GL.BindBuffer(BufferTarget.ArrayBuffer, data[i].vbo);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.DrawElements(PrimitiveType.Triangles, (WorldGenerator.ChunkSize - 1) * (WorldGenerator.ChunkSize - 1) * 6, DrawElementsType.UnsignedInt,0);
            }

            GL.BindVertexArray(0);
        }

        private struct RenderData
        {
            public int vbo;
            public MeshData data;
        }
    }
}
