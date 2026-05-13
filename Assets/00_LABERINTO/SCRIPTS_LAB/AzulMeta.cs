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

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Azul")
        {
            if (other.TryGetComponent<MazeMovement>(out MazeMovement mazeMovement))
            {
                Debug.Log($"Jugador Azul congelado: {other.name}");
                mazeMovement.rb.constraints = RigidbodyConstraints2D.FreezeAll;
                mazeMovement.transform.position = new Vector3(0, -0.404f, 0);
                jugadoresAzules += 1;
            }
        }
    }
}
