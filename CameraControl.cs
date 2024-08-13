using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    private Vector3 mouseWorldPosStart;
    private float zoomScale = 25f;
    private float minZoom = 30f;
    private float maxZoom = 100f;
    private float smoothTime = 0.25f;
    private float velocity = 0f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            mouseWorldPosStart = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        //Zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    private void Pan()
    {
        if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {
            Vector3 mouseWorldPosDiff = mouseWorldPosStart - cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mouseWorldPosDiff;
        }
    }

    private void Zoom(float zoomDiff)
    {
        if (zoomDiff != 0f)
        {
            mouseWorldPosStart = cam.ScreenToWorldPoint(Input.mousePosition);
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomDiff * zoomScale, minZoom, maxZoom);
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoomDiff, ref velocity, smoothTime);
            Vector3 mouseWorldPosDiff = mouseWorldPosStart - cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position += mouseWorldPosDiff;
        }
    }

}