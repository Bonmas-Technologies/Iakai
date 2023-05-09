using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System.Xml.Linq;

namespace Iakai.Core
{
    internal class GLMesh
    {
        private int _vao;

        public GLMesh()
        {
            _vao = GL.GenVertexArray();

            GL.BindVertexArray(_vao);
            {
                var _vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                
                var _ebo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            }
            GL.BindVertexArray(0);
        }

        public void InsertData(uint[] indices, Vector3[] positions, Vector3[] normals)
        {
            int length = positions.Length;

            if (length != normals.Length)
                throw new ArgumentException("Length of arrays not similar");

            int elementBufferLength = length * 3 + length * 3;

            float[] elements = new float[elementBufferLength];

            for (int i = 0, j = 0; i < length; i++)
            {
                elements[j++] = positions[i].X;
                elements[j++] = positions[i].Y;
                elements[j++] = positions[i].Z;

                elements[j++] = normals[i].X;
                elements[j++] = normals[i].Y;
                elements[j++] = normals[i].Z;
            }

            InsertData(elements, indices);
        }

        public void InsertData(float[] elements, uint[] indices)
        {
            GL.BindVertexArray(_vao);
            {
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * elements.Length, elements, BufferUsageHint.StaticDraw);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
            }
            GL.BindVertexArray(0);
        }

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
