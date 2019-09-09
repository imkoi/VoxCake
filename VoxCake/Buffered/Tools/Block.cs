using UnityEngine;

namespace VoxCake.Buffered
{
    public class Block : ICommand
    {
        private VoxelBuffered data;

        public void Do(byte mode, Vector3Int start, Vector3Int end, Volume volume)
        {
            uint value = mode == 0 ? 0 : Editor.color;
            data.x = end.x;
            data.y = end.y;
            data.z = end.z;
            data.previousColor = volume.GetData(end.x, end.y, end.z);
            data.currentColor = value;
            Make(volume, value);
        }
        public void Redo(Volume volume)
        {
            Make(volume, data.currentColor);
        }
        public void Undo(Volume volume)
        {
            Make(volume, data.previousColor);
        }

        private void Make(Volume volume, uint value)
        {
            volume.SetData(data.x, data.y, data.z, value);
            Chunk.AddForVoxel(data.x, data.y, data.z, volume);
        }
    }
}