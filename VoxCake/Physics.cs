using UnityEngine;
using VoxCake.Common;

namespace VoxCake
{
    public class Physics
    {
        public static void Check(int x, int y, int z, Volume volume)
        {
            int p = x + 1;
            int n = x - 1;
            if (GetData(p, y, z, volume) != 0) PillarManager.AddSub(new PillarSub(p, y, z, volume));
            if (GetData(n, y, z, volume) != 0) PillarManager.AddSub(new PillarSub(n, y, z, volume));

            p = z + 1;
            n = z - 1;
            if (GetData(x, y, p, volume) != 0) PillarManager.AddSub(new PillarSub(x, y, p, volume));
            if (GetData(x, y, n, volume) != 0) PillarManager.AddSub(new PillarSub(x, y, n, volume));

            p = y + 1;
            n = y - 1;
            if (GetData(x, n, z, volume) != 0) PillarManager.AddSub(new PillarSub(x, n, z, volume));
            if (GetData(x, p, z, volume) != 0) PillarManager.AddSub(new PillarSub(x, p, z, volume));
        }

        private static uint GetData(int x, int y, int z, Volume volume)
        {
            Vector3Int size = volume.size;
            if (x >= size.x || x < 0 || y >= size.y || y < 0 || z >= size.z || z < 0) return 0;
            return volume.data[x, y, z];
        }
    }
}