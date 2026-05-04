using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(PG_PlatformContainer), true)]
[CanEditMultipleObjects]
public class EDITOR_PG_PlatformContainer : Editor
{
    PG_PlatformContainer m_container;
    private void OnEnable()
    {
        m_container = (PG_PlatformContainer)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Delete"))
        {
            m_container.DeletePlatform();
        }
    }

}
