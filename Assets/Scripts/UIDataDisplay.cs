using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDataDisplay : MonoBehaviour
{
    [SerializeField]
    GameObject pickable;
    TMP_Text tmp;
    // Start is called before the first frame update
    void Awake(){
        tmp = GetComponent<TMP_Text>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tmp.text = "Weight: " + Mathf.FloorToInt(pickable.GetComponent<Pickable>().weight) + "KG";
    }
}
