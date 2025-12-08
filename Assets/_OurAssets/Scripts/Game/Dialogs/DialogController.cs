using System;
using System.Linq;
using Ami.BroAudio;
using CursedOnion.Game.Audio;
using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Settings;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Locators;
using Fungus;
using Reflex.Attributes;
using Reflex.Core;
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
        RuntimeVariableLocator variableLocator;
        AudioGallery audioGallery;
        GameSettings gameSettings;
        PauseService pauseService;
        LevelManager levelManager;
        LevelEvents levelEvents;
        
        public Flowchart Flowchart;

        [Header("Dialog Music")]
        [SerializeField] private SoundID dialogMusic;
        
        [Header("Extras")]
        [SerializeField] private Text nameText;
        [SerializeField] private CanvasGroup mainCanvasGroup;
        [SerializeField] CanvasGroup background;

        private int processedDialogID;
        public void Initialize()
        {
            var container = gameObject.scene.GetSceneContainer();
            variableLocator = container.Resolve<RuntimeVariableLocator>();
            gameSettings = container.Resolve<GameSettings>();
            pauseService = container.Resolve<PauseService>();
            audioGallery = container.Resolve<AudioGallery>();
            
            Debug.Log(pauseService != null);
            
            DontDestroyOnLoad(gameObject);
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
        public void RequestPlayMusic(SoundID music)
        {
            audioGallery.PlayMusic(music);
        }
        public void RequestStopMusic()
        {
            audioGallery.StopAllMusic();
        }
        #endregion
        
        #region PlayDialog
        public bool PlayDialog(DialogBlock dialogBlock, Container sceneContainer)
        {
            string blockName = dialogBlock.Name + GetLanguageTermination();
            if (dialogBlock.ID >= 0 && variableLocator.LastDialogCompleted >= dialogBlock.ID || !Flowchart.HasBlock(blockName)) return false;
            
            processedDialogID = dialogBlock.ID;
            
            if (sceneContainer.HasBinding<LevelManager>())
            {
                levelManager = sceneContainer.Resolve<LevelManager>();
                levelEvents = levelManager.LevelEvents;
            }
            
            pauseService.Pause(PauseLevel.Dialog);
            Flowchart.ExecuteBlock(blockName);
            RequestPlayMusic(dialogMusic);
            return true;
        }
        private string GetLanguageTermination()
        {
            var language = gameSettings?.LanguageSettings.GetCurrentLanguage();
            return language switch
            {
                LanguageSetting.Language.Spanish => "",
                _ => "_en"
            };
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
            Debug.Log("Unpausing game from dialog");
            pauseService.Unpause(PauseLevel.Dialog);
        }
        void CallLevelIntro()
        {
            levelEvents?.CallIntro();
        }

        void CallLevelResults()
        {
            levelManager?.TrySetNewState(LevelState.InResults);
        }
        void SaveDialogId()
        {
            if (processedDialogID < 0) return;
            variableLocator?.SetLastDialogCompleted(processedDialogID);
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
