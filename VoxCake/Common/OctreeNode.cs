using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxCake.Common
{
    public class OctreeNode
    {
        public Vector3Int position;
        public Vector3Int chunkPosition;
        public Vector3 min;
        public Vector3 max;
        public int size;
        public OctreeNode[] subNodes;

        public OctreeNode(Vector3Int pos, int size)
        {
            position = pos;
            this.size = size;
            chunkPosition = new Vector3Int(pos.x / Chunk.size, pos.y / Chunk.size, pos.z / Chunk.size);

            float hs = size * 0.5f;
            min = new Vector3(position.x - hs + 0.5f, position.y - hs + 0.5f, position.z - hs + 0.5f);
            max = new Vector3(position.x + hs + 0.5f, position.y + hs + 0.5f, position.z + hs + 0.5f);
        }

        public void Subdivide(int depth = 0)
        {
            subNodes = new OctreeNode[8];
            for (int i = 0; i < 8; ++i)
            {
                Vector3Int newPos = position;
                int smf = (int) (size * 0.25f);
                if ((i & 4) == 4)
                    newPos.y += smf;
                else
                    newPos.y -= smf;

                if ((i & 2) == 2)
                    newPos.x += smf;
                else
                    newPos.x -= smf;

                if ((i & 1) == 1)
                    newPos.z += smf;
                else
                    newPos.z -= smf;
                subNodes[i] = new OctreeNode(newPos, (int) (size * 0.5f));
                if (depth > 0)
                    subNodes[i].Subdivide(depth - 1);
            }
        }
    }
}