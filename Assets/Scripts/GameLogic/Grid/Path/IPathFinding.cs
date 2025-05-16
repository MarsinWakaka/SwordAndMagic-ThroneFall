using System;
using System.Collections.Generic;
using System.Linq;
using MyFramework.Algorithm;
using MyFramework.DataStructure;
using MyFramework.Utilities.Extensions;
using UnityEngine;

namespace GameLogic.Grid.Path
{
    public interface IPathFinding
    {
        /// <summary>
        /// 寻找找到一条从A通往B的路径
        /// </summary>
        /// <param name="a">起点</param>
        /// <param name="b">终点，如果终点被占用，则返回NULL</param>
        /// <param name="includeStart"></param>
        /// <returns>如果没有路径则返回NULL</returns>
        public List<Vector2Int> FindPath(Vector2Int a, Vector2Int b, bool includeStart = false);
        
        /// <summary>
        /// 找到一条最接近目标点的路径
        /// </summary>
        /// <param name="a">起点</param>
        /// <param name="b">终点</param>
        /// <param name="includeStart"></param>
        /// <param name="distThreshold">当节点与目标相差的曼哈顿距离小于等于指定值时直接返回结果,等于0时退化为找到目的地算法</param>
        /// <returns>返回路径列表，起点与终点一致时，返回空路径列表，如果没有路径则返回NULL</returns>
        public List<Vector2Int> TryFindPathToCloseTarget(Vector2Int a, Vector2Int b, int distThreshold, bool includeStart = false);
        public List<Vector2Int> GetAllMoveableArea(Vector2Int startPos, int moveRange)
        {
            var pathTreeNodes = CalculateAllMoveablePath(startPos, moveRange);
            return pathTreeNodes.Select(node => node.Coord).ToList();
        }
        public List<PathTreeNode> CalculateAllMoveablePath(Vector2Int startPos, int moveRange, bool includeStart = false);
    }

    public class PathTreeNode: IProvideCoordinate
    {
        public Vector2Int Coord;
        public PathTreeNode Parent;

        /// <summary>
        /// 反向推导得到路径
        /// </summary>
        /// <param name="includeStart">如果需要包含起点，则返回的列表中第一个元素为起点</param>
        /// <returns></returns>
        public List<Vector2Int> ToPathWayList(bool includeStart = false)
        {
            var reverseList = new List<Vector2Int>();
            var curNode = this;
            while (curNode != null)
            {
                reverseList.Add(curNode.Coord);
                curNode = curNode.Parent;
            }
            if (!includeStart) reverseList.RemoveAt(reverseList.Count - 1);
            reverseList.Reverse();
            return reverseList;
        }

        public Vector2Int Coordinate => Coord;
    }
    
    public class PathFindImpl : IPathFinding
    {
        private readonly IGridManager _gridManager;
        private bool CanMoveTo(Vector2Int coord) => _gridManager.CanMoveTo(coord.x, coord.y);
        
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

            private int _gCost;

            /// G值：从起点到当前节点的实际代价
            public int GCost
            {
                get => _gCost;
                set
                {
                    _gCost = value;
                    FCost = _gCost + HCost;
                }
            }
            /// H值：从当前节点到终点的启发(预估)代价(通常不更新)
            public readonly int HCost;

            public Node From;
            
            /// F值：G值 + H值, 表示从起点到终点的预估总代价
            public int FCost { get; private set; }

