using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PongMovement : MonoBehaviour
{
        [SerializeField] private float speed = 5f;
        [SerializeField] private float min;
        [SerializeField] private float max;

        public PlayerInputHandler mandoMovimiento;
        
        public static PongMovement instance;

        private void Start()
        {
            instance = this;
        }

        public void ConectarMando(PlayerInputHandler mando)
        {
            mandoMovimiento = mando;
        }

        public void ConfigurarAngulos(float minAngulo, float maxAngulo)
        {
            min = minAngulo;
            max = maxAngulo;
        }

        void FixedUpdate()
        {
            Vector2 direction = mandoMovimiento.moveInput;
            
            // Obtener la rotación actual en el eje Z
            float currentRotation = transform.eulerAngles.z;
            
            
            // Calcular la nueva rotación
            float newRotation = currentRotation - direction.x * speed;
            
            // Solo permitir movimiento dentro del rango [min, max]
            if (newRotation >= min && newRotation <= max)
            {
                // Dentro del rango, permitir el movimiento
                transform.eulerAngles = new Vector3(0f, 0f, newRotation);
            }
            else if (newRotation < min)
            {
                // Si intenta pasar el mínimo, quedarse en el mínimo
                transform.eulerAngles = new Vector3(0f, 0f, min);
            }
            else if (newRotation > max)
            {
                // Si intenta pasar el máximo, quedarse en el máximo
                transform.eulerAngles = new Vector3(0f, 0f, max);
            }
        }
}
