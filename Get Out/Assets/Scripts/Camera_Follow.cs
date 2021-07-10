using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void Update()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, -0.5f, 0.5f);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, smoothedPosition.z);
    }
}
