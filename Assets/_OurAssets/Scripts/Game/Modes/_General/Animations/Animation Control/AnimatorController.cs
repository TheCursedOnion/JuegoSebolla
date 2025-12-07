using System;
using System.Collections.Generic;
using CursedOnion.Game.Entity;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.Animations
{
    public class AnimatorController : MonoBehaviour
    {
        [SerializeField] private List<Animator> entityAnimators = new List<Animator>();
        
        [SerializeField] private string testAnimationName;
        private SimpleEntity assignedEntity;
        private string previousAnimationName;
        public void SetupController(SimpleEntity assignedEntity)
        {
            entityAnimators?.Clear();
            this.assignedEntity = assignedEntity;
        }

        public void AddAnimator(Animator animator)
        {
            entityAnimators.Add(animator);
        }
        public void PlayAnimation(string animationName)
        {
            foreach (Animator animator in entityAnimators)
            {
                animator.Play(animationName, -1, 0f);
            }
            
        }
        public void TestPlayAnimation()
        {
            PlayAnimation(testAnimationName);
        }
        
        public void ProcessStartedAnimation(string animationName)
        {
            if(string.IsNullOrEmpty(animationName) || string.Equals(animationName, previousAnimationName)) return;
            
            previousAnimationName = animationName;
            //Debug.Log($"{assignedEntity.name}: Animation {animationName} started.");
            
            switch (animationName)
            {
                case "punch":
                case "shoot":
                    assignedEntity.BeingInspected = false;
                    break;
                
                case "idle": PlayAnimation("idle"); break;
                
                case "hurt": PlayAnimation("hurt"); break;
            }
            
            bool isIdle = string.Equals("idle", animationName);
            bool isThinking = string.Equals("think", animationName);
            
            
            if (!isIdle && !isThinking)
            {
                assignedEntity.ActionHandler.RaiseFlag(ActionFlag.IsNotIdle, false);
            }
            else
            {
                assignedEntity.ActionHandler.ResetFlag(ActionFlag.IsNotIdle, !isIdle);
                
                if(isIdle && assignedEntity.HasTurn && assignedEntity.BeingInspected) PlayAnimation("think");
                else if (isIdle) PlayAnimation("idle");
            }
            
            
                
        }
        public void ProcessAnimationEvent(string eventName)
        {
            //Debug.Log($"{assignedEntity.name}: Animation event {eventName} raised.");

            switch (eventName)
            {
                case "damage": assignedEntity.EntityController.AttackComponent.ApplyAttack(); break;
            }
        }
        public void ProcessFinishedAnimation(string animationName)
        {
            //Debug.Log($"{assignedEntity.name}: Animation {animationName} finished.");
            switch (animationName)
            {
                case "hurt":
                    if (assignedEntity.StatusHandler.IsConfused)
                    {
                        PlayAnimation("dizzy");
                    }
                    else
                    {
                        if (assignedEntity.StatusHandler.HasCounterAttackTarget(out var target))
                        {
                            assignedEntity.EntityController.AttackComponent.DoAttack(target, false);
                        }
                    }
                    break;
                
            }
            //TODO: Process finished animation
        }
        
        
    }
}
