using UnityEngine;
using System.Collections.Generic;

namespace VoxCake.Common
{
    public class Pillar
    {
        private Vector3 position;
        private Vector3Int hitPoint;
        private List<Voxel> voxels;
        private Volume volume;

        public Pillar(Vector3 position, Vector3Int hitPoint, List<Voxel> voxels, Volume volume)
        {
            this.position = position;
            this.hitPoint = hitPoint;
            this.voxels = voxels;
            this.volume = volume;
        }

        public void Instantiate()
        {
            Voxel voxelZero = voxels[0];
            int xMin = voxelZero.x;
            int xMax = voxelZero.x;
            int yMin = voxelZero.y;
            int yMax = voxelZero.y;
            int zMin = voxelZero.z;
            int zMax = voxelZero.z;

            float xp = 0f;
            float yp = 0f;
            float zp = 0f;
            int count = voxels.Count;
            for (int i = 0; i < count; i++)
            {
                Voxel voxel = voxels[i];
                if (voxel.x < xMin) xMin = voxel.x;
                else if (voxel.x > xMax) xMax = voxel.x;

                if (voxel.y < yMin) yMin = voxel.y;
                else if (voxel.y > yMax) yMax = voxel.y;

                if (voxel.z < zMin) zMin = voxel.z;
                else if (voxel.z > zMax) zMax = voxel.z;

                xp += voxel.x;
                yp += voxel.y;
                zp += voxel.z;
            }

            xp /= count;
            yp /= count;
            zp /= count;

            int cs = Chunk.size;
            int xmindcs = xMin / cs;
            int ymindcs = yMin / cs;
            int zmindcs = zMin / cs;
            int xmaxdcs = xMax / cs + 1;
            int ymaxdcs = yMax / cs + 1;
            int zmaxdcs = zMax / cs + 1;
            for (int cx = xmindcs; cx < xmaxdcs; cx++)
            for (int cy = ymindcs; cy < ymaxdcs; cy++)
            for (int cz = zmindcs; cz < zmaxdcs; cz++)
                Chunk.Add(new Chunk((byte) cx, (byte) cy, (byte) cz, volume));

            int width = xMax - xMin + 1;
            int height = yMax - yMin + 1;
            int depth = zMax - zMin + 1;
            ConstructionMesh.SetParameters(width, height, depth, xMin, yMin, zMin, voxels);

            uint[,,] voxelsToBuild = new uint[width, height, depth];
            for (int i = 0; i < count; i++)
            {
                Voxel voxel = voxels[i];
                voxelsToBuild[voxel.x - xMin, voxel.y - yMin, voxel.z - zMin] = voxel.value;
            }

            int yDistance = yMin;
            Vector3Int raycastOrigin = new Vector3Int();
            for (int x = 0; x < width; x++)
            for (int z = 0; z < depth; z++)
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
                                raycastOrigin = new Vector3Int(x + xMin, yMin + v, z + zMin);
                                yDistance = y + v;
                            }

                            break;
                        }
                    }

                    break;
                }
            }

            GameObject physicConstruction = new GameObject("Pillar");
            physicConstruction.tag = "Pillar";
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
            Mesh mesh = ConstructionMesh.GetCulled();
            physicConstruction.AddComponent<MeshFilter>().mesh = mesh;
            physicConstruction.AddComponent<MeshCollider>().sharedMesh = mesh;
            physicConstruction.AddComponent<MeshRenderer>().material = MaterialManager.pillar;
        }

        private static int GetValue(float value, float dim)
        {
            return (int) (value - (dim / 2) + 0.5f);
        }
    }
}