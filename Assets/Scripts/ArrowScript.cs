using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public GameObject arrow;
    public GameObject head;
    public GameObject tail;


    Vector3 value;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void setValue(Vector3 value){
        this.value = value;

        Color col = new Color(this.value.normalized.x, this.value.normalized.y, this.value.normalized.z, 1);

        MeshRenderer mrArrow = arrow.GetComponent<MeshRenderer>();
        mrArrow.material.color = col;

        // Uncomment to color each part of the arrow individually
        // MeshRenderer mrTail = tail.GetComponent<MeshRenderer>();
        // mrTail.material.color = col;
        // MeshRenderer mrHead = head.GetComponent<MeshRenderer>();
        // mrHead.material.color = col;
    }

    public Vector3 getValue(){
        return this.value;
    }
}
