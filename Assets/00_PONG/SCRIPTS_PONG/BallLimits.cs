using System;
using Unity.VisualScripting;
using UnityEngine;

public class BallLimits : MonoBehaviour
{
    [SerializeField]private int live;

    private int liveMax = 3;
    
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject wall;
    public BombMovement bomb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        live = liveMax;
        player.SetActive(true);
        wall.SetActive(false);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy(collision.gameObject);
        //bomb.RespawnBomb();
        RestarVida();
    }

    int RestarVida()
    {
        live -= 1;
        
        Debug.Log($"Vidas de {player.name} : {live}");
        if (live > 0)
        {
            bomb.RespawnBomb();    
            Debug.Log("bomba respawneada");
        }
        
        
        if (live == 0)
        {
            
            Destroy(player);
            
            //wall.SetActive(true);
            
        }
        
        return live;
    }
    
}
