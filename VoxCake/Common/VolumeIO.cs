using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace VoxCake.Common
{
    public static class VolumeIO
    {
        public static void OpenMap(string path, uint innerColor, Volume volume)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                uint[,,] data = new uint[volume.width, volume.height, volume.depth];

                volume.data = new uint[volume.width, volume.height, volume.depth];

                int pos = 0;
                for (int x = 0; x < volume.width; ++x)
                {
                    for (int z = 0; z < volume.depth; ++z)
                    {
                        int y = 0;
                        for (; y < volume.height; ++y) data[x, y, z] = innerColor;

                        y = 0;
                        while (true)
                        {
                            int number4ByteChunks = bytes[pos + 0];
                            int topColorStart = bytes[pos + 1];
                            int topColorEnd = bytes[pos + 2];
                            int colorPos = pos + 4;

                            for (; y < topColorStart; ++y) data[x, y, z] = 0;

                            for (; y <= topColorEnd; y++)
                            {
                                data[x, y, z] = BitConverter.ToUInt32(bytes, colorPos);
                                colorPos += 4;
                            }

                            if (topColorEnd == volume.height - 2) data[x, y, volume.height - 1] = data[x, y, volume.height - 2];

                            int lenBottom = topColorEnd - topColorStart + 1;
                            if (number4ByteChunks == 0)
                            {
                                pos += 4 * (lenBottom + 1);
                                break;
                            }

                            int lenTop = number4ByteChunks - 1 - lenBottom;

                            pos += bytes[pos] * 4;
                            int bottomColorEnd = bytes[pos + 3];
                            int bottomColorStart = bottomColorEnd - lenTop;

                            for (y = bottomColorStart; y < bottomColorEnd; y++)
                            {
                                data[x, y, z] = BitConverter.ToUInt32(bytes, colorPos);
                                colorPos += 4;
                            }

                            if (bottomColorEnd == volume.height - 1) data[x, y, volume.height - 1] = data[x, y, volume.height - 2];
                        }
                    }
                }

                for (int x = 0; x < volume.width; x++)
                {
                    for (int y = 0; y < volume.height; y++)
                    {
                        for (int z = 0; z < volume.depth; z++)
                        {
                            uint i = data[x, y, z];
                            if (i != 0)
                                volume.data[x, volume.height - y - 1, z] = i;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error("VolumeIO: OpenMap();", true);
                Application.Quit();
            }
        }
        public static void SaveMap(string path, uint innerColor, Volume volume)
        {
            List<byte> bytes = new List<byte>();
            bool[,,] map = new bool[volume.width, volume.depth, volume.height];
            uint[,,] color = new uint[volume.width, volume.depth, volume.height];
            for (int x = 0; x < volume.width; x++)
            {
                for (int y = 0; y < volume.height; y++)
                {
                    for (int z = 0; z < volume.depth; z++)
                    {
                        if (volume.data[z, volume.height - y - 1, x] == 0x00000000) map[x, z, y] = false;
                        else map[x, z, y] = true;
                        color[x, z, y] = volume.data[z, volume.height - y - 1, x];
                    }
                }
            }

            int i, j, k;
            for (j = 0; j < volume.width; ++j)
            {
                for (i = 0; i < volume.depth; ++i)
                {
                    k = 0;
                    while (k < volume.height)
                    {
                        int z;

                        int airStart = k;
                        while (k < volume.height && !map[i, j, k]) ++k;

                        int topColorsStart = k;
                        while (k < volume.height && IsSurface(i, j, k, map, volume)) ++k;
                        int topColorsEnd = k;

                        while (k < volume.height && map[i, j, k] && !IsSurface(i, j, k, map, volume)) ++k;

                        int bottomColorsStart = k;

                        z = k;
                        while (z < volume.height && IsSurface(i, j, z, map, volume)) ++z;

                        if (z == volume.height || false) ;
                        else
                            while (IsSurface(i, j, k, map, volume))
                                ++k;

                        int bottomColorsEnd = k;

                        int topColorsLen = topColorsEnd - topColorsStart;
                        int bottomColorsLen = bottomColorsEnd - bottomColorsStart;

                        int colors = topColorsLen + bottomColorsLen;

                        if (k == volume.height) WriteByte(0, bytes);
                        else WriteByte((byte)(colors + 1), bytes);

                        WriteByte((byte)topColorsStart, bytes);
                        WriteByte((byte)(topColorsEnd - 1), bytes);
                        WriteByte((byte)airStart, bytes);

                        for (z = 0; z < topColorsLen; ++z) WriteColor(color[i, j, topColorsStart + z], bytes);
                        for (z = 0; z < bottomColorsLen; ++z) WriteColor(color[i, j, bottomColorsStart + z], bytes);
                    }
                }
            }
            File.WriteAllBytes(path, bytes.ToArray());
        }
        public static void SaveModel(string path, Volume volume)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)volume.width);
            bytes.Add((byte)volume.height);
            bytes.Add((byte)volume.depth);
            bytes.Add(0);
            for(int x = 0; x< volume.width; x++)
            {
                for (int y = 0; y < volume.height; y++)
                {
                    for (int z = 0; z < volume.depth; z++)
                    {
                        uint value = volume.GetData(x, y, z);
                        bytes.Add((byte)(value >> 16));
                        bytes.Add((byte)(value >> 8));
                        bytes.Add((byte)(value >> 0));
                        bytes.Add((byte)(value >> 24));
                    }
                }
            }
            File.WriteAllBytes(path, bytes.ToArray());
        }
        public static void OpenModel(string path, Volume volume)
        {
            byte[] bytes = File.ReadAllBytes(path);
            int width = bytes[0];
            int height = bytes[1];
            int depth = bytes[2];
            int somthing = bytes[3];

            volume.data = new uint[width, height, depth];
            int idx = 4;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        volume.SetData(x, y, z, UColor.RGBAToUint(bytes[idx + 0], bytes[idx + 1], bytes[idx + 2], bytes[idx + 3]));
                        idx += 4;
                    }
                }
            }
        }

        private static bool IsSurface(int x, int y, int z, bool[,,] map, Volume volume)
        {
            if (!map[x, y, z]) return false;
            if (x > 0 && !map[x - 1, y, z]) return true;
            if (x + 1 < volume.width && !map[x + 1, y, z]) return true;
            if (y > 0 && !map[x, y - 1, z]) return true;
            if (y + 1 < volume.depth && !map[x, y + 1, z]) return true;
            if (z > 0 && !map[x, y, z - 1]) return true;
            if (z + 1 < volume.height && !map[x, y, z + 1]) return true;
            return false;
        }
        private static void WriteColor(uint color, List<byte> bytes)
        {
            WriteByte((byte)(color >> 0), bytes);
            WriteByte((byte)(color >> 8), bytes);
            WriteByte((byte)(color >> 16), bytes);
            WriteByte((byte)(color >> 24), bytes);
        }
        private static void WriteByte(byte value, List<byte> bytes)
        {
            bytes.Add(value);
        }
    }
}