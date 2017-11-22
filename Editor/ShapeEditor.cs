using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ShapeCreator))]
public class ShapeEditor : Editor
{

    ShapeCreator shapeCreator;
    SelectionInfo selectionInfo;
    bool shapeChangedSinceLastRepaint;

    public override void OnInspectorGUI()
    {
        string helpMessage = "Left click to add points.\nShift-click on point to selete.\nShift-Left Click on empty space to create new shape";
        EditorGUILayout.HelpBox(helpMessage, MessageType.Info);


        base.OnInspectorGUI();
        int shapeDeleteIndex = -1;
        shapeCreator.showShapesList = EditorGUILayout.Foldout(shapeCreator.showShapesList, "Show Shapes List");
        if (shapeCreator.showShapesList)
        {
            for (int i = 0; i < shapeCreator.shapes.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Shape" + (i + 1));

                GUI.enabled = i != selectionInfo.selectedShapeIndex;
                if (GUILayout.Button("Select"))
                {
                    selectionInfo.selectionShapeIndex = i;
                }
                if (GUILayout.Button("Delete"))
                {
                    shapeDeleteIndex = i;
                }
                GUILayout.EndHorizontal();

            }
        }
        if (shapeDeleteIndex != -1)
        {
            Undo.RecordObject(shapeCreator, "Delete Shape");
            shapeCreator.shapes.RemoveAt(shapeDeleteIndex);
            selectionInfo.selectedShapeIndex = Mathf.Clamp(selectionInfo.selectedShapeIndex,0,shapeCreator.shapes.Count-1);
        }
        if (GUI.changed)
        {
            shapeChangedSinceLastRepaint = true;
            SceneView.RepaintAll();
        }
    }

    void OnSceneGUI()
    {
        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.Repaint)
        {
            Draw();
        }
        else if (guiEvent.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else
        {
            HandleInput(guiEvent);
            if (shapeChangedSinceLastRepaint)
            {
                HandleUtility.Repaint();
            }
        }
    }

    void CreateNewShape() {
        Undo.RecordObject(shapeCreator, "Create Shape");
        shapeCreator.shapes.Add(new Shape());
        selectionInfo.selectionShapeIndex = shapeCreator.shapes.Count - 1;
    }
    void CreateNewPoint (Vector3 position) {
        bool mouseIsOverSelectedShape = selectionInfo.mouseOverShapeIndex = selectionInfo.selectedShapeIndex
        int newPointIndex = (selectionInfo.mouseIsOverLine && mouseIsOverSelectedShape) ? selectionInfo.lineIndex + 1 : shapeCreator.points.Count;
        Undo.RecordObject(shapeCreator, "Add Point");
        selectedShape.points.Insert(newPointIndex, position);
        selectionInfo.pointIndex = newPointIndex;
        selectionInfo.mouseOverShapeIndex = selectionInfo.selectionShapeIndex;
        shapeChangedSinceLastRepaint = true;

        SelectPointUnderMouse();
    }

    DeletePointUnderMouse()
    {
        Undo.RecordObject(shapeCreator, "DeletePoint");
        selectedShape.points.RemoveAt(selectionInfo.pointIndex);
        selectionInfo.isSelected = false;
        selectionInfo.mouseIsOverPoint = false;
        shapeChangedSinceLastRepaint = true;
    }

    void SelectPointUnderMouse()
    {
        selectionInfo.isSelected = true;
        selectionInfo.mouseIsOverPoint = true;
        selectionInfo.mouseIsOverLine = false;
        selectionInfo.lineIndex = -1;

        selectionInfo.positionAtStartOfDrag = selectedShape.points[selectionInfo.pointIndex];
        shapeChangedSinceLastRepaint =true;
    }

    void SelectShapeUnderMouse()
    {
        if (selectionInfo.mouseIsOverShapeIndex != -1) 
        {
            selectionInfo.selectionShapeIndex = selectionInfo.mouseOverShapeIndex;
            shapeChangedSinceLastRepaint = true;
        }
    }

    void HandleInput(Event guiEvent)
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneHeight = 0;
        float dstToDrawPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
        Vector3 mousePosition = mouseRay.GetPoint(dstToDrawPlane);

        if (guiEvent.type == EventType.mouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.Shift)
        {
            HandleShiftLeftMouseDown(mousePosition);
        }
        if (guiEvent.type == EventType.mouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseDown(mousePosition);
        }
        if (guiEvent.type == EventType.mouseUp && guiEvent.button == 0)
        {
            HandleLeftMouseUp(mousePosition);
        }
        if (guiEvent.type == EventType.mouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseDrag(mousePosition);
        }
        if (!selectionInfo.isSelected)
        {
            UpdateMouseOverInfo(mousePosition);
        }
    }

    void HandleShiftLeftMouseDown(Vector3 mousePosition) {
        if (selectionInfo.mouseIsOverPoint)
        {
            SelectShapeUnderMouse();
            DeletePointUnderMouse();
        }
        else {
            CreateNewShape();
            CreateNewPoint(mousePosition;)
        }
    }

    void HandleLeftMouseDown(Vector3 mousePosition)
    {
        if (shapeCreator.shapes.Count == 0) {
            CreateNewShape();
        }

        SelectShapeUnderMouse();

        if (!selectionInfo.mouseIsOverPoint)
        {
            SelectPointUnderMouse();
        }
        else 
        {
            CreateNewPoint(mousePosition);
        }
    }

    void HandleLeftMouseUp(Vector3 mousePosition)
    {
        if (selectionInfo.isSelected)
        {
            selectedShape.points[selectionInfo.pointIndex] = selectionInfo.positionAtStartOfDrag;
            Undo.RecordObject(shapeCreator, "Move Point");
            selectedShape.points[selectionInfo.pointIndex] = mousePosition;
            selectionInfo.isSelected = false;
            selectionInfo.pointIndex = -1;
            shapeChangedSinceLastRepaint = true;
        }
    }

    void HandleLeftMouseDrag(Vector3 mousePosition)
    {
        if (selectionInfo.isSelected)
        {
            selectedShape.points[selectionInfo.pointIndex] = mousePosition;
            shapeChangedSinceLastRepaint = true;
        }
    }

    void UpdateMouseOverInfo(Vector3 mousePosition)
    {
      int mouseOverPointIndex = -1;
      int mouseOverShapeIndex = -1;
      for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++) {
        Shape currentShape = ShapeCreator.shapes[shapeIndex];

        for (int i = 0; i < currentShape.points.Count; i++)
        {
            if (Vector3.Distance(mousePosition, currentShape.points[i]) < currentShape.handleRadius)
            {
                mouseOverPointIndex = i;
                mouseOverShapeIndex = shapeIndex;
                break;
            }
        }
      }
        if (mouseOverPointIndex != selectionInfo.pointIndex || mouseOverShapeIndex != selectionInfo.mouseOverShapeIndex)
        {
            selectionInfo.mouseOverShapeIndex = mouseOverShapeIndex;
            selectionInfo.pointIndex = mouseOverPointIndex;
            selectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;
            shapeChangedSinceLastRepaint = true;
        }
        if (selectionInfo.mouseIsOverPoint)
        {
            selectionInfo.mouseIsOverLine = false;
            selectionInfo.lineIndex = -1;
        }
        else
        {
            int mouseOverLineIndex = -1;
            float closestLineDst = shapeCreator.handleRadius;
            for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++) {
              Shape currentShape = ShapeCreator.shapes[shapeIndex];
              for (int i = 0; i < shapeCreator.points.Count; i++)
              {
                Vector3 nextPointInShape = currentShape.points[(i + 1) % currentShape.points.Count];
                float dstFromMouseToLine = HandleUtility.DistancePointToLineSegment(mousePosition.ToXZ(), currentShape.points[i].ToXZ(), nextPointInShape.ToXZ());
                if (dstFromMouseToLine < closestLineDst)
                {
                    closestLineDst = dstFromMouseToLine;
                    mouseOverLineIndex = i;
                    mouseOverShapeIndex = shapeIndex;
                }
              }
            }

            if (selectionInfo.lineIndex != mouseOverLineIndex || mouseOverShapeIndex != selectionInfo.mouseOverShapeIndex)
            {
                selectionInfo.shapeIndex = mouseOverShapeIndex;
                selectionInfo.lineIndex = mouseOverLineIndex;
                selectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
                shapeChangedSinceLastRepaint = true;
            }
        }
    }

    void Draw()
    {
      for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++) 
      {
        Shape shapeToDraw = ShapeCreator.shapes[shapeIndex];
        bool shapeIsSelected = shapeIndex = selectionInfo.selectionShapeIndex;
        bool mouseIsOverShape = shapeIndex = selectionInfo.mouseIsOverShape;
        Color deselectedShapeColor = Color.grey;

        for (int i = 0; i < shapeToDraw.points.Count; i++)
        {
            Vector3 nextPoint = shapeCreator.points[(i + 1) % shapeCreator.points.Count];
            if (i == selectionInfo.lineIndex && mouseIsOverShape)
            {
                Handles.color = Color.red;
                Handles.DrawLine(shapeToDraw.points[i], nextPoint);
            }
            else
            {
                Handles.color = (shapeIsSelected)?Color.black:deselectedShapeColor;
                Handles.DrawDottedLine(shapeToDraw.points[i], nextPoint, 4);
            }
            if (i == selectionInfo.pointIndex && mouseIsOverShape)
            {
                Handles.color = (selectionInfo.isSelected) ? Color.black : Color.red;
            }
            else
            {
                Handles.color = (shapeIsSelected)?Color.white:deselectedShapeColor;
            }

            Handles.DrawSolidDisc(shapeToDraw.points[i], Vector3.up, shapeToDraw.handleRadius);
            
          }
        }
        if (shapeChangedSinceLastRepaint)
        {
            shapeCreator.UpdateMeshDisplay()
        }
        shapeChangedSinceLastRepaint = false;
    }

    void OnEnable()
    {
        shapeChangedSinceLastRepaint = true;
        shapeCreator = target as ShapeCreator;
        selectionInfo = new SelectionInfo();
        Undo.undoRedoPerformed += onUndoOrRedo();
        Tools.hidden = true;
    }

    void OnDisable()
    {
        Undo.undoRedoPerformed -= onUndoOrRedo();
        Tools.hidden = true;
    }

    void onUndoOrRedo()
    {
        if (selectionShapeIndex >= shapeCreator.shapes.Count || selectionInfo.selectionShapeIndex == -1)
        {
            selectionInfo.selectionShapeIndex = shapeCreator.shapes.Count -1;
        }
        shapeChangedSinceLastRepaint = true;
    }

    Shape selectedShape
    {
      get {
        return shapeCreator.shapes[selectionInfo.selectionShapeIndex];
      }
    }

    public class SelectionInfo
    {
        public int selectionShapeIndex;
        public int mouseOverShapeIndex;

        public int pointIndex;
        public bool mouseIsOverPoint;
        public bool isSelected;
        public Vector3 positionAtStartOfDrag;

        public int lineIndex = -1;
        public bool mouseIsOverLine;
    }
}
