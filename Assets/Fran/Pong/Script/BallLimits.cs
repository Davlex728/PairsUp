using Unity.VisualScripting;
using UnityEngine;

public class BallLimits : MonoBehaviour
{
    private int live;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);

        RestarVida();
    }

    int RestarVida()
    {
        live -= 1;
        return live;
    }
    
}
