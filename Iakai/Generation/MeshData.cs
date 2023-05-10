using OpenTK.Mathematics;
using System.Xml.Linq;

namespace Iakai.Generation
{
    internal class MeshData
    {
        public Matrix4 offset;

        public Vector3[] vertices;
        public Vector3[] normals;

        private float[] _elements;

        public MeshData(int size)
        {
            _elements = Array.Empty<float>();
            vertices = new Vector3[size * size];
            normals  = new Vector3[size * size];
        }

        public void PackMesh()
        {
            _elements = new float[vertices.Length * 3 + normals.Length * 3];

            for (int i = 0, j = 0; i < vertices.Length; i++)
            {
                _elements[j++] = vertices[i].X;
                _elements[j++] = vertices[i].Y;
                _elements[j++] = vertices[i].Z;

                _elements[j++] = normals[i].X;
                _elements[j++] = normals[i].Y;
                _elements[j++] = normals[i].Z;
            }
        }

        public float[] GetPacked()
        {
            return _elements;
        }
    }
}
