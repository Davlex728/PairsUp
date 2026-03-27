using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController controller;
    [SerializeField] private float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMove(InputAction.CallbackContext callback)
    {
        Vector2 direction = callback.ReadValue<Vector2>();
        Debug.Log("rotando");
        if (callback.phase == InputActionPhase.Performed)
        { 
            transform.Rotate(0f, 0f, direction.x * speed * Time.deltaTime, Space.Self);    
            
            Debug.Log(direction.x);
                      
        }
  
        //float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        
    }
}
