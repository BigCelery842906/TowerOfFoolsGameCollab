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
    SerializedProperty m_spawnPowerups;
    SerializedProperty m_powerupSpawnChance;

    public void OnEnable()
    {
        m_chunks = serializedObject.FindProperty("m_chunksPerRoom");
        m_chunkScale = serializedObject.FindProperty("m_chunkSizeMultiplier");
        m_worldScale = serializedObject.FindProperty("m_worldScale");
        m_spawnPowerups = serializedObject.FindProperty("m_spawnPowerups");
        m_powerupSpawnChance = serializedObject.FindProperty("m_powerupSpawnChance");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUIStyle divider = new GUIStyle();
        divider.normal.textColor = UnityEngine.Color.white;
        divider.alignment = TextAnchor.MiddleCenter;
        divider.fontStyle = FontStyle.Bold;
        PG_GenerationManager manager = (PG_GenerationManager)target;

        //Generation Settings
        EditorGUILayout.LabelField("Generation", divider);
        EditorGUILayout.Slider(m_worldScale, 0.1f, 10.0f, new GUIContent("World Scale"));
        EditorGUILayout.IntSlider(m_chunks, 1, 10, new GUIContent("Number of Chunks"));
        EditorGUILayout.IntSlider(m_chunkScale, 1, 10, new GUIContent("Chunk Size Multiplier"));
        if (GUILayout.Button("Reset"))
        {
            manager.RegenerateRoom();
        }

        //Powerup Settings
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Powerups", divider);
        m_spawnPowerups.boolValue = GUILayout.Toggle(m_spawnPowerups.boolValue, " Spawn Powerups");
        EditorGUILayout.Slider(m_powerupSpawnChance, 0.0f, 100.0f);
        

        serializedObject.ApplyModifiedProperties();

    }


}
