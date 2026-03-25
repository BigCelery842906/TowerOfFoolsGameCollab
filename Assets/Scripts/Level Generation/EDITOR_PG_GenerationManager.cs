//-------------------------------------
//      Property of Dan.
//      Break it and you suffer.
//      Respectfully of course...
//-------------------------------------

using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PG_GenerationManager))]
[CanEditMultipleObjects]

public class EDITOR_PG_GenerationManager : Editor
{

    SerializedProperty m_chunks;
    SerializedProperty m_chunkScale;
    SerializedProperty m_worldScale;

    public void OnEnable()
    {
        m_chunks = serializedObject.FindProperty("m_chunksPerRoom");
        m_chunkScale = serializedObject.FindProperty("m_chunkSizeMultiplier");
        m_worldScale = serializedObject.FindProperty("m_worldScale");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        PG_GenerationManager manager = (PG_GenerationManager)target;
        EditorGUILayout.Slider(m_worldScale, 0.1f, 10.0f, new GUIContent("World Scale"));
        EditorGUILayout.IntSlider(m_chunks, 1, 10, new GUIContent("Number of Chunks"));
        EditorGUILayout.IntSlider(m_chunkScale, 1, 10, new GUIContent("Chunk Size Multiplier"));
        if (GUILayout.Button("Reset"))
        {
            manager.RegenerateRoom();
        }
        serializedObject.ApplyModifiedProperties();

    }


}
