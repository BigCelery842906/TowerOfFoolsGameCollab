//-------------------------------------
//      Property of Dan.
//      Break it and you suffer.
//      Respectfully of course...
//-------------------------------------

using NUnit.Framework;
using System.Drawing;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PG_PlatformGenerator), true)]
[CanEditMultipleObjects]

public class EDITOR_PG_PlatformGenerator : Editor
{

    SerializedProperty end;
    SerializedProperty middle;
    SerializedProperty xPos;
    SerializedProperty yPos;
    SerializedProperty platformSize;

    PG_GridMap m_currentGrid;
    PG_PlatformGenerator m_generator;
    GUIStyle m_warningStyle;
    void OnEnable()
    {

        end = serializedObject.FindProperty("m_endToSpawn");
        middle = serializedObject.FindProperty("m_middleToSpawn");
        xPos = serializedObject.FindProperty("m_xSpawnLocation");
        yPos = serializedObject.FindProperty("m_ySpawnLocation");
        platformSize = serializedObject.FindProperty("m_platformSpawnSize");



    }
    public override void OnInspectorGUI()
    {
        m_generator = (PG_PlatformGenerator)target;
        var gridProp = serializedObject.FindProperty("m_currentGrid");
        m_currentGrid = gridProp != null ? gridProp.objectReferenceValue as PG_GridMap : null;
        PG_PlatformGenerator generator = (PG_PlatformGenerator)target;
        DrawDefaultInspector();
        {
            GUIStyle horizontalLine;
            horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            horizontalLine.margin = new RectOffset(0, 0, 4, 4);
            horizontalLine.fixedHeight = 1;
            UnityEngine.Color c = GUI.color;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;

        }
        GUILayout.Label("Spawn Individual Platforms");

        EditorGUILayout.PropertyField(end);
        EditorGUILayout.PropertyField(middle);
        if (m_currentGrid)
        {
            int maxX = Mathf.Max(1, m_currentGrid.m_width - 1);
            int maxY = Mathf.Max(1, m_currentGrid.m_height - 1);
            int currentX = (xPos != null) ? xPos.intValue : 1;
            int currentY = (yPos != null) ? yPos.intValue : 1;
            int maxSize = Mathf.Max(1, m_currentGrid.m_width - currentX);


            xPos.intValue = EditorGUILayout.IntSlider(new GUIContent("X Spawn Location"), currentX, 1, maxX);
            yPos.intValue = EditorGUILayout.IntSlider(new GUIContent("Y Spawn Location"), yPos.intValue, 1, maxY);
            platformSize.intValue = EditorGUILayout.IntSlider(new GUIContent("Platform Size"), platformSize.intValue, 1, maxSize - 1);
            if(GUILayout.Button(new GUIContent("Spawn Platform")))
            {
                generator.SpawnPlatformAtCoords(generator.m_xSpawnLocation, generator.m_ySpawnLocation, generator.m_platformSpawnSize);
            }
            GUILayout.Space(20);
            if(m_warningStyle == null)
            {
                m_warningStyle = new GUIStyle();
            }
            m_warningStyle.normal.textColor = UnityEngine.Color.red;
            GUILayout.Label("WARNING - DO NOT USE IN CONJUNCTION WITH PLATFORM CONTAINER DELETE BUTTON", m_warningStyle);
            GUILayout.Label("ONLY USE IMMEDIATELY AFTER CREATION OF A PLATFORM", m_warningStyle);
            if(GUILayout.Button(new GUIContent("Undo Platform Placement")))
            {
                generator.UndoPlatformPlacement();
            }

        }
        serializedObject.ApplyModifiedProperties();
    }
    public void OnSceneGUI()
    {

        if (m_generator == null) return;
        PG_GridMap grid = m_generator.m_currentGrid;
        if (grid == null) return;

        int currentX = m_generator.m_xSpawnLocation;
        int currentY = m_generator.m_ySpawnLocation;
        int size = m_generator.m_platformSpawnSize;
        float worldScale = grid.GetWorldScale();

        UnityEngine.Color prev = Handles.color;
        Handles.color = UnityEngine.Color.cyan;
        if(grid.m_roomComplete)
        {

        for (int i = 0; i < size; i++)
        {
            
            Vector2 pos = grid.GetWorldPosFromCell(currentX + i, currentY);
            Vector3 wireframePos = new Vector3(pos.x, pos.y, 0f);
            Vector3 wireframeSize = Vector3.one * worldScale;
            Handles.DrawWireCube(wireframePos, wireframeSize);
        }
        }

        Handles.color = prev;
    }
}
