using System;
using System.Linq;
using CursedOnion.Game.General.UI.Canvases.Level;
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
        [Inject] LevelEvents levelEvents;
        
        public Flowchart Flowchart;
        public string StartingDialogBlockName;
        
        [Header("Extras")]
        [SerializeField] CanvasGroup background;
        public void Start()
        {
            Debug.LogWarning("FALTA LOGICA DE SI YA HAS VISTO EL DIALOGO");
            pauseService.Pause(PauseLevel.Dialog);
            if(!string.IsNullOrEmpty(StartingDialogBlockName))
                Flowchart.ExecuteBlock(StartingDialogBlockName);
        }
        
        public void OnDialogEnd()
        {
            levelEvents.CallIntro();
        }
        public void SetDialogBackgroundAlpha(float alpha, float time)
        {
            LeanTween.cancel(background.gameObject);
            LeanTween.alphaCanvas(background, alpha, time);
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
