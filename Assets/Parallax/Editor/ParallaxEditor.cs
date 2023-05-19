#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Parallax))]
public class ParallaxEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Parallax parallaxScript = (Parallax)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Create Background"))
        {
            parallaxScript.CreateGUI();
        }

        if (GUILayout.Button("Delete Background"))
        {
            parallaxScript.DeleteGUI();
        }

        GUILayout.EndHorizontal();
    }
}

#endif
