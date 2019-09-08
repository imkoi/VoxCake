using UnityEngine;

namespace VoxCake
{
    public static class Collision
    {
        public static int width = 2;
        public static int height = 2;
        public static int depth = 2;
        public static float playerHeight;
        public static GameObject[,,] colliders;

        public static void Init()
        {
            int h = Mathf.CeilToInt(height + playerHeight);
            colliders = new GameObject[width, h, depth];
            GameObject parent = new GameObject("Colliders");
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        colliders[x, y, z] = new GameObject("Collider[" + x + ", " + y + ", " + z + "]");
                        colliders[x, y, z].AddComponent<BoxCollider>();
                        colliders[x, y, z].transform.parent = parent.transform;
                        colliders[x, y, z].tag = "Volume";
                    }
                }
            }
        }

        public static void Collide(Vector3 position, Volume volume)
        {
            int h = Mathf.CeilToInt(height + playerHeight);
            int cx = Mathf.RoundToInt(position.x);
            int cy = Mathf.RoundToInt(position.y);
            int cz = Mathf.RoundToInt(position.z);
            
//            int cx = Mathf.FloorToInt(position.x);
//            int cy = Mathf.FloorToInt(position.y);
//            int cz = Mathf.FloorToInt(position.z);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        int cxpx = cx + x - 1;
                        int cypy = cy + y - h/2;
                        int czpz = cz + z - 1;
                        colliders[x, y, z].transform.position = new Vector3(cxpx + 0.5f, cypy + 0.5f, czpz + 0.5f);
                        if (volume.GetData(cxpx, cypy, czpz) != 0)
                            colliders[x, y, z].GetComponent<BoxCollider>().enabled = true;
                        else
                            colliders[x, y, z].GetComponent<BoxCollider>().enabled = false;
                    }
                }
            }
        }
    }
}