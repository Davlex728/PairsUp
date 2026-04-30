using System;
using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    public static ButtonLogic instance;
    public InputBoton botonEsperado; // Lo asignas al spawnear cada símbolo
    public InputBoton[] secuencia;
    private int posicionSecuencia = 0;  
    
    private void Start()
    {
        instance = this;
    }

    public void ComprobarInput(InputBoton botonPulsado)
    {
        if (botonPulsado == botonEsperado)
        {
            posicionSecuencia += 1;
            if (posicionSecuencia == secuencia.Length)
            {
                Debug.Log("Secuencia terminada");
                return;
            }
            botonEsperado = secuencia[posicionSecuencia];
            
        }
        else
        {
            Debug.Log("Incorrecto");
            // Penalización, etc.
        }
    }

    public void GenerarSecuencia(int cantidad)
    {
        for(int i = 0; i < cantidad; i++)
        {
            InputBoton botonAleatorio = (InputBoton)UnityEngine.Random.Range(1, Enum.GetValues(typeof(InputBoton)).Length);
            secuencia[i] = botonAleatorio;
        }
    }
}
