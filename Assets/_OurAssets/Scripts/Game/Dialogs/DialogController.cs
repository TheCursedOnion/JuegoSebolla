using System;
using System.Linq;
using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Systems.Level;
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
        [Inject] PauseService pauseService;
        public Flowchart Flowchart;
        public CanvasGroup Background;
        
        public string StartingDialogBlockName;
        public void Start()
        {
            Debug.LogWarning("FALTA LOGICA DE SI YA HAS VISTO EL DIALOGO");
            pauseService.Pause(PauseLevel.Dialog);
            if(!string.IsNullOrEmpty(StartingDialogBlockName))
                Flowchart.ExecuteBlock(StartingDialogBlockName);
        }
        
        public void OnDialogEnd()
        {
            pauseService.UnpauseCurrentLevel();

            var levelManager = gameObject.scene.GetSceneContainer().Resolve<LevelManager>();
            levelManager?.SetNewLevelState(LevelState.InBattleEditor);
        }
        
        public void SetBackgroundAlpha(float alpha) => Background.alpha = alpha;

        public void SetBackgroundAlpha(float alpha, float time)
        {
            LeanTween.cancel(Background.gameObject);
            LeanTween.alphaCanvas(Background, alpha, time);
        }
    }

    #if UNITY_EDITOR
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
    #endif
}
