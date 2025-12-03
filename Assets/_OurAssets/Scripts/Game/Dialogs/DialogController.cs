using System;
using System.Linq;
using CursedOnion.Game.General.UI.Canvases.Level;
using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Locators;
using Fungus;
using NaughtyAttributes;
using Reflex.Attributes;
using Reflex.Extensions;
using UnityEngine;
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
        
        void TryPlayEndDialog(LevelState _, LevelState newState)
        {
            if(newState == LevelState.Finished) PlayDialog(EndDialogBlockName);
        }
        public void PlayDialog(string blockName)
        {
            pauseService.Pause(PauseLevel.Dialog);
            Flowchart.ExecuteBlock(blockName);
        }
        
        public void SetDialogBackgroundAlpha(float alpha, float time)
        {
            LeanTween.cancel(background.gameObject);
            LeanTween.alphaCanvas(background, alpha, time);
        }
        
        public void CallLevelIntro()
        {
            if (levelEvents != null)
            {
                levelEvents?.CallIntro();
            }
        }

        public void CallLevelResults()
        {
            if (levelManager != null && levelManager.CurrentLevelState == LevelState.Finished)
            {
                levelManager.TrySetNewState(LevelState.InResults);
            }
        }
        public void UnpauseGameFromDialog()
        {
            pauseService.Unpause(PauseLevel.Dialog);
        }

        public void SaveDialogId()
        {
            if (dialogId < 0) return;
            
            variableLocator?.SetLastDialogCompleted(dialogId);
        }
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
