using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float horizMove = 90f;

    public void MoveHorizontal(bool left)
    {
        float dir = 1;
        if (!left)
            dir *= -1;
        transform.RotateAround(target.position, Vector3.up, horizMove * dir);
    }
}
