using System;
using System.Linq;
using CursedOnion.Game.Audio;
using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Locators;
using Fungus;
using Reflex.Attributes;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace CursedOnion.Game.Dialog
{
    public class DialogController : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator variableLocator;
        [Inject] PauseService pauseService;
        
        LevelManager levelManager;
        LevelEvents levelEvents;
        
        public Flowchart Flowchart;
        public string StartingDialogBlockName;
        public string EndDialogBlockName;

        [Header("Extras")]
        [SerializeField] private Text nameText;
        [SerializeField] private CanvasGroup mainCanvasGroup;
        [SerializeField] CanvasGroup background;
        
        [Header("DialogData")]
        [SerializeField] int dialogId = -1;
        public void Start()
        {
            if (dialogId >= 0 && variableLocator.LastDialogCompleted >= dialogId) return;
            
            var container = gameObject.scene.GetSceneContainer();
            if (container.HasBinding<LevelManager>())
            {
                levelManager = container.Resolve<LevelManager>();
                levelEvents = levelManager.LevelEvents;
                levelEvents.OnLevelStateChange += TryPlayEndDialog;

                if (!string.IsNullOrEmpty(StartingDialogBlockName) && levelManager.CurrentLevelState == LevelState.InDialog)
                {
                    PlayDialog(StartingDialogBlockName);
                    return;
                }
            }
            else
            {
                if(!string.IsNullOrEmpty(StartingDialogBlockName))
                    PlayDialog(StartingDialogBlockName);
            }
        }

        void OnDestroy()
        {
            if (levelEvents != null)
            {
                levelEvents.OnLevelStateChange -= TryPlayEndDialog;
            }
        }

        #region Special Functions
        public void SetDialogCanvasAlpha(float alpha, float time)
        {
            LeanTween.cancel(mainCanvasGroup.gameObject);
            LeanTween.alphaCanvas(mainCanvasGroup, alpha, time);
        }
        public void SetDialogBackgroundAlpha(float alpha, float time)
        {
            LeanTween.cancel(background.gameObject);
            LeanTween.alphaCanvas(background, alpha, time);
        }
        #endregion
        
        #region MusicFunctions
        public void RequestPlayMusic(MusicType musicType)
        {
            variableLocator.MusicPlayer.RequestMusic(musicType);
        }
        public void RequestStopMusic()
        {
            variableLocator.MusicPlayer.StopMusic();
        }
        #endregion
        
        #region PlayDialog
        void TryPlayEndDialog(LevelState _, LevelState newState)
        {
            if(newState == LevelState.Finished) PlayDialog(EndDialogBlockName);
        }
        public void PlayDialog(string blockName)
        {
            pauseService.Pause(PauseLevel.Dialog);
            Flowchart.ExecuteBlock(blockName);
            RequestPlayMusic(MusicType.Dialog);
        }
        #endregion

        #region EndDialog
        public void EndMapDialog()
        {
            pauseService.Unpause(PauseLevel.Dialog);
            SaveDialogId();
        }
        public void EndLevelDialog(bool isIntro)
        {
            pauseService.Unpause(PauseLevel.Dialog);
            if(isIntro) CallLevelIntro();
            else CallLevelResults();

            SaveDialogId();
        }
        void UnpauseGameFromDialog()
        {
            pauseService.Unpause(PauseLevel.Dialog);
        }
        void CallLevelIntro()
        {
            levelEvents.CallIntro();
        }

        void CallLevelResults()
        {
            levelManager.TrySetNewState(LevelState.InResults);
        }
        void SaveDialogId()
        {
            if (dialogId < 0) return;
            
            variableLocator?.SetLastDialogCompleted(dialogId);
        }
        #endregion
    }

    /*#if UNITY_EDITOR
        [CustomEditor(typeof(DialogController))]
        public class DialogControllerEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                var script = (DialogController)target;

                script.Flowchart = (Flowchart)EditorGUILayout.ObjectField(
                    "Flowchart", script.Flowchart, typeof(Flowchart), true);

                if (script.Flowchart != null)
                {
                    var blocks = script.Flowchart.GetComponents<Block>();
                    string blockName = "";
                    if (blocks.Length > 0)
                    {
                        string[] names = blocks.Select(b => b.BlockName).ToArray();

                        int index = Mathf.Max(0, Array.IndexOf(names, script.StartingDialogBlockName));
                        int newIndex = EditorGUILayout.Popup("Block", index, names);
                        blockName = names[newIndex];
                    }
                    script.StartingDialogBlockName = blockName;
                }

                script.Background = (CanvasGroup)EditorGUILayout.ObjectField("Background", script.Background, typeof(CanvasGroup), true);
                EditorUtility.SetDirty(script);
            }
        }
    #endif*/
}
