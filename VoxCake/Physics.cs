using UnityEngine;
using System.Collections.Generic;
using VoxCake.Common;
using System.Collections;

namespace VoxCake
{
    public class Physics
    {
        public static void Check(int x, int y, int z, Volume volume)
        {
            volume.StartCoroutine(ICheck(x, y, z, volume));
        }

        private static IEnumerator ICheck(int x, int y, int z, Volume volume)
        {
            int p = x + 1;
            int n = x - 1;
            if (GetData(p, y, z, volume) != 0) CheckSub(p, y, z, volume);
            yield return null;
            if (GetData(n, y, z, volume) != 0) CheckSub(n, y, z, volume);
            yield return null;

            p = z + 1;
            n = z - 1;
            if (GetData(x, y, p, volume) != 0) CheckSub(x, y, p, volume);
            yield return null;
            if (GetData(x, y, n, volume) != 0) CheckSub(x, y, n, volume);
            yield return null;

            p = y + 1;
            n = y - 1;
            if (GetData(x, n, z, volume) != 0) CheckSub(x, n, z, volume);
            yield return null;
            if (GetData(x, p, z, volume) != 0) CheckSub(x, p, z, volume);
            yield return null;
        }
        private static void CheckSub(int x, int y, int z, Volume volume)
        {
            Vector3Int point = new Vector3Int(x, y, z);
            List<Voxel> stack = new List<Voxel>();
            stack.Add(new Voxel(x, y, z, volume.GetData(x, y, z), true));

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
                int level_best = 0;

                for (int k = 0; k < stack.Count; k++)
                {
                    if (!stack[k].processed)
                    {
                        if (stack[k].y == y - 1 && stack[k].x == x && stack[k].z == z)
                        {
                            level = 4;
                            level_best = k;
                            break;
                        }
                        if (stack[k].y < y && level < 3)
                        {
                            level = 3;
                            level_best = k;
                            continue;
                        }
                        if (stack[k].y == y && level < 2)
                        {
                            level = 2;
                            level_best = k;
                            continue;
                        }
                        if (level < 1)
                        {
                            level = 1;
                            level_best = k;
                            continue;
                        }
                    }
                }
                if (level < 0) break;
                else
                {
                    x = stack[level_best].x;
                    y = stack[level_best].y;
                    z = stack[level_best].z;
                    stack[level_best].processed = true;
                }
            }

            if (stack.Count == 0) return;
            foreach (Voxel voxel in stack)
                volume.SetData(voxel.x, voxel.y, voxel.z, 0);
            PhysicConstruction.Instantiate(new Vector3(x, y, z), point, stack, volume);
        }
        private static uint GetData(int x, int y, int z, Volume volume)
        {
            if (x >= volume.width || x < 0 || y >= volume.height || y < 0 || z >= volume.depth || z < 0) return 0;
            return volume.data[x, y, z];
        }
        private static bool StackContains(int x, int y, int z, List<Voxel> stack)
        {
            foreach (Voxel voxel in stack) if (voxel.x == x && voxel.y == y && voxel.z == z) return true;
            return false;
        }
    }
}