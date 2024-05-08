using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [SerializeField] private Transform _gridSystemVisualSinglePrefab;

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
    }
    //Temporarly in update
    private void Update()
    {
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
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform =
                    Instantiate(_gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity, transform);

                _gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        if (selectedAction != null)
            ShowGridPositionList(selectedAction.GetValidActionGridPositionList());
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
    /// Shows wanted grids
    /// </summary>
    private void ShowGridPositionList(List<GridPosition> gridPositionList)
    {
        foreach (GridPosition position in gridPositionList)
        {
            _gridSystemVisualSingleArray[position.x, position.z].Show();
        }
    }

}