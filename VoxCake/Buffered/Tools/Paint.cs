using UnityEngine;

namespace VoxCake.Buffered
{
    public class Paint : ICommand //TODO: Implement paint command
    {
        public void Do(byte mode, Vector3Int start, Vector3Int end, Volume volume)
        {
            
        }

        public void Redo(Volume volume)
        {

        }

        public void Undo(Volume volume)
        {
        
        }
    }
}
