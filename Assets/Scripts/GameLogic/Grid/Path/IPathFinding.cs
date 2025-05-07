using System;
using System.Collections.Generic;
using System.Linq;
using MyFramework.DataStructure;
using UnityEngine;

namespace GameLogic.Grid.Path
{
    public interface IPathFinding
    {
        // 获取到某个位置的路径，并选择是否包含终点
        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, bool includeStart, bool includeEnd);
        public List<Vector2Int> GetAllMoveableArea(Vector2Int startPos, int moveRange)
        {
            var pathTreeNodes = CalculateAllMoveablePath(startPos, moveRange);
            return pathTreeNodes.Select(node => node.Coord).ToList();
        }
        public List<PathTreeNode> CalculateAllMoveablePath(Vector2Int startPos, int moveRange);
    }

    public class PathTreeNode
    {
        public Vector2Int Coord;
        public PathTreeNode Parent;

        public List<Vector2Int> ToPathWayList(bool isReverse = false)
        {
            var reverseList = new List<Vector2Int>();
            var curNode = this;
            while (curNode != null)
            {
                reverseList.Add(curNode.Coord);
                curNode = curNode.Parent;
            }
            if (!isReverse) reverseList.Reverse();
            return reverseList;
        }
    }
    
    public class PathFindImpl : IPathFinding
    {
        private readonly IGridManager _gridManager;
        private bool IsGridExist(Vector2Int coord) => _gridManager.IsGridExist(coord.x, coord.y);
        private bool IsGridExist(int x, int y) => _gridManager.IsGridExist(x, y);
        
        private static readonly Vector2Int[] Directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        
        public PathFindImpl(IGridManager gridManager)
        {
            _gridManager = gridManager;
        }

        #region A*寻路相关
        
        // 使用 readonly struct 提升性能（C# 7.2+）
        public class Node : IComparable<Node>, IEquatable<Node>
        {
            public readonly Vector2Int Coord;
            // G值：从起点到当前节点的实际代价
            public int GCost;
            // H值：从当前节点到终点的预估代价
            public int HCost;

            public Node From;
            
            // F值：G值 + H值, 表示从起点到终点的预估总代价
            public int FCost { get; private set; }

            public Node(Vector2Int coord, int gCost, int hCost)
            {
                Coord = coord;
                GCost = gCost;
                HCost = hCost;
                FCost = gCost + hCost;
            }

            // 调试友好输出
            public override string ToString() => 
                $"Pos:{Coord} G:{GCost} H:{HCost} F:{FCost}";

            public int CompareTo(Node other)
            {
                return FCost.CompareTo(other.FCost);
            }

            public bool Equals(Node other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                return Coord.Equals(other.Coord);
            }
        }
        
        // 使用SortedList来管理开放列表，按F值排序
        private readonly PriorityQueue<Node> _costPriorityQueue = new();
        // 使用HashSet来管理关闭列表，避免重复节点
        private readonly Dictionary<Vector2Int, Node> _visitedNode = new ();
        
        // 使用过A*算法
        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, bool includeStart, bool includeEnd)
        {
            _costPriorityQueue.Clear();
            _visitedNode.Clear();
            
            var startNode = new Node(start, 0, Heuristic(start, end));
            _costPriorityQueue.Enqueue(startNode);
            
            while (_costPriorityQueue.Count > 0)
            {
                // 2.1 找到F值最小的节点
                var currentNode = _costPriorityQueue.Dequeue();
                _visitedNode.Add(currentNode.Coord, currentNode);
                
                // 2.2 检查是否到达目标节点
                if (currentNode.Coord == end)
                {
                    return RetracePath(currentNode, includeStart, includeEnd);
                }
                
                // 2.3 扩展邻居节点
                foreach (var direction in Directions)
                {
                    var neighborCoord = currentNode.Coord + direction;
                    if (!IsGridExist(neighborCoord)) continue;
                    if (_visitedNode.TryGetValue(neighborCoord, out var neighborNode))
                    {
                        // 如果该节点已经在关闭列表中，并且F值更小，则跳过
                        if (_visitedNode[neighborCoord].FCost <= currentNode.FCost)
                        {
                            continue;
                        }

                        // 更新该节点
                        neighborNode.GCost = currentNode.GCost + GetCost(currentNode.Coord, neighborCoord);
                        neighborNode.From = currentNode;
                        // 添加至探索列表
                        _costPriorityQueue.Enqueue(neighborNode);
                    }
                    else
                    {
                        // 计算G值
                        var gCost = currentNode.GCost + GetCost(currentNode.Coord, neighborCoord);
                        var hCost = Heuristic(neighborCoord, end);
                        var newNode = new Node(neighborCoord, gCost, hCost)
                        {
                            From = currentNode
                        };
                        _costPriorityQueue.Enqueue(newNode);
                    }
                }
            }
            return new List<Vector2Int>(); // 如果没有路径，返回空列表
        }

        // 启发式函数
        private int Heuristic(Vector2Int start, Vector2Int end)
        {
            // 曼哈顿距离
            return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
        }
        
        private int GetCost(Vector2Int start, Vector2Int end)
        {
            // 曼哈顿距离
            return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
        }
        
        private List<Vector2Int> RetracePath(Node endNode, bool includeStart, bool includeEnd)
        {
            var path = new List<Vector2Int>();
            var currentNode = endNode;
            while (currentNode != null)
            {
                path.Add(currentNode.Coord);
                currentNode = currentNode.From;
            }
            path.Reverse();
            
            // 如果不包含起点和终点，则移除
            if (!includeStart) path.RemoveAt(0);
            if (!includeEnd) path.RemoveAt(path.Count - 1);
            
            return path;
        }

        #endregion

        public List<PathTreeNode> CalculateAllMoveablePath(Vector2Int startPos, int moveRange)
        {
            var result = new List<PathTreeNode>();
            var queue = new Queue<PathTreeNode>();
            var visited = new HashSet<Vector2Int>();
            var startNode = new PathTreeNode
            {
                Coord = new Vector2Int(startPos.x, startPos.y), 
                Parent = null
            };
            result.Add(startNode);
            queue.Enqueue(startNode);
            visited.Add(startPos);
            var curStep = 0;
            while (queue.Count > 0)
            {
                var curNodeCount = queue.Count;
                if (++curStep > moveRange) break;
                for (var i = 0; i < curNodeCount; i++)
                {
                    var curNode = queue.Dequeue();
                    var curCoord = curNode.Coord;
                    var curHeight = _gridManager.QueryHeight(curCoord.x, curCoord.y);
                    foreach (var direction in Directions)
                    {
                        var nextX = curCoord.x + direction.x;
                        var nextY = curCoord.y + direction.y;
                        var nextCoord = new Vector2Int(nextX, nextY);
                        if (!IsGridExist(nextCoord) || visited.Contains(nextCoord)) continue;
                        var nextHeight = _gridManager.QueryHeight(curCoord.x, curCoord.y);
                        if (Mathf.Abs(curHeight - nextHeight) > 1) continue;
                        var nextNode = new PathTreeNode
                        {
                            Coord = new Vector2Int(nextCoord.x, nextCoord.y),
                            Parent = curNode
                        };
                        result.Add(nextNode);
                        queue.Enqueue(nextNode);
                        visited.Add(nextCoord);
                    }
                }
            }
            return result;
        }

        // /// 检查格子是否可通行
        // private bool IsGridValid(Vector2Int coord)
        // {
        //     // 首先检查是否在范围内
        //     if (!InRange(coord)) return false;
        //     // 检查格子是否被阻挡
        //     var grid = _gridManager.GetGridController(coord);
        //     if (grid == null) return false;
        //     if (grid.RuntimeData.EntitiesOnThis == null) return false;
        //     
        //     return true;
        // }
    }
}