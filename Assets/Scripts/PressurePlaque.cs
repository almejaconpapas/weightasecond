using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlaque : MonoBehaviour
{
    Rigidbody physics;
    [SerializeField]
    GameObject particles;
    bool levelPassed;
    public float weightThreshold = 10;
    // Start is called before the first frame update
    void Awake(){
        physics = GetComponent<Rigidbody>();
        particles.SetActive(false);
    }
    void Start()
    {
        physics.useGravity = false;
        this.physics.constraints = RigidbodyConstraints.FreezeAll;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.tag == "Pickable" && collision.gameObject.GetComponent<Pickable>().weight > weightThreshold){
            Debug.Log("Activado");
            physics.useGravity = true;
            this.physics.constraints = RigidbodyConstraints.FreezeRotation;
            particles.SetActive(true);
        }
    }
}
