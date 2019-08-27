using UnityEngine;
using System.Collections.Generic;

namespace VoxCake.Buffered
{
    public class Octagon : ICommand
    {
        
        public List<VoxelBuffered> Data;
        private bool _firstTime = true;

        private int gyMin;
        private int gzMin;
        private int gyMax;
        private int gzMax;
        private int gxMax;

        public Vector3Int vec0;
        public Vector3Int vec1;

        public void Do(Volume volume)
        {
            /*
            Data.Voxel = new Voxel[(xMax - xMin + 1) * (yMax - yMin + 1) * (zMax - zMin + 1)];

            for (int x = xMin; x < xMax + 1; x++)
            {
                for (int y = yMin; y < yMax + 1; y++)
                {
                    for (int z = zMin; z < zMax + 1; z++)
                    {
                        if (Data.Mode == 0)
                        {
                            Data.Voxel[GetIndex(x, y, z)] =
                                new Voxel(new Vector3Int(x, y, z), Map.Data.Get(x, y, z), 0);
                            Data.Voxel[GetIndex(x, y, z)].NewColor = ColorHelper.Randomize(Editor.CurrentColor, Editor.ColorNoise);

                            Map.Data.Set(x, y, z, Data.Voxel[GetIndex(x, y, z)].NewColor, 100);
                        }
                        else if (Data.Mode == 1)
                        {
                            Data.Voxel[GetIndex(x, y, z)] =
                                new Voxel(new Vector3Int(x, y, z), Map.Data.Get(x, y, z), 0);
                            //Data.Voxel[GetIndex(x, y, z)].NewColor = Editor.CurrentColor;

                            Map.Data.Set(x, y, z, 0x00000000, 0);
                        }
                        else if (Data.Mode == 2)
                        {
                            if (Map.Data.Get(x, y, z) != 0x00000000)
                            {
                                Data.Voxel[GetIndex(x, y, z)] =
                                    new Voxel(new Vector3Int(x, y, z), Map.Data.Get(x, y, z), 0);
                                Data.Voxel[GetIndex(x, y, z)].NewColor = ColorHelper.Randomize(Editor.CurrentColor, Editor.ColorNoise);

                                Map.Data.Set(x, y, z, Data.Voxel[GetIndex(x, y, z)].NewColor, 100);
                            }
                            else
                            {
                                Data.Voxel[GetIndex(x, y, z)] =
                                    new Voxel(new Vector3Int(x, y, z), 0x00000000, 0);
                            }
                        }
                        else Debug.LogError("Unknown mode");
                    }
                }
            }

            if (xMax - xMin == 0 && yMax - yMin == 0 && zMax - zMin == 0)
            {
                if (Data.Mode == 0) Map.Data.Chunks.Update(xMin, yMin, zMin);
                else if (Data.Mode == 1) Map.Data.Chunks.Update(xMin, yMin, zMin);
                else if (Data.Mode == 2) Map.Data.Chunks.Update(xMin, yMin, zMin);
                else Debug.LogError("Unknown mode");
            }*/
        }
        public void Redo(Volume volume)
        {

        }
        public void Undo(Volume volume)
        {
            /*
            int xMin = Data.V0.x < Data.V1.x ? Data.V0.x : Data.V1.x;
            int xMax = Data.V0.x > Data.V1.x ? Data.V0.x : Data.V1.x;
            int yMin = Data.V0.y < Data.V1.y ? Data.V0.y : Data.V1.y;
            int yMax = Data.V0.y > Data.V1.y ? Data.V0.y : Data.V1.y;
            int zMin = Data.V0.z < Data.V1.z ? Data.V0.z : Data.V1.z;
            int zMax = Data.V0.z > Data.V1.z ? Data.V0.z : Data.V1.z;

            int cxMin = xMin / Chunk.size;
            int cxMax = xMax / Chunk.size;
            int cyMin = yMin / Chunk.size;
            int cyMax = yMax / Chunk.size;
            int czMin = zMin / Chunk.size;
            int czMax = zMax / Chunk.size;

            for (int x = xMin; x < xMax + 1; x++)
            {
                for (int y = yMin; y < yMax + 1; y++)
                {
                    for (int z = zMin; z < zMax + 1; z++)
                    {
                        if (Data.Mode == 0)
                        {
                            Map.Data.Set(x, y, z, Data.Voxel[GetIndex(x, y, z)].OldColor);
                        }
                        else if (Data.Mode == 1)
                        {
                            Map.Data.Set(x, y, z, Data.Voxel[GetIndex(x, y, z)].OldColor);
                        }
                        else if (Data.Mode == 2)
                        {
                            Map.Data.Set(x, y, z, Data.Voxel[GetIndex(x, y, z)].OldColor);
                        }
                        else Debug.LogError("Unknown mode");
                    }
                }
            }

            for (int x = cxMin; x < cxMax + 1; x++)
            {
                for (int y = cyMin; y < cyMax + 1; y++)
                {
                    for (int z = czMin; z < czMax + 1; z++)
                    {
                        Map.Data.Chunks.SetMesh(x, y, z);
                    }
                }
            }*/
        }
        /*
        private int GetIndex(int x, int y, int z)
        {
            return ((gxMax - x) * (gyMax - gyMin + 1) + (gyMax - y)) * (gzMax - gzMin + 1) + (gzMax - z);
        }

        public void Do()
        {
            int xMin = Data.V0.x < Data.V1.x ? Data.V0.x : Data.V1.x;
            int xMax = Data.V0.x > Data.V1.x ? Data.V0.x : Data.V1.x;
            int yMin = Data.V0.y < Data.V1.y ? Data.V0.y : Data.V1.y;
            int yMax = Data.V0.y > Data.V1.y ? Data.V0.y : Data.V1.y;
            int zMin = Data.V0.z < Data.V1.z ? Data.V0.z : Data.V1.z;
            int zMax = Data.V0.z > Data.V1.z ? Data.V0.z : Data.V1.z;

            gyMin = yMin;
            gzMin = zMin;
            gyMax = yMax;
            gzMax = zMax;
            gxMax = xMax;

            int cxMin = xMin / Chunk.Size;
            int cxMax = xMax / Chunk.Size;
            int cyMin = yMin / Chunk.Size;
            int cyMax = yMax / Chunk.Size;
            int czMin = zMin / Chunk.Size;
            int czMax = zMax / Chunk.Size;

            if (_firstTime)
            {
                

                else
                {
                    if (Data.Mode == 0)
                    {
                        for (int x = cxMin; x < cxMax + 1; x++)
                        {
                            for (int y = cyMin; y < cyMax + 1; y++)
                            {
                                Map.Data.Chunks.SetMesh(x, y, czMin);
                                Map.Data.Chunks.SetMesh(x, y, czMax);
                            }
                        }

                        for (int x = cxMin; x < cxMax + 1; x++)
                        {
                            for (int z = czMin; z < czMax + 1; z++)
                            {
                                Map.Data.Chunks.SetMesh(x, cyMin, z);
                                Map.Data.Chunks.SetMesh(x, cyMax, z);
                            }
                        }

                        for (int y = cyMin; y < cyMax + 1; y++)
                        {
                            for (int z = czMin; z < czMax + 1; z++)
                            {
                                Map.Data.Chunks.SetMesh(cxMin, y, z);
                                Map.Data.Chunks.SetMesh(cxMax, y, z);
                            }
                        }
                    }
                    else if (Data.Mode == 1)
                    {
                        for (int x = cxMin; x < cxMax + 1; x++)
                        {
                            for (int y = cyMin; y < cyMax + 1; y++)
                            {
                                for (int z = czMin; z < czMax + 1; z++)
                                {
                                    Map.Data.Chunks.SetMesh(x, y, z);
                                }
                            }
                        }
                    }
                    else if (Data.Mode == 2)
                    {
                        for (int x = cxMin; x < cxMax + 1; x++)
                        {
                            for (int y = cyMin; y < cyMax + 1; y++)
                            {
                                for (int z = czMin; z < czMax + 1; z++)
                                {
                                    Map.Data.Chunks.SetMesh(x, y, z);
                                }
                            }
                        }
                    }
                    else Debug.LogError("Unknown mode");
                }

                _firstTime = false;
            }
            else //////////       REDO
            {
                for (int x = xMin; x < xMax + 1; x++)
                {
                    for (int y = yMin; y < yMax + 1; y++)
                    {
                        for (int z = zMin; z < zMax + 1; z++)
                        {
                            if (Data.Mode == 0)
                            {
                                Map.Data.Set(x, y, z, Data.Voxel[GetIndex(x, y, z)].NewColor, 100);
                            }
                            else if (Data.Mode == 1)
                            {
                                Map.Data.Set(x, y, z, 0x00000000, 0);
                            }
                            else if (Data.Mode == 2)
                            {
                                if (Map.Data.Get(x, y, z) != 0x00000000)
                                {
                                    Map.Data.Set(x, y, z, Data.Voxel[GetIndex(x, y, z)].NewColor, 100);
                                }
                            }
                            else Debug.LogError("Unknown mode");
                        }
                    }
                }

                if (xMax - xMin == 0 && yMax - yMin == 0 && zMax - zMin == 0)
                {
                    if (Data.Mode == 0) Map.Data.Chunks.Update(xMin, yMin, zMin);
                    else if (Data.Mode == 1) Map.Data.Chunks.Update(xMin, yMin, zMin);
                    else if (Data.Mode == 2) Map.Data.Chunks.Update(xMin, yMin, zMin);
                    else Debug.LogError("Unknown mode");
                }
                else
                {
                    if (Data.Mode == 0)
                    {
                        for (int x = cxMin; x < cxMax + 1; x++)
                        {
                            for (int y = cyMin; y < cyMax + 1; y++)
                            {
                                Map.Data.Chunks.SetMesh(x, y, czMin);
                                Map.Data.Chunks.SetMesh(x, y, czMax);
                            }
                        }

                        for (int x = cxMin; x < cxMax + 1; x++)
                        {
                            for (int z = czMin; z < czMax + 1; z++)
                            {
                                Map.Data.Chunks.SetMesh(x, cyMin, z);
                                Map.Data.Chunks.SetMesh(x, cyMax, z);
                            }
                        }

                        for (int y = cyMin; y < cyMax + 1; y++)
                        {
                            for (int z = czMin; z < czMax + 1; z++)
                            {
                                Map.Data.Chunks.SetMesh(cxMin, y, z);
                                Map.Data.Chunks.SetMesh(cxMax, y, z);
                            }
                        }
                    }
                    else if (Data.Mode == 1)
                    {
                        for (int x = cxMin; x < cxMax + 1; x++)
                        {
                            for (int y = cyMin; y < cyMax + 1; y++)
                            {
                                for (int z = czMin; z < czMax + 1; z++)
                                {
                                    Map.Data.Chunks.SetMesh(x, y, z);
                                }
                            }
                        }
                    }
                    else if (Data.Mode == 2)
                    {
                        for (int x = cxMin; x < cxMax + 1; x++)
                        {
                            for (int y = cyMin; y < cyMax + 1; y++)
                            {
                                for (int z = czMin; z < czMax + 1; z++)
                                {
                                    Map.Data.Chunks.SetMesh(x, y, z);
                                }
                            }
                        }
                    }
                    else Debug.LogError("Unknown mode");
                }
            }
        }

        public void Undo()
        {
            
        }*/
    }
}