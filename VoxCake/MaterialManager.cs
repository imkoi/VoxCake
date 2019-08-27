using UnityEngine;

namespace VoxCake
{
    public static class MaterialManager
    {
        public static Material block = new Material(Shader.Find("VoxCake/Block"));
        public static Material glow = new Material(Shader.Find("VoxCake/Glow"));
        public static Material glass = new Material(Shader.Find("VoxCake/Glass"));
        public static Material model = new Material(Shader.Find("VoxCake/Model"));
        public static Material pillar = new Material(Shader.Find("VoxCake/Pillar"));
        public static Material grid = new Material(Shader.Find("VoxCake/Grid"));

        public static void Init()
        {
            block.mainTexture = Resources.Load<Texture>("LightyEdges");
            glass.mainTexture = Resources.Load<Texture>("LightyEdges");
        }
    }
}
