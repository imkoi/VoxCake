using System.Collections.Generic;

namespace VoxCake.Common
{
    public static class PillarManager
    {
        public static Stack<PillarSub> subs = new Stack<PillarSub>();
        public static Stack<Pillar> pillars = new Stack<Pillar>();

        public static void UpdateStack()
        {
            if (subs.Count != 0)
            {
                PillarSub pillarSub = subs.Pop();
                pillarSub.Calculate();
            }
            else if (pillars.Count != 0)
            {
                Pillar pillar = pillars.Pop();
                pillar.Instantiate();
            }
        }

        public static void AddPillar(Pillar pillar)
        {
            pillars.Push(pillar);
        }

        public static void AddSub(PillarSub pillarSub)
        {
            subs.Push(pillarSub);
        }
    }
}