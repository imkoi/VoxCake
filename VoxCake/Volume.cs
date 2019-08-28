using UnityEngine;
using VoxCake.Common;
using System.Collections.Generic;


namespace VoxCake
{
    public class Volume : MonoBehaviour
    {
        public int width;
        public int height;
        public int depth;
        [HideInInspector] public uint[,,] data;
        [HideInInspector] public int wdc;
        [HideInInspector] public int hdc;
        [HideInInspector] public int ddc;
        [HideInInspector] public Chunk[,,] chunks;
        public Octree octree;

        private void Awake()
        {
            if (width < Chunk.size || height < Chunk.size || depth < Chunk.size)
            {
                Chunk.size = (byte)width;
                Chunk.size = (byte)width;
                Chunk.size = (byte)depth;
            }

            wdc = Mathf.CeilToInt((float)width / Chunk.size);
            hdc = Mathf.CeilToInt((float)height / Chunk.size);
            ddc = Mathf.CeilToInt((float)depth / Chunk.size);

            data = new uint[width, height, depth];
            chunks = new Chunk[wdc, hdc, ddc];
            for (byte x = 0; x < wdc; x++)
            {
                for (byte y = 0; y < hdc; y++)
                {
                    for (byte z = 0; z < ddc; z++)
                    {
                        chunks[x, y, z] = new Chunk(x, y, z, this);
                    }
                }
            }

            int md = GetMaxDimension();
            int size = md / 2;
            octree = new Octree(new Vector3Int(size, size, size), md, GetDepth(md), this);
        }
        private void LateUpdate()
        {
            Chunk.UpdateStack();
        }

        public void Render()
        {
            Renderer.RenderVolume(this);
        }
        public void Collide(Vector3 position)
        {
            Collision.Collide(position, this);
        }
        public void SetData(int x, int y, int z, uint value)
        {
            if (x >= width || x < 0 || y >= height || y < 0 || z >= depth || z < 0)
                return;
            data[x, y, z] = value;
        }
        public uint GetData(int x, int y, int z)
        {
            if (x >= width || x < 0 || y >= height || y < 0 || z >= depth || z < 0)
                return 1;
            return data[x, y, z];
        }
        public void New()
        {
            data = new uint[width, height, depth];
        }
        public void Resize()
        {

        }
        public void Reshade()
        {
            uint[,,] data = new uint[width, height, depth];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        if (this.data[x, y, z] != 0)
                        {
                            data[x, y, z] = GetRandomColor(x, y, z);
                        }
                    }
                }
            }
            this.data = data;
        }
        public void SetGrid()
        {
            float width = this.width + 0.08f;
            float height = this.height + 0.08f;
            float depth = this.depth + 0.08f;
            Vector3[] vertices =
            {
                new Vector3(width, 0.02f, 0.02f),
                new Vector3(width, height, 0.02f),
                new Vector3(width, height, depth),
                new Vector3(width, 0.02f, depth),

                new Vector3(0.02f, 0.02f, 0.02f),
                new Vector3(0.02f, height - 0.02f, 0.02f),
                new Vector3(0.02f, height - 0.02f, depth),
                new Vector3(0.02f, 0.02f, depth),

                new Vector3(0.02f, height, 0.02f),
                new Vector3(0.02f, height, depth),
                new Vector3(width, height, depth),
                new Vector3(width, height, 0.02f),

                new Vector3(0.02f, 0.02f, 0.02f),
                new Vector3(0.02f, 0.02f, depth),
                new Vector3(width, 0.02f, depth),
                new Vector3(width, 0.02f, 0.02f),

                new Vector3(0.02f, 0.02f, depth),
                new Vector3(0.02f, height, depth),
                new Vector3(width, height, depth),
                new Vector3(width, 0.02f, depth),

                new Vector3(0.02f, 0.02f, 0.02f),
                new Vector3(0.02f, height, 0.02f),
                new Vector3(width, height, 0.02f),
                new Vector3(width, 0.02f, 0.02f)
            };

            int[] triangles =
            {
                3, 2, 0, 2, 1, 0,
                6, 4, 5, 4, 6, 7,
                8, 10, 9, 10, 8, 11,
                14, 12, 13, 12, 14, 15,
                18, 16, 17, 16, 18, 19,
                20, 22, 21, 22, 20, 23
            };

            gameObject.AddComponent<MeshFilter>().mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles
            };
            gameObject.AddComponent<MeshRenderer>().material = MaterialManager.grid;
        }
        public void SetWater(Color32 waterColor, GameObject parent)
        {
            Vector3[] vertices =
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, -0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, -0.5f, -0.5f)
            };
            Color32[] colors32 =
            {
                waterColor,
                waterColor,
                waterColor,
                waterColor
            };
            int[] triangles =
            {
                2, 0, 1, 0, 2, 3
            };

            parent.AddComponent<MeshFilter>();
            parent.AddComponent<MeshRenderer>();
            parent.GetComponent<MeshFilter>().sharedMesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                colors32 = colors32
            };
            parent.GetComponent<MeshRenderer>().material = MaterialManager.block;
        }
        public void SetWater(int width, int depth, Color32 waterColor, GameObject parent)
        {
            Vector3[] vertices =
            {
                new Vector3(-width -0.5f, -0.5f, -depth -0.5f), //0
                new Vector3(-width -0.5f, -0.5f, depth - 0.5f), //1
                new Vector3(width - 0.5f, -0.5f, depth - 0.5f), //2
                new Vector3(width - 0.5f, -0.5f, -depth -0.5f) //3
            };
            Color32[] colors32 =
            {
                waterColor,
                waterColor,
                waterColor,
                waterColor
            };
            int[] triangles =
            {
                2, 0, 1, 0, 2, 3
            };

            parent.AddComponent<MeshFilter>();
            parent.AddComponent<MeshRenderer>();
            parent.GetComponent<MeshFilter>().sharedMesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                colors32 = colors32
            };
            parent.GetComponent<MeshRenderer>().material = MaterialManager.block;
        }
        public void SaveMap(string name, uint color)
        {
            VolumeIO.SaveMap(name, color, this);
        }
        public void OpenModel(string name)
        {
            VolumeIO.OpenModel(name, this);
            if (width < 17 && depth < 17)
            {
                Chunk.Add(new Chunk(0, 0, 0, this));
            }
            else
            {
                for (byte x = 0; x < wdc; x++)
                {
                    for (byte y = 0; y < hdc; y++)
                    {
                        for (byte z = 0; z < ddc; z++)
                        {
                            Chunk.Add(new Chunk(x, y, z, this));
                        }
                    }
                }
            }
        }
        public void SaveModel(string name)
        {
            VolumeIO.SaveModel(name, this);
        }
        public void LoadNearCamera(string name, uint innerColor, Camera camera)
        {
            VolumeIO.OpenMap(name, innerColor, this);

            int camX = Mathf.RoundToInt(camera.transform.position.x / Chunk.size);
            int camY = Mathf.RoundToInt(camera.transform.position.y / Chunk.size);
            int camZ = Mathf.RoundToInt(camera.transform.position.z / Chunk.size);
            List<Chunk> chunkList = new List<Chunk>();

            for (byte x = 0; x < wdc; x++)
            {
                for (byte z = 0; z < ddc; z++)
                {
                    for (byte y = 0; y < hdc; y++)
                    {
                        Chunk chunk = new Chunk(x, y, z, this);
                        chunk.sqrDistanceToCamera =
                            (camX - x) * (camX - x) +
                            (camY - y) * (camY - y) +
                            (camZ - z) * (camZ - z);
                        chunkList.Add(chunk);
                    }
                }
            }
            chunkList.Sort();
            foreach (Chunk chunk in chunkList)
            {
                Chunk.Add(chunk);
            }
        }
        public void SetVolumeCollider()
        {
            Vector3[] vertices =
            {
                new Vector3(width - 0.5f, -0.5f, -0.5f),
                new Vector3(width - 0.5f, height - 0.5f, -0.5f),
                new Vector3(width - 0.5f, height - 0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, -0.5f, depth - 0.5f),

                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, height - 0.5f, -0.5f),
                new Vector3(-0.5f, height - 0.5f, depth - 0.5f),
                new Vector3(-0.5f, -0.5f, depth - 0.5f),

                new Vector3(-0.5f, height - 0.5f, -0.5f),
                new Vector3(-0.5f, height - 0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, height - 0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, height - 0.5f, -0.5f),

                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, -0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, -0.5f, -0.5f),

                new Vector3(-0.5f, -0.5f, depth - 0.5f),
                new Vector3(-0.5f, height - 0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, height - 0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, -0.5f, depth - 0.5f),

                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, height - 0.5f, -0.5f),
                new Vector3(width - 0.5f, height - 0.5f, -0.5f),
                new Vector3(width - 0.5f, -0.5f, -0.5f)
            };

            int[] triangles =
            {
                3, 2, 0, 2, 1, 0,
                6, 4, 5, 4, 6, 7,
                8, 10, 9, 10, 8, 11,
                14, 12, 13, 12, 14, 15,
                18, 16, 17, 16, 18, 19,
                20, 22, 21, 22, 20, 23
            };

            gameObject.AddComponent<MeshCollider>();
            GetComponent<MeshCollider>().sharedMesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles
            };
        }
        public void SetVolumeCollider(int width, int height, int depth, GameObject parent)
        {
            Vector3[] vertices =
            {
                new Vector3(width - 0.5f, -0.5f, -0.5f),
                new Vector3(width - 0.5f, height - 0.5f, -0.5f),
                new Vector3(width - 0.5f, height - 0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, -0.5f, depth - 0.5f),

                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, height - 0.5f, -0.5f),
                new Vector3(-0.5f, height - 0.5f, depth - 0.5f),
                new Vector3(-0.5f, -0.5f, depth - 0.5f),

                new Vector3(-0.5f, height - 0.5f, -0.5f),
                new Vector3(-0.5f, height - 0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, height - 0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, height - 0.5f, -0.5f),

                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, -0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, -0.5f, -0.5f),

                new Vector3(-0.5f, -0.5f, depth - 0.5f),
                new Vector3(-0.5f, height - 0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, height - 0.5f, depth - 0.5f),
                new Vector3(width - 0.5f, -0.5f, depth - 0.5f),

                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, height - 0.5f, -0.5f),
                new Vector3(width - 0.5f, height - 0.5f, -0.5f),
                new Vector3(width - 0.5f, -0.5f, -0.5f)
            };
            int[] triangles =
            {
                3, 2, 0, 2, 1, 0,
                6, 4, 5, 4, 6, 7,
                8, 10, 9, 10, 8, 11,
                14, 12, 13, 12, 14, 15,
                18, 16, 17, 16, 18, 19,
                20, 22, 21, 22, 20, 23
            };

            parent.AddComponent<MeshCollider>();
            parent.GetComponent<MeshCollider>().sharedMesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles
            };
        }

        private uint GetRandomColor(int x, int y, int z)
        {
            Color32 color = UColor.UintToColor32(data[x, y, z]);

            int random = Random.Range(27 + y / 10, 30 + y / 10);
            int r = color.r;
            int g = color.g;
            int b = color.b;

            r = r + (y / 4) - r / (y + 4);
            g = g + (y / 4) - g / (y + 4);
            b = b + (y / 4) - b / (y + 4);
            r = r - r / random;
            g = g - g / random;
            b = b - b / random;

            if (r < 0) r = 1;
            if (r > 255) r = 255;
            if (g < 0) g = 1;
            if (g > 255) g = 255;
            if (b < 0) b = 1;
            if (b > 255) b = 255;

            return UColor.RGBAToUint((byte)r, (byte)g, (byte)b, 100);
        }

        private int GetMaxDimension()
        {
            if (width >= height && width >= depth)
                return width;
            if (height >= width && height >= depth)
                return height;
            if (depth >= width && depth >= height)
                return depth;
            return 0;
        }
        private int GetDepth(int maxDimension)
        {
            return maxDimension / Chunk.size / 8;
        }
    }
}