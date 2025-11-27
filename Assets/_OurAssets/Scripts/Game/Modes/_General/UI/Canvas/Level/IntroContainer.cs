using System.Collections;
using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Canvases.Level
{
    public class IntroContainer : MonoBehaviour
    {
        [Inject] PauseService pauseService;
        [Inject] LevelManager levelManager;
        
        [SerializeField] private CanvasGroup introGroup;
        [SerializeField] private float introOnAwakeDelay = 1f;
        [SerializeField] private float fadeTimes = 1f;
        
        bool isIntroActive = false;

        bool doingExit = false;
        bool hasEntered = false;
        bool hasExited = false;

        void Awake()
        {
            if (levelManager.CurrentLevelState != LevelState.InDialog)
            {
                StartCoroutine(IEStartIntroAfterDelay(introOnAwakeDelay));
            }
            levelManager.LevelEvents.OnIntroCalled += StartIntroAnimation;
        }
        void OnDisable()
        {
            levelManager.LevelEvents.OnIntroCalled -= StartIntroAnimation;
        }

        IEnumerator IEStartIntroAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartIntroAnimation();
        }
        public void StartIntroAnimation()
        {
            isIntroActive = true;
            hasEntered = hasExited = false;
            Enter(fadeTimes);
        }
        void Enter(float time)
        {
            LeanTween.cancel(introGroup.gameObject);
            LeanTween.alphaCanvas(introGroup, 1, time).setOnComplete(() =>
            {
                hasEntered = true;
            });
        }
        void SkipEnter()
        {
            Enter(0);
        }

        void Exit(float time)
        {
            doingExit = true;
            LeanTween.cancel(introGroup.gameObject);
            LeanTween.alphaCanvas(introGroup, 0, time).setOnComplete(() =>
                {
                    hasExited = true;
                    StartGame();
                }
                );
        }
        void SkipExit()
        {
            Exit(0);
        }
        
        void StartGame()
        {
            levelManager.LevelEvents.InvokeIntroFinished();
            levelManager?.SetNewLevelState(LevelState.InBattleEditor);
            
            pauseService.UnpauseCurrentLevel();
            this.gameObject.SetActive(false);
        }
        void Update()
        {
            if (hasEntered && hasExited || !isIntroActive)
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (!hasEntered)
                {
                    SkipEnter();
                }
                else if(!hasExited)
                {
                    if(!doingExit)
                        Exit(fadeTimes);
                    else
                        SkipExit();
                }
            }
        }
    }
}