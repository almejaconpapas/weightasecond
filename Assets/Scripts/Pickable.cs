using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Pickable : MonoBehaviour
{
    public bool isPicked;
    bool startCounting;
    public bool canPickObject;
    public float weight = 0;
    public float increaseWeight = 2;
    public GameObject pressKey;
    public TMP_Text presionado;
    private Rigidbody pickablePhysics;
    private RigidbodyConstraints originalConstrains;

    //public float gravity = -10;
    // Start is called before the first frame update
    void Awake(){
        pickablePhysics = GetComponent<Rigidbody>();
        originalConstrains = pickablePhysics.constraints;
        //Physics.gravity = 
        startCounting = false;
    }
    void Start()
    {
        isPicked = false;
        canPickObject = false;
    }
    void FixedUpdate(){
        if(startCounting){
            weight += Time.deltaTime * increaseWeight;
        }
    }

    void OnTriggerEnter(Collider collision){
        if(collision.gameObject.tag == "Player"){
            canPickObject = true;
            pressKey.SetActive(true);
            Debug.Log("Can pick object");
            changePressedText();
        } 
    }
    void OnTriggerExit(Collider collision){
        if(collision.gameObject.tag == "Player"){
            canPickObject = false;
            pressKey.SetActive(false);
            Debug.Log("Cannot pick object anymore");
            changePressedText();
        } else if(collision.gameObject.tag == "Terrain" || collision.gameObject.tag == "Structure"){
            startCounting = false;
        }
    }

    void OnTriggerEnter(Collision collider){
        if(collider.gameObject.tag == "Terrain" || collider.gameObject.tag == "Structure"){
            startCounting = false;
        }
    }
    public void changePressedText(){
        if(isPicked){
            presionado.text = "Soltar";
        }
        else{
            presionado.text = "Agarrar";
        }
    }
    public void changePositionPhysics(){
        if(isPicked){
            pickablePhysics.useGravity = false;
            this.pickablePhysics.constraints = RigidbodyConstraints.FreezeAll;
            this.transform.localPosition = new Vector3(0f, 1.4f, 1.5f);
            this.transform.eulerAngles = new Vector3(0f,0f,0f);
            // this.transform.position = new Vector3(-0.65f, 1.5f, 1.69f);
            startCounting = false;
            weight = 0f;
        }
        else{
            pickablePhysics.useGravity = true;
            this.pickablePhysics.constraints = RigidbodyConstraints.None;
            // this.pickablePhysics.constraints = originalConstrains;
            startCounting = true;
        }
    }
}
