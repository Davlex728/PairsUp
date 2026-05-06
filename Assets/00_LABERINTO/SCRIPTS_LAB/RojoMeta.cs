using System;
using UnityEngine;

public class RojoMeta : MonoBehaviour
{
    public static RojoMeta instance;
    public int jugadoresRojos;

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
        if (other.tag == "Rojo")
        {
            other.TryGetComponent<MazeMovement>(out MazeMovement mazeMovement);
            mazeMovement.rb.constraints = RigidbodyConstraints2D.FreezeAll;
            mazeMovement.transform.position = new Vector3(0,0.222f,0);

            jugadoresRojos += 1;
        }
           
    }
}
