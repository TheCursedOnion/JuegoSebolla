using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CursedOnion.Game.Modes.General.Animations.EntityAnimatorController))]
public class EntityAnimatorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var controller = (CursedOnion.Game.Modes.General.Animations.EntityAnimatorController)target;

        if (GUILayout.Button("Play Test Animation"))
        {
            controller.TestPlayAnimation();
        }
    }
}

[CustomEditor(typeof(CursedOnion.Game.Modes.General.Animations.LayeredEntity))]
public class AnimationLayerManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var manager = (CursedOnion.Game.Modes.General.Animations.LayeredEntity)target;

        if (GUILayout.Button("Play Test Animation"))
        {
            manager.TestPlayAnimation();
        }
    }
}