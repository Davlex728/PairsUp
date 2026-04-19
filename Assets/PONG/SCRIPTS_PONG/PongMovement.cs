using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PongMovement : MonoBehaviour
{
        PlayerInput playerInput;
        CharacterController controller;
        [SerializeField] private float speed = 5f;

        private PlayerInputHandler mandoMovimiento;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }
        
        public void ConectarMando(PlayerInputHandler mando)
        {
            mandoMovimiento = mando;
        }
        // Update is called once per frame
        void FixedUpdate()
        {
            Vector2 direction = mandoMovimiento.moveInput;
            //Debug.Log("rotando");
             
            transform.Rotate(0f, 0f, -direction.x * speed * Time.deltaTime, Space.Self);    
                
            
            
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
           
                Debug.Log("LIMITE!");                                                     
            
        }
}
