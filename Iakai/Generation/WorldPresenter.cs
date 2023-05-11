using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iakai.Generation
{
    public class WorldPresenter
    {
        public int DistanceOfView { get; set; } = 5;

        public int CountOfDrawableMeshes => (int)MathHelper.Pow(DistanceOfView * 2 - 1, 2);

        private Dictionary<Vector2i, WorldChunkContainer> _chunks;

        public WorldPresenter()
        {
            _chunks = new Dictionary<Vector2i, WorldChunkContainer>();
        }

        public WorldChunkContainer[] GetReachableChunks(Vector2 cameraPosition)
        {
            //TODO: Multithreading
            var chunks = new List<WorldChunkContainer>(CountOfDrawableMeshes);

            List<Vector2i> positions = GetGlobalChunkPositions(cameraPosition / (WorldGenerator.ChunkSize * WorldGenerator.MeshScale), CountOfDrawableMeshes);

            for (int i = 0; i < positions.Count;)
            {
                if (_chunks.ContainsKey(positions[i]))
                {
                    chunks.Add(_chunks[positions[i]]);
                }
                else
                {
                    var layer = WorldGenerator.GenerateLayerData(positions[i]);

                    var chunk = new WorldChunkContainer(positions[i], layer);
                    
                    chunk.GenerateMesh();

                    _chunks.Add(positions[i], chunk);

                    chunks.Add(chunk);
                }

                positions.Remove(positions[i]);
            }

            return chunks.ToArray();
        }

        private List<Vector2i> GetGlobalChunkPositions(Vector2 cameraPosition, int maxChunksCount)
        {
            List<Vector2i> positions = new List<Vector2i>(maxChunksCount);

            for (int x = -(DistanceOfView - 1); x < DistanceOfView; x++)
            {
                for (int y = -(DistanceOfView - 1); y < DistanceOfView; y++)
                {
                    positions.Add(new Vector2i(x, y) + (Vector2i)cameraPosition);
                }
            }

            return positions;
        }
    }
}
