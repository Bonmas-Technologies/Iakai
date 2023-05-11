using OpenTK.Mathematics;
using System;
using DotnetNoise;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Iakai.Generation
{
    public static class WorldGenerator
    {
        public const float MeshScale = 2f;
        public const int ChunkSize = 128;
        public const float MapScale = 5;

        private static FastNoise noiseGen;
        
        static WorldGenerator()
        {
            noiseGen = new FastNoise(949);

            noiseGen.Gain = 0.8f;
            noiseGen.Frequency = 5.5f;
            noiseGen.Octaves = 4;
        }

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

            Vector3 normal = Vector3.Zero;

            if (SafeHeightMapGet(x + 1, z, heightMap, out float xTop_yCen))
                normal += Vector3.Normalize(GetNewVector(center - xTop_yCen, 1, 0));

            if (SafeHeightMapGet(x - 1, z, heightMap, out float xBot_yCen))
                normal += Vector3.Normalize(GetNewVector(xBot_yCen - center, 1, 0));

            if (SafeHeightMapGet(x, z + 1, heightMap, out float xCen_yTop))
                normal += Vector3.Normalize(GetNewVector(0, 1, center - xCen_yTop));

            if (SafeHeightMapGet(x, z - 1, heightMap, out float xCen_yBot))
                normal += Vector3.Normalize(GetNewVector(0, 1, xCen_yBot - center));
            

            if (SafeHeightMapGet(x + 1, z + 1, heightMap, out float xTop_yTop))
                normal += Vector3.Normalize(GetNewVector(center - xTop_yTop, 1, center - xTop_yTop));

            if (SafeHeightMapGet(x - 1, z - 1, heightMap, out float xTop_yBot))
                normal += Vector3.Normalize(GetNewVector(center - xTop_yBot, 1, xTop_yBot - center));

            if (SafeHeightMapGet(x - 1, z + 1, heightMap, out float xBot_yTop))
                normal += Vector3.Normalize(GetNewVector(xBot_yTop - center, 1, center - xBot_yTop));

            if (SafeHeightMapGet(x + 1, z - 1, heightMap, out float xBot_yBot))
                normal += Vector3.Normalize(GetNewVector(xBot_yBot - center, 1, xBot_yBot - center));

            normal.Normalize();

            return normal;
        }

        private static Vector3 GetNewVector(float x, float y, float z)
        {
            return new Vector3(x, y, z);
        }

        private static bool SafeHeightMapGet(int x, int z, float[] heightMap, out float position)
        {
            position = 0;
            
            if (x < 0 || z < 0)
                return false;

            if (x >= ChunkSize || z >= ChunkSize)
                return false;

            position = heightMap[x + ChunkSize * z];
            return true;
        }

        public static WorldLayer GenerateLayerData(Vector2i globalPosition)
        {
            var heightMap = new float[ChunkSize * ChunkSize];

            for (int z = 0; z < ChunkSize; z++)
            {
                for (int x = 0; x < ChunkSize; x++)
                {
                    float worldScale = (ChunkSize - 1);

                    float localX = x / worldScale - globalPosition.X;
                    float localZ = z / worldScale - globalPosition.Y;


                    heightMap[x + ChunkSize * z] = (noiseGen.GetValueFractal(localX / MapScale * 0.2f, localZ / MapScale * 0.2f) * 4 + noiseGen.GetCubicFractal(localX / MapScale * 2, localZ / MapScale * 2) * 6f + noiseGen.GetCellular(localX / MapScale, localZ / MapScale) * 5f);
                }
            }

            return new WorldLayer(heightMap);
        }
    }
}
