//-------------------------------------
//      Property of Dan.
//      Break it and you suffer.
//      Respectfully of course...
//-------------------------------------

using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PG_PlatformParent), true)]
[CanEditMultipleObjects]


public class EDITOR_PG_PlatformParent : Editor
{
    

    void OnEnable()
    {
        
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PG_PlatformParent platform = (PG_PlatformParent)target;

        if (GUILayout.Button("Unfreeze"))
        {
            
            platform.EnableGravity();
        }
        if(GUILayout.Button("Spawn Random Powerup"))
        {
            GameObject room = platform.transform.parent.gameObject;
            platform.SpawnPowerup(room);
        }
    }
}
