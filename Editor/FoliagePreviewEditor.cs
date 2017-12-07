using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FoliagePreview))]
public class FoliagePreviewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FoliagePreview foliagePreview = (FoliagePreview)target;

        if (DrawDefaultInspector())
        {
            if (foliagePreview.autoUpdate)
            {
                foliagePreview.DrawInEditor();
            }
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            foliagePreview.DrawInEditor();
        }
        if (GUILayout.Button("Destroy All Objects"))
        {
            foliagePreview.DestroyAllObjects();
        }
        GUILayout.EndHorizontal();
    }
}