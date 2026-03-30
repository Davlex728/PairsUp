using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int initialLives;
    private int player1Lives;
    private int player2Lives;
    private int player3Lives;
    
    bool roundEnded = false;
    
    private void Awake()
    {
        player1Lives = initialLives;
        player2Lives = initialLives;
        player3Lives = initialLives;
        
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoseLife( int playerId, int amount = 1)
    {
        if (roundEnded)
        {
            return;
        }

        switch (playerId)
        {
            case 1:
                player1Lives = Mathf.Max(0,player1Lives - amount);
                break;
            case 2:
                player2Lives = Mathf.Max(0,player2Lives - amount);
                break;
            case 3:
                player3Lives = Mathf.Max(0,player3Lives - amount);
                break;
        }
        
    }
}
