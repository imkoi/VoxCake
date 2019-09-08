using System.IO;
using UnityEngine;
using VoxCake.Common;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace VoxCake
{
    public static class ModelMesh
    {
        public static bool center = false;
        public static float scale = 15f;
        public static float[] ambientOcclusion = new[] { 7f, 4f, 2.5f };
        public static float[] edgeLighting = new[] { 14f, 12f, 8f };

        private static byte _team;
        private static byte _width;
        private static byte _depth;
        private static byte _height;
        private static uint[,,] _data;

        public static Mesh GetCulled(string name, byte team)
        {
            SetModelData(name);
            _team = team;
            
            int faceCount = 0;
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _depth; y++)
                {
                    for (int z = 0; z < _height; z++)
                    {
                        if (Data(x, y, z) != 0)
                        {
                            if (Data(x + 1, y, z) == 0) faceCount++;
                            if (Data(x - 1, y, z) == 0) faceCount++;
                            if (Data(x, y + 1, z) == 0) faceCount++;
                            if (Data(x, y - 1, z) == 0) faceCount++;
                            if (Data(x, y, z + 1) == 0) faceCount++;
                            if (Data(x, y, z - 1) == 0) faceCount++;
                        }
                    }
                }
            }

            if (faceCount != 0)
            {
                Vector3[] vertices = new Vector3[faceCount * 4];
                int[] triangles = new int[faceCount * 6];
                Color32[] colors32 = new Color32[faceCount * 4];

                int faceIndex = 0;
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        for (int z = 0; z < _depth; z++)
                        {
                            uint color = Data(x, y, z);

                            if (color != 0)
                            {
                                uint block01 = Data(x - 1, y - 1, z + 1);
                                uint block02 = Data(x, y - 1, z + 1);
                                uint block03 = Data(x + 1, y - 1, z + 1);
                                uint block04 = Data(x - 1, y - 1, z);
                                uint block05 = Data(x, y - 1, z);
                                uint block06 = Data(x + 1, y - 1, z);
                                uint block07 = Data(x - 1, y - 1, z - 1);
                                uint block08 = Data(x, y - 1, z - 1);
                                uint block09 = Data(x + 1, y - 1, z - 1);

                                uint block11 = Data(x - 1, y, z + 1);
                                uint block12 = Data(x, y, z + 1);
                                uint block13 = Data(x + 1, y, z + 1);
                                uint block14 = Data(x - 1, y, z);
                                uint block16 = Data(x + 1, y, z);
                                uint block17 = Data(x - 1, y, z - 1);
                                uint block18 = Data(x, y, z - 1);
                                uint block19 = Data(x + 1, y, z - 1);

                                uint block21 = Data(x - 1, y + 1, z + 1);
                                uint block22 = Data(x, y + 1, z + 1);
                                uint block23 = Data(x + 1, y + 1, z + 1);
                                uint block24 = Data(x - 1, y + 1, z);
                                uint block25 = Data(x, y + 1, z);
                                uint block26 = Data(x + 1, y + 1, z);
                                uint block27 = Data(x - 1, y + 1, z - 1);
                                uint block28 = Data(x, y + 1, z - 1);
                                uint block29 = Data(x + 1, y + 1, z - 1);

                                float i = 1 / scale;
                                float xScale = x / scale - 0.5f;
                                float yScale = y / scale - 0.5f + i;
                                float zScale = z / scale - 0.5f;
                                if (center)
                                {
                                    xScale = x / scale - 0.5f - (float)_width/2;
                                    yScale = y / scale - 0.5f + i;
                                    zScale = z / scale - 0.5f - (float)_depth/2;
                                }

                                if (block16 == 0)
                                {
                                    SetFace(new Vector3(xScale + i, yScale - i, zScale),
                                        new Vector3(xScale + i, yScale, zScale),
                                        new Vector3(xScale + i, yScale, zScale + i),
                                        new Vector3(xScale + i, yScale - i, zScale + i),
                                        faceIndex, color, 0.20f,

                                        block09, block06, block03,
                                        block19, block13,
                                        block29, block26, block23,

                                        block08, block05, block02,
                                        block18, block12,
                                        block28, block25, block22,

                                        vertices, triangles, colors32);
                                    faceIndex++;
                                }

                                if (block14 == 0)
                                {
                                    SetFace(new Vector3(xScale, yScale - i, zScale + i),
                                        new Vector3(xScale, yScale, zScale + i),
                                        new Vector3(xScale, yScale, zScale),
                                        new Vector3(xScale, yScale - i, zScale),
                                        faceIndex, color, 0.20f,

                                        block01, block04, block07,
                                        block11, block17,
                                        block21, block24, block27,

                                        block02, block05, block08,
                                        block12, block18,
                                        block22, block25, block28,

                                        vertices, triangles, colors32);
                                    faceIndex++;
                                }

                                if (block25 == 0)
                                {
                                    SetFace(new Vector3(xScale, yScale, zScale + i),
                                        new Vector3(xScale + i, yScale, zScale + i),
                                        new Vector3(xScale + i, yScale, zScale),
                                        new Vector3(xScale, yScale, zScale),
                                        faceIndex, color, 0f,

                                        block21, block24, block27,
                                        block22, block28,
                                        block23, block26, block29,

                                        block11, block14, block17,
                                        block12, block18,
                                        block13, block16, block19,

                                        vertices, triangles, colors32);
                                    faceIndex++;
                                }

                                if (block05 == 0)
                                {
                                    SetFace(new Vector3(xScale, yScale - i, zScale),
                                        new Vector3(xScale + i, yScale - i, zScale),
                                        new Vector3(xScale + i, yScale - i, zScale + i),
                                        new Vector3(xScale, yScale - i, zScale + i),
                                        faceIndex, color, 0.5f,

                                        block07, block04, block01,
                                        block08, block02,
                                        block09, block06, block03,

                                        block17, block14, block11,
                                        block18, block12,
                                        block19, block16, block13,

                                        vertices, triangles, colors32);
                                    faceIndex++;
                                }

                                if (block12 == 0)
                                {
                                    SetFace(new Vector3(xScale + i, yScale - i, zScale + i),
                                        new Vector3(xScale + i, yScale, zScale + i),
                                        new Vector3(xScale, yScale, zScale + i),
                                        new Vector3(xScale, yScale - i, zScale + i),
                                        faceIndex, color, 0.25f,

                                        block03, block02, block01,
                                        block13, block11,
                                        block23, block22, block21,

                                        block06, block05, block04,
                                        block16, block14,
                                        block26, block25, block24,

                                        vertices, triangles, colors32);
                                    faceIndex++;
                                }

                                if (block18 == 0)
                                {
                                    SetFace(new Vector3(xScale, yScale - i, zScale),
                                        new Vector3(xScale, yScale, zScale),
                                        new Vector3(xScale + i, yScale, zScale),
                                        new Vector3(xScale + i, yScale - i, zScale),
                                        faceIndex, color, 0.25f,

                                        block07, block08, block09,
                                        block17, block19,
                                        block27, block28, block29,

                                        block04, block05, block06,
                                        block14, block16,
                                        block24, block25, block26,

                                        vertices, triangles, colors32);
                                    faceIndex++;
                                }
                            }
                        }
                    }
                }
                return new Mesh
                {
                    vertices = vertices,
                    triangles = triangles,
                    colors32 = colors32,
                };
            }
            return new Mesh();
        }
        public static Mesh GetVoxel()
        {
            var vertices = new List<Vector3>();
            var colors32 = new List<Color32>();
            var triangles = new List<int>();

            /*
            SetFace(new Vector3(), new Vector3(), new Vector3(), new Vector3(), // PX
                new Color32(), 1, false, vertices, colors32, triangles);
            SetFace(new Vector3(), new Vector3(), new Vector3(), new Vector3(), // NX
                new Color32(), 1, false, vertices, colors32, triangles);
            SetFace(new Vector3(), new Vector3(), new Vector3(), new Vector3(), // PY
                new Color32(), 1, false, vertices, colors32, triangles);
            SetFace(new Vector3(), new Vector3(), new Vector3(), new Vector3(), // NY
                new Color32(), 1, false, vertices, colors32, triangles);
            SetFace(new Vector3(), new Vector3(), new Vector3(), new Vector3(), // PZ
                new Color32(), 1, false, vertices, colors32, triangles);
            SetFace(new Vector3(), new Vector3(), new Vector3(), new Vector3(), // NZ
                new Color32(), 1, false, vertices, colors32, triangles);
                */
            return new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                colors32 = colors32.ToArray()
            };
        }
        public static uint[,,] GetArray(string name)
        {
            SetModelData(name);
            //byte[,,] byteData = _data;
            uint[,,] uintData = new uint[15, 15, 15];
            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    for (int z = 0; z < 15; z++)
                    {
                        //uintData[x, y, z] = byteData[x, y, z];
                    }
                }
            }
            return uintData;
        }

        private static void SetModelData(string name)
        {
            Vox.Load(name);
            _width = (byte)Vox.width;
            _height = (byte)Vox.height;
            _depth = (byte)Vox.depth;
            _data = Vox.data;
        }

        private static void SetFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int index, uint inputColor, float light,
            uint ao1, uint ao2, uint ao3, uint ao4, uint ao6, uint ao7, uint ao8, uint ao9,
            uint le1, uint le2, uint le3, uint le4, uint le6, uint le7, uint le8, uint le9,
            Vector3[] vertices, int[] triangles, Color32[] colors32)
        {
            bool flip = false;
            Color32 color = UColor.UintToColor32(inputColor);
            index = index * 4;
            color = new Color32((byte)(color.r - color.r * light), (byte)(color.g - color.g * light), (byte)(color.b - color.b * light), 255);

            Color32 color1 = GetLight(color, le2, le1, le4, ao2, ao1, ao4, 1, ref flip);
            Color32 color2 = GetLight(color, le4, le7, le8, ao4, ao7, ao8, 2, ref flip);
            Color32 color3 = GetLight(color, le8, le9, le6, ao8, ao9, ao6, 3, ref flip);
            Color32 color4 = GetLight(color, le6, le3, le2, ao6, ao3, ao2, 4, ref flip);

            vertices[index + 0] = v1;
            vertices[index + 1] = v2;
            vertices[index + 2] = v3;
            vertices[index + 3] = v4;

            colors32[index + 0] = GetAmbient(color1, ao2, ao1, ao4, 1, ref flip);
            colors32[index + 1] = GetAmbient(color2, ao4, ao7, ao8, 2, ref flip);
            colors32[index + 2] = GetAmbient(color3, ao8, ao9, ao6, 3, ref flip);
            colors32[index + 3] = GetAmbient(color4, ao6, ao3, ao2, 4, ref flip);

            int face = index / 4;
            index = index / 4 * 6;
            if (!flip)
            {
                triangles[index] = face * 4;
                triangles[index + 1] = face * 4 + 1;
                triangles[index + 2] = face * 4 + 2;
                triangles[index + 3] = face * 4;
                triangles[index + 4] = face * 4 + 2;
                triangles[index + 5] = face * 4 + 3;
            }
            else
            {
                triangles[index] = face * 4 + 3;
                triangles[index + 1] = face * 4;
                triangles[index + 2] = face * 4 + 1;
                triangles[index + 3] = face * 4 + 3;
                triangles[index + 4] = face * 4 + 1;
                triangles[index + 5] = face * 4 + 2;
            }
        }
        private static uint Data(int x, int y, int z)
        {
            if (x >= _width || x < 0 || y >= _height || y < 0 || z >= _depth || z < 0)
                return 0;
            return _data[x, y, z];
        }
        private static Color32 GetLight(Color32 color, uint b1, uint b2, uint b3, uint b11, uint b12, uint b13, byte vertex, ref bool flip)
        {
            Color32[] le = new[] {
                CalculateLight(color.r, color.g, color.b, edgeLighting[0]),
                CalculateLight(color.r, color.g, color.b, edgeLighting[1]),
                CalculateLight(color.r, color.g, color.b, edgeLighting[2])
            };

            if (b1 == 0 && b3 == 0 && b11 == 0 && b13 == 0) // true
            {
                if (vertex == 1 || vertex == 3) flip = true;
                return le[2];
            }

            else if ((b1 == 0 && b11 == 0 && b2 == 0 && b12 == 0) || (b2 == 0 && b12 == 0 && b3 == 0 && b13 == 0))
            {
                return le[1];
            }

            else if ((b1 == 0 && b11 == 0) || (b3 == 0 && b13 == 0)) //c
            {
                return le[0];
            }

            else if (b2 == 0 && b12 == 0) // true
            {
                if (vertex == 1 || vertex == 3) flip = true;
                return le[0];
            }
            return color;
        }
        private static Color32 GetAmbient(Color32 color, uint b1, uint b2, uint b3, uint vertex, ref bool flip)
        {
            Color32[] ao = new[] { CalculateAo(color.r, color.g, color.b, ambientOcclusion[0]),
            CalculateAo(color.r, color.g, color.b, ambientOcclusion[1]),
            CalculateAo(color.r, color.g, color.b, ambientOcclusion[2]) };

            if (b1 != 0 && b3 != 0)
            {
                if (vertex == 1 || vertex == 3) flip = true;
                return ao[2];
            }

            else if ((b1 != 0 && b2 != 0) || (b2 != 0 && b3 != 0))
            {
                if (vertex == 1 || vertex == 3) flip = true;
                return ao[1];
            }

            else if (b1 != 0 || b3 != 0)
            {
                return ao[0];
            }

            else if (b2 != 0)
            {
                if (vertex == 1 || vertex == 3) flip = true;
                return ao[0];
            }
            return color;
        }
        private static Color32 CalculateAo(byte r, byte g, byte b, float ao)
        {
            float fr = r - r / ao;
            float fg = g - g / ao;
            float fb = b - b / ao;

            if (fr > 255) fr = 255;
            if (fr < 0) fr = 0;
            if (fg > 255) fg = 255;
            if (fg < 0) fg = 0;
            if (fb > 255) fb = 255;
            if (fb < 0) fb = 0;

            return new Color32((byte)fr, (byte)fg, (byte)fb, 255);
        }
        private static Color32 CalculateLight(byte r, byte g, byte b, float ao)
        {
            float fr = r + r / ao;
            float fg = g + g / ao;
            float fb = b + b / ao;

            if (fr > 255) fr = 255;
            if (fr < 0) fr = 0;
            if (fg > 255) fg = 255;
            if (fg < 0) fg = 0;
            if (fb > 255) fb = 255;
            if (fb < 0) fb = 0;

            return new Color32((byte)fr, (byte)fg, (byte)fb, 255);
        }
        private static Color32 GetSideShading(Color32 color, int side, bool flip)
        {
            float lightX = 0.2f;
            float lightPy = 0;
            float lightNy = 0.4f;
            float lightZ = 0.25f;

            if (side == 0) return new Color32(
                (byte)(color.r - color.r * lightX),
                (byte)(color.g - color.g * lightX),
                (byte)(color.b - color.b * lightX),
                255);
            if (side == 1 && flip == false) return new Color32(
                (byte)(color.r - color.r * lightPy),
                (byte)(color.g - color.g * lightPy),
                (byte)(color.b - color.b * lightPy),
                255);
            if (side == 1 && flip == true) return new Color32(
                (byte)(color.r - color.r * lightNy),
                (byte)(color.g - color.g * lightNy),
                (byte)(color.b - color.b * lightNy),
                255);
            if (side == 2) return new Color32(
                (byte)(color.r - color.r * lightZ),
                (byte)(color.g - color.g * lightZ),
                (byte)(color.b - color.b * lightZ),
                255);
            return new Color32();
        }
    }
}