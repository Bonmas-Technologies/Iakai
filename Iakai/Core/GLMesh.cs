using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Iakai.Core
{
    internal abstract class GLMesh
    {
        protected int _vao;
        protected int _vbo;
        protected int _ebo;

        public GLMesh()
        {
            _vao = GL.GenVertexArray();

            GL.BindVertexArray(_vao);
            {
                _vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                
                _ebo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            }
            GL.BindVertexArray(0);
        }

        public abstract void InsertData(float[] elements, uint[] indices);

        public abstract int GetIndicesCount();
        
        public void Use()
        {
            GL.BindVertexArray(_vao);
        }

        public static void UnUse()
        {
            GL.BindVertexArray(0);
        }
    }
}
