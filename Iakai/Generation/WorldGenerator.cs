using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static OpenTK.Graphics.OpenGL.GL;

namespace Iakai.Generation
{
    public static class WorldGenerator
    {
        public const int ChunkSize = 64;

        internal static uint[] GenerateIndices()
        {
            var indices = new uint[(ChunkSize - 1) * (ChunkSize - 1) * 6];

            int triangleIndex = 0;
            uint vertexIndex = 0;

            for (int z = 0; z < ChunkSize; z++)
            {
                for (int x = 0; x < ChunkSize; x++)
                {
                    if (x < (ChunkSize - 1) && z < (ChunkSize - 1))
                    {
                        indices[triangleIndex++] = vertexIndex;
                        indices[triangleIndex++] = vertexIndex + 1;
                        indices[triangleIndex++] = vertexIndex + ChunkSize + 1;

                        indices[triangleIndex++] = vertexIndex;
                        indices[triangleIndex++] = vertexIndex + ChunkSize + 1;
                        indices[triangleIndex++] = vertexIndex + ChunkSize;
                    }
                    vertexIndex++;
                }
            }

            return indices;
        }

        internal static MeshData GenerateMesh(WorldLayer layer)
        {
            MeshData mesh = new MeshData(ChunkSize);

            float centerX = (ChunkSize - 1) / 2f;
            float centerZ = (ChunkSize - 1) / 2f;

            uint vertexIndex = 0;

            for (int z = 0; z < ChunkSize; z++)
            {
                for (int x = 0; x < ChunkSize; x++)
                {
                    float localX = centerX - x;
                    float localZ = centerZ - z;

                    mesh.vertices[vertexIndex] = new Vector3(localX, layer.heightMap[vertexIndex], localZ);
                    mesh.normals[vertexIndex] = CalcuateNormal(x, z, layer.heightMap);

                    vertexIndex++;
                }
            }

            return mesh;
        }

        private static Vector3 CalcuateNormal(int x, int z, float[] heightMap)
        {
            float center = heightMap[x + ChunkSize * z];

            float xTop_yCen = SafeHeightMapGet(x + 1, z, center, heightMap);
            float xBot_yCen = SafeHeightMapGet(x - 1, z, center, heightMap);
            float xCen_yTop = SafeHeightMapGet(x, z + 1, center, heightMap);
            float xCen_yBot = SafeHeightMapGet(x, z - 1, center, heightMap);

            float xTop_yTop = SafeHeightMapGet(x + 1, z + 1, center, heightMap);
            float xTop_yBot = SafeHeightMapGet(x + 1, z - 1, center, heightMap);
            float xBot_yTop = SafeHeightMapGet(x - 1, z + 1, center, heightMap);
            float xBot_yBot = SafeHeightMapGet(x - 1, z - 1, center, heightMap);
            
            Vector3 normal = Vector3.Zero;

            normal += Vector3.Normalize(new Vector3(center - xTop_yCen, 1, 0));
            normal += Vector3.Normalize(new Vector3(xBot_yCen - center, 1, 0));
            normal += Vector3.Normalize(new Vector3(0, 1, center - xCen_yTop));
            normal += Vector3.Normalize(new Vector3(0, 1, xCen_yBot - center));

            normal += Vector3.Normalize(new Vector3(center - xTop_yTop, 1, center - xTop_yTop));
            normal += Vector3.Normalize(new Vector3(center - xTop_yBot, 1, xTop_yBot - center));
            normal += Vector3.Normalize(new Vector3(xBot_yTop - center, 1, center - xBot_yTop));
            normal += Vector3.Normalize(new Vector3(xBot_yBot - center, 1, xBot_yBot - center));

            normal /= 8;

            return normal;
        }

        private static float SafeHeightMapGet(int x, int z, float origin, float[] heightMap)
        {
            if (x < 0 || z < 0)
                return origin;
            if (x >= ChunkSize || z >= ChunkSize)
                return origin;

            return heightMap[x + ChunkSize * z];
        }

        public static WorldLayer GenerateLayerData(Vector2i globalPosition)
        {
            var heightMap = new float[ChunkSize * ChunkSize];

            for (int z = 0; z < ChunkSize; z++)
            {
                for (int x = 0; x < ChunkSize; x++)
                {
                    float localX = (float)x / (ChunkSize - 1) + globalPosition.X * (ChunkSize - 1);
                    float localZ = (float)z / (ChunkSize - 1)+ globalPosition.Y * (ChunkSize - 1);

                    float positonX = localX * MathHelper.TwoPi * 4f + 0.5f;
                    float positonZ = localZ * MathHelper.TwoPi * 4f;

                    heightMap[x + ChunkSize * z] = ((float)MathHelper.Sin(positonX) + (float)MathHelper.Sin(positonZ)) / 2f * 5f;
                }
            }

            return new WorldLayer(heightMap);
        }
    }
}
