using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System.Xml.Linq;

namespace Iakai.Core
{
    internal class GLPositionNormalMesh : GLMesh
    {
        private int _indicesCount;

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

        public override int GetIndicesCount()
        {
            return _indicesCount;
        }

        public override void InsertData(float[] elements, uint[] indices)
        {
            _indicesCount = indices.Length;

            GL.BindVertexArray(_vao);
            {
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * elements.Length, elements, BufferUsageHint.StreamDraw);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StreamDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
            }
            GL.BindVertexArray(0);
        }
    }
}
