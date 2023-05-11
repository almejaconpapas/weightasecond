using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // private InputPlayer inputPlayer;
    // void OnExitGame(InputValue value){
    //     Application.Quit();
    //     Debug.Log("Se cierra el juego");
    // }
    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update(){
        if(Input.GetMouseButtonDown(0) && Cursor.visible){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if(Input.GetMouseButtonDown(0) && !Cursor.visible){
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
