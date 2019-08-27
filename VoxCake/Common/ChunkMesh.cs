using UnityEngine;
using Unity.Mathematics;

namespace VoxCake.Common
{
    public class ChunkMesh
    {
        public static bool modelEditor = false;
        public static float[] ambientOcclusion = new[] { 6f, 5f, 3f };
        public static float[] edgeLighting = new[] { 12f, 10f, 8f };

        private const float _scale = 1f;

        public static Mesh Get(int chunkX, int chunkY, int chunkZ, Volume volume)
        {
            int cs = Chunk.size;
            int dx = chunkX * cs;
            int dy = chunkY * cs;
            int dz = chunkZ * cs;
            int vx = volume.width;
            int vy = volume.height;
            int vz = volume.depth;

            int faceCount = 0;
            for (int x = 0; x < cs; x++)
            {
                for (int y = 0; y < cs; y++)
                {
                    for (int z = 0; z < cs; z++)
                    {
                        uint voxel = Data(x, y, z, dx, dy, dz, vx, vy, vz, volume);
                        if (voxel != 0 && voxel != 1)
                        {
                            if (Data(x + 1, y, z, dx, dy, dz, vx, vy, vz, volume) == 0) faceCount++;
                            if (Data(x - 1, y, z, dx, dy, dz, vx, vy, vz, volume) == 0) faceCount++;
                            if (Data(x, y + 1, z, dx, dy, dz, vx, vy, vz, volume) == 0) faceCount++;
                            if (Data(x, y - 1, z, dx, dy, dz, vx, vy, vz, volume) == 0) faceCount++;
                            if (Data(x, y, z + 1, dx, dy, dz, vx, vy, vz, volume) == 0) faceCount++;
                            if (Data(x, y, z - 1, dx, dy, dz, vx, vy, vz, volume) == 0) faceCount++;
                        }
                    }
                }
            }

            if (faceCount != 0)
            {
                Vector3[] vertices = new Vector3[faceCount * 4];
                int[] triangles = new int[faceCount * 6];
                Color32[] colors32 = new Color32[faceCount * 4];
                Vector2[] uv = new Vector2[faceCount * 4];

                int faceIndex = 0;
                for (int x = 0; x < cs; x++)
                {
                    for (int y = 0; y < cs; y++)
                    {
                        for (int z = 0; z < cs; z++)
                        {
                            uint color = Data(x, y, z, dx, dy, dz, vx, vy, vz, volume);
                            if (color != 0 && color != 1)
                            {
                                int xp = x + 1;
                                int xn = x - 1;
                                int yp = y + 1;
                                int yn = y - 1;
                                int zp = z + 1;
                                int zn = z - 1;

                                uint block01 = Data(xn, yn, zp, dx, dy, dz, vx, vy, vz, volume);
                                uint block02 = Data(x, yn, zp, dx, dy, dz, vx, vy, vz, volume);
                                uint block03 = Data(xp, yn, zp, dx, dy, dz, vx, vy, vz, volume);
                                uint block04 = Data(xn, yn, z, dx, dy, dz, vx, vy, vz, volume);
                                uint block05 = Data(x, yn, z, dx, dy, dz, vx, vy, vz, volume);
                                uint block06 = Data(xp, yn, z, dx, dy, dz, vx, vy, vz, volume);
                                uint block07 = Data(xn, yn, zn, dx, dy, dz, vx, vy, vz, volume);
                                uint block08 = Data(x, yn, zn, dx, dy, dz, vx, vy, vz, volume);
                                uint block09 = Data(xp, yn, zn, dx, dy, dz, vx, vy, vz, volume);

                                uint block11 = Data(xn, y, zp, dx, dy, dz, vx, vy, vz, volume);
                                uint block12 = Data(x, y, zp, dx, dy, dz, vx, vy, vz, volume);
                                uint block13 = Data(xp, y, zp, dx, dy, dz, vx, vy, vz, volume);
                                uint block14 = Data(xn, y, z, dx, dy, dz, vx, vy, vz, volume);
                                uint block16 = Data(xp, y, z, dx, dy, dz, vx, vy, vz, volume);
                                uint block17 = Data(xn, y, zn, dx, dy, dz, vx, vy, vz, volume);
                                uint block18 = Data(x, y, zn, dx, dy, dz, vx, vy, vz, volume);
                                uint block19 = Data(xp, y, zn, dx, dy, dz, vx, vy, vz, volume);

                                uint block21 = Data(xn, yp, zp, dx, dy, dz, vx, vy, vz, volume);
                                uint block22 = Data(x, yp, zp, dx, dy, dz, vx, vy, vz, volume);
                                uint block23 = Data(xp, yp, zp, dx, dy, dz, vx, vy, vz, volume);
                                uint block24 = Data(xn, yp, z, dx, dy, dz, vx, vy, vz, volume);
                                uint block25 = Data(x, yp, z, dx, dy, dz, vx, vy, vz, volume);
                                uint block26 = Data(xp, yp, z, dx, dy, dz, vx, vy, vz, volume);
                                uint block27 = Data(xn, yp, zn, dx, dy, dz, vx, vy, vz, volume);
                                uint block28 = Data(x, yp, zn, dx, dy, dz, vx, vy, vz, volume);
                                uint block29 = Data(xp, yp, zn, dx,dy,dz, vx, vy, vz, volume);

                                float xScale = x / _scale;
                                float yScale = y / _scale + 1f;
                                float zScale = z / _scale;
                                float i = 1 / _scale;

                                float xScaleP = xScale + i;
                                float yScaleN = yScale - i;
                                float zScaleP = zScale + i;

                                if (block16 == 0)
                                {
                                    SetFace(new Vector3(xScaleP, yScaleN, zScale),
                                        new Vector3(xScaleP, yScale, zScale),
                                        new Vector3(xScaleP, yScale, zScaleP),
                                        new Vector3(xScaleP, yScaleN, zScaleP),
                                        faceIndex, color, 0.25f,
                                        block09, block06, block03,
                                        block19,          block13,
                                        block29, block26, block23,

                                        block08, block05, block02,
                                        block18,          block12,
                                        block28, block25, block22,
                                        vertices, triangles, colors32, uv);
                                    faceIndex++;
                                }

                                if (block14 == 0)
                                {
                                    SetFace(new Vector3(xScale, yScaleN, zScaleP),
                                        new Vector3(xScale, yScale, zScaleP),
                                        new Vector3(xScale, yScale, zScale),
                                        new Vector3(xScale, yScaleN, zScale),
                                        faceIndex, color, 0.25f,
                                        block01, block04, block07,
                                        block11,          block17,
                                        block21, block24, block27,

                                        block02, block05, block08,
                                        block12,          block18,
                                        block22, block25, block28,
                                        vertices, triangles, colors32, uv);
                                    faceIndex++;
                                }

                                if (block25 == 0)
                                {
                                    SetFace(new Vector3(xScale, yScale, zScaleP),
                                        new Vector3(xScaleP, yScale, zScaleP),
                                        new Vector3(xScaleP, yScale, zScale),
                                        new Vector3(xScale, yScale, zScale),
                                        faceIndex, color, 0.05f,
                                        block21, block24, block27,
                                        block22,          block28,
                                        block23, block26, block29,

                                        block11, block14, block17,
                                        block12,          block18,
                                        block13, block16, block19,
                                        vertices, triangles, colors32, uv);
                                    faceIndex++;
                                }

                                if (block05 == 0)
                                {
                                    SetFace(new Vector3(xScale, yScaleN, zScale),
                                        new Vector3(xScaleP, yScaleN, zScale),
                                        new Vector3(xScaleP, yScaleN, zScaleP),
                                        new Vector3(xScale, yScaleN, zScaleP),
                                        faceIndex, color, 0.45f,
                                        block07, block04, block01,
                                        block08,          block02,
                                        block09, block06, block03,

                                        block17, block14, block11,
                                        block18,          block12,
                                        block19, block16, block13,
                                        vertices, triangles, colors32, uv);
                                    faceIndex++;
                                }

                                if (block12 == 0)
                                {
                                    SetFace(new Vector3(xScaleP, yScaleN, zScaleP),
                                        new Vector3(xScaleP, yScale, zScaleP),
                                        new Vector3(xScale, yScale, zScaleP),
                                        new Vector3(xScale, yScaleN, zScaleP),
                                        faceIndex, color, 0.30f,
                                        block03, block02, block01,
                                        block13,          block11,
                                        block23, block22, block21,

                                        block06, block05, block04,
                                        block16,          block14,
                                        block26, block25, block24,
                                        vertices, triangles, colors32, uv);
                                    faceIndex++;
                                }

                                if (block18 == 0)
                                {
                                    SetFace(new Vector3(xScale, yScaleN, zScale),
                                        new Vector3(xScale, yScale, zScale),
                                        new Vector3(xScaleP, yScale, zScale),
                                        new Vector3(xScaleP, yScaleN, zScale),
                                        faceIndex, color, 0.30f,
                                        block07, block08, block09,
                                        block17,          block19,
                                        block27, block28, block29,

                                        block04, block05, block06,
                                        block14,          block16,
                                        block24, block25, block26,
                                        vertices, triangles, colors32, uv);
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
                    uv = uv
                };
            }

            return new Mesh();
        }

        private static void SetFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int index, uint inputColor, float light,
            uint ao1, uint ao2, uint ao3, uint ao4, uint ao6, uint ao7, uint ao8, uint ao9,
            uint le1, uint le2, uint le3, uint le4, uint le6, uint le7, uint le8, uint le9,
            Vector3[] vertices, int[] triangles, Color32[] colors32, Vector2[] uv)
        {
            bool flip = false;
            Color32 color = UColor.UintToColor32(inputColor);

            byte voxelDamage = UColor.GetA(inputColor);
            byte hp = UColor.GetA(inputColor);
            if (hp < 100)
            color = CalculateDamage(color.r, color.g, color.a, hp, voxelDamage);
            color = new Color32((byte)(color.r - color.r * light), (byte)(color.g - color.g * light), (byte)(color.b - color.b * light), 255);

            Color32 color0 = GetLight(color, le2, le1, le4, ao2, ao1, ao4, 0, ref flip);
            Color32 color1 = GetLight(color, le4, le7, le8, ao4, ao7, ao8, 1, ref flip);
            Color32 color2 = GetLight(color, le8, le9, le6, ao8, ao9, ao6, 2, ref flip);
            Color32 color3 = GetLight(color, le6, le3, le2, ao6, ao3, ao2, 3, ref flip);

            float textureUnit = 0.1f;
            //byte 
            Vector3 texture = CalculateTexture(le1, le4, le7, le2, le8, le3, le6, le9);
            Vector3 tut = new Vector3(textureUnit * texture.x, textureUnit * texture.y);

            index         *= 4;
            int idx0       = index;
            int idx1       = index + 1;
            int idx2       = index + 2;
            int idx3       = index + 3;
            vertices[idx0] = v1;
            vertices[idx1] = v2;
            vertices[idx2] = v3;
            vertices[idx3] = v4;
            colors32[idx0] = GetAmbient(color0, ao2, ao1, ao4, 0, ref flip);
            colors32[idx1] = GetAmbient(color1, ao4, ao7, ao8, 1, ref flip);
            colors32[idx2] = GetAmbient(color2, ao8, ao9, ao6, 2, ref flip);
            colors32[idx3] = GetAmbient(color3, ao6, ao3, ao2, 3, ref flip);
            uv[idx0]       = new Vector2(tut.x + textureUnit, tut.y);
            uv[idx1]       = new Vector2(tut.x + textureUnit, tut.y + textureUnit);
            uv[idx2]       = new Vector2(tut.x, tut.y + textureUnit);
            uv[idx3]       = new Vector2(tut.x, tut.y);

            int face = index / 4;
            index = face * 6;
            face *= 4;
            if (!flip)
            {
                triangles[index]     = face;
                triangles[index + 1] = face + 1;
                triangles[index + 2] = face + 2;
                triangles[index + 3] = face;
                triangles[index + 4] = face + 2;
                triangles[index + 5] = face + 3;
            }
            else
            {
                triangles[index]     = face + 3;
                triangles[index + 1] = face;
                triangles[index + 2] = face + 1;
                triangles[index + 3] = face + 3;
                triangles[index + 4] = face + 1;
                triangles[index + 5] = face + 2;
            }
        }

        private static uint Data(int x, int y, int z, int dx, int dy, int dz, int vx, int vy, int vz, Volume volume)
        {
            int cxpx = dx + x;
            int cypy = dy + y;
            int czpz = dz + z;

            if (cxpx >= vx || cxpx < 0 || cypy >= vy || cypy < 0 || czpz >= vz || czpz < 0)
            {
                if (modelEditor)
                    return 0;
                return 1;
            }
            uint voxel = volume.data[cxpx, cypy, czpz];
            //if (UColor.GetA(voxel) > 200) return 0;
            return voxel;
        }
        private static Color32 GetLight(Color32 color, uint b1, uint b2, uint b3, uint b11, uint b12, uint b13, byte vertex, ref bool flip)
        {
            if (b1 == 0 && b3 == 0 && b11 == 0 && b13 == 0)
            {
                if (vertex == 0 || vertex == 2) flip = true;
                return CalculateLight(color.r, color.g, color.b, edgeLighting[2]);
            }
            if ((b1 == 0 && b11 == 0 && b2 == 0 && b12 == 0) || (b2 == 0 && b12 == 0 && b3 == 0 && b13 == 0))
            {
                return CalculateLight(color.r, color.g, color.b, edgeLighting[1]);
            }
            if ((b1 == 0 && b11 == 0) || (b3 == 0 && b13 == 0))
            {
                return CalculateLight(color.r, color.g, color.b, edgeLighting[0]);
            }
            if (b2 == 0 && b12 == 0)
            {
                if (vertex == 0 || vertex == 2) flip = true;
                return CalculateLight(color.r, color.g, color.b, edgeLighting[0]);
            }
            return color;
        }
        private static Color32 GetAmbient(Color32 color, uint b1, uint b2, uint b3, byte vertex, ref bool flip)
        {
            if (b1 != 0 && b3 != 0)
            {
                if (vertex == 0 || vertex == 2) flip = true;
                return CalculateAmbient(color.r, color.g, color.b, ambientOcclusion[2]);
            }
            if ((b1 != 0 && b2 != 0) || (b2 != 0 && b3 != 0))
            {
                return CalculateAmbient(color.r, color.g, color.b, ambientOcclusion[1]);
            }
            if (b1 != 0 || b3 != 0)
            {
                return CalculateAmbient(color.r, color.g, color.b, ambientOcclusion[0]);
            }
            if (b2 != 0)
            {
                if (vertex == 0 || vertex == 2) flip = true;
                return CalculateAmbient(color.r, color.g, color.b, ambientOcclusion[0]);
            }
            return color;
        }
        private static Color32 CalculateLight(byte r, byte g, byte b, float ao)
        {
            float fr = r + r / ao;
            float fg = g + g / ao;
            float fb = b + b / ao;

            if (fr > 255) fr = 255;
            else if (fr < 0) fr = 0;
            if (fg > 255) fg = 255;
            else if (fg < 0) fg = 0;
            if (fb > 255) fb = 255;
            if (fb < 0) fb = 0;

            return new Color32((byte)fr, (byte)fg, (byte)fb, 255);
        }
        private static Color32 CalculateDamage(byte r, byte g, byte b, byte hp, float damage)
        {
            damage = damage / (hp / damage);

            float fr = r - r / damage;
            float fg = g - g / damage;
            float fb = b - b / damage;

            if (fr > 255) fr = 255;
            else if (fr < 0) fr = 0;
            if (fg > 255) fg = 255;
            else if (fg < 0) fg = 0;
            if (fb > 255) fb = 255;
            else if (fb < 0) fb = 0;

            return new Color32((byte)fr, (byte)fg, (byte)fb, 255);
        }
        private static Color32 CalculateAmbient(byte r, byte g, byte b, float ao)
        {
            float fr = r - r / ao;
            float fg = g - g / ao;
            float fb = b - b / ao;

            if (fr > 255) fr = 255;
            else if (fr < 0) fr = 0;
            if (fg > 255) fg = 255;
            else if(fg < 0) fg = 0;
            if (fb > 255) fb = 255;
            else if(fb < 0) fb = 0;

            return new Color32((byte)fr, (byte)fg, (byte)fb, 255);
        }
        private static Vector3 CalculateTexture(uint b0, uint b1, uint b2, uint b3, uint b4, uint b5, uint b6, uint b7)
        {
            Vector2 le00 = new Vector2(1, 1);
            Vector2 le10 = new Vector2(3, 1);
            Vector2 le20 = new Vector2(5, 1);
            Vector2 le30 = new Vector2(7, 1);
            Vector2 le01 = new Vector2(1, 3);
            Vector2 le11 = new Vector2(3, 3);
            Vector2 le21 = new Vector2(5, 3);
            Vector2 le31 = new Vector2(7, 3);
            Vector2 le02 = new Vector2(1, 5);
            Vector2 le12 = new Vector2(3, 5);
            Vector2 le22 = new Vector2(5, 5);
            Vector2 le32 = new Vector2(7, 5);
            Vector2 le03 = new Vector2(1, 7);
            Vector2 le13 = new Vector2(3, 7);
            Vector2 le23 = new Vector2(5, 7);
            Vector2 le33 = new Vector2(7, 7);

            byte le = 0;
            if (b0 != 0) le += 1;
            if (b1 != 0) le += 2;
            if (b2 != 0) le += 4;
            if (b3 != 0) le += 8;
            if (b4 != 0) le += 16;
            if (b5 != 0) le += 32;
            if (b6 != 0) le += 64;
            if (b7 != 0) le += 128;

            return new Vector3(le12.x, le12.y, 0);
        }
    }
}