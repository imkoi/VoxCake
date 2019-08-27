using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace VoxCake
{
    public static class Log
    {
        private static string date = DateTime.Now.ToString("MM/dd/yyyy h:mm").Replace(".", "-").Replace(":", "_");

        public static string path = Path.Combine(new[] { Application.streamingAssetsPath, "Logs" });
        public static string logPath = Path.Combine(new[] { path, date + ".txt" });
        public static List<string> logCollection = new List<string>();
        public static bool enable = false;
        public static bool showLogs = true;

        public static void Write(string log, bool isEngine = false)
        {
            if (enable)
            {
                if (isEngine)
                    log = "VoxCake." + log;
                log = "[Log] " + log;
                logCollection.Add(log);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                File.WriteAllLines(logPath, logCollection.ToArray());
            }
#if UNITY_EDITOR
            if(showLogs)
                Debug.Log(log);
#endif
        }
        public static void Error(string log, bool isEngine = false)
        {
            if (enable)
            {

                if (isEngine)
                    log = "VoxCake." + log;
                log = "[Error] " + log;
                logCollection.Add(log);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                File.WriteAllLines(logPath, logCollection.ToArray());
            }
#if UNITY_EDITOR
            if (showLogs)
                Debug.LogError(log);
#endif
        }
    }
}
