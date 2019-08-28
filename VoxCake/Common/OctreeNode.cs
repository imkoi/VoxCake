using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxCake.Common
{
    public class OctreeNode
    {
        public Vector3Int position;
        public Vector3Int chunkPosition;
        public Vector3Int min;
        public Vector3Int max;
        public int size;
        public OctreeNode[] subNodes;

        public OctreeNode(Vector3Int position, int size)
        {
            this.position = position;
            this.size = size;
            chunkPosition = new Vector3Int(position.x / Chunk.size, position.y / Chunk.size, position.z / Chunk.size);
            int hs = (int)(size * 0.5f);
            min = new Vector3Int(position.x - hs, position.y - hs, position.z - hs);
            max = new Vector3Int(position.x + hs, position.y + hs, position.z + hs);
        }

        public bool IsLeaf()
        {
            return subNodes == null;
        }
        public void Subdivide(int depth = 0)
        {
            subNodes = new OctreeNode[8];
            for (int i = 0; i < 8; ++i)
            {
                Vector3Int newPos = position;
                int smf = (int)(size * 0.25f);
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
                subNodes[i] = new OctreeNode(newPos, (int)(size * 0.5f));
                if (depth > 0)
                    subNodes[i].Subdivide(depth - 1);
            }
        }
    }
}