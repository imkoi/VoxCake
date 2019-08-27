namespace VoxCake.Buffered
{
    public struct VoxelBuffered
    {
        public int x;
        public int y;
        public int z;
        public uint previousColor;
        public uint currentColor;

        public VoxelBuffered(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            previousColor = 0;
            currentColor = 0;
        }
        public VoxelBuffered(int x, int y, int z, uint value)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            previousColor = value;
            currentColor = 0;
        }
    }
}
