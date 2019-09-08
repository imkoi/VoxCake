using UnityEngine;
using UnityEngine.Rendering;

namespace VoxCake
{
    public static class RenderSettings
    {
        public static float viewDistance = 16;
        
        public static void Set()
        {
            UnityEngine.RenderSettings.skybox = null;
            UnityEngine.RenderSettings.fog = true;
            UnityEngine.RenderSettings.ambientLight = Color.white;
            UnityEngine.RenderSettings.ambientMode = AmbientMode.Flat;
            UnityEngine.RenderSettings.fogMode = FogMode.Linear;
            UnityEngine.RenderSettings.fogColor = Camera.main.backgroundColor;
            
            SetViewDistance(16);
        }
        
        public static void SetViewDistance(float distance)
        {
            if (distance < 256)
            {
                viewDistance = distance;
                UnityEngine.RenderSettings.fogStartDistance = distance*0.45f;
                UnityEngine.RenderSettings.fogEndDistance = distance*0.55f;
                Camera.main.farClipPlane = UnityEngine.RenderSettings.fogEndDistance;
            }
        }
    }
}
