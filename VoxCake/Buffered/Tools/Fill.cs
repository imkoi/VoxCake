using UnityEngine;

namespace VoxCake.Buffered
{
    
    public class Fill : ICommand //TODO: Implement fill command
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
