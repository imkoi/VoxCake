using UnityEngine;
using VoxCake.Common.Meshing;
using System.Collections.Generic;

namespace VoxCake.Common
{
    public static class PhysicConstruction
    {
        public static void Instantiate(Vector3 position, Vector3Int hitPoint, List<Voxel> voxels, Volume volume)
        {
            int xMin = voxels[0].x;
            int xMax = voxels[0].x;
            int yMin = voxels[0].y;
            int yMax = voxels[0].y;
            int zMin = voxels[0].z;
            int zMax = voxels[0].z;

            float xp = 0f;
            float yp = 0f;
            float zp = 0f;

            for (int i = 0; i < voxels.Count; i++)
            {
                if (voxels[i].x < xMin) xMin = voxels[i].x;
                else if (voxels[i].x > xMax) xMax = voxels[i].x;

                if (voxels[i].y < yMin) yMin = voxels[i].y;
                else if (voxels[i].y > yMax) yMax = voxels[i].y;

                if (voxels[i].z < zMin) zMin = voxels[i].z;
                else if (voxels[i].z > zMax) zMax = voxels[i].z;

                xp += voxels[i].x;
                yp += voxels[i].y;
                zp += voxels[i].z;
            }
            xp /= voxels.Count;
            yp /= voxels.Count;
            zp /= voxels.Count;
            for (int cx = xMin / Chunk.size; cx < xMax / Chunk.size + 1; cx++)
            {
                for (int cy = yMin / Chunk.size; cy < yMax / Chunk.size + 1; cy++)
                {
                    for (int cz = zMin / Chunk.size; cz < zMax / Chunk.size + 1; cz++)
                    {
                        Chunk.Add(new Chunk((byte)cx, (byte)cy, (byte)cz, volume));
                    }
                }
            }

            int width = xMax - xMin + 1;
            int height = yMax - yMin + 1;
            int depth = zMax - zMin + 1;
            UConstructionMesh.SetParameters(width, height, depth, xMin, yMin, zMin, voxels);

            uint[,,] voxelsToBuild = new uint[width, height, depth];
            foreach (Voxel voxel in voxels)
            {
                voxelsToBuild[voxel.x - xMin, voxel.y - yMin, voxel.z - zMin] = voxel.value;
            }

            int yDistance = yMin;
            Vector3Int raycastOrigin = new Vector3Int();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    for (int v = 0; v < height; v++)
                    {
                        uint voxel = voxelsToBuild[x, v, z];
                        if (voxel != 0)
                        {
                            for (int y = 0; y <= yMin; y++)
                            {
                                uint underVoxel = volume.GetData(x + xMin, yMin - y, z + zMin);
                                if (underVoxel != 0 && underVoxel != 1)
                                {
                                    if (yDistance > (y + v))
                                    {
                                        raycastOrigin = new Vector3Int(x+xMin,yMin+v,z+zMin);
                                        yDistance = y + v;
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }

            

            Mesh mesh = UConstructionMesh.GetGreedy(null);
            GameObject physicConstruction = new GameObject("PhysicConstruction");
            physicConstruction.AddComponent<PillarController>()
                .SetValues(position, hitPoint,
                raycastOrigin, voxels.Count, yDistance,
                new Vector3Int(width, height, depth),
                new Vector3Int(xMin, yMin, zMin),
                new Vector3Int(xMax, yMax, zMax),
                voxelsToBuild, volume);
            physicConstruction.transform.Translate(new Vector3(
                GetValue(xp, --width),
                GetValue(yp, --height),
                GetValue(zp, --depth)));
            physicConstruction.AddComponent<MeshFilter>().mesh = mesh;
            physicConstruction.AddComponent<MeshCollider>().sharedMesh = mesh;
            physicConstruction.AddComponent<MeshRenderer>().material = MaterialManager.pillar;
        }
        private static int GetValue(float value, float dim)
        {
            return (int)(value - (dim / 2) + 0.5f);
        }
    }
}