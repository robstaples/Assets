using UnityEngine;
using System.Collections;

public class UpdatableData : ScriptableObject
{

    public event System.Action OnValuesUpdated;
    [Tooltip("Option to automatically apply updates in real time to the editor")]
    public bool autoUpdate;

#if UNITY_EDITOR

    protected virtual void OnValidate()
    {
        if (autoUpdate)
        {
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
        }
    }

    public void NotifyOfUpdatedValues()
    {
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }

#endif

}
