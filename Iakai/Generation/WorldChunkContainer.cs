using OpenTK.Mathematics;

namespace Iakai.Generation
{
    public class WorldChunkContainer
    {
        public const int LodChangeDistance = 2;
        
        private Vector2i _globalPosition;

        private MeshData? _mesh;
        private WorldLayer _data;

        public WorldChunkContainer(Vector2i globalPosition, WorldLayer data)
        {
            _data = data;
            _globalPosition = globalPosition;
        }

        internal void GenerateMesh()
        {
            _mesh = WorldGenerator.GenerateMesh(_data);
            _mesh.offset = GetOffsetMatrix();
        }

        internal MeshData GetMesh()
        {
            return _mesh;
        }

        private Matrix4 GetOffsetMatrix()
        {
            return Matrix4.Identity * Matrix4.CreateTranslation(_globalPosition.X * (WorldGenerator.ChunkSize - 1), 0, _globalPosition.Y * (WorldGenerator.ChunkSize - 1));
        }
    }

    public class WorldLayer
    {
        public float[] heightMap;

        public WorldLayer(float[] heightMap)
        {
            this.heightMap = heightMap;
        }
    }
}
