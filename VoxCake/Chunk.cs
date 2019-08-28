using System;
using UnityEngine;
using VoxCake.Common;
using System.Collections.Generic;

namespace VoxCake
{
    public class Chunk : IComparable
    {
        public static byte size = 16;
        public byte x, y, z;
        public float sqrDistanceToCamera;
        public Mesh mesh;
        public Volume volume;
        public static Stack<Chunk> stack = new Stack<Chunk>();

        public Chunk(byte x, byte y, byte z, Volume volume)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.volume = volume;
        }

        public static void UpdateStack()
        {
            int count = stack.Count;
            int cpu = Config.cpuCount;
            if (count != 0)
            {
                if (count < cpu)
                {
                    for (int i = 0; i < count; i++)
                    {
                        Chunk chunk = stack.Pop();
                        Update(chunk.x, chunk.y, chunk.z, chunk.volume);
                    }
                }
                else
                {
                    for (int i = 0; i < cpu; i++)
                    {
                        Chunk chunk = stack.Pop();
                        Update(chunk.x, chunk.y, chunk.z, chunk.volume);
                    }
                }
            }
        }
        public static void Add(Chunk chunk)
        {
            stack.Push(chunk);
        }
        public static void Add(byte x, byte y, byte z, Volume volume)
        {
            Add(new Chunk(x, y, z, volume));
        }
        public static void AddForVoxel(int x, int y, int z, Volume volume)
        {
            byte ux = (byte)(x / size);
            byte uy = (byte)(y / size);
            byte uz = (byte)(z / size);

            Add(new Chunk(ux, uy, uz, volume));

            int xp = x - size * ux;
            int yp = y - size * uy;
            int zp = z - size * uz;

            if (xp == size - 1 && ux != volume.wdc - 1) Add(new Chunk((byte)(ux + 1), uy, uz, volume));
            if (xp == 0 && ux != 0) Add(new Chunk((byte)(ux - 1), uy, uz, volume));
            if (yp == size - 1 && uy != volume.hdc - 1) Add(new Chunk(ux, (byte)(uy + 1), uz, volume));
            if (yp == 0 && uy != 0) Add(new Chunk(ux, (byte)(uy - 1), uz, volume));
            if (zp == size - 1 && uz != volume.ddc - 1) Add(new Chunk(ux, uy, (byte)(uz + 1), volume));
            if (zp == 0 && uz != 0) Add(new Chunk(ux, uy, (byte)(uz - 1), volume));
        }

        public static void Update(int x, int y, int z, Volume volume)
        {
            if (x >= 0 && x < volume.wdc && y >= 0 && y < volume.hdc && z >= 0 && z < volume.ddc)
            {
                volume.chunks[x, y, z].mesh = ChunkMesh.Get(x, y, z, volume);
            }
        }

        public int CompareTo(object obj)
        {
            return -sqrDistanceToCamera.CompareTo(((Chunk)obj).sqrDistanceToCamera);
        }
    };
}