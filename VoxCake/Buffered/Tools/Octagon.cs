using UnityEngine;
using System.Collections.Generic;

namespace VoxCake.Buffered
{
    public class Octagon : ICommand
    {
        private List<VoxelBuffered> data = new List<VoxelBuffered>();

        public void Do(byte mode, Vector3Int start, Vector3Int end, Volume volume)
        {
            uint value = mode == 0 ? 0 : Editor.color;
            int xMin = start.x < end.x ? start.x : end.x;
            int xMax = start.x > end.x ? start.x : end.x;
            int yMin = start.y < end.y ? start.y : end.y;
            int yMax = start.y > end.y ? start.y : end.y;
            int zMin = start.z < end.z ? start.z : end.z;
            int zMax = start.z > end.z ? start.z : end.z;
            List<Chunk> stack = new List<Chunk>();
            int xmaxp = xMax + 1;
            int ymaxp = yMax + 1;
            int zmaxp = zMax + 1;
            for (int x = xMin; x < xmaxp; x++)
            {
                for (int y = yMin; y < ymaxp; y++)
                {
                    for (int z = zMin; z < zmaxp; z++)
                    {
                        data.Add(new VoxelBuffered {
                            previousColor = volume.GetData(x, y, z),
                            currentColor = value,
                            x = x,
                            y = y,
                            z = z
                        });
                        volume.SetData(x, y, z, value);
                        Editor.UpdateChunk(x, y, z, stack);
                    }
                }
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