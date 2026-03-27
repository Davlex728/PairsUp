using UnityEngine;
using System.Collections.Generic;

public class RandomWall : MonoBehaviour
{
    public static RandomWall Instance { get; private set; }

    [Header("All walls in the scene")]
    public List<Wall> walls = new List<Wall>();

    void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        foreach (Wall wall in walls)
            wall.isActive = wall.gameObject.activeSelf;
    }

   
    public void OnWallHit(Wall hitWall)
    {
        
        hitWall.SetActive(false);
        Debug.Log($"[CollisionManager] Wall '{hitWall.name}' destroyed.");

        
        List<Wall> inactiveWalls = walls.FindAll(w => !w.isActive && w != hitWall);

        if (inactiveWalls.Count == 0)
        {
            Debug.Log("[CollisionManager] No inactive walls left to activate.");
            return;
        }

        
        Wall randomWall = inactiveWalls[Random.Range(0, inactiveWalls.Count)];
        randomWall.SetActive(true);
        Debug.Log($"[CollisionManager] Wall '{randomWall.name}' activated.");
    }

    
    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
