using System;
using UnityEngine;

public class AzulLogic : MonoBehaviour
{ 
    private EdgeCollider2D collider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<EdgeCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerExit2D(Collider2D other)
    {
            
            collider.isTrigger = false;
        
            Debug.Log(other.gameObject.tag);
        
    }
  private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Azul")
        {
            
            collider.isTrigger = true;
        }

        if (other.gameObject.tag == "Rojo")
        {
            
            collider.isTrigger = false;
        }

        if (other.gameObject.tag == "Amarillo")
        {
           
            collider.isTrigger = false;
        }
    }
   
}