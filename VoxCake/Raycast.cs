using UnityEngine;

namespace VoxCake
{
    public static class Raycast
    {
        public static bool Hit(byte mode, float maxDistance, Vector3 origin, Vector3 direction, Volume volume)
        {
            float gx0 = origin.x;
            float gy0 = origin.y;
            float gz0 = origin.z;
            float rayX = direction.x;
            float rayY = direction.y;
            float rayZ = direction.z;

            float gx1 = gx0 + rayX * maxDistance;
            float gy1 = gy0 + rayY * maxDistance;
            float gz1 = gz0 + rayZ * maxDistance;

            int gx0idx = Mathf.FloorToInt(gx0);
            int gy0idx = Mathf.FloorToInt(gy0);
            int gz0idx = Mathf.FloorToInt(gz0);
            int gx1idx = Mathf.FloorToInt(gx1);
            int gy1idx = Mathf.FloorToInt(gy1);
            int gz1idx = Mathf.FloorToInt(gz1);

            int sx = gx1idx > gx0idx ? 1 : gx1idx < gx0idx ? -1 : 0;
            int sy = gy1idx > gy0idx ? 1 : gy1idx < gy0idx ? -1 : 0;
            int sz = gz1idx > gz0idx ? 1 : gz1idx < gz0idx ? -1 : 0;

            int gx = gx0idx;
            int gy = gy0idx;
            int gz = gz0idx;

            int gxp = gx0idx + (gx1idx > gx0idx ? 1 : 0);
            int gyp = gy0idx + (gy1idx > gy0idx ? 1 : 0);
            int gzp = gz0idx + (gz1idx > gz0idx ? 1 : 0);

            float vx = gx1 == gx0 ? 1 : gx1 - gx0;
            float vy = gy1 == gy0 ? 1 : gy1 - gy0;
            float vz = gz1 == gz0 ? 1 : gz1 - gz0;

            float vxvy = vx * vy;
            float vxvz = vx * vz;
            float vyvz = vy * vz;

            float errx = (gxp - gx0) * vyvz;
            float erry = (gyp - gy0) * vxvz;
            float errz = (gzp - gz0) * vxvy;

            float derrx = sx * vyvz;
            float derry = sy * vxvz;
            float derrz = sz * vxvy;

            int gxPre = gx, gyPre = gy, gzPre = gz;
            while (true)
            {
                switch (mode)
                {
                    case 0:
                        if (volume.GetData(gx, gy, gz) != 0 && volume.GetData(gxPre, gyPre, gzPre) == 0)
                            return true;
                        break;
                    case 1:
                        if (volume.GetData(gx, gy, gz) != 0)
                            return true;
                        break;
                }
                gxPre = gx;
                gyPre = gy;
                gzPre = gz;

                if (gx == gx1idx && gy == gy1idx && gz == gz1idx) break;

                int xr = (int)Mathf.Abs(errx);
                int yr = (int)Mathf.Abs(erry);
                int zr = (int)Mathf.Abs(errz);

                if (sx != 0 && (sy == 0 || xr < yr) && (sz == 0 || xr < zr))
                {
                    gx += sx;
                    errx += derrx;
                }
                else if (sy != 0 && (sz == 0 || yr < zr))
                {
                    gy += sy;
                    erry += derry;
                }
                else if (sz != 0)
                {
                    gz += sz;
                    errz += derrz;
                }
            }
            return false;
        }

        public static bool Hit(byte mode, float maxDistance, out Vector3Int hitPoint,
            Vector3 origin, Vector3 direction, Volume volume)
        {
            float gx0 = origin.x;
            float gy0 = origin.y;
            float gz0 = origin.z;
            float rayX = direction.x;
            float rayY = direction.y;
            float rayZ = direction.z;

            float gx1 = gx0 + rayX * maxDistance;
            float gy1 = gy0 + rayY * maxDistance;
            float gz1 = gz0 + rayZ * maxDistance;

            int gx0idx = Mathf.FloorToInt(gx0);
            int gy0idx = Mathf.FloorToInt(gy0);
            int gz0idx = Mathf.FloorToInt(gz0);
            int gx1idx = Mathf.FloorToInt(gx1);
            int gy1idx = Mathf.FloorToInt(gy1);
            int gz1idx = Mathf.FloorToInt(gz1);

            int sx = gx1idx > gx0idx ? 1 : gx1idx < gx0idx ? -1 : 0;
            int sy = gy1idx > gy0idx ? 1 : gy1idx < gy0idx ? -1 : 0;
            int sz = gz1idx > gz0idx ? 1 : gz1idx < gz0idx ? -1 : 0;

            int gx = gx0idx;
            int gy = gy0idx;
            int gz = gz0idx;

            int gxp = gx0idx + (gx1idx > gx0idx ? 1 : 0);
            int gyp = gy0idx + (gy1idx > gy0idx ? 1 : 0);
            int gzp = gz0idx + (gz1idx > gz0idx ? 1 : 0);

            float vx = gx1 == gx0 ? 1 : gx1 - gx0;
            float vy = gy1 == gy0 ? 1 : gy1 - gy0;
            float vz = gz1 == gz0 ? 1 : gz1 - gz0;

            float vxvy = vx * vy;
            float vxvz = vx * vz;
            float vyvz = vy * vz;

            float errx = (gxp - gx0) * vyvz;
            float erry = (gyp - gy0) * vxvz;
            float errz = (gzp - gz0) * vxvy;

            float derrx = sx * vyvz;
            float derry = sy * vxvz;
            float derrz = sz * vxvy;

            int gxPre = gx, gyPre = gy, gzPre = gz;
            while (true)
            {
                switch (mode)
                {
                    case 0:
                        if (volume.GetData(gx, gy, gz) != 0 && volume.GetData(gxPre, gyPre, gzPre) == 0)
                        {
                            hitPoint = new Vector3Int(gx, gy, gz);
                            return true;
                        }
                        break;
                    case 1:
                        if (volume.GetData(gx, gy, gz) != 0)
                        {
                            hitPoint = new Vector3Int(gxPre, gyPre, gzPre);
                            return true;
                        }
                        break;
                }
                gxPre = gx;
                gyPre = gy;
                gzPre = gz;

                if (gx == gx1idx && gy == gy1idx && gz == gz1idx) break;

                int xr = (int)Mathf.Abs(errx);
                int yr = (int)Mathf.Abs(erry);
                int zr = (int)Mathf.Abs(errz);

                if (sx != 0 && (sy == 0 || xr < yr) && (sz == 0 || xr < zr))
                {
                    gx += sx;
                    errx += derrx;
                }
                else if (sy != 0 && (sz == 0 || yr < zr))
                {
                    gy += sy;
                    erry += derry;
                }
                else if (sz != 0)
                {
                    gz += sz;
                    errz += derrz;
                }
            }
            hitPoint = new Vector3Int();
            return false;
        }

        public static bool Hit(byte mode, float maxDistance, out Vector3Int point, Camera camera, Volume volume)
        {
            float gx0 = camera.transform.position.x;
            float gy0 = camera.transform.position.y;
            float gz0 = camera.transform.position.z;
            float rayX = camera.transform.forward.x;
            float rayY = camera.transform.forward.y;
            float rayZ = camera.transform.forward.z;

            float gx1 = gx0 + rayX * maxDistance;
            float gy1 = gy0 + rayY * maxDistance;
            float gz1 = gz0 + rayZ * maxDistance;

            int gx0idx = Mathf.FloorToInt(gx0);
            int gy0idx = Mathf.FloorToInt(gy0);
            int gz0idx = Mathf.FloorToInt(gz0);
            int gx1idx = Mathf.FloorToInt(gx1);
            int gy1idx = Mathf.FloorToInt(gy1);
            int gz1idx = Mathf.FloorToInt(gz1);

            int sx = gx1idx > gx0idx ? 1 : gx1idx < gx0idx ? -1 : 0;
            int sy = gy1idx > gy0idx ? 1 : gy1idx < gy0idx ? -1 : 0;
            int sz = gz1idx > gz0idx ? 1 : gz1idx < gz0idx ? -1 : 0;

            int gx = gx0idx;
            int gy = gy0idx;
            int gz = gz0idx;

            int gxp = gx0idx + (gx1idx > gx0idx ? 1 : 0);
            int gyp = gy0idx + (gy1idx > gy0idx ? 1 : 0);
            int gzp = gz0idx + (gz1idx > gz0idx ? 1 : 0);

            float vx = gx1 == gx0 ? 1 : gx1 - gx0;
            float vy = gy1 == gy0 ? 1 : gy1 - gy0;
            float vz = gz1 == gz0 ? 1 : gz1 - gz0;

            float vxvy = vx * vy;
            float vxvz = vx * vz;
            float vyvz = vy * vz;

            float errx = (gxp - gx0) * vyvz;
            float erry = (gyp - gy0) * vxvz;
            float errz = (gzp - gz0) * vxvy;

            float derrx = sx * vyvz;
            float derry = sy * vxvz;
            float derrz = sz * vxvy;

            int gxPre = gx, gyPre = gy, gzPre = gz;
            while (true && CameraInVolumeBounds(camera, volume))
            {
                switch (mode)
                {
                    case 0:
                        if (volume.GetData(gx, gy, gz) != 0 && volume.GetData(gxPre, gyPre, gzPre) == 0)
                        {
                            point = new Vector3Int(gx, gy, gz);
                            return true;
                        }
                        break;
                    case 1:
                        if (volume.GetData(gx, gy, gz) != 0)
                        {
                            point = new Vector3Int(gxPre, gyPre, gzPre);
                            return true;
                        }
                        break;
                }
                gxPre = gx;
                gyPre = gy;
                gzPre = gz;

                if (gx == gx1idx && gy == gy1idx && gz == gz1idx) break;

                int xr = (int)Mathf.Abs(errx);
                int yr = (int)Mathf.Abs(erry);
                int zr = (int)Mathf.Abs(errz);

                if (sx != 0 && (sy == 0 || xr < yr) && (sz == 0 || xr < zr))
                {
                    gx += sx;
                    errx += derrx;
                }
                else if (sy != 0 && (sz == 0 || yr < zr))
                {
                    gy += sy;
                    erry += derry;
                }
                else if (sz != 0)
                {
                    gz += sz;
                    errz += derrz;
                }
            }
            point = new Vector3Int();
            return false;
        }

        private static bool CameraInVolumeBounds(Camera camera, Volume volume)
        {
            if (camera.transform.position.x < 0 || camera.transform.position.x >= volume.width ||
                camera.transform.position.y < 0 || camera.transform.position.y >= volume.height ||
                camera.transform.position.z < 0 || camera.transform.position.z >= volume.depth)
                return false;
            return true;
        }
    }
}

