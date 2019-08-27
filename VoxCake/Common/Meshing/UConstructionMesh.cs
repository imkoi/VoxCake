using UnityEngine;
using System.Collections.Generic;

namespace VoxCake.Common.Meshing
{
    public class UConstructionMesh
    {
        public static int width;
        public static int depth;
        public static int height;
        public static uint[,,] voxels;
        public static float[] ambientOcclusion = new[] { 6f, 5f, 3.5f };
        public static float[] edgeLighting = new[] { 12f, 10f, 8f };

        private const float _scale = 1f;

        public static void SetParameters(int width, int height, int depth, int xMin, int yMin, int zMin, List<Voxel> voxelStack)
        {
            UConstructionMesh.width = width;
            UConstructionMesh.height = height;
            UConstructionMesh.depth = depth;
            UConstructionMesh.voxels = new uint[width, height, depth];
            foreach (Voxel voxelInStack in voxelStack)
                UConstructionMesh.voxels[voxelInStack.x - xMin, voxelInStack.y - yMin, voxelInStack.z - zMin] = voxelInStack.value;

        }

        public static Mesh GetCulled(List<Voxel> voxels)
        {
            int xMin = 0;
            int xMax = 0;
            int yMin = 0;
            int yMax = 0;
            int zMin = 0;
            int zMax = 0;

            for (int i = 0; i < voxels.Count; i++)
            {
                if (i == 0)
                {
                    xMin = voxels[i].x;
                    yMin = voxels[i].y;
                    zMin = voxels[i].z;

                    xMax = voxels[i].x;
                    yMax = voxels[i].y;
                    zMax = voxels[i].z;
                }
                else
                {
                    if (voxels[i].x < xMin) xMin = voxels[i].x;
                    if (voxels[i].x > xMax) xMax = voxels[i].x;

                    if (voxels[i].y < yMin) yMin = voxels[i].y;
                    if (voxels[i].y > yMax) yMax = voxels[i].y;

                    if (voxels[i].z < zMin) zMin = voxels[i].z;
                    if (voxels[i].z > zMax) zMax = voxels[i].z;
                }
            }

            width = xMax - xMin+1;
            height = yMax - yMin+1;
            depth = zMax - zMin+1;

            UConstructionMesh.voxels = new uint[width, height, depth];

            foreach (Voxel voxel in voxels)
            {
                UConstructionMesh.voxels[voxel.x - xMin, voxel.y - yMin, voxel.z - zMin] = voxel.value;
            }

            int faceCount = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
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
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int z = 0; z < depth; z++)
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

                                float xScale = x / _scale;
                                float yScale = y / _scale + 1f;
                                float zScale = z / _scale;
                                float i = 1 / _scale;

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
                    colors32 = colors32
                };
            }

            return new Mesh();
        }
        public static Mesh GetGreedy(List<Voxel> voxels)
        {
            if(voxels != null)
            {
                int xMin = 0;
                int xMax = 0;
                int yMin = 0;
                int yMax = 0;
                int zMin = 0;
                int zMax = 0;

                for (int i = 0; i < voxels.Count; i++)
                {
                    if (i == 0)
                    {
                        xMin = voxels[i].x;
                        yMin = voxels[i].y;
                        zMin = voxels[i].z;

                        xMax = voxels[i].x;
                        yMax = voxels[i].y;
                        zMax = voxels[i].z;
                    }
                    else
                    {
                        if (voxels[i].x < xMin) xMin = voxels[i].x;
                        if (voxels[i].x > xMax) xMax = voxels[i].x;

                        if (voxels[i].y < yMin) yMin = voxels[i].y;
                        if (voxels[i].y > yMax) yMax = voxels[i].y;

                        if (voxels[i].z < zMin) zMin = voxels[i].z;
                        if (voxels[i].z > zMax) zMax = voxels[i].z;
                    }
                }

                width = xMax - xMin + 1;
                height = yMax - yMin + 1;
                depth = zMax - zMin + 1;

                UConstructionMesh.voxels = new uint[width, height, depth];

                foreach (Voxel voxel in voxels)
                {
                    UConstructionMesh.voxels[voxel.x - xMin, voxel.y - yMin, voxel.z - zMin] = voxel.value;
                }
            }

            var vertices = new List<Vector3>();
            var colors32 = new List<Color32>();
            var triangles = new List<int>();

            var dimensions = new[] { width, height, depth };

            for (var d = 0; d < 3; d++)
            {
                var u = (d + 1) % 3;
                var v = (d + 2) % 3;
                var x = new int[3];
                var q = new int[3]; q[d] = 1;
                var mask = new int[dimensions[u] * dimensions[v]];
                var colorMask = new uint[dimensions[u] * dimensions[v]];

                for (x[d] = -1; x[d] < dimensions[d];)
                {
                    var n = 0;
                    for (x[v] = 0; x[v] < dimensions[v]; ++x[v])
                    {
                        for (x[u] = 0; x[u] < dimensions[u]; ++x[u], ++n)
                        {
                            var current = (x[d] >= 0 ? Data(x[0], x[1], x[2]) : 0);
                            var next = (x[d] < dimensions[d] - 1 ? Data(x[0] + q[0], x[1] + q[1], x[2] + q[2]) : 0);
                            if (current != next)
                            {
                                if (current > 0 && next > 0)
                                {
                                    mask[n] = 0;
                                    colorMask[n] = 0;
                                }
                                else if (current == 0)
                                {
                                    mask[n] = -1;
                                    colorMask[n] = next;
                                }
                                else if (next == 0)
                                {
                                    mask[n] = 1;
                                    colorMask[n] = current;
                                }
                                else
                                {
                                    mask[n] = 0;
                                    colorMask[n] = 0;
                                }
                            }
                            else
                            {
                                mask[n] = 0;
                                colorMask[n] = 0;
                            }
                        }
                    }

                    x[d]++;
                    n = 0;
                    for (var j = 0; j < dimensions[v]; ++j)
                    {
                        for (var i = 0; i < dimensions[u];)
                        {
                            var maskValue = mask[n];
                            var colorValue = colorMask[n];
                            if (maskValue != 0)
                            {
                                int l, k;

                                var w = 1;
                                for (;
                                    n + w < mask.Length && mask[n + w] == maskValue && colorValue == colorMask[n + w] &&
                                    i + w < dimensions[u];
                                    ++w)
                                {
                                }

                                var h = 1;
                                var done = false;
                                for (; j + h < dimensions[v]; ++h)
                                {
                                    for (k = 0; k < w; ++k)
                                    {
                                        if (mask[n + k + h * dimensions[u]] != maskValue ||
                                            colorMask[n + k + h * dimensions[u]] != colorValue)
                                        {
                                            done = true;
                                            break;
                                        }
                                    }

                                    if (done) break;
                                }

                                var xp = new Vector3();
                                xp[u] = i;
                                xp[v] = j;
                                xp[d] = x[d];
                                xp *= _scale;
                                var du = new Vector3();
                                du[u] = w * _scale;
                                var dv = new Vector3();
                                dv[v] = h * _scale;
                                SetFace(
                                    new Vector3(xp[0], xp[1], xp[2]),
                                    new Vector3(xp[0] + du[0], xp[1] + du[1], xp[2] + du[2]),
                                    new Vector3(xp[0] + du[0] + dv[0], xp[1] + du[1] + dv[1], xp[2] + du[2] + dv[2]),
                                    new Vector3(xp[0] + dv[0], xp[1] + dv[1], xp[2] + dv[2]),
                                    UColor.UintToColor32(colorValue), d, maskValue < 0,
                                    vertices, colors32, triangles);
                                for (l = 0; l < h; ++l)
                                {
                                    for (k = 0; k < w; ++k)
                                    {
                                        mask[(n + k) + l * dimensions[u]] = 0;
                                        colorMask[(n + k) + l * dimensions[u]] = 0;
                                    }
                                }

                                i += w;
                                n += w;
                            }
                            else
                            {
                                ++i;
                                ++n;
                            }
                        }
                    }
                }
            }

            return new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                colors32 = colors32.ToArray()
            };
        }

        private static uint Data(int x, int y, int z)
        {
            if (x >= width || x < 0 || y >= height || y < 0 || z >= depth || z < 0)
                return 0;
            return voxels[x, y, z];
        }
        private static void SetFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int index, uint inputColor, float light,
            uint ao1, uint ao2, uint ao3, uint ao4, uint ao6, uint ao7, uint ao8, uint ao9,
            uint le1, uint le2, uint le3, uint le4, uint le6, uint le7, uint le8, uint le9,
            Vector3[] vertices, int[] triangles, Color32[] colors32)
        {
            bool flip = false;
            Color32 color;
            byte voxelDamage = UColor.GetA(inputColor);

            color = UColor.UintToColor32(inputColor);
            byte hp = UColor.GetA(inputColor);
            if (hp < 100)
            {
                color = CalculateDamage(color, hp, voxelDamage);
            }



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
        private static void SetFace(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color32 color, int side, bool flip,
            List<Vector3> vertices, List<Color32> colors32, List<int> triangles)
        {
            var start = vertices.Count;
            color = GetFaceShading(color, side, flip);

            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);
            vertices.Add(d);

            colors32.Add(color);
            colors32.Add(color);
            colors32.Add(color);
            colors32.Add(color);

            if (flip)
            {
                triangles.Add(start + 2);
                triangles.Add(start + 1);
                triangles.Add(start + 3);
                triangles.Add(start + 1);
                triangles.Add(start);
                triangles.Add(start + 3);
            }
            else
            {
                triangles.Add(start);
                triangles.Add(start + 1);
                triangles.Add(start + 2);
                triangles.Add(start);
                triangles.Add(start + 2);
                triangles.Add(start + 3);
            }
        }
        private static void SetShadow(int x, int y, int z, int height, int index, Color32[] colors32, Volume volume)
        {
            index = index * 4;
            for (int i = 1; i < height + 1; i++)
            {
                if (Data(x, y + i, z) != 0)
                {
                    colors32[index + 0] = CalculateShadow(colors32[index + 0].r, colors32[index + 0].g, colors32[index + 0].b, i);
                    colors32[index + 1] = CalculateShadow(colors32[index + 1].r, colors32[index + 1].g, colors32[index + 1].b, i);
                    colors32[index + 2] = CalculateShadow(colors32[index + 2].r, colors32[index + 2].g, colors32[index + 2].b, i);
                    colors32[index + 3] = CalculateShadow(colors32[index + 3].r, colors32[index + 3].g, colors32[index + 3].b, i);
                    break;
                }
            }
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
        private static Color32 GetAmbient(Color32 color, uint b1, uint b2, uint b3, byte vertex, ref bool flip)
        {
            Color32[] ao = new[] { CalculateAmbient(color.r, color.g, color.b, ambientOcclusion[0]),
            CalculateAmbient(color.r, color.g, color.b, ambientOcclusion[1]),
            CalculateAmbient(color.r, color.g, color.b, ambientOcclusion[2]) };

            if (b1 != 0 && b3 != 0)
            {
                if (vertex == 1 || vertex == 3) flip = true;
                return ao[2];
            }

            else if ((b1 != 0 && b2 != 0) || (b2 != 0 && b3 != 0))
            {
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
        private static Color32 GetFaceShading(Color32 color, int side, bool flip)
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
        private static Color32 CalculateDamage(Color32 color, byte hp, float damage)
        {
            damage = damage / (hp / damage);

            float fr = color.r - color.r / damage;
            float fg = color.g - color.g / damage;
            float fb = color.b - color.b / damage;

            if (fr > 255) fr = 255;
            if (fr < 0) fr = 0;
            if (fg > 255) fg = 255;
            if (fg < 0) fg = 0;
            if (fb > 255) fb = 255;
            if (fb < 0) fb = 0;

            return new Color32((byte)fr, (byte)fg, (byte)fb, 255);
        }
        private static Color32 CalculateShadow(byte r, byte g, byte b, float i)
        {
            float fr = r - 8 / i;
            float fg = g - 8 / i;
            float fb = b - 8 / i;

            if (fr > 255) fr = 255;
            if (fr < 0) fr = 0;
            if (fg > 255) fg = 255;
            if (fg < 0) fg = 0;
            if (fb > 255) fb = 255;
            if (fb < 0) fb = 0;

            return new Color32((byte)fr, (byte)fg, (byte)fb, 255);
        }
        private static Color32 CalculateAmbient(byte r, byte g, byte b, float ao)
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
    }
}
