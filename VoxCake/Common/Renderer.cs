using VoxCake;
using UnityEngine;
using VoxCake.Common;

public static class Renderer
{
    public static void RenderVolume(Volume volume)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        DrawVisable(volume.octree.node, 0, volume, planes);
    }

    private static void DrawVisable(OctreeNode node, int nodeDepth, Volume volume, Plane[] planes)
    {
        Vector3Int min = node.min;
        Vector3Int max = node.max;
        if (!node.IsLeaf() && InFrustum(min, max, planes))
        {
            for (int i = 0; i < node.subNodes.Length; i++)
            {
                OctreeNode subNode = node.subNodes[i];
                DrawVisable(subNode, nodeDepth + 1, volume, planes);
            }
        }
        else
        {
            Vector3Int cp = node.chunkPosition;
            if (cp.x < volume.wdc && cp.y < volume.hdc && cp.z < volume.ddc)
            {
                var chunk = volume.chunks[cp.x, cp.y, cp.z];
                Vector3Int p = node.position;
                if (chunk.mesh != null)
                    Graphics.DrawMesh(
                        chunk.mesh,
                        new Vector3(p.x, p.y, p.z),
                        Quaternion.identity,
                        MaterialManager.block,
                        0);
            }
        }
    }
    private static bool InFrustum(Vector3Int min, Vector3Int max, Plane[] planes)
    {
        int minx = min.x;
        int miny = min.y;
        int minz = min.z;
        int maxx = max.x;
        int maxy = max.y;
        int maxz = max.z;
        for (int i = 0; i < planes.Length; i++)
        {
            float nx = planes[i].normal.x;
            float ny = planes[i].normal.y;
            float nz = planes[i].normal.z;
            float pmminx = nx * minx;
            float pmmaxx = nx * maxx;
            float pmminy = ny * miny;
            float pmmaxy = ny * maxy;
            float pmminz = nz * minz;
            float pmmaxz = nz * maxz;
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
}
