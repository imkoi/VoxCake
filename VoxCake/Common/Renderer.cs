using VoxCake;
using UnityEngine;
using VoxCake.Common;
using RenderSettings = VoxCake.RenderSettings;

public static class Renderer
{
    public static void UseNative(Camera camera, Volume volume)
    {
        int mx = volume.chunks.GetLength(0);
        int my = volume.chunks.GetLength(1);
        int mz = volume.chunks.GetLength(2);
        for (int x = 0; x < mx; x++)
        {
            for (int y = 0; y < my; y++)
            {
                for (int z = 0; z < mz; z++)
                {
                    RenderChunk(volume.chunks[x, y, z]);
                }
            }
        }
    }

    public static void UseFrustumCulling(Camera camera, Volume volume)
    {
        Vector3 cameraPos = camera.transform.position;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        int[] view = GetChunksInDistance(camera, volume.sizeChunks, Chunk.size);

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
                    if (chunk.mesh != null)
                    {
                        Vector3 min = chunk.min;
                        Vector3 max = chunk.max;
                        Vector3 distance = chunk.position - cameraPos;
                        if(distance.magnitude < RenderSettings.viewDistance)
                            if (InFrustum(min, max, planes))
                                RenderChunkUnsafe(chunk);
                    }
                }
            }
        }
    }

    public static void UseOctreeCulling(Camera camera, Volume volume)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        Vector3 cameraPosition = camera.transform.position;
        DrawVisable(volume.octree.node, 0, volume, planes, cameraPosition);
    }

    private static void DrawVisable(OctreeNode node, int nodeDepth, Volume volume, Plane[] planes, Vector3 cameraPos)
    {
        Vector3 min = node.min;
        Vector3 max = node.max;
        Vector3 distance = node.position - cameraPos;
        if (nodeDepth == 0)
        {
            if (true) // distance.magnitude < 1024
            {
                if (node.subNodes != null && InFrustum(min, max, planes))
                    for (int i = 0; i < node.subNodes.Length; i++)
                        DrawVisable(node.subNodes[i], nodeDepth + 1, volume, planes, cameraPos);
                else
                {
                    Vector3Int cp = node.chunkPosition;
                    if (cp.x < volume.wdc && cp.y < volume.hdc && cp.z < volume.ddc)
                    {
                        var chunk = volume.chunks[cp.x, cp.y, cp.z];
                        RenderChunk(chunk);
                    }
                }
            }
        }
        else
        {
            if (true) //distance.magnitude < 1024/nodeDepth
            {
                if (node.subNodes != null && InFrustum(min, max, planes))
                    for (int i = 0; i < node.subNodes.Length; i++)
                        DrawVisable(node.subNodes[i], nodeDepth + 1, volume, planes, cameraPos);
                else
                {
                    Vector3Int cp = node.chunkPosition;
                    if (cp.x < volume.wdc && cp.y < volume.hdc && cp.z < volume.ddc)
                    {
                        var chunk = volume.chunks[cp.x, cp.y, cp.z];
                        RenderChunk(chunk);
                    }
                }
            }
        }
    }

    private static void RenderChunk(Chunk chunk)
    {
        if (chunk.mesh != null)
        {
            Vector3 p = chunk.position;
            Graphics.DrawMesh(
                chunk.mesh,
                new Vector3(p.x, p.y, p.z),
                Quaternion.identity,
                MaterialManager.block,
                0);
        }
    }

    private static void RenderChunkUnsafe(Chunk chunk)
    {
        Vector3 p = chunk.position;
        Graphics.DrawMesh(
            chunk.mesh,
            new Vector3(p.x, p.y, p.z),
            Quaternion.identity,
            MaterialManager.block,
            0);
    }

    private static int[] GetChunksInDistance(Camera camera, Vector3Int whd, int chunkSize)
    {
        Vector3 cp = camera.transform.position;
        int camX = Mathf.RoundToInt(cp.x / chunkSize);
        int camY = Mathf.RoundToInt(cp.y / chunkSize);
        int camZ = Mathf.RoundToInt(cp.z / chunkSize);

        int vd = (int)RenderSettings.viewDistance;
        int cxMin = camX - vd;
        int cxMax = camX + vd;
        int cyMin = camY - vd;
        int cyMax = camY + vd;
        int czMin = camZ - vd;
        int czMax = camZ + vd;

        if (cxMin < 0) cxMin = 0;
        if (cxMax > whd.x) cxMax = whd.x;
        if (cyMin < 0) cyMin = 0;
        if (cyMax > whd.y) cyMax = whd.y;
        if (czMin < 0) czMin = 0;
        if (czMax > whd.z) czMax = whd.z;

        return new int[6] {cxMin, cxMax, cyMin, cyMax, czMin, czMax};
    }

    private static bool InFrustum(Vector3 min, Vector3 max, Plane[] planes)
    {
        for (int i = 0; i < planes.Length; i++)
        {
            float d = planes[i].distance;
            Vector3 normal = planes[i].normal;
            float pmminx = normal.x * min.x;
            float pmmaxx = normal.x * max.x;
            float pmminy = normal.y * min.y;
            float pmmaxy = normal.y * max.y;
            float pmminz = normal.z * min.z;
            float pmmaxz = normal.z * max.z;

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
}