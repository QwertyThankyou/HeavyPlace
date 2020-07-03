using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private CameraOrbit _cam;

    private void Start()
    {
        _cam = GetComponent<CameraOrbit>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) _cam.MoveHorizontal(true);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) _cam.MoveHorizontal(false);
    }
}
