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
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Unfreeze"))
        {
            PG_PlatformParent platform = (PG_PlatformParent)target;
            platform.EnableGravity();
        }
    }
}
