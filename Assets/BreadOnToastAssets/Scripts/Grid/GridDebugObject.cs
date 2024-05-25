using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro _debugObjectTMP;

    private object _gridObject;

    protected virtual void Update()
    {
        _debugObjectTMP.text = _gridObject.ToString();
    }

    public virtual void SetGridObject(object gridObject)
    {
        _gridObject = gridObject;
    }

}