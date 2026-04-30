using System;
using UnityEngine;

public class AzulMeta : MonoBehaviour
{
    public static AzulMeta instance;
    public int jugadoresAzules;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Azul")
        {
            Debug.Log(other.name);
            other.TryGetComponent<MazeMovement>(out MazeMovement mazeMovement);
            mazeMovement.rb.constraints = RigidbodyConstraints2D.FreezeAll;

            mazeMovement.transform.position = new Vector3(0,-0.404f,0);
        }
         jugadoresAzules+= 1;  
    }
}
