using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class camera : MonoBehaviour
{
    public float zoom;
    private readonly float zoomMultiplier = 150f;
    public readonly float minZoom = 100f;
    public readonly float maxZoom =350f;
    private float velocity = 0f;
    private float smoothTime = 0.25f;


    public float newVelocity = 0.0f;
    public float newSmoothing = 10f;

    public static float minScaleX = 10.5f;
    public static float minScaleY = 5f;

    public float maxScaleX = minScaleX * 4;
    public float maxScaleY = minScaleY * 4;

    public float scaleX;
    public float scaleY;

    private List<GameObject> elements = new();

    [SerializeField]
    private Camera cam;
    public GameManager gameManager;

    private void Start()
    {
        zoom = cam.orthographicSize;
    }

    private Func<Vector3> GetCameraFollowPositionFunc;
    public void Setup(Func<Vector3> GetCameraFollowPositionFunc)
    {
        this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
    }

    public void SetGetCameraFollowPositionFunc(Func<Vector3> GetCameraFollowPositionFunc)
    {
        this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scroll * zoomMultiplier;
        elements = gameManager.units;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);

        foreach (GameObject element in elements)
        {
            if (element != null)
            {
                scaleX = Mathf.SmoothDamp(scaleX, scroll, ref newVelocity, newSmoothing);
                scaleY = Mathf.SmoothDamp(scaleY, scroll, ref newVelocity, newSmoothing);
                scaleX = gameManager.Map(zoom, minZoom, maxZoom, minScaleX, maxScaleX);
                scaleY = gameManager.Map(zoom, minZoom, maxZoom, minScaleY, maxScaleY);

                var scale = new Vector3(scaleX, scaleY, 0f);

                //Toggle element scaling (not defeault)

                //element.transform.localScale = scale;
                
            }
        }

        Vector3 cameraFollowPosition = GetCameraFollowPositionFunc();
        cameraFollowPosition.z = transform.position.z;

        Vector3 cameraMoveDir = (cameraFollowPosition - transform.position).normalized;
        float distance = Vector3.Distance(cameraFollowPosition, transform.position);

        float cameraMoveSpeed = gameManager.Map(zoom, 35, maxZoom, 1f, 10f);
        //smoothTime = gameManager.Map(zoom, minZoom, maxZoom, 0.05f, 0.25f);


        if (distance > 0f)
        {
            Vector3 newCameraPosition = transform.position = transform.position + cameraMoveSpeed * distance * Time.deltaTime * cameraMoveDir;
            float distanceAfterMoving = Vector3.Distance(newCameraPosition, cameraFollowPosition);

            if (distanceAfterMoving > distance)
            {
                newCameraPosition = cameraFollowPosition;
            }

            transform.position = newCameraPosition;
        }
    }

}