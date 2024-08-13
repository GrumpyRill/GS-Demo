using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStar
{
    private Province start;
    private Province goal;

    public AStar(Province start, Province goal)
    {
        this.start = start;
        this.goal = goal;
    }

    private double HeuristicCostEstimate(Province start, Province goal)
    {
        return Vector2.Distance(start.pos, goal.pos); ;
    }

    private double DistBetween(Province current, Province neighbor)
    {
        return neighbor.weight;
    }

    private List<Province> ReconstructPath(Dictionary<Province, Province> cameFrom, Province current)
    {
        List<Province> path = new();
        while (current != null)
        {
            path.Add(current);
            if (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
            }
            else
            {
                break;
            }
        }
        path.Reverse();
        return path;
    }

    public List<Province> FindPath(Unit unit)
    {
        List<Province> closedSet = new();
        List<Province> openSet = new() { start };
        Dictionary<Province, Province> cameFrom = new();
        Dictionary<Province, double> gScore = new() { { start, 0 } };
        Dictionary<Province, double> fScore = new() { { start, HeuristicCostEstimate(start, goal) } };

        while (openSet.Count > 0)
        {
            Province current = openSet.OrderBy(Province => fScore[Province]).First();

            if (current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Province neighbor in current._PROV_Neighbours)
            {

                /*if (neighbor.ROOT_nation != unit.ROOT_nation)
                {
                    continue;
                }
*/
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                double tentativeGScore = gScore[current] + DistBetween(current, neighbor);
                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, goal);
            }
        }

        return null;  // No path was found
    }
}