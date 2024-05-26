using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public enum GridVisualType { White, Green, Yellow, Red, Orange }
[Serializable]
public struct GridVisualTypeMaterial { public GridVisualType gridVisualType; public Material material; }
public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [SerializeField] private Transform _gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> _gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,] _gridSystemVisualSingleArray;
    private int _gridSystemWidth, _gridSystemHeight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"There's more than one GridSystemVisual! {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        //needs to be called from somewhere else if grid doesn't begin on Start
        DrawGridSystem();
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        UpdateGridVisual();
    }

    /// <summary>
    /// First grid instantiation
    /// </summary>
    public void DrawGridSystem()
    {
        _gridSystemWidth = LevelGrid.Instance.GetGridWidth();
        _gridSystemHeight = LevelGrid.Instance.GetGridHeight();
        _gridSystemVisualSingleArray = new GridSystemVisualSingle[_gridSystemWidth, _gridSystemHeight];

        for (int x = 0; x < _gridSystemWidth; x++)
        {
            for (int z = 0; z < _gridSystemHeight; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z, 0);
                Transform gridSystemVisualSingleTransform =
                    Instantiate(_gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity, transform);

                _gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        GridVisualType gridVisualType;

        if (selectedAction != null)
        {
            switch (selectedAction)
            {
                default:
                case MoveAction moveAction:
                    gridVisualType = GridVisualType.White;
                    break;
                case SpinAction spinAction:
                    gridVisualType = GridVisualType.Orange;
                    break;
                case RangeAction rangeAction:
                    gridVisualType = GridVisualType.Red;
                    ShowGridPositionRangeCircle(selectedUnit.GetGridPosition(), rangeAction.GetActionRange(), GridVisualType.Orange);
                    break;
            }

            ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
        }
    }
    /// <summary>
    /// Hides all grids
    /// </summary>
    private void HideAllGridPosition()
    {
        for (int x = 0; x < _gridSystemWidth; x++)
        {
            for (int z = 0; z < _gridSystemHeight; z++)
            {
                _gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }
    /// <summary>
    /// Shows wanted grid positions by list
    /// </summary>
    private void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition position in gridPositionList)
        {
            _gridSystemVisualSingleArray[position.x, position.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }
    /// <summary>
    /// Shows wanted grid positions by GridPosition and Range (Circular)
    /// </summary>
    private void ShowGridPositionRangeCircle(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; } // if position is outside the gridSystem

                //Collective distance from position. Makes valid grid circular instead of square
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range) { continue; }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }
    /// <summary>
    /// Shows wanted grid positions by GridPosition and Range (Square)
    /// </summary>
    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; } // if position is outside the gridSystem

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }
    /// <summary>
    /// Get Material by VisualType
    /// </summary>
    /// <param name="gridVisualType"></param>
    /// <returns></returns>
    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in _gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }
        Debug.LogError($"Could NOT find GridVisualTypeMaterial for GridVisualType {gridVisualType}");
        return null;
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs empty)
    {
        UpdateGridVisual();
    }

}