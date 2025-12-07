using System;
using System.Collections;
using System.Threading.Tasks;
using UltEvents;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Modes.General.UI.Transitions
{
    public enum TransitionType
    {
        None,
        Stripe,
        Fade,
    }
    public abstract class UITransition : MonoBehaviour
    {
        protected Image Image;
        protected Material MaterialInstance;

        protected readonly UITransitionData TransitionData = new UITransitionData();
        
        protected bool DoingTransition;
        
        #region Set Up
        protected virtual void Awake()
        {
            Image = GetComponent<Image>();
            MaterialInstance = Instantiate(Image.material);
            Image.material = MaterialInstance;
        }
        #endregion
        
        #region Builder
        public UITransition SetInBetweenTime(float betweenTime)
        {
            TransitionData.InBetweenTime = betweenTime;
            return this;
        }
        public UITransition SetColor(Color color)
        {
            TransitionData.Color = color;
            return this;
        }
        public UITransition SetMidAction(Func<Task> midPoint)
        {
            TransitionData.MidPointAction = midPoint;
            return this;
        }
        public UITransition SetEndAction(Action endPoint)
        {
            TransitionData.EndPointAction = endPoint;
            return this;
        }
        #endregion
        
        #region Transitioning
        public void StartFullTransition(float duration)
        {
            if(DoingTransition) return;
            
            PrepareTransition(duration,() => StartCoroutine(HandleFullTransition(duration)));
        }
            protected abstract IEnumerator HandleFullTransition(float duration);
        
            
        public void StartOpenTransition(float duration)
        {
            if(DoingTransition) return;
            
            PrepareTransition(duration,() => StartCoroutine(HandleHalfTransition(duration, true)));
        }
        public void StartCloseTransition(float duration)
        {
            if(!DoingTransition) return;
            
            PrepareTransition(duration,() => StartCoroutine(HandleHalfTransition(duration, false)));
        }
            protected abstract IEnumerator HandleHalfTransition(float duration, bool isOpening);

            
        void PrepareTransition(float duration, Action transitionAction)
        {
            TransitionData.Duration = duration;
            
            TransitionData.EndPointAction += ResetProperties;
            DoingTransition = true;
            transitionAction.Invoke();
        }
        #endregion
        protected abstract void ResetProperties();
    }
}