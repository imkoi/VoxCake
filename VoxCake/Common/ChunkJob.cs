using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;

[BurstCompile]
public struct ChunkJob : IJob
{
    [ReadOnly] public NativeArray<uint> data;
    public int vx;
    public int vy;
    public int vz;

    public int cx;
    public int cy;
    public int cz;
    public int cs;
    [ReadOnly] private int dx;
    [ReadOnly] private int dy;
    [ReadOnly] private int dz;
    [ReadOnly] private bool flip;

    public NativeArray<Vector3> vertices;
    public NativeArray<int> triangles;
    public NativeArray<Color32> colors;
    public NativeArray<Vector2> uv;

    public void Execute()
    {
        dx = cx * cs;
        dy = cy * cs;
        dz = cz * cs;

        int faceCount = 0;
        for (int x = 0; x < cs; x++)
        {
            for (int y = 0; y < cs; y++)
            {
                for (int z = 0; z < cs; z++)
                {
                    uint voxel = Data(x, y, z);
                    if (voxel != 0)
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
            vertices = new NativeArray<Vector3>(faceCount * 4, Allocator.Temp);
            triangles = new NativeArray<int>(faceCount * 6, Allocator.Temp);
            colors = new NativeArray<Color32>(faceCount * 4, Allocator.Temp);
            uv = new NativeArray<Vector2>(faceCount * 4, Allocator.Temp);

            int faceIndex = 0;
            for (int x = 0; x < cs; x++)
            {
                for (int y = 0; y < cs; y++)
                {
                    for (int z = 0; z < cs; z++)
                    {
                        uint color = Data(x, y, z);
                        if (color != 0)
                        {
                            int xp = x + 1;
                            int xn = x - 1;
                            int yp = y + 1;
                            int yn = y - 1;
                            int zp = z + 1;
                            int zn = z - 1;

                            uint block01 = Data(xn, yn, zp);
                            uint block02 = Data(x, yn, zp);
                            uint block03 = Data(xp, yn, zp);
                            uint block04 = Data(xn, yn, z);
                            uint block05 = Data(x, yn, z);
                            uint block06 = Data(xp, yn, z);
                            uint block07 = Data(xn, yn, zn);
                            uint block08 = Data(x, yn, zn);
                            uint block09 = Data(xp, yn, zn);

                            uint block11 = Data(xn, y, zp);
                            uint block12 = Data(x, y, zp);
                            uint block13 = Data(xp, y, zp);
                            uint block14 = Data(xn, y, z);
                            uint block16 = Data(xp, y, z);
                            uint block17 = Data(xn, y, zn);
                            uint block18 = Data(x, y, zn);
                            uint block19 = Data(xp, y, zn);

                            uint block21 = Data(xn, yp, zp);
                            uint block22 = Data(x, yp, zp);
                            uint block23 = Data(xp, yp, zp);
                            uint block24 = Data(xn, yp, z);
                            uint block25 = Data(x, yp, z);
                            uint block26 = Data(xp, yp, z);
                            uint block27 = Data(xn, yp, zn);
                            uint block28 = Data(x, yp, zn);
                            uint block29 = Data(xp, yp, zn);

                            y++;
                            float xScaleP = x + 1;
                            float yScaleN = y - 1;
                            float zScaleP = z + 1;

                            if (block16 == 0)
                            {
                                SetFace(new Vector3(xScaleP, yScaleN, z),
                                    new Vector3(xScaleP, y, z),
                                    new Vector3(xScaleP, y, zScaleP),
                                    new Vector3(xScaleP, yScaleN, zScaleP),
                                    faceIndex, color, 0.25f,
                                    block09, block06, block03,
                                    block19, block13,
                                    block29, block26, block23,
                                    block08, block05, block02,
                                    block18, block12,
                                    block28, block25, block22);
                                faceIndex++;
                            }

                            if (block14 == 0)
                            {
                                SetFace(new Vector3(x, yScaleN, zScaleP),
                                    new Vector3(x, y, zScaleP),
                                    new Vector3(x, y, z),
                                    new Vector3(x, yScaleN, z),
                                    faceIndex, color, 0.25f,
                                    block01, block04, block07,
                                    block11, block17,
                                    block21, block24, block27,
                                    block02, block05, block08,
                                    block12, block18,
                                    block22, block25, block28);
                                faceIndex++;
                            }

                            if (block25 == 0)
                            {
                                SetFace(new Vector3(x, y, zScaleP),
                                    new Vector3(xScaleP, y, zScaleP),
                                    new Vector3(xScaleP, y, z),
                                    new Vector3(x, y, z),
                                    faceIndex, color, 0.05f,
                                    block21, block24, block27,
                                    block22, block28,
                                    block23, block26, block29,
                                    block11, block14, block17,
                                    block12, block18,
                                    block13, block16, block19);
                                faceIndex++;
                            }

                            if (block05 == 0)
                            {
                                SetFace(new Vector3(x, yScaleN, z),
                                    new Vector3(xScaleP, yScaleN, z),
                                    new Vector3(xScaleP, yScaleN, zScaleP),
                                    new Vector3(x, yScaleN, zScaleP),
                                    faceIndex, color, 0.45f,
                                    block07, block04, block01,
                                    block08, block02,
                                    block09, block06, block03,
                                    block17, block14, block11,
                                    block18, block12,
                                    block19, block16, block13);
                                faceIndex++;
                            }

                            if (block12 == 0)
                            {
                                SetFace(new Vector3(xScaleP, yScaleN, zScaleP),
                                    new Vector3(xScaleP, y, zScaleP),
                                    new Vector3(x, y, zScaleP),
                                    new Vector3(x, yScaleN, zScaleP),
                                    faceIndex, color, 0.30f,
                                    block03, block02, block01,
                                    block13, block11,
                                    block23, block22, block21,
                                    block06, block05, block04,
                                    block16, block14,
                                    block26, block25, block24);
                                faceIndex++;
                            }

                            if (block18 == 0)
                            {
                                SetFace(new Vector3(x, yScaleN, z),
                                    new Vector3(x, y, z),
                                    new Vector3(xScaleP, y, z),
                                    new Vector3(xScaleP, yScaleN, z),
                                    faceIndex, color, 0.30f,
                                    block07, block08, block09,
                                    block17, block19,
                                    block27, block28, block29,
                                    block04, block05, block06,
                                    block14, block16,
                                    block24, block25, block26);
                                faceIndex++;
                            }
                        }
                    }
                }
            }
        }
    }

    private void SetFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int index, uint inputColor, float light,
        uint ao1, uint ao2, uint ao3, uint ao4, uint ao6, uint ao7, uint ao8, uint ao9,
        uint le1, uint le2, uint le3, uint le4, uint le6, uint le7, uint le8, uint le9)
    {
        flip = false;
        Color32 color = UintToColor32(inputColor);

        byte voxelDamage = (byte)(inputColor >> 24);
        byte hp = (byte)(inputColor >> 24);
        if (hp < 100)
            color = CalculateDamage(color.r, color.g, color.a, hp, voxelDamage);
        color = new Color32((byte) (color.r - color.r * light), (byte) (color.g - color.g * light),
            (byte) (color.b - color.b * light), 255);

        Color32 color0 = GetLight(color, le2, le1, le4, ao2, ao1, ao4, 0);
        Color32 color1 = GetLight(color, le4, le7, le8, ao4, ao7, ao8, 1);
        Color32 color2 = GetLight(color, le8, le9, le6, ao8, ao9, ao6, 2);
        Color32 color3 = GetLight(color, le6, le3, le2, ao6, ao3, ao2, 3);

        //float textureUnit = 0.1f;
        //Vector3 texture = CalculateTexture(le1, le4, le7, le2, le8, le3, le6, le9);
        //Vector3 tut = new Vector3(textureUnit * texture.x, textureUnit * texture.y);

        index *= 4;
        int idx0 = index;
        int idx1 = index + 1;
        int idx2 = index + 2;
        int idx3 = index + 3;
        vertices[idx0] = v1;
        vertices[idx1] = v2;
        vertices[idx2] = v3;
        vertices[idx3] = v4;
        colors[idx0] = GetAmbient(color0, ao2, ao1, ao4, 0);
        colors[idx1] = GetAmbient(color1, ao4, ao7, ao8, 1);
        colors[idx2] = GetAmbient(color2, ao8, ao9, ao6, 2);
        colors[idx3] = GetAmbient(color3, ao6, ao3, ao2, 3);
        uv[idx0] = new Vector2();
        uv[idx1] = new Vector2();
        uv[idx2] = new Vector2();
        uv[idx3] = new Vector2();
        //uv[idx0] = new Vector2(tut.x + textureUnit, tut.y);
        //uv[idx1] = new Vector2(tut.x + textureUnit, tut.y + textureUnit);
        //uv[idx2] = new Vector2(tut.x, tut.y + textureUnit);
        //uv[idx3] = new Vector2(tut.x, tut.y);

        int face = index / 4;
        index = face * 6;
        face *= 4;
        if (!flip)
        {
            triangles[index] = face;
            triangles[index + 1] = face + 1;
            triangles[index + 2] = face + 2;
            triangles[index + 3] = face;
            triangles[index + 4] = face + 2;
            triangles[index + 5] = face + 3;
        }
        else
        {
            triangles[index] = face + 3;
            triangles[index + 1] = face;
            triangles[index + 2] = face + 1;
            triangles[index + 3] = face + 3;
            triangles[index + 4] = face + 1;
            triangles[index + 5] = face + 2;
        }
    }
    
    private Color32 GetLight(Color32 color, uint b1, uint b2, uint b3, uint b11, uint b12, uint b13, byte vertex)
        {
            if (b1 == 0 && b3 == 0 && b11 == 0 && b13 == 0)
            {
                if (vertex == 0 || vertex == 2) flip = true;
                return CalculateLight(color.r, color.g, color.b, 12f);
            }
            if ((b1 == 0 && b11 == 0 && b2 == 0 && b12 == 0) || (b2 == 0 && b12 == 0 && b3 == 0 && b13 == 0))
            {
                return CalculateLight(color.r, color.g, color.b, 10f);
            }
            if ((b1 == 0 && b11 == 0) || (b3 == 0 && b13 == 0))
            {
                return CalculateLight(color.r, color.g, color.b, 12f);
            }
            if (b2 == 0 && b12 == 0)
            {
                if (vertex == 0 || vertex == 2) flip = true;
                return CalculateLight(color.r, color.g, color.b, 12f);
            }
            return color;
        }
        private Color32 GetAmbient(Color32 color, uint b1, uint b2, uint b3, byte vertex)
        {
            if (b1 != 0 && b3 != 0)
            {
                if (vertex == 0 || vertex == 2) flip = true;
                return CalculateAmbient(color.r, color.g, color.b, 3f);
            }
            if ((b1 != 0 && b2 != 0) || (b2 != 0 && b3 != 0))
            {
                return CalculateAmbient(color.r, color.g, color.b, 5f);
            }
            if (b1 != 0 || b3 != 0)
            {
                return CalculateAmbient(color.r, color.g, color.b, 6f);
            }
            if (b2 != 0)
            {
                if (vertex == 0 || vertex == 2) flip = true;
                return CalculateAmbient(color.r, color.g, color.b, 6f);
            }
            return color;
        }
        private Color32 CalculateLight(byte r, byte g, byte b, float ao)
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
        private Color32 CalculateDamage(byte r, byte g, byte b, byte hp, float damage)
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
        private Color32 CalculateAmbient(byte r, byte g, byte b, float ao)
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
    
    private Color32 UintToColor32(uint value)
    {
        byte r = (byte)(value >> 16);
        byte g = (byte)(value >> 8);
        byte b = (byte)(value >> 0);
        return new Color32(r, g, b, 255);
    }
    private uint Data(int x, int y, int z)
    {
        int cxpx = dx + x;
        int cypy = dy + y;
        int czpz = dz + z;
        if (cxpx >= vx || cxpx < 0 || cypy >= vy || cypy < 0 || czpz >= vz || czpz < 0)
            return 0;
        uint voxel = data[(cxpx * vy + cypy) * vz + czpz];
        return voxel;
    }
}