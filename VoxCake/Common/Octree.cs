using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxCake.Common
{
    public enum OctreeIndex
    {
        BottomLeftFront = 0,
        BottomRightFront = 2,
        BottomRightBack = 3,
        BottomLeftBack = 1,
        TopLeftFront = 4,
        TopRightFront = 6,
        TopRightBack = 7,
        TopLeftBack = 5,
    }

    public class Octree
    {
        public OctreeNode node;
        private int depth;

        public Octree(int x, int y, int z)
        {
            int maxDim = 0;
            if (x >= y && x >= z)
                maxDim = x;
            else if (y >= x && y >= z)
                maxDim = y;
            else if (z >= x && z >= y)
                maxDim = z;
            int mdHalf = maxDim / 2;
            int depth = maxDim / Chunk.size / 8;
            
            Vector3Int position = new Vector3Int(mdHalf, mdHalf, mdHalf);
            node = new OctreeNode(position, maxDim);
            node.Subdivide(depth);
        }
        private int GetIndexOfPosition(Vector3Int lookupPosition, Vector3 nodePosition)
        {
            int index = 0;
            index |= lookupPosition.y > nodePosition.y ? 4 : 0;
            index |= lookupPosition.x > nodePosition.x ? 2 : 0;
            index |= lookupPosition.z > nodePosition.z ? 1 : 0;
            return index;
        }
    }
}