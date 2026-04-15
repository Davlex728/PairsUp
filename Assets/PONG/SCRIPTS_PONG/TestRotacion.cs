using System;
using UnityEngine;
using UnityEditor;

public class TestRotacion : MonoBehaviour
{
    public float angle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Handles.color =  Color.red;  
        Handles.DrawWireArc( 
            transform.position,
            Vector3.forward,  
            Vector3.up,         
             angle,
            10f);
    }


                
}
