using UnityEngine;
using TMPro;

public class PathfinidingDebugObject : GridDebugObject
{
    [SerializeField] private TextMeshPro _gCostTMP;
    [SerializeField] private TextMeshPro _hCostTMP;
    [SerializeField] private TextMeshPro _fCostTMP;

    private PathNode _pathNode;

    protected override void Update()
    {
        base.Update();
        _gCostTMP.text = _pathNode.GetGCost().ToString();
        _hCostTMP.text = _pathNode.GetHCost().ToString();
        _fCostTMP.text = _pathNode.GetFCost().ToString();
    }

    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        _pathNode = (PathNode)gridObject;
    }

}