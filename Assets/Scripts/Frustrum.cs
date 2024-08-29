using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Frustrum : MonoBehaviour
{
    Camera cam;

    private const int maxFrustrumPlanes = 6;
    private const int aabbPoints = 8;

    public Plane[] planes = new Plane[maxFrustrumPlanes];

    public List<Vector3> farPoints = new List<Vector3>();
    public List<Vector3> nearPoints = new List<Vector3>();
    public float distance;

    [SerializeField] List<GameObject> TestObjests = new List<GameObject>();


    [SerializeField] public Vector3 nearTopLeft;
    [SerializeField] public Vector3 nearTopRight;
    [SerializeField] public Vector3 nearBottomLeft;
    [SerializeField] public Vector3 nearBottomRight;

    [SerializeField] public Vector3 farTopLeft;
    [SerializeField] public Vector3 farTopRight;
    [SerializeField] public Vector3 farBottomLeft;
    [SerializeField] public Vector3 farBottomRight;

    [SerializeField] public Vector3 farMidleRight;
    [SerializeField] public Vector3 farMidleLeft;

    [SerializeField] public Vector3 nearCenter;
    [SerializeField] public Vector3 farCenter;

    float halfCameraHeightNear;
    float CameraHalfWidthNear;

    float halfCameraHeightfar;
    float CameraHalfWidthFar;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        for (int i = 0; i < maxFrustrumPlanes; i++)
        {
            planes[i] = new Plane();
        }

        halfCameraHeightNear = Mathf.Tan((cam.fieldOfView / 2) * Mathf.Deg2Rad) * cam.nearClipPlane;
        CameraHalfWidthNear = (cam.aspect * halfCameraHeightNear);

        halfCameraHeightfar = Mathf.Tan((cam.fieldOfView / 2) * Mathf.Deg2Rad) * cam.farClipPlane;
        CameraHalfWidthFar = (cam.aspect * halfCameraHeightfar);

        distance = cam.farClipPlane;
    }


    private void FixedUpdate()
    {
        UpdatePlanes();
    }

    public void setLineDistance(float value) 
    {
        distance = value;
    }
    public float GetDistance() 
    {
        return distance;
    }
    void UpdatePlanes()
    {
        Vector3 frontMultFar = cam.farClipPlane * cam.transform.forward;

        Vector3 nearPos = cam.transform.position;
        nearPos += cam.transform.forward * cam.nearClipPlane;
        planes[0].SetNormalAndPosition(cam.transform.forward, nearPos);


        Vector3 farPos = cam.transform.position;
        farPos += (cam.transform.forward) * cam.farClipPlane;
        planes[1].SetNormalAndPosition(cam.transform.forward * -1, farPos);

        SetNearPoints(nearPos);
        SetFarPoints(farPos);

        planes[2].Set3Points(cam.transform.position, farBottomLeft, farTopLeft);//left
        planes[3].Set3Points(cam.transform.position, farTopRight, farBottomRight);//right
        planes[4].Set3Points(cam.transform.position, farTopLeft, farTopRight);//top
        planes[5].Set3Points(cam.transform.position, farBottomRight, farBottomLeft);//bottom

        for (int i = 2; i < maxFrustrumPlanes; i++)
        {
            planes[i].Flip();
        }
    }

    public void SetNearPoints(Vector3 nearPos)
    {

        Vector3 nearPlaneDistance = cam.transform.position + (cam.transform.forward * cam.nearClipPlane);

        nearTopLeft = nearPlaneDistance + (cam.transform.up * halfCameraHeightNear) - (cam.transform.right * CameraHalfWidthNear);

        nearTopRight = nearPlaneDistance + (cam.transform.up * halfCameraHeightNear) + (cam.transform.right * CameraHalfWidthNear);

        nearBottomLeft = nearPlaneDistance - (cam.transform.up * halfCameraHeightNear) - (cam.transform.right * CameraHalfWidthNear);

        nearBottomRight = nearPlaneDistance - (cam.transform.up * halfCameraHeightNear) + (cam.transform.right * CameraHalfWidthNear);
    }
    public void SetFarPoints(Vector3 farPos)
    {

        Vector3 farPlaneDistance = cam.transform.position + (cam.transform.forward * cam.farClipPlane);


        farTopLeft = farPlaneDistance + (cam.transform.up * halfCameraHeightfar) - (cam.transform.right * CameraHalfWidthFar);

        farTopRight = farPlaneDistance + (cam.transform.up * halfCameraHeightfar) + (cam.transform.right * CameraHalfWidthFar);

        farBottomLeft = farPlaneDistance - (cam.transform.up * halfCameraHeightfar) - (cam.transform.right * CameraHalfWidthFar);

        farBottomRight = farPlaneDistance - (cam.transform.up * halfCameraHeightfar) + (cam.transform.right * CameraHalfWidthFar);

        farMidleLeft = farPlaneDistance + (cam.transform.right * CameraHalfWidthFar);
        farMidleRight = farPlaneDistance - (cam.transform.right * CameraHalfWidthFar);
    }

    private Vector3 Rotate(Vector3 vector, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
        float vx = vector.x;
        float vy = vector.y;
        vector.x = (cos * vx) - (sin * vy);
        vector.y = (sin * vx) + (cos * vy);
        return vector;
    }


    public void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        Gizmos.color = Color.green;

        //Plano Cercano
        DrawPlane(nearTopRight, nearBottomRight, nearBottomLeft, nearTopLeft);

        //Plano Lejano
        DrawPlane(farTopRight, farBottomRight, farBottomLeft, farTopLeft);

        // Plano Derecho
        DrawPlane(nearTopRight, farTopRight, farBottomRight, nearBottomRight);

        // Plano Izquierdo
        DrawPlane(nearTopLeft, farTopLeft, farBottomLeft, nearBottomLeft);

        // Plano Superior
        DrawPlane(nearTopLeft, farTopLeft, farTopRight, nearTopRight);

        //Plano Inferior
        DrawPlane(nearBottomLeft, farBottomLeft, farBottomRight, nearBottomRight);

        Gizmos.color = Color.red;
        int fov = (int)cam.fieldOfView;

        Gizmos.color = Color.green;
    }
    public void DrawPlane(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(p1, p3);
        //Gizmos.DrawLine(p2, p4);
        Gizmos.color = Color.green;
    }
}