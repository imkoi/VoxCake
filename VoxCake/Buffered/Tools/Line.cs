using System.Collections.Generic;
using UnityEngine;

namespace VoxCake.Buffered
{
    public class Line : ICommand
    {
        private List<VoxelBuffered> data = new List<VoxelBuffered>();

        public void Do(byte mode, Vector3Int start, Vector3Int end, Volume volume)
        {
            uint value = mode == 0 ? 0 : Editor.color;
            int x0 = start.x;
            int x1 = end.x;
            int y0 = start.y;
            int y1 = end.y;
            int z0 = start.z;
            int z1 = end.z;
            List<Chunk> stack = new List<Chunk>();
            if (start != end)
            {
                bool steepXY = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
                if (steepXY)
                {
                    Editor.Swap(ref x0, ref y0);
                    Editor.Swap(ref x1, ref y1);
                }

                bool steepXZ = Mathf.Abs(z1 - z0) > Mathf.Abs(x1 - x0);
                if (steepXZ)
                {
                    Editor.Swap(ref x0, ref z0);
                    Editor.Swap(ref x1, ref z1);
                }

                int deltaX = Mathf.FloorToInt(Mathf.Abs(x1 - x0));
                int deltaY = Mathf.FloorToInt(Mathf.Abs(y1 - y0));
                int deltaZ = Mathf.FloorToInt(Mathf.Abs(z1 - z0));
                int errorXY = deltaX / 2, errorXZ = deltaX / 2;
                int stepX = x0 > x1 ? -1 : 1;
                int stepY = y0 > y1 ? -1 : 1;
                int stepZ = z0 > z1 ? -1 : 1;

                int y = y0, z = z0;
                int xCopy, yCopy, zCopy;
                int prevX = 0;
                int prevY = 0;
                int prevZ = 0;
                for (int x = x0; stepX > 0 ? x != x1 + 1 : x != x1 - 1; x += stepX)
                {
                    xCopy = x;
                    yCopy = y;
                    zCopy = z;
                    if (steepXZ) Editor.Swap(ref xCopy, ref zCopy);
                    if (steepXY) Editor.Swap(ref xCopy, ref yCopy);

                    data.Add(new VoxelBuffered
                    {
                        x = xCopy,
                        y = yCopy,
                        z = zCopy,
                        currentColor = value,
                        previousColor = volume.GetData(xCopy,yCopy,zCopy)
                    });
                    volume.SetData(xCopy, yCopy, zCopy, value);
                    Editor.UpdateChunk(xCopy, yCopy, zCopy, stack);

                    errorXY -= deltaY;
                    errorXZ -= deltaZ;
                    if (errorXY < 0)
                    {
                        y += stepY;
                        errorXY += deltaX;
                    }

                    if (errorXZ < 0)
                    {
                        z += stepZ;
                        errorXZ += deltaX;
                    }

                    if (x != x0)
                    {
                        if (prevX != xCopy && prevY != yCopy && prevZ == zCopy)
                        {
                            data.Add(new VoxelBuffered
                            {
                                x = prevX,
                                y = yCopy,
                                z = zCopy,
                                currentColor = value,
                                previousColor = volume.GetData(prevX,yCopy,zCopy)
                            });
                            volume.SetData(prevX, yCopy, zCopy, value);
                            Editor.UpdateChunk(prevX, yCopy, zCopy, stack);
                        }
                        else if (prevX != xCopy && prevY == yCopy && prevZ != zCopy)
                        {
                            data.Add(new VoxelBuffered
                            {
                                x = prevX,
                                y = yCopy,
                                z = zCopy,
                                currentColor = value,
                                previousColor = volume.GetData(prevX,yCopy,zCopy)
                            });
                            volume.SetData(prevX, yCopy, zCopy, value);
                            Editor.UpdateChunk(prevX, yCopy, zCopy, stack);
                        }
                        else if (prevX == xCopy && prevY != yCopy && prevZ != zCopy)
                        {
                            data.Add(new VoxelBuffered
                            {
                                x = xCopy,
                                y = yCopy,
                                z = prevZ,
                                currentColor = value,
                                previousColor = volume.GetData(xCopy,yCopy,prevZ)
                            });
                            volume.SetData(xCopy, yCopy, prevZ, value);
                            Editor.UpdateChunk(xCopy, yCopy, prevZ, stack);
                        }
                        else if (prevX != xCopy && prevY != yCopy && prevZ != zCopy)
                        {
                            data.Add(new VoxelBuffered
                            {
                                x = prevX,
                                y = yCopy,
                                z = zCopy,
                                currentColor = value,
                                previousColor = volume.GetData(prevX,yCopy,zCopy)
                            });
                            data.Add(new VoxelBuffered
                            {
                                x = prevX,
                                y = yCopy,
                                z = prevZ,
                                currentColor = value,
                                previousColor = volume.GetData(prevX,yCopy,prevZ)
                            });
                            volume.SetData(prevX, yCopy, zCopy, value);
                            volume.SetData(prevX, yCopy, prevZ, value);
                            Editor.UpdateChunk(prevX, yCopy, zCopy, stack);
                            Editor.UpdateChunk(prevX, yCopy, prevZ, stack);
                        }
                    }

                    prevX = xCopy;
                    prevY = yCopy;
                    prevZ = zCopy;
                }
            }
            else
            {
                data.Add(new VoxelBuffered
                {
                    x = x0,
                    y = y0,
                    z = y1,
                    currentColor = value,
                    previousColor = volume.GetData(x0,y0,z0)
                });
                volume.SetData(x0, y0, z0, value);
                Editor.UpdateChunk(x0, y0, z0, stack);
            }

            int count = stack.Count;
            for (int i = 0; i < count; i++)
            {
                Chunk chunk = stack[i];
                Chunk.Add(chunk);
            }
        }

        public void Redo(Volume volume)
        {
            Make(volume, 0);
        }

        public void Undo(Volume volume)
        {
            Make(volume, 1);
        }
        
        private void Make(Volume volume, byte mode)
        {
            int count = data.Count;
            List<Chunk> stack = new List<Chunk>();
            for (int i = 0; i < count; i++)
            {
                VoxelBuffered v = data[i];
                uint value = mode == 0 ? v.currentColor : v.previousColor;
                volume.SetData(v.x, v.y, v.z, value);
                Editor.UpdateChunk(v.x,v.y, v.z, stack);
            }
            
            count = stack.Count;
            for (int i = 0; i < count; i++)
            {
                Chunk chunk = stack[i];
                Chunk.Add(chunk);
            }
        }
    }
}