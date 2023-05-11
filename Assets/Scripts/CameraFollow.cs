using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
    private InputPlayer inputPlayer;
    CinemachineFreeLook CMCamera;
    Vector3 mousePos;
    public Transform Camera;

    public float mouseSensitivityX = 1;
    public float mouseSensitivityY = 1;
    private float X = 0.0f;
    private float Y = 0.0f;

    void Awake(){
        inputPlayer = new InputPlayer();
        CMCamera = GetComponent<CinemachineFreeLook>();
        mousePos = Mouse.current.position.ReadValue();
        
    }
    void FixedUpdate(){
        mousePos = Mouse.current.position.ReadValue();
        Debug.Log(mousePos);
        X += mouseSensitivityX * mousePos.x;
        Y -= mouseSensitivityY * mousePos.y;

        transform.eulerAngles = new Vector3(X, Y, 0.0f);
    }
}
