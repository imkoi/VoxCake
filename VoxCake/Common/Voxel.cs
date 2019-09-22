namespace VoxCake 
{
    public struct Voxel
    {
        public int x, y, z;
        public uint value;
        public bool processed;
        
        public Voxel(int x, int y, int z, uint value, bool processed)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.value = value;
            this.processed = processed;
        }
    }
}

