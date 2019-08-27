using VoxCake;
using UnityEngine;
using RenderSettings = VoxCake.RenderSettings;

public static class Renderer
{
    public static void RenderVolume(Volume volume)
    {
        int cs = Chunk.size;
        float csph = cs + 0.5f;
        Camera camera = Camera.main;
        int[] view = GetViewChunks(camera, volume.wdc, volume.hdc, volume.ddc, cs);
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        int vxmin = view[0];
        int vxmax = view[1];
        int vymin = view[2];
        int vymax = view[3];
        int vzmin = view[4];
        int vzmax = view[5];
        for (int x = vxmin; x < vxmax; x++)
        {
            for (int y = vymin; y < vymax; y++)
            {
                for (int z = vzmin; z < vzmax; z++)
                {
                    Chunk chunk = volume.chunks[x, y, z];
                    if (ChunkInFrustum(chunk, planes, csph))
                    {
                        Graphics.DrawMesh(
                            chunk.mesh,
                            new Vector3(
                                x * cs,
                                y * cs,
                                z * cs),
                            Quaternion.identity,
                            MaterialManager.block,
                            0);
                    }
                }
            }
        }
    }

    private static int[] GetViewChunks(Camera camera, int wdc, int hdc, int ddc, int chunkSize)
    {
        Vector3 cp = camera.transform.position;
        int camX = Mathf.RoundToInt(cp.x / chunkSize);
        int camY = Mathf.RoundToInt(cp.y / chunkSize);
        int camZ = Mathf.RoundToInt(cp.z / chunkSize);

        int vd = RenderSettings.viewDistance;
        int cxMin = camX - vd;
        int cxMax = camX + vd;
        int cyMin = camY - vd;
        int cyMax = camY + vd;
        int czMin = camZ - vd;
        int czMax = camZ + vd;

        if (cxMin < 0) cxMin = 0;
        if (cxMax > wdc) cxMax = wdc;
        if (cyMin < 0) cyMin = 0;
        if (cyMax > hdc) cyMax = hdc;
        if (czMin < 0) czMin = 0;
        if (czMax > ddc) czMax = ddc;

        return new int[6] { cxMin, cxMax, cyMin, cyMax, czMin, czMax };
    }
    private static bool ChunkInFrustum(Chunk chunk, Plane[] planes, float chunkSize)
    {
        if (!chunk.isEmpty && RenderSettings.useFrustumCulling)
        {
            float minx = chunk.x * chunkSize;
            float miny = chunk.y * chunkSize;
            float minz = chunk.z * chunkSize;
            float maxx = minx + chunkSize;
            float maxy = miny + chunkSize;
            float maxz = minz + chunkSize;

            for (int i = 0; i < planes.Length; i++)
            {
                float pmminx = planes[i].normal.x * minx;
                float pmmaxx = planes[i].normal.x * maxx;
                float pmminy = planes[i].normal.y * miny;
                float pmmaxy = planes[i].normal.y * maxy;
                float pmminz = planes[i].normal.z * minz;
                float pmmaxz = planes[i].normal.z * maxz;
                float d = planes[i].distance;

                if (pmminx + pmminy + pmminz + d > 0)
                    continue;
                if (pmmaxx + pmminy + pmminz + d > 0)
                    continue;
                if (pmminx + pmmaxy + pmminz + d > 0)
                    continue;
                if (pmmaxx + pmmaxy + pmminz + d > 0)
                    continue;
                if (pmminx + pmminy + pmmaxz + d > 0)
                    continue;
                if (pmmaxx + pmminy + pmmaxz + d > 0)
                    continue;
                if (pmminx + pmmaxy + pmmaxz + d > 0)
                    continue;
                if (pmmaxx + pmmaxy + pmmaxz + d > 0)
                    continue;
                return false;
            }
            return true;
        }
        return true;
    }
}
