namespace VoxCake.Buffered
{
    public interface ICommand
    {
        void Do(Volume volume);
        void Undo(Volume volume);
        void Redo(Volume volume);
    }
}