using System.IO;

namespace VoxCake.Common.Format
{
    public class Kv6 : VoxelModel
    {
        private static uint[,,] _temp;

        public Kv6(string path)
        {
            using (Stream stream = new FileStream(path, FileMode.Open))
            {
                try
                {
                    BindData(stream);

                    data = new uint[width, height, depth];
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            for (int z = 0; z < depth; z++)
                            {
                                data[x, height - y - 1, z] = _temp[x, z, y];
                            }
                        }
                    }

                    _temp = null;
                }
                catch
                {
                    Log.Error("Kv6: cannot load model!", true);
                }
            }
        }

        protected override void BindData(Stream stream)
        {
            var reader = new BinaryReader(stream);
            {
                var buf = new byte[4];
                if (stream.Read(buf, 0, 4) < 4)
                {
                    throw new IOException("Magic not read");
                }

                if (buf[0] != 'K' ||
                    buf[1] != 'v' ||
                    buf[2] != 'x' ||
                    buf[3] != 'l')
                {
                    throw new IOException("Invalid magic");
                }
            }

            int xsiz = reader.ReadInt32();
            int ysiz = reader.ReadInt32();
            int zsiz = reader.ReadInt32();
            float xpivot = reader.ReadSingle();
            float ypivot = reader.ReadSingle();
            float zpivot = reader.ReadSingle();
            int numblocks = reader.ReadInt32();
            var blocks = new Kv6Block[numblocks];

            for (int i = 0; i < blocks.Length; ++i)
            {
                blocks[i].color = reader.ReadUInt32();
                blocks[i].zpos = reader.ReadUInt16();
                reader.ReadUInt16();
            }

            var xyoffset = new int[xsiz * ysiz];
            for (int i = 0; i < xsiz; ++i)
                reader.ReadInt32();
            for (int i = 0; i < xyoffset.Length; ++i)
                xyoffset[i] = (int) reader.ReadUInt16();

            int pos = 0;
            width = xsiz;
            height = zsiz;
            depth = ysiz;
            _temp = new uint [xsiz, ysiz, zsiz];
            for (int x = 0; x < xsiz; ++x)
            {
                for (int y = 0; y < ysiz; ++y)
                {
                    int sb = xyoffset[x * ysiz + y];
                    for (int i = 0; i < sb; ++i)
                    {
                        var b = blocks[pos];
                        _temp[x, y, b.zpos] = b.color;
                        pos += 1;
                    }
                }
            }
        }

        private struct Kv6Block
        {
            public uint color;
            public int zpos;
        }
    }
}