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
