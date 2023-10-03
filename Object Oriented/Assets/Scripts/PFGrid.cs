using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PFGrid
{
    public PFGrid(int[,] inputGrid)
    {
        width = inputGrid.GetLength(0);
        height = inputGrid.GetLength(1);

        nodes = new PFNode[width, height];
        walkableNodes = new List<PFNode>();

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                nodes[i,j] = new PFNode(inputGrid[i,j] == 0, i, j);
                if(inputGrid[i,j] == 0)
                {
                    walkableNodes.Add(nodes[i,j]);
                }
            }
        }
    }

    public PFNode [,] nodes;
    public List<PFNode> walkableNodes; //This is inefficient however, we use this to pick a random node to 'walk to'

    public int width, height;

    public List<PFNode> GetNeighbours(PFNode node)
    {
        List<PFNode> neighbours = new();
        int x = node.x;
        int y = node.y;
        if(x > 0)
        {
            neighbours.Add(nodes[x - 1, y]);
        }

        if(x < width -1)
        {
            neighbours.Add(nodes[x + 1, y]);
        }

        if(y > 0)
        {
            neighbours.Add(nodes[x, y - 1]);
        }

        if(y < height - 1)
        {
            neighbours.Add(nodes[x, y + 1]);
        }

        return neighbours;
    }

    public PFNode GetNode(Vector3Int nodePos)
    {
        return nodes[nodePos.x, nodePos.y];
    }

    public int GetDistance(PFNode a, PFNode b)
    {
        int dX = Mathf.Abs(a.x - b.x);
        int dY = Mathf.Abs(a.y - b.y);

        if(dX > dY)
        {
            return 14 * dY + 10 * (dX - dY);
        }
        else
        {
            return 14 * dX + 10 * (dY - dX);
        }
    }


    public List<PFNode> FindPath(Vector3Int startPos, Vector3Int endPos)
    {
        PFNode startNode = GetNode(startPos);
        PFNode endNode = GetNode(endPos);
        List<PFNode> openSet = new();
        HashSet<PFNode> closedSet = new();    
        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            PFNode currentNode = openSet[0];
            for(int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost & openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == endNode)
            {
                //return function here that creates our path
                List<PFNode> path = new();
                currentNode = endNode;

                while(currentNode != startNode)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.Parent;
                }

                path.Reverse();

                return path;
            }

            foreach(PFNode neighbour in GetNeighbours(currentNode))
            {
                if(!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int costToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour); //Flat cost for movement as we're only oding cardinals
                if(costToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = costToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, endNode);
                    neighbour.Parent = currentNode;

                    if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        Debug.Log("no path found");
        return null;
    }

}
