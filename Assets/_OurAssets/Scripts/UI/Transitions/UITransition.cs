using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.UI
{
    public enum TransitionType
    {
        None,
        Stripe,
    }
    public abstract class UITransition : MonoBehaviour
    {
        protected Action MidPointAction;
        protected Action EndPointAction;
        
        protected Image Image;
        protected Material MaterialInstance;
        
        protected bool DoingTransition = false;
        #region Set Up
        protected virtual void Awake()
        {
            Image = GetComponent<Image>();
            
            MaterialInstance = Instantiate(Image.material);
            Image.material = MaterialInstance;
        }
        #endregion
        #region Builder
        public UITransition SetColor(Color color)
        {
            Image.color = color;
            return this;
        }
        public UITransition SetMidAction(Action midPoint)
        {
            MidPointAction = midPoint;
            return this;
        }
        public UITransition SetEndAction(Action endPoint)
        {
            EndPointAction = endPoint;
            return this;
        }
        #endregion
        
        #region Transitioning
        public void StartFullTransition(float duration)
        {
            if(DoingTransition) return;
            
            PrepareTransition(() => StartCoroutine(HandleFullTransition(duration)));
        }
        protected abstract IEnumerator HandleFullTransition(float duration);
        
        public void StartOpenTransition(float duration)
        {
            if(DoingTransition) return;
            
            PrepareTransition(() => StartCoroutine(HandleHalfTransition(duration, true)));
        }
        public void StartCloseTransition(float duration)
        {
            if(!DoingTransition) return;

            PrepareTransition(() => StartCoroutine(HandleHalfTransition(duration, false)));
        }
        protected abstract IEnumerator HandleHalfTransition(float duration, bool isOpening);

        void PrepareTransition(Action transitionAction)
        {
            EndPointAction += ResetProperties;
            DoingTransition = true;
            transitionAction.Invoke();
        }
        #endregion
        protected abstract void ResetProperties();
    }
}