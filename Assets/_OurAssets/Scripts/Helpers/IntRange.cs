using UnityEngine;

namespace CursedOnion.Helpers
{
    [System.Serializable]
    public class IntRange
    {
        public int Start;
        public int End;
        public IntRange(int start, int end)
        {
            this.Start = start;
            this.End = end;
        }
    }
}
