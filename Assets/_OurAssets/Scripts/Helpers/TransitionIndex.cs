namespace CursedOnion.Helpers
{
    public class TransitionIndex
    {
        public int transitionIndex = -1;
        void ResetTransitionIndex() => transitionIndex = -1;
        public void SetTransitionIndex(int index) => transitionIndex = index;
        public bool IsIndexEquals(int value)
        {
            bool result = transitionIndex == value;
            if(result) ResetTransitionIndex();
            return result;
        }
    }
}