using UnityEngine;

namespace VoxCake.Buffered
{
    public static class SMTED
    {
        public static byte Color = 12;

        private static Vector3Int start;
        private static Vector3Int end;

        public static void Add()
        {
            start = GetVectorOn();
        }

        public static void Add(string tool)
        {
            if (tool == "Block")
            {
                //Buffer.Add(new Block {Data = new Voxel(GetVectorOn(), 0)});
            }
            else if (tool == "Rectangle")
            {
                
                end = GetVectorOn();
                if (start != new Vector3Int(-6, -6, -6) && end != new Vector3Int(-6, -6, -6))
                {
                    //Buffer.Add(new Rectangle {Data = new Voxels(start, end, 0)});
                    start = new Vector3Int(-6, -6, -6);
                    end = new Vector3Int(-6, -6, -6);
                }
            }
            else if (tool == "Line")
            {
                Debug.LogError("Editor.Add(): You can`t use the Line tool without start and end vector! " +
                               "Use Add(\"Line\", startVector, endVector)");
            }
            else Debug.LogError("Editor.Add(): Wrong tool name!");
        }

        public static void Delete()
        {
            start = GetVectorIn();
        }

        public static void Delete(string tool)
        {
            if (tool == "Block")
            {
                //Buffer.Add(new Block {Data = new Voxel(GetVectorIn(), 1)});
            }
            else if (tool == "Rectangle")
            {
                end = GetVectorIn();
                if (start != new Vector3Int(-6, -6, -6) && end != new Vector3Int(-6, -6, -6))
                {
                    //Buffer.Add(new Rectangle {Data = new Voxels(start, end, 1)});
                    start = new Vector3Int(-6, -6, -6);
                    end = new Vector3Int(-6, -6, -6);
                }
            }
            else if (tool == "Line")
            {
                Debug.LogError("Editor.Delete(): You can`t use the Line tool without start and end vector! " +
                               "Use Delete(\"Line\", startVector, endVector)");
            }
            else Debug.LogError("Editor.Delete(): Wrong tool name!");
        }

        public static void Paint()
        {
            start = GetVectorIn();
        }

        public static void Paint(string tool)
        {
            if (tool == "Block")
            {
                //Buffer.Add(new Block {Data = new Voxel(GetVectorIn(), 2)});
            }
            else if (tool == "Rectangle")
            {
                end = GetVectorIn();
                if (start != new Vector3Int(-6, -6, -6) && end != new Vector3Int(-6, -6, -6))
                {
                    //Buffer.Add(new Rectangle {Data = new Voxels(start, end, 2)});
                    start = new Vector3Int(-6, -6, -6);
                    end = new Vector3Int(-6, -6, -6);
                }
            }
            else if (tool == "Line")
            {
                Debug.LogError("Editor.Paint(): You can`t use the Line tool without start and end vector! " +
                               "Use Paint(\"Line\", startVector, endVector)");
            }
            else Debug.LogError("Editor.Paint(): Wrong tool name!");
        }

        public static void GetColor()
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (UnityEngine.Physics.Raycast(ray, out hit))
            {
                if (hit.distance < 128)
                {
                    if (hit.transform.name != "World")
                    {
                        Vector3 position = hit.point;
                        position += hit.normal * -0.5f;

                        int x = Mathf.RoundToInt(position.x);
                        int y = Mathf.RoundToInt(position.y);
                        int z = Mathf.RoundToInt(position.z);

                        //Color = MapData.GetValue(x, y, z);
                    }
                }
            }
        }

        public static byte GetColorT()
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (UnityEngine.Physics.Raycast(ray, out hit))
            {
                if (hit.distance < 128)
                {
                    if (hit.transform.name != "World")
                    {
                        Vector3 position = hit.point;
                        position += hit.normal * -0.5f;

                        int x = Mathf.RoundToInt(position.x);
                        int y = Mathf.RoundToInt(position.y);
                        int z = Mathf.RoundToInt(position.z);

                        //Color = MapData.GetValue(x, y, z);
                    }
                }
            }
            return Color;
        }

        public static Vector3Int GetVectorOn()
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            int x = -6;
            int y = -6;
            int z = -6;

            if (UnityEngine.Physics.Raycast(ray, out hit))
            {
                if (hit.distance < 128)
                {
                    Vector3 position = hit.point;
                    position += hit.normal * 0.5f;

                    x = Mathf.RoundToInt(position.x);
                    y = Mathf.RoundToInt(position.y);
                    z = Mathf.RoundToInt(position.z);
                }
            }

            return new Vector3Int(x, y, z);
        }

        public static Vector3Int GetVectorIn()
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            int x = -6;
            int y = -6;
            int z = -6;

            if (UnityEngine.Physics.Raycast(ray, out hit))
            {
                if (hit.distance < 128)
                {
                    if (hit.transform.name != "World")
                    {
                        Vector3 position = hit.point;
                        position += hit.normal * -0.5f;

                        x = Mathf.RoundToInt(position.x);
                        y = Mathf.RoundToInt(position.y);
                        z = Mathf.RoundToInt(position.z);
                    }
                }
            }

            return new Vector3Int(x, y, z);
        }
    }
}