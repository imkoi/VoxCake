using UnityEngine;

namespace VoxCake
{
    public static class RenderSettings
    {
        public static int viewDistance = 12;
        public static bool useFrustumCulling = true;

        public static void SetViewDistance(int distance)
        {
            viewDistance = distance;
            UnityEngine.RenderSettings.fogStartDistance = distance * 12;
            UnityEngine.RenderSettings.fogEndDistance = distance * 16;
            Camera.main.farClipPlane = UnityEngine.RenderSettings.fogEndDistance;
        }
    }
}
