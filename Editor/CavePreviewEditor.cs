using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapPreview))]
public class CavePreviewEditor : Editor
{
  public override void OnInspectorGUI()
  {
      CavePreview cavePreview = (CavePreview)target;

      if (DrawDefaultInspector())
      {
          if (cavePreview.autoUpdate)
          {
              cavePreview.DrawMapInEditor();
          }
      }

      if (GUILayout.Button("Generate"))
      {
          cavePreview.DrawMapInEditor();
      }
  }
}
