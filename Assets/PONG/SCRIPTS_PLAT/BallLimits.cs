using System;
using Unity.VisualScripting;
using UnityEngine;

public class BallLimits : MonoBehaviour
{
    private int live;

    private int liveMax = 3;
    
    [SerializeField] private GameObject player;
    public BombMovement bomb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        live = liveMax;
        player.SetActive(true);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy(collision.gameObject);

        RestarVida();
    }

    int RestarVida()
    {
        live -= 1;
        
        Debug.Log($"Vidas de {player.name} : {live}");

        bomb.RespawnBomb();
        
        if (live == 0)
        {
            player.SetActive(false);
            
        }
        
        return live;
    }
    
}
