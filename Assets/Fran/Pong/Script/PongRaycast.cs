using UnityEngine;

public class PongRaycast : MonoBehaviour
{
    public RaycastHit2D hit;
        public Ray2D ray;
        [SerializeField] private float rayLength;
    
        public static PongRaycast instance;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            instance = this;
        }
    
        // Update is called once per frame
        void Update()
        {
            hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength);
            Physics2D.Raycast(transform.position, transform.right, rayLength);
            Debug.DrawRay(transform.position, transform.right * rayLength, Color.red);
        }
        
}
