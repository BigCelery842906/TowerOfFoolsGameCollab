using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PG_TransitionManager), true)]
public class EDITOR_PG_TransitionManager : Editor
{
    void OnEnable()
    {

    }
    public override void OnInspectorGUI()
    {
        PG_TransitionManager manager = target as PG_TransitionManager;
        DrawDefaultInspector();
        if(GUILayout.Button("Generate Next Room"))
        {
            manager.GenerateNextRoom();
        }
    }
}
