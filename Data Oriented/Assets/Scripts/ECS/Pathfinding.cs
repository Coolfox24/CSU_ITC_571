using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using System;
using Unity.Entities;
using Unity.Burst;
using System.IO;
using Unity.Entities.UniversalDelegates;

[BurstCompile]
public partial struct Pathfinding : ISystem
{
    public void OnCreate(ref SystemState state)
    {    
    }

    public void OnDestroy(ref SystemState state)
    {    }


    public void OnUpdate(ref SystemState state)
    {

    }   

    [BurstCompile]
    public partial struct FindPathJob : IJobEntity
    {
        public void Execute(PathfindingAspect p, ref DynamicBuffer<PathPosition> pathPos)
        {
            if(p.pathIndex.ValueRW.pathIndex != -1)
            {
                return;
            }

            int2 startPos = p.pfParam.ValueRO.startPos;
            int2 endPos = p.pfParam.ValueRO.endPos;
            int2 gridSize = new int2(20,20);

            NativeArray<PathNode> pathNodeArray = new(gridSize.x * gridSize.y, Allocator.Temp);

            for(int x = 0; x < gridSize.x; x++)
            {
                for(int y = 0; y < gridSize.y; y++)
                {
                    PathNode node = new PathNode(x, y, CalcIndex(x, y, gridSize.x), true, CalcDistance(new int2(x, y), endPos));
                    node.CalculateFCost();

                    pathNodeArray[node.index] = node;
                }
            }

            NativeArray<int2> neighbourOffsets = new NativeArray<int2>(8, Allocator.Temp);
            neighbourOffsets[0] = new int2(-1, 0); // Left
            neighbourOffsets[1] = new int2(+1, 0); // Right
            neighbourOffsets[2] = new int2(0, +1); // Up
            neighbourOffsets[3] = new int2(0, -1); // Down
            
            int endNodeIndex = CalcIndex(endPos.x, endPos.y, gridSize.x);

            PathNode startNode = pathNodeArray[CalcIndex(startPos.x, startPos.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodeArray[startNode.index] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.index);

            while(openList.Length > 0)
            {
                int currentIndex = GetLowestFNodeIndex(openList, pathNodeArray);
                PathNode curNode = pathNodeArray[currentIndex];

                if(currentIndex == endNodeIndex)
                {   
                    break;
                }

                for(int i = 0; i < openList.Length; i++)
                {
                    if(openList[i] == currentIndex)
                    {
                        openList.RemoveAt(i);
                        break;
                    }
                }

                closedList.Add(currentIndex);

                for(int i = 0; i < neighbourOffsets.Length; i++)
                {
                    int2 neighbourPos = new int2(curNode.x + neighbourOffsets[i].x, curNode.y + neighbourOffsets[i].y);

                    if(!IsValidPosition(neighbourPos, gridSize))
                    {
                        continue;
                    }

                    int neighbourIndex = CalcIndex(neighbourPos.x, neighbourPos.y, gridSize.x);

                    if(closedList.Contains(neighbourIndex))
                    {
                        continue;
                    } 

                    PathNode neighbourNode = pathNodeArray[neighbourIndex];
                    if(!neighbourNode.isWalkable)
                    {
                        continue;
                    }

                    int tentativeGcost = curNode.gCost + CalcDistance(new int2(curNode.x, curNode.y), neighbourPos);
                    if(tentativeGcost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromIndex = currentIndex;
                        neighbourNode.gCost = tentativeGcost;
                        neighbourNode.CalculateFCost();
                        pathNodeArray[neighbourIndex] = neighbourNode;

                        if(!openList.Contains(neighbourIndex))
                        {
                            openList.Add(neighbourIndex);
                        }
                    }
                }
            }

            pathPos.Clear();
            PathNode endNode = pathNodeArray[endNodeIndex];
            if(endNode.cameFromIndex == -1)
            {
                //Debug.Log("No Path");
            }
            else
            {
                // var path = CalculatePath(pathNodeArray, endNode);

                // path.Dispose();

                CalculatePath(pathNodeArray, endNode, pathPos);
                p.pathIndex.ValueRW.pathIndex = pathPos.Length - 1;
            }

            neighbourOffsets.Dispose();
            openList.Dispose();
            closedList.Dispose();
            pathNodeArray.Dispose();
        }

        private void CalculatePath(NativeArray<PathNode> nodeArray, PathNode endNode, DynamicBuffer<PathPosition> pathPos)
        {
            if(endNode.cameFromIndex == -1)
            {
                // return new NativeList<int2>(Allocator.Temp);
            }
            else
            {
                pathPos.Add(new PathPosition { position = new int2(endNode.x, endNode.y)});

                PathNode curNode = endNode;
                while(curNode.cameFromIndex != -1)
                {
                    curNode = nodeArray[curNode.cameFromIndex];
                    pathPos.Add(new PathPosition{ position = new int2(curNode.x, curNode.y)});
                }
            }
        }


        private NativeList<int2> CalculatePath(NativeArray<PathNode> nodeArray, PathNode endNode)
        {
            if(endNode.cameFromIndex == -1)
            {
                return new NativeList<int2>(Allocator.Temp);
            }
            else
            {
                NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
                path.Add(new int2(endNode.x, endNode.y));

                PathNode curNode = endNode;
                while(curNode.cameFromIndex != -1)
                {
                    curNode = nodeArray[curNode.cameFromIndex];
                    path.Add(new int2(curNode.x, curNode.y));
                }

                return path;
            }
        }


        private int CalcIndex(int x, int y, int width)
        {
            return x + y * width;
        }

        private bool IsValidPosition(int2 gridPos, int2 gridSize)
        {
            return 
                gridPos.x >= 0 &&
                gridPos.y > 0 && 
                gridPos.x < gridSize.x && 
                gridPos.y < gridSize.y;
        }

        private int CalcDistance(int2 start, int2 end)
        {
            int xDistance = math.abs(start.x - end.x);
            int yDistance = math.abs(start.y - end.y);
            int remaining = math.abs(xDistance - yDistance);
            return 14 * math.min(xDistance, yDistance) + 10 * remaining;
        }

        private int GetLowestFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
        {
            PathNode lowestCostPathNode = pathNodeArray[openList[0]];
            for(int i = 0; i < openList.Length; i++)
            {
                if(pathNodeArray[openList[i]].fCost < lowestCostPathNode.fCost)
                {
                    lowestCostPathNode = pathNodeArray[openList[i]];
                }
            }

            return lowestCostPathNode.index;
        }

        private struct PathNode
        {
            public int x;
            public int y;

            public int index;

            public int gCost;
            public int hCost;
            public int fCost;

            public bool isWalkable;
            public int cameFromIndex;

            public PathNode(int x, int y, int index, bool isWalkable, int hCost)
            {
                this.x = x;
                this.y = y;

                this.index = index;
                this.isWalkable = isWalkable;
                gCost = int.MaxValue;
                fCost = cameFromIndex = -1;
                this.hCost = hCost;
            }

            public void CalculateFCost()
            {
                fCost = gCost + hCost;
            }
        }
    }
}
