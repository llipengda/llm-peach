using Peach.Core;
using Peach.Core.Dom;


namespace Peach.Pro.Core.Mutators.LLM
{
    public abstract class LLMMutator : Mutator
    {
        public LLMMutator(DataElement obj) : base(obj) { }

        public int PickWeighted(int[] weights)
        {
            int sum = 0;
            foreach (int w in weights) sum += w;
            if (sum <= 0) return 0;
            int r = Next(sum);
            for (int i = 0; i < weights.Length; i++)
            {
                if (r < weights[i]) return i;
                r -= weights[i];
            }
            return weights.Length - 1;
        }

        public int Next(int max) 
        {
            return context.Random.Next(max);
        }

        public void NextBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)Next(256);
            }
        }

        // public override int SelectionWeight => 100;

        // public override int weight => 100;
    }
}