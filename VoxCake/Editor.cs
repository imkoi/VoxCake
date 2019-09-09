using System.Collections.Generic;
using UnityEngine;
using VoxCake.Buffered;

namespace VoxCake
{
    public static class Editor
    {
        public static Volume volume;
        public static uint color = 0xffffffff;
        public static byte tool = 0;

        private static Vector3Int _startPoint;
        private static Vector3Int _endPoint;

        public static void SetVolume(Volume volume)
        {
            Editor.volume = volume;
        }
        public static void SetStartPoint(byte mode)
        {
            Vector3Int point;
            if (Raycast.Hit(mode, 256, out point, Camera.main, volume))
                _startPoint = point;
            else _startPoint = new Vector3Int(-6, -6, -6);
        }
        public static void SetEndPoint(byte mode)
        {
            Vector3Int point;
            if (Raycast.Hit(mode, 256, out point, Camera.main, volume))
                _endPoint = point;
            else _endPoint = new Vector3Int(-6, -6, -6);
        }
        public static void SetVoxel(byte mode, bool buffered = false, bool usePhysics = false)
        {
            byte raycastMode = mode == 1 ? (byte)1 : (byte)0;
            SetEndPoint(raycastMode);
            if (buffered)
                CommandBuffer.Do(
                    new Block(),
                    mode,
                    _endPoint,
                    _endPoint,
                    volume);
            else
            {
                if (mode == 0)
                {
                    volume.SetData(_endPoint.x, _endPoint.y, _endPoint.z, 0);
                    Chunk.AddForVoxel(_endPoint.x, _endPoint.y, _endPoint.z, volume);
                    if(usePhysics)
                        Physics.Check(_endPoint.x, _endPoint.y, _endPoint.z, volume);
                }
                else if (mode == 1)
                {
                    volume.SetData(_endPoint.x, _endPoint.y, _endPoint.z, color);
                    Chunk.AddForVoxel(_endPoint.x, _endPoint.y, _endPoint.z, volume);
                }
                else if (mode == 2)
                {
                    volume.SetData(_endPoint.x, _endPoint.y, _endPoint.z, color);
                    Chunk.AddForVoxel(_endPoint.x, _endPoint.y, _endPoint.z, volume);
                }
            }
        }
        public static void SetLine(byte mode, bool buffered = false, bool usePhysics = false)
        {
            byte raycastMode = mode == 1 ? (byte)1 : (byte)0;
            SetEndPoint(raycastMode);

            if (buffered)
                CommandBuffer.Do(new Line(), mode, _startPoint, _endPoint, volume);
            else
            {
                uint value = mode == 0 ? 0 : color;
                int x0 = _startPoint.x;
                int x1 = _endPoint.x;
                int y0 = _startPoint.y;
                int y1 = _endPoint.y;
                int z0 = _startPoint.z;
                int z1 = _endPoint.z;
                List<Chunk> stack = new List<Chunk>();
                if (_startPoint != _endPoint)
                {
                    bool steepXY = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
                    if (steepXY)
                    {
                        Swap(ref x0, ref y0);
                        Swap(ref x1, ref y1);
                    }
                    bool steepXZ = Mathf.Abs(z1 - z0) > Mathf.Abs(x1 - x0);
                    if (steepXZ)
                    {
                        Swap(ref x0, ref z0);
                        Swap(ref x1, ref z1);
                    }

                    int deltaX = Mathf.FloorToInt(Mathf.Abs(x1 - x0));
                    int deltaY = Mathf.FloorToInt(Mathf.Abs(y1 - y0));
                    int deltaZ = Mathf.FloorToInt(Mathf.Abs(z1 - z0));
                    int errorXY = deltaX / 2, errorXZ = deltaX / 2;
                    int stepX = x0 > x1 ? -1 : 1;
                    int stepY = y0 > y1 ? -1 : 1;
                    int stepZ = z0 > z1 ? -1 : 1;

                    int y = y0, z = z0;
                    int xCopy, yCopy, zCopy;
                    int prevX = 0;
                    int prevY = 0;
                    int prevZ = 0;
                    for (int x = x0; stepX > 0 ? x != x1 + 1 : x != x1 - 1; x += stepX)
                    {
                        xCopy = x;
                        yCopy = y;
                        zCopy = z;
                        if (steepXZ) Swap(ref xCopy, ref zCopy);
                        if (steepXY) Swap(ref xCopy, ref yCopy);

                        volume.SetData(xCopy, yCopy, zCopy, value);
                        UpdateChunk(xCopy, yCopy, zCopy, stack);

                        errorXY -= deltaY;
                        errorXZ -= deltaZ;
                        if (errorXY < 0)
                        {
                            y += stepY;
                            errorXY += deltaX;
                        }
                        if (errorXZ < 0)
                        {
                            z += stepZ;
                            errorXZ += deltaX;
                        }
                        if (x != x0)
                        {
                            if (prevX != xCopy && prevY != yCopy && prevZ == zCopy)
                            {
                                volume.SetData(prevX, yCopy, zCopy, value);
                                UpdateChunk(prevX, yCopy, zCopy, stack);
                            }
                            else if (prevX != xCopy && prevY == yCopy && prevZ != zCopy)
                            {
                                volume.SetData(prevX, yCopy, zCopy, value);
                                UpdateChunk(prevX, yCopy, zCopy, stack);
                            }
                            else if (prevX == xCopy && prevY != yCopy && prevZ != zCopy)
                            {
                                volume.SetData(xCopy, yCopy, prevZ, value);
                                UpdateChunk(xCopy, yCopy, prevZ, stack);
                            }
                            else if (prevX != xCopy && prevY != yCopy && prevZ != zCopy)
                            {
                                volume.SetData(prevX, yCopy, zCopy, value);
                                volume.SetData(prevX, yCopy, prevZ, value);
                                UpdateChunk(prevX, yCopy, zCopy, stack);
                                UpdateChunk(prevX, yCopy, prevZ, stack);
                            }
                        }
                        prevX = xCopy;
                        prevY = yCopy;
                        prevZ = zCopy;
                    }
                    int count = stack.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Chunk chunk = stack[i];
                        Chunk.Add(chunk);
                    }
                }
                else
                {
                    volume.SetData(x0, y0, z0, value);
                    UpdateChunk(x0, y0, z0, stack);
                }
            }
        }
        public static void SetOctagon(byte mode, bool buffered = false, bool usePhysics = false)
        {
            byte raycastMode = mode == 1 ? (byte)1 : (byte)0;
            SetEndPoint(raycastMode);

            if (buffered)
            {
                CommandBuffer.Do(
                    new Octagon(),
                    mode,
                    _startPoint,
                    _endPoint,
                    volume);
            }
            else
            {
                uint value = mode == 0 ? 0 : color;
                int xMin = _startPoint.x < _endPoint.x ? _startPoint.x : _endPoint.x;
                int xMax = _startPoint.x > _endPoint.x ? _startPoint.x : _endPoint.x;
                int yMin = _startPoint.y < _endPoint.y ? _startPoint.y : _endPoint.y;
                int yMax = _startPoint.y > _endPoint.y ? _startPoint.y : _endPoint.y;
                int zMin = _startPoint.z < _endPoint.z ? _startPoint.z : _endPoint.z;
                int zMax = _startPoint.z > _endPoint.z ? _startPoint.z : _endPoint.z;
                List<Chunk> stack = new List<Chunk>();
                for (int x = xMin; x < xMax + 1; x++)
                {
                    for (int y = yMin; y < yMax + 1; y++)
                    {
                        for (int z = zMin; z < zMax + 1; z++)
                        {
                            volume.SetData(x, y, z, value);
                            UpdateChunk(x, y, z, stack);
                        }
                    }
                }
                int count = stack.Count;
                for (int i = 0; i < count; i++)
                {
                    Chunk chunk = stack[i];
                    Chunk.Add(chunk);
                }
            }
        }
        public static void SetSphere(byte mode)
        {
            byte raycastMode = mode == 1 ? (byte)1 : (byte)0;
            SetEndPoint(raycastMode);

            uint value = mode == 0 ? 0 : color;
            int xMin = _startPoint.x < _endPoint.x ? _startPoint.x : _endPoint.x;
            int xMax = _startPoint.x > _endPoint.x ? _startPoint.x : _endPoint.x;
            int yMin = _startPoint.y < _endPoint.y ? _startPoint.y : _endPoint.y;
            int yMax = _startPoint.y > _endPoint.y ? _startPoint.y : _endPoint.y;
            int zMin = _startPoint.z < _endPoint.z ? _startPoint.z : _endPoint.z;
            int zMax = _startPoint.z > _endPoint.z ? _startPoint.z : _endPoint.z;
            Vector3Int c = new Vector3Int(
                xMax - Mathf.RoundToInt((float)xMax / 2 + xMin),
                yMax - Mathf.RoundToInt((float)yMax / 2 + yMin),
                zMax - Mathf.RoundToInt((float)zMax / 2 + zMin)
                );
            Vector3Int r = new Vector3Int(
                xMax - c.x,
                yMax - c.y,
                zMax - c.z
                );
            List<Chunk> stack = new List<Chunk>();
            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    for (int k = zMin; k < zMax; k++)
                    {
                        Vector3Int p = new Vector3Int(i, j, k);
                        int d = (int)Mathf.Sqrt(
                            (c.x - p.x) * (c.x - p.x) +
                            (c.y - p.y) * (c.y - p.y) +
                            (c.z - p.z) * (c.z - p.z)
                            );

                        if (d < r.x && d < r.y && d < r.z)
                        {
                            volume.SetData(i, j, k, value);
                            UpdateChunk(i,j,k, stack);
                        }
                    }
                }
            }
            int count = stack.Count;
            for (int i = 0; i < count; i++)
            {
                Chunk chunk = stack[i];
                Chunk.Add(chunk);
            }
        }
        public static void SetExplosion(int x, int y, int z, int radius, byte damage, Volume volume)
        {
            int xMin = x - radius;
            int xMax = x + radius;
            int yMin = y - radius;
            int yMax = y + radius;
            int zMin = z - radius;
            int zMax = z + radius;

            int cxMin = xMin / Chunk.size;
            int cxMax = xMax / Chunk.size;
            int czMin = zMin / Chunk.size;
            int czMax = zMax / Chunk.size;

            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    for (int k = zMin; k < zMax; k++)
                    {
                        var pos = new Vector3Int(i, j, k);
                        int dis = (int)Vector3Int.Distance(pos, new Vector3Int(x, y, z)) + 1;

                        uint voxel = volume.GetData(i, j, k);
                        int hp = UColor.GetA(voxel) - damage / dis;

                        if (dis < radius) volume.SetData(i, j, k, UColor.SetA(voxel, hp));
                    }
                }
            }
        } //TODO: REWRITE THIS SHIT!
        public static void GetVoxelColor()
        {
            color = volume.GetData(_startPoint.x, _startPoint.y, _startPoint.z);
        }
        public static void UpdateChunk(int x, int y, int z, List<Chunk> stack)
        {
            byte ux = (byte)(x / Chunk.size);
            byte uy = (byte)(y / Chunk.size);
            byte uz = (byte)(z / Chunk.size);
            StackAdd(ux, uy, uz, stack);

            int xp = x - Chunk.size * ux;
            int yp = y - Chunk.size * uy;
            int zp = z - Chunk.size * uz;
            int csm = Chunk.size - 1;
            if (xp == csm && ux != volume.wdc - 1) StackAdd(ux + 1, uy, uz, stack);
            if (xp == 0 && ux != 0) StackAdd(ux - 1, uy, uz, stack);
            if (yp == csm && uy != volume.hdc - 1) StackAdd(ux, uy + 1, uz, stack);
            if (yp == 0 && uy != 0) StackAdd(ux, uy - 1, uz, stack);
            if (zp == csm && uz != volume.ddc - 1) StackAdd(ux, uy, uz + 1, stack);
            if (zp == 0 && uz != 0) StackAdd(ux, uy, uz - 1, stack);
        }
        public static void StackAdd(int ux, int uy, int uz, List<Chunk> stack)
        {
            if (!StackContains(ux, uy, uz, stack))
                stack.Add(new Chunk((byte)ux, (byte)uy, (byte)uz, volume));
        }
        public static bool StackContains(int x, int y, int z, List<Chunk> stack)
        {
            int count = stack.Count;
            for (int i = 0; i < count; i++)
            {
                Chunk chunk = stack[i];
                if (chunk.x == x && chunk.y == y && chunk.z == z) 
                    return true;
            }
            return false;
        }
        public static void Swap<T>(ref T x, ref T y)
        {
            T tmp = y;
            y = x;
            x = tmp;
        }
    }
}

