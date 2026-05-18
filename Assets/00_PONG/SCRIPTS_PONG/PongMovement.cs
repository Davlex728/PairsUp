using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PongMovement : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float min;
    [SerializeField] private float max;

    public PlayerInputHandler mandoMovimiento;
    [HideInInspector] public float inputEfectivo = 0f;

    public static PongMovement instance;

    private void Start()
    {
        instance = this;
        mandoMovimiento.LastBullet();
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
        // Obtener la rotación actual en el eje Z
        float currentRotation = transform.eulerAngles.z;

        // Calcular la nueva rotación usando inputEfectivo (calculado por el GameManager)
        float newRotation = currentRotation - inputEfectivo * speed;
        
        if(PongGameManager.Instance.equipo == 1)
        {
            newRotation = currentRotation + inputEfectivo * speed;
        }

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