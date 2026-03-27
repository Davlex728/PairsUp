using UnityEngine;

public class Raycast : MonoBehaviour
{
    Ray2D ray2D;
    RaycastHit2D hit2D;
    [SerializeField] private int rayLength;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ray2D = new Ray2D(transform.position, transform.up);
        hit2D = Physics2D.Raycast(transform.position, transform.up, rayLength);
        Debug.DrawRay(transform.position, transform.up* rayLength, Color.blue);
        
    }
}