            public Node(Vector2Int coord, int gCost, int hCost)
            {
                Coord = coord;
                HCost = hCost;
                GCost = gCost;
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
        
        /// <summary>
        /// 优先队列，存储待访问的节点
        /// </summary>
        private readonly PriorityQueue<Node> _costPq = new();
        /// <summary>
        /// 使用HashSet来管理关闭列表，避免重复节点
        /// </summary>
        private readonly Dictionary<Vector2Int, Node> _visitedNode = new ();
        
        /// <summary>
        /// 使用过A*算法，尝试寻找找到一条从A通往B的路径
        /// </summary>
        /// <param name="a">起点</param>
        /// <param name="b">终点，如果终点被占用，则返回NULL</param>
        /// <param name="includeStart"></param>
        /// <returns>如果没有路径则返回NULL</returns>
        public List<Vector2Int> FindPath(Vector2Int a, Vector2Int b, bool includeStart = false)
        {
            return TryFindPathToCloseTarget(a, b, 0, includeStart);
        }

        /// <summary>
        /// 找到一条最接近目标点的路径
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="includeStart"></param>
        /// <param name="distThreshold">当节点与目标相差的曼哈顿距离小于等于指定值时直接返回结果,等于0时退化为找到目的地算法</param>
        /// <returns>返回路径列表，起点与终点一致时，返回空路径列表，如果没有路径则返回NULL</returns>
        public List<Vector2Int> TryFindPathToCloseTarget(Vector2Int a, Vector2Int b, int distThreshold, bool includeStart = false)
        {
            switch (distThreshold)
            {
                case < 0:
                    Debug.LogError($"距离阈值 {distThreshold} 不合法");
                    return null;
                case 0 when !CanMoveTo(b):
                    Debug.LogWarning($"目标点 {b} 被占用，无法到达");
                    return null;
            }
            if (a == b) return null;
            _costPq.Clear();
            _visitedNode.Clear();
            
            var startNode = new Node(a, 0, Heuristic(a, b));
            _costPq.Enqueue(startNode);
            _visitedNode.TryAdd(startNode.Coord, startNode);
            
            while (_costPq.Count > 0)
            {
                // 2.1 找到F值最小的节点
                var curNode = _costPq.Dequeue();
                // if (_visitedNode.ContainsKey(currentNode.Coord)) continue;
                Debug.Log($"正在访问: [{curNode.Coord}] G:{curNode.GCost} H:{curNode.HCost} F:{curNode.FCost}");
                
                // 2.2 检查是否到达目标节点
                if (curNode.Coord.ManhattanDistance(b) <= distThreshold)
                {
                    // 直接返回路径
                    _costPq.Clear();
                    _visitedNode.Clear();
                    return RetracePath(curNode, includeStart);
                }
                
                // 2.3 扩展邻居节点
                foreach (var direction in Directions)
                {
                    var neighborCoord = curNode.Coord + direction;
                    if (!CanMoveTo(neighborCoord)) continue;
                    // 经过当前节点到达邻居节点的代价
                    var moveCost = curNode.GCost + GetCost(curNode.Coord, neighborCoord);
                    
                    if (_visitedNode.TryGetValue(neighborCoord, out var visitedNode))
                    {
                        continue;
                        // // TODO 如果不同地形的消耗不一样(并非能否行走)，则需要重新计算，
                        // 这里需要想办法删除优先队列之前存的旧节点，或者添加标记等处理
                        // // 如果该节点已经在关闭列表中，并且G值相等或者更小，则跳过
                        // if (visitedNode.GCost <= moveCost) continue;
                        // _visitedNode.Remove(neighborCoord);
                        //
                        // // 更新旧节点
                        // visitedNode.GCost = moveCost;
                        // visitedNode.From = curNode;
                        // // TODO 删掉优先队列中原有的节点
                        // _costPq.Enqueue(visitedNode);
                    }
                    else
                    {
                        // 添加新的节点
                        var newNode = new Node(neighborCoord, moveCost, Heuristic(neighborCoord, b))
                        {
                            From = curNode
                        };
                        _costPq.Enqueue(newNode);
                        if (!_visitedNode.TryAdd(neighborCoord, newNode))
                        {
                            Debug.LogError($"当前节点 {neighborCoord} 已经在关闭列表中，无法添加");
                        }
                    }
                }
            }
            
            _costPq.Clear();
            _visitedNode.Clear();
            return null; // 如果没有路径，返回null
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
        
        private List<Vector2Int> RetracePath(Node endNode, bool includeStart)
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
            
            return path;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="moveRange"></param>
        /// <param name="includeStart"></param>
        /// <returns></returns>
        public List<PathTreeNode> CalculateAllMoveablePath(Vector2Int startPos, int moveRange, bool includeStart = false)
        {
            var result = new List<PathTreeNode>();
            var queue = new Queue<PathTreeNode>();
            var visited = new HashSet<Vector2Int>();
            var startNode = new PathTreeNode
            {
                Coord = new Vector2Int(startPos.x, startPos.y), 
                Parent = null
            };
            if(includeStart) result.Add(startNode);
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
                        if (!CanMoveTo(nextCoord) || visited.Contains(nextCoord)) continue;
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
    }
}