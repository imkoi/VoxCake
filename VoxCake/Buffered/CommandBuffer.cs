using System.Collections.Generic;
using UnityEngine;

namespace VoxCake.Buffered
{
    public static class CommandBuffer
    {
        public delegate void Changed(bool haveUnsavedChanges);
        public static event Changed OnChanged = h => { };

        private static List<ICommand> commands = new List<ICommand>();
        private static int lastExecuted = -1;
        private static int lastSaved = -1;

        
        public static bool modified { get { return lastSaved != lastExecuted; } }
        public static int size { get { return commands.Count; } }
        public static int LastExecuted { get { return lastExecuted; } }

        public static void Clear()
        {
            commands.Clear();
            lastExecuted = -1;
            lastSaved = -1;

            OnChanged(false);
        }
        public static void Save()
        {
            lastSaved = lastExecuted;

            OnChanged(false);
        }
        public static void Limit(int numCommands)
        {
            while (commands.Count > numCommands)
            {
                commands.RemoveAt(0);
                if (lastExecuted >= 0)
                {
                    lastExecuted--;
                }

                if (lastSaved >= 0)
                {
                    lastSaved--;
                }
            }
        }

        public static void Do(ICommand command,byte mode, Vector3Int start, Vector3Int end, Volume volume)
        {
            if (lastExecuted + 1 < commands.Count)
            {
                int numCommandsToRemove = commands.Count - (lastExecuted + 1);
                for (int i = 0; i < numCommandsToRemove; i++)
                {
                    commands.RemoveAt(lastExecuted + 1);
                }
                lastSaved = -1;
            }
            command.Do(mode, start, end, volume);

            if (!commands.Contains(command)){
                commands.Add(command);
            }
            
            lastExecuted = commands.Count - 1;

            OnChanged(true);
        }
        public static void Undo(Volume volume)
        {
            if (lastExecuted >= 0)
            {
                if (commands.Count > 0)
                {
                    commands[lastExecuted].Undo(volume);
                    lastExecuted--;
                    OnChanged(lastExecuted != lastSaved);
                }
            }
        }
        public static void Redo(Volume volume)
        {
            if (lastExecuted + 1 < commands.Count)
            {
                commands[lastExecuted + 1].Redo(volume);
                lastExecuted++;
                OnChanged(lastExecuted != lastSaved);
            }
        }
    }
}