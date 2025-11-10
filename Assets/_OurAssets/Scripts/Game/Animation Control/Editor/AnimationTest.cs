using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CursedOnion.EntityAnimatorController))]
public class EntityAnimatorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var controller = (CursedOnion.EntityAnimatorController)target;

        if (GUILayout.Button("Play Test Animation"))
        {
            controller.TestPlayAnimation();
        }
    }
}

[CustomEditor(typeof(CursedOnion.LayeredEntity))]
public class AnimationLayerManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var manager = (CursedOnion.LayeredEntity)target;

        if (GUILayout.Button("Play Test Animation"))
        {
            manager.TestPlayAnimation();
        }
    }
}