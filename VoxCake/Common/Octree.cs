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
        private Volume volume;
        public OctreeNode node;
        private int depth;

        public Octree(Vector3Int position, int size, int depth, Volume volume)
        {
            node = new OctreeNode(position, size);
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