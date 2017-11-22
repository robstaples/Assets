using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ShapeCreator))]
public class ShapeEditor : Editor
{

    ShapeCreator shapeCreator;
    SelectionInfo selectionInfo;
    bool needsRepaint;

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
            if (needsRepaint)
            {
                HandleUtility.Repaint();
            }
        }
    }

    void CreateNewShape() {
        shapeCreator.shapes.Add(new Shape());
        selectionInfo.selectioninfo
    }

    void HandleInput(Event guiEvent)
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneHeight = 0;
        float dstToDrawPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
        Vector3 mousePosition = mouseRay.GetPoint(dstToDrawPlane);

        if (guiEvent.type == EventType.mouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseDown(mousePosition);
        }
        if (guiEvent.type == EventType.mouseUp && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
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

    void HandleLeftMouseDown(Vector3 mousePosition)
    {
        if (!selectionInfo.mouseIsOverPoint)
        {
            int newPointIndex = (selectionInfo.mouseIsOverLine) ? selectionInfo.lineIndex + 1 : shapeCreator.points.Count;
            Undo.RecordObject(shapeCreator, "Add Point");
            selectedShape.points.Insert(newPointIndex, mousePosition);
            selectionInfo.pointIndex = newPointIndex;
        }

        selectionInfo.isSelected = true;
        needsRepaint = true;
        selectionInfo.positionAtStartOfDrag = mousePosition;
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
            needsRepaint = true;
        }
    }

    void HandleLeftMouseDrag(Vector3 mousePosition)
    {
        if (selectionInfo.isSelected)
        {
            selectedShape.points[selectionInfo.pointIndex] = mousePosition;
            needsRepaint = true;
        }
    }

    void UpdateMouseOverInfo(Vector3 mousePosition)
    {
      int mouseOverPointIndex = -1;
      for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++) {
        Shape currentShape = ShapeCreator.shapes[shapeIndex];

        for (int i = 0; i < currentShape.points.Count; i++)
        {
            if (Vector3.Distance(mousePosition, currentShape.points[i]) < currentShape.handleRadius)
            {
                mouseOverPointIndex = i;
                break;
            }
        }
      }
        if (mouseOverPointIndex != selectionInfo.pointIndex)
        {
            selectionInfo.pointIndex = mouseOverPointIndex;
            selectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;
            needsRepaint = true;
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
                }
              }
            }

            if (selectionInfo.lineIndex != mouseOverLineIndex)
            {
                selectionInfo.lineIndex = mouseOverLineIndex;
                selectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
                needsRepaint = true;
            }
        }
    }

    void Draw()
    {
      for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++) {
        Shape shapeToDraw = ShapeCreator.shapes[shapeIndex];
        for (int i = 0; i < shapeToDraw.points.Count; i++)
        {
            Vector3 nextPoint = shapeCreator.points[(i + 1) % shapeCreator.points.Count];
            if (i == selectionInfo.lineIndex)
            {
                Handles.color = Color.red;
                Handles.DrawLine(shapeToDraw.points[i], nextPoint);
            }
            else
            {
                Handles.color = Color.black;
                Handles.DrawDottedLine(shapeToDraw.points[i], nextPoint, 4);
            }
            if (i == selectionInfo.pointIndex)
            {
                Handles.color = (selectionInfo.isSelected) ? Color.black : Color.red;
            }
            else
            {
                Handles.color = Color.white;
            }

            Handles.DrawSolidDisc(shapeToDraw.points[i], Vector3.up, shapeToDraw.handleRadius);
            needsRepaint = false;
          }
        }
    }

    void OnEnable()
    {
        shapeCreator = target as ShapeCreator;
        selectionInfo = new SelectionInfo();
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
        public int pointIndex;
        public bool mouseIsOverPoint;
        public bool isSelected;
        public Vector3 positionAtStartOfDrag;

        public int lineIndex = -1;
        public bool mouseIsOverLine;
    }
}
