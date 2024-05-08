using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro _debugObjectTMP;

    private GridObject _gridObject;

    private void Update()
    {
        _debugObjectTMP.text = _gridObject.ToString();
    }

    public void SetGridObject(GridObject gridObject)
    {
        _gridObject = gridObject;
    }

}