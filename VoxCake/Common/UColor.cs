using UnityEngine;

namespace VoxCake
{
    public static class UColor
    {
        public static byte GetA(uint value)
        {
            return (byte)(value >> 24);
        }
        public static byte UintToByte(uint value)
        {
            byte r = (byte)(value >> 16);
            byte g = (byte)(value >> 8);
            byte b = (byte)(value >> 0);
            return (byte)(((r / 32) << 5) + ((g / 32) << 2) + (b / 64));
        }
        public static uint SetA(uint value, int a)
        {
            byte r = (byte)(value >> 16);
            byte g = (byte)(value >> 8);
            byte b = (byte)(value >> 0);
            if (a > 0) { return (uint)((a << 24) | (r << 16) | (g << 8) | (b << 0)); }
            return 0;
        }
        public static uint RGBAToUint(byte r, byte g, byte b, byte a)
        {
            return (uint)((a << 24) | (r << 16) | (g << 8) | (b << 0));
        }
        public static Color32 UintToColor32(uint value)
        {
            byte r = (byte)(value >> 16);
            byte g = (byte)(value >> 8);
            byte b = (byte)(value >> 0);
            return new Color32(r, g, b, 255);
        }
        public static Color32 ByteToColor32(byte value)
        {
            var color = new Color32();
            color.r = (byte)((value >> 5) * 32);
            color.g = (byte)((value >> 2) * 32);
            color.b = (byte)(value * 64);
            color.a = 255;
            return color;
        }
        public static Color FromHex(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            return new Color(
                byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
        }

        public static Color FromHex32(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);
            return new Color32(
                byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                255);
        }
    }
}