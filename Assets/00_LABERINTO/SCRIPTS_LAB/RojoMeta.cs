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

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Rojo")
        {
            if (other.TryGetComponent<MazeMovement>(out MazeMovement mazeMovement))
            {
                Debug.Log($"Jugador Rojo congelado: {other.name}");
                mazeMovement.rb.constraints = RigidbodyConstraints2D.FreezeAll;
                mazeMovement.transform.position = new Vector3(0, 0.222f, 0);
                jugadoresRojos += 1;
            }
        }
    }
}
