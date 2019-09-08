using System.Collections.Generic;
using UnityEngine;

namespace VoxCake.Common
{
    public class PillarSub
    {
        public int x;
        public int y;
        public int z;
        public Volume volume;

        public PillarSub(int x, int y, int z, Volume volume)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.volume = volume;
        }

        public void Calculate()
        {
            Vector3Int point = new Vector3Int(x, y, z);
            List<Voxel> stack = new List<Voxel>();
            stack.Add(new Voxel(x, y, z, volume.GetData(x, y, z), true));

            int count = stack.Count;
            while (true)
            {
                if (y <= 0) return;

                int p = y + 1;
                int n = y - 1;
                if (GetData(x, n, z, volume) != 0 && !StackContains(x, n, z, stack))
                    stack.Add(new Voxel(x, n, z, volume.GetData(x, n, z), false));
                if (GetData(x, p, z, volume) != 0 && !StackContains(x, p, z, stack))
                    stack.Add(new Voxel(x, p, z, volume.GetData(x, p, z), false));

                p = x + 1;
                n = x - 1;
                if (GetData(p, y, z, volume) != 0 && !StackContains(p, y, z, stack))
                    stack.Add(new Voxel(p, y, z, volume.GetData(p, y, z), false));
                if (GetData(n, y, z, volume) != 0 && !StackContains(n, y, z, stack))
                    stack.Add(new Voxel(n, y, z, volume.GetData(n, y, z), false));

                p = z + 1;
                n = z - 1;
                if (GetData(x, y, p, volume) != 0 && !StackContains(x, y, p, stack))
                    stack.Add(new Voxel(x, y, p, volume.GetData(x, y, p), false));
                if (GetData(x, y, n, volume) != 0 && !StackContains(x, y, n, stack))
                    stack.Add(new Voxel(x, y, n, volume.GetData(x, y, n), false));

                int level = -1;
                int levelBest = 0;

                count = stack.Count;
                for (int k = 0; k < count; k++)
                {
                    Voxel voxel = stack[k];
                    if (!voxel.processed)
                    {
                        if (voxel.y == y - 1 && voxel.x == x && voxel.z == z)
                        {
                            level = 4;
                            levelBest = k;
                            break;
                        }
                        if (voxel.y < y && level < 3)
                        {
                            level = 3;
                            levelBest = k;
                            continue;
                        }
                        if (voxel.y == y && level < 2)
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
                if (level < 0) break;
                else
                {
                    Voxel voxel = stack[levelBest];
                    x = voxel.x;
                    y = voxel.y;
                    z = voxel.z;
                    uint value = voxel.value;
                    stack[levelBest] = new Voxel(x,y,z,value,true);
                }
            }
            
            if (count == 0) 
                return;
            for (int i = 0; i < count; i++)
            {
                Voxel voxel = stack[i];
                volume.SetData(voxel.x, voxel.y, voxel.z, 0);
            }
            PillarManager.AddPillar(new Pillar(new Vector3(x, y, z), point, stack, volume));
        }
        
        private static bool StackContains(int x, int y, int z, List<Voxel> stack)
        {
            int count = stack.Count;
            for (int i = 0; i < count; i++)
            {
                Voxel voxel = stack[i];
                if (voxel.x == x && voxel.y == y && voxel.z == z) return true;
            }
            return false;
        }
        private static uint GetData(int x, int y, int z, Volume volume)
        {
            Vector3Int size = volume.size;
            if (x >= size.x || x < 0 || y >= size.y || y < 0 || z >= size.z || z < 0)
                return 0;
            return volume.data[x, y, z];
        }
    }
}

