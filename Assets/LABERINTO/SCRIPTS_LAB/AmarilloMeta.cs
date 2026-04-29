using UnityEngine;

public class AmarilloMeta : MonoBehaviour
{
    public int jugadoresAmarillos;
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
        if (other.tag == "Amarillo")
        {
            other.TryGetComponent<MazeMovement>(out MazeMovement mazeMovement);
            mazeMovement.rb.constraints = RigidbodyConstraints2D.FreezeAll;
            MazeMovement.instance.transform.position = new Vector3(0,-0.104f,0);
        }
        jugadoresAmarillos+= 1;
    }
}
