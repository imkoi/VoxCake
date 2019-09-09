using UnityEngine;

namespace VoxCake.Buffered
{
    public interface ICommand
    {
        void Do(byte mode, Vector3Int start, Vector3Int end, Volume volume);
        void Undo(Volume volume);
        void Redo(Volume volume);
    }
}