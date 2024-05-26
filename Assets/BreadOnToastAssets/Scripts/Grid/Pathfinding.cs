using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] private Transform _pathfindingDebugObjectPrefab;
    [SerializeField] private bool _pathDebugEnabled = false;
    [SerializeField] internal LayerMask _obstacleLayerMask;

    private GridSystem<PathNode> _gridSystem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"There's more than one Pathfinding! {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetUp(int width, int length, float cellSize)//Needs adressing to block more than just obstacle
    {
        _gridSystem = new GridSystem<PathNode>(width, length, cellSize,
            (GridSystem<PathNode> gridSystem, GridPosition gridPosition) => new PathNode(gridPosition));

        if (_pathDebugEnabled)
            _gridSystem.CreateDebugObjects(_pathfindingDebugObjectPrefab, transform);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float rayCastOffsetDistance = 5f;

                //If true grid has obstacle, set walkable false
                if (Physics.Raycast(worldPosition + Vector3.down * rayCastOffsetDistance, Vector3.up, rayCastOffsetDistance * 2, _obstacleLayerMask))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }

                //if true the position has an moveable layer like Unit
                //if (Physics.Raycast(worldPosition + Vector3.down * rayCastOffsetDistance, Vector3.up, rayCastOffsetDistance * 2, movableLayerMask))
                //{
                //    SetOccupiedGridPosition(gridPosition, true);
                //}

                //if (Physics.Raycast(worldPosition + Vector3.down * rayCastOffsetDistance, Vector3.up, rayCastOffsetDistance * 2, EnemyobstacleLayerMask))
                //{
                //    SetWalkableGridPositionForEnemy(gridPosition, false);
                //}
            }
        }
    }
    /// <summary>
    /// How close is it to the final Node (streight distance, no walls)
    /// </summary>
    /// <param name="gridPositionA"></param>
    /// <param name="gridPositionB"></param>
    /// <returns></returns>
    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;

        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remainingDistance = Mathf.Abs(xDistance - zDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remainingDistance;
    }
    //line 146 needs adressing
    /// <summary>
    /// Tries to find the best path to the endNode and calculates the node path when a path is found
    /// </summary>
    /// <param name="startGridPosition"></param>
    /// <param name="endGridPosition"></param>
    /// <returns></returns>
    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        //Nodes queued up for searching
        List<PathNode> openList = new List<PathNode>();
        //Nodes that have already been searched
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = _gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = _gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        //Initializing all pathfinding Nodes
        for (int x = 0; x < _gridSystem.GetGridWidth(); x++)
        {
            for (int z = 0; z < _gridSystem.GetGridHeight(); z++)
            {
                //getting position
                GridPosition gridPosition = new GridPosition(x, z);
                //Getting the pathNode
                PathNode pathNode = _gridSystem.GetGridObject(gridPosition);

                //Resetting the pathNode
                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        //Cost calculations
        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        //Cicle while we have nodes on the list (finding best path)
        while (openList.Count > 0)
        {
            //Get current node
            PathNode currentNode = GetLowestFCostPathNode(openList);

            //If best node is end node, calculate path
            if (currentNode == endNode)
            {
                //Reached final Node
                return CalculatePath(endNode);
            }

            //Reaching here indicates we already searched this node as not endNode
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //Checks all neighbours on current node for the next best node toward the end
            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                //If node already checked then skip
                if (closedList.Contains(neighbourNode)) { continue; }

                //If not walkable then skip
                if (!neighbourNode.IsWalkable()/* || neighbourNode.IsOccupied()*/)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                //Move cost from current node to neighbour node
                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                //If true, better path found to get to neighbourNode from currentNode
                if (tentativeGCost < neighbourNode.GetGCost())
                {
                    //Update neighbour data
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    //If neighbour not on the list, add so we can go though it
                    if (!openList.Contains(neighbourNode))
                        openList.Add(neighbourNode);
                }
            }
        }

        //No path found
        return null;
    }

    /// <summary>
    /// Returns the node from the grid system
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private PathNode GetNode(int x, int z)
    {
        return _gridSystem.GetGridObject(new GridPosition(x, z));
    }
    /// <summary>
    /// Returns Grid positions of the path found
    /// </summary>
    /// <param name="endNode"></param>
    /// <returns></returns>
    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        //Create path node list and start and the end
        List<PathNode> pathNodeList = new List<PathNode> { endNode };
        PathNode currentNode = endNode;

        //Has some node connected to it
        while (currentNode.GetCameFromPathNode() != null)
        {
            //Add connected node to path list
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        //Started from end so flip list
        pathNodeList.Reverse();

        //Create list for created path
        List<GridPosition> gridPositionList = new List<GridPosition>();

        //Fill gridPosition list with path data
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }
    /// <summary>
    /// Returns 8 Direction nodes around given Node if are available
    /// </summary>
    /// <param name="currentNode"></param>
    /// <returns></returns>
    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x - 1 >= 0)//Testing extra space to make sure not exiting grid
        {
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));//Left

            if (gridPosition.z + 1 < _gridSystem.GetGridHeight())//Testing extra space to make sure not exiting grid
            {
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));//Left Up
            }

            if (gridPosition.z - 1 >= 0)//Testing extra space to make sure not exiting grid
            {
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));//Left Down
            }
        }

        if (gridPosition.x + 1 < _gridSystem.GetGridWidth())//Testing extra space to make sure not exiting grid
        {
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));//Right

            if (gridPosition.z + 1 < _gridSystem.GetGridHeight())//Testing extra space to make sure not exiting grid
            {
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));//Right Up
            }

            if (gridPosition.z - 1 >= 0)//Testing extra space to make sure not exiting grid
            {
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));//Right Down
            }
        }

        if (gridPosition.z + 1 < _gridSystem.GetGridHeight())//Testing extra space to make sure not exiting grid
        {
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));//Up
        }

        if (gridPosition.z - 1 >= 0)//Testing extra space to make sure not exiting grid
        {
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));//Down
        }

        return neighbourList;
    }
    /// <summary>
    /// Getting the next closest pathNode toward the end
    /// </summary>
    /// <param name="pathNodeList"></param>
    /// <returns></returns>
    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];

        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }

        return lowestFCostPathNode;
    }

}