using VoxCake.Common;

namespace VoxCake
{
    public static class Physics
    {
        public static void Check(int x, int y, int z, Volume volume)
        {
            int p = x + 1;
            int n = x - 1;
            if (volume.GetData(p, y, z) != 0) PillarManager.AddSub(new PillarSub(p, y, z, volume));
            if (volume.GetData(n, y, z) != 0) PillarManager.AddSub(new PillarSub(n, y, z, volume));

            p = z + 1;
            n = z - 1;
            if (volume.GetData(x, y, p) != 0) PillarManager.AddSub(new PillarSub(x, y, p, volume));
            if (volume.GetData(x, y, n) != 0) PillarManager.AddSub(new PillarSub(x, y, n, volume));

            p = y + 1;
            n = y - 1;
            if (volume.GetData(x, n, z) != 0) PillarManager.AddSub(new PillarSub(x, n, z, volume));
            if (volume.GetData(x, p, z) != 0) PillarManager.AddSub(new PillarSub(x, p, z, volume));
        }
    }
}