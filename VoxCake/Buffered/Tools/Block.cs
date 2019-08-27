namespace VoxCake.Buffered
{
    public class Block : ICommand
    {
        public byte operation;
        public VoxelBuffered data;

        public void Do(Volume volume)
        {
            data.previousColor = volume.GetData(data.x, data.y, data.z);
            if (operation == 0)
            {
                data.currentColor = 0;
                volume.SetData(data.x, data.y, data.z, 0);
            }
            else if (operation == 1)
            {
                data.currentColor = Editor.color;
                volume.SetData(data.x, data.y, data.z, data.currentColor);
            }
            else if (operation == 2)
            {
                if(data.previousColor != 0 && data.previousColor != 1)
                {
                    data.currentColor = Editor.color;
                    volume.SetData(data.x, data.y, data.z, data.currentColor);
                }
            }
            Chunk.AddForVoxel(data.x, data.y, data.z, volume);
        }
        public void Redo(Volume volume)
        {
            if (operation == 0)
                volume.SetData(data.x, data.y, data.z, 0);
            else if (operation == 1)
                volume.SetData(data.x, data.y, data.z, data.currentColor);
            else if (operation == 2)
                volume.SetData(data.x, data.y, data.z, data.currentColor);

            Chunk.AddForVoxel(data.x, data.y, data.z, volume);
        }
        public void Undo(Volume volume)
        {
            volume.SetData(data.x, data.y, data.z, data.previousColor);
            Chunk.AddForVoxel(data.x, data.y, data.z, volume);
        }
    }
}