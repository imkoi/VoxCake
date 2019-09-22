using System.Collections.Generic;

namespace VoxCake.Common
{
    public static class PillarManager
    {
        private static Stack<PillarSub> _subs = new Stack<PillarSub>();
        private static Stack<Pillar> _pillars = new Stack<Pillar>();
        private static bool _executedLastTime;

        public static void UpdateStack()
        {
            _executedLastTime = !_executedLastTime;
            if (!_executedLastTime)
            {
                if (_subs.Count != 0)
                {
                    PillarSub pillarSub = _subs.Pop();
                    pillarSub.Calculate();
                }
                else if (_pillars.Count != 0)
                {
                    Pillar pillar = _pillars.Pop();
                    pillar.Instantiate();
                }
            }
        }

        public static int GetPillarToUpdate()
        {
            return _subs.Count;
        }
        
        public static void AddPillar(Pillar pillar)
        {
            _pillars.Push(pillar);
        }

        public static void AddSub(PillarSub pillarSub)
        {
            _subs.Push(pillarSub);
        }
    }
}