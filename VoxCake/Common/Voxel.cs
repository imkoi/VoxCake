namespace VoxCake 
{
    public struct Voxel
    {
        public short x, y, z;
        public uint value;
        public bool processed;
        
        public Voxel(int x, int y, int z, uint value, bool processed)
        {
            this.x = (short)x;
            this.y = (short)y;
            this.z = (short)z;
            this.value = value;
            this.processed = processed;
        }
    }
}

