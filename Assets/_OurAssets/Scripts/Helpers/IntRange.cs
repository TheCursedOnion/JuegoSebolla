using System;
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
        public bool BoundedInArray(Array array)
        {
            return this.Start >= 0 && this.End >=0 && this.Start < array.Length && this.End < array.Length;
        }
    }
}
