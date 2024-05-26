using UnityEngine;
using TMPro;

public class PathfinidingDebugObject : GridDebugObject
{
    [SerializeField] private TextMeshPro _gCostTMP;
    [SerializeField] private TextMeshPro _hCostTMP;
    [SerializeField] private TextMeshPro _fCostTMP;
    [SerializeField] private SpriteRenderer _isWalkableSpriterenderer;

    private PathNode _pathNode;

    protected override void Update()
    {
        base.Update();
        _gCostTMP.text = _pathNode.GetGCost().ToString();
        _hCostTMP.text = _pathNode.GetHCost().ToString();
        _fCostTMP.text = _pathNode.GetFCost().ToString();
        if (_isWalkableSpriterenderer.gameObject.activeInHierarchy)
            _isWalkableSpriterenderer.color = _pathNode.IsWalkable() ? Color.green : Color.red;
    }

    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        _pathNode = (PathNode)gridObject;
    }

}