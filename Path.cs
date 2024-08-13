using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField]
    public Unit unit;
    public LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!unit.isMoving) 
        { 
            lineRenderer.enabled = false;
        } else
        {
            lineRenderer.enabled = true;
        }

        if (unit.path != null)
        {
            if (unit.path.Count > 1)
            {
                int pathLength = unit.tempPath;
                lineRenderer.positionCount = pathLength;
                for (int i = unit.pathLength - 1, j = pathLength - 1; i < pathLength; i++, j--)
                {

                    lineRenderer.SetPosition(pathLength - 1, unit.pos);

                    Vector2 point = new Vector3(unit.path[i].pos.x, unit.path[i].pos.y);
                    lineRenderer.SetPosition(j, point);
                }
            }
        }
        
    }
}
