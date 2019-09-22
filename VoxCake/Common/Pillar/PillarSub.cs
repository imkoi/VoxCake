using System.Collections.Generic;
using UnityEngine;

namespace VoxCake.Common
{
    public struct PillarSub
    {
        public int x;
        public int y;
        public int z;
        public Volume volume;
        public List<Voxel> stack;

        public PillarSub(int x, int y, int z, Volume volume)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.volume = volume;
            stack = new List<Voxel>();
        }

        public void Calculate()
        {
            stack.Add(new Voxel {x = x, y = y, z = z, value = volume.GetData(x, y, z), processed = true});
            int count = 1;
            Vector3Int point = new Vector3Int(x, y, z);
            while (true)
            {
                if (y <= 0) return;

                int i = y - 1;
                uint color = volume.GetData(x, i, z);
                if (color != 0 && !Contains(x,i,z, count))
                {
                    stack.Add(new Voxel {x = x, y = i, z = z, value = color, processed = false});
                    count++;
                }
                i = y + 1;
                color = volume.GetData(x, i, z);
                if (color != 0 && !Contains(x,i,z, count))
                {
                    stack.Add(new Voxel {x = x, y = i, z = z, value = color, processed = false});
                    count++;
                }

                i = x + 1;
                color = volume.GetData(i, y, z);
                if (color != 0 && !Contains(i,y,z, count))
                {
                    stack.Add(new Voxel {x = i, y = y, z = z, value = color, processed = false});
                    count++;
                }
                i = x - 1;
                color = volume.GetData(i, y, z);
                if (color != 0 && !Contains(i,y,z, count))
                {
                    stack.Add(new Voxel {x = i, y = y, z = z, value = color, processed = false});
                    count++;
                }

                i = z + 1;
                color = volume.GetData(x, y, i);
                if (color != 0 && !Contains(x,y,i, count))
                {
                    stack.Add(new Voxel {x = x, y = y, z = i, value = color, processed = false});
                    count++;
                }
                i = z - 1;
                color = volume.GetData(x, y, i);
                if (color != 0 && !Contains(x,y,i, count))
                {
                    stack.Add(new Voxel {x = x, y = y, z = i, value = color, processed = false});
                    count++;
                }

                int level = -1;
                int levelBest = 0;
                for (int k = 0; k < count; k++)
                {
                    Voxel v = stack[k];
                    if (!v.processed)
                    {
                        if (v.y == y - 1 && v.x == x && v.z == z)
                        {
                            level = 4;
                            levelBest = k;
                            break;
                        }

                        if (v.y < y && level < 3)
                        {
                            level = 3;
                            levelBest = k;
                            continue;
                        }

                        if (v.y == y && level < 2)
                        {
                            level = 2;
                            levelBest = k;
                            continue;
                        }

                        if (level < 1)
                        {
                            level = 1;
                            levelBest = k;
                            continue;
                        }
                    }
                }

                if (level < 0)
                    break;
                Voxel vox = stack[levelBest];
                x = vox.x;
                y = vox.y;
                z = vox.z;
                stack[levelBest] = new Voxel(x, y, z, vox.value, true);
            }

            if (count == 0)
                return;
            for (int i = 0; i < count; i++)
            {
                Voxel v = stack[i];
                volume.SetData(v.x, v.y, v.z, 0);
            }

            PillarManager.AddPillar(new Pillar(new Vector3(x, y, z), point, stack, volume));
        }

        private bool Contains(int x, int y, int z, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Voxel v = stack[i];
                if (v.x == x && v.y == y && v.z == z)
                    return true;
            }

            return false;
        }
    }
}