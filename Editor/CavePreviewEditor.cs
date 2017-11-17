using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapPreview))]
public class CavePreviewEditor : Editor
{
  public override void OnInspectorGUI()
  {
      CavePreview mapPreview = (CavePreview)target;

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
