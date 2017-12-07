using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(WorldPreview))]
public class MapPreviewEditor : Editor
{

    public override void OnInspectorGUI()
    {
        WorldPreview mapPreview = (WorldPreview)target;

        if (DrawDefaultInspector())
        {
            if (mapPreview.autoUpdate)
            {
                mapPreview.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapPreview.DrawMapInEditor();
        }
    }
}