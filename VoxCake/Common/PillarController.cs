using System.Collections.Generic;
using UnityEngine;

namespace VoxCake.Common
{
    public class PillarController : MonoBehaviour
    {
        public float weight;
        public Volume volume;

        private int _yDistance;
        private float _fallSpeed = 10f;
        private uint[,,] _voxelsToBuild;
        private Vector3 _fallDirection = new Vector3(0, -1, 0);
        private Vector3Int _raycastPoint = new Vector3Int();
        private Vector3Int _size = new Vector3Int();
        private Vector3Int _min = new Vector3Int();
        private Vector3Int _max = new Vector3Int();
        private int _yDifference = new int();

        public void SetValues(Vector3 position, Vector3 hitPoint,
            Vector3Int lowestPoint, float weight, int yDistance,
            Vector3Int size, Vector3Int min, Vector3Int max,
            uint[,,] voxelsToBuild, Volume volume)
        {
            this.weight = weight;
            this.volume = volume;
            _size = size;
            _min = min;
            _max = max;
            _raycastPoint = lowestPoint;
            _yDistance = yDistance;
            _voxelsToBuild = voxelsToBuild;
            _yDifference = (int)position.y - _raycastPoint.y;
        }

        private void Start()
        {
            weight /= 3000;
        }

        private void Update()
        {
            Vector3 raycastOrigin = new Vector3(_raycastPoint.x, transform.position.y - _yDifference, _raycastPoint.z);
            if (Raycast.Hit(1, 0f, raycastOrigin, Vector3.down, volume))
            {
                Debug.Log(raycastOrigin);
                SetVoxels();
                Destroy(gameObject);
            }
            else
            {
                _fallSpeed += 0.2f + weight;
                transform.position += _fallDirection * _fallSpeed * Time.deltaTime;
            }
        }

        private void SetVoxels()
        {
            List<Chunk> stack = new List<Chunk>();
            for (int x = 0; x < _size.x; x++)
            {
                for (int z = 0; z < _size.z; z++)
                {
                    for (int y = 0; y < _size.y; y++)
                    {
                        if (_voxelsToBuild[x, y, z] != 0)
                        {
                            int px = x + _min.x;
                            int py = y + _min.y - _yDistance + 1;
                            int pz = z + _min.z;
                            volume.SetData(px, py, pz, _voxelsToBuild[x, y, z]);
                            UpdateChunk(px, py, pz, stack);
                        }
                    }
                }
            }
            foreach (Chunk chunk in stack)
                Chunk.Add(chunk);
        }

        private void UpdateChunk(int x, int y, int z, List<Chunk> stack)
        {
            byte ux = (byte)(x / Chunk.size);
            byte uy = (byte)(y / Chunk.size);
            byte uz = (byte)(z / Chunk.size);
            StackAdd(ux, uy, uz, stack);

            int xp = x - Chunk.size * ux;
            int yp = y - Chunk.size * uy;
            int zp = z - Chunk.size * uz;
            if (xp == Chunk.size - 1 && ux != volume.wdc - 1) StackAdd(ux + 1, uy, uz, stack);
            if (xp == 0 && ux != 0) StackAdd(ux - 1, uy, uz, stack);
            if (yp == Chunk.size - 1 && uy != volume.hdc - 1) StackAdd(ux, uy + 1, uz, stack);
            if (yp == 0 && uy != 0) StackAdd(ux, uy - 1, uz, stack);
            if (zp == Chunk.size - 1 && uz != volume.ddc - 1) StackAdd(ux, uy, uz + 1, stack);
            if (zp == 0 && uz != 0) StackAdd(ux, uy, uz - 1, stack);
        }
        private void StackAdd(int ux, int uy, int uz, List<Chunk> stack)
        {
            if (!StackContains(ux, uy, uz, stack))
            {
                stack.Add(new Chunk((byte)ux, (byte)uy, (byte)uz, volume));
            }
        }
        private bool StackContains(int x, int y, int z, List<Chunk> stack)
        {
            foreach (Chunk chunk in stack) if (chunk.x == x && chunk.y == y && chunk.z == z) return true;
            return false;
        }
    }
}