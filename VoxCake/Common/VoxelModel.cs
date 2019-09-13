using System.IO;

namespace VoxCake.Common
{
    public abstract class VoxelModel
    {
        protected int width;
        protected int height;
        protected int depth;
        protected uint[,,] data;
    
        protected virtual void BindData(Stream stream) { }
        protected virtual void BindData(BinaryReader stream) { }

        public uint[,,] GetData()
        {
            return data;
        }
        public int GetWidth()
        {
            return width;
        }
        public int GetHeight()
        {
            return height;
        }
        public int GetDepth()
        {
            return depth;
        }
    }
}
