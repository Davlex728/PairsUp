using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonLogic : MonoBehaviour
{
    public static ButtonLogic instance;
    public InputBoton botonEsperado; // Lo asignas al spawnear cada símbolo
    public InputBoton[] secuencia ;
    private int posicionSecuencia = 0;
    public Image imagenSimbolo;
    public Sprite[] imagenSecuencia;

    private bool esperando;

    public GameObject imagen;
    
    private void Start()
    {
        instance = this;
        InvokeRepeating("GenerarSecuencia", 20, 2);
        //GenerarSecuencia(4);
        
    }

    public void ComprobarInput(InputBoton botonPulsado)
    {
        if (esperando)
        {
            return;
        }
        
        if (botonPulsado == botonEsperado)
        {
            StartCoroutine(Time(1));   
            Debug.Log("Hola");
            posicionSecuencia += 1;
            if (posicionSecuencia == secuencia.Length)
            {
                Debug.Log("Secuencia terminada");
                imagen.SetActive(false);
                posicionSecuencia = 0;
                return;
            }
            botonEsperado = secuencia[posicionSecuencia];
            imagenSimbolo.sprite = imagenSecuencia[(int)botonEsperado - 1];
            
        }
        else
        {
            imagen.SetActive(false);
            posicionSecuencia = 0;
            return;
            // Penalización, etc.
        }
    }

    IEnumerator Time(float segundos)
    {
        esperando = true;
        yield return new WaitForSeconds(segundos);
        esperando = false;

    }

    public void GenerarSecuencia(int cantidad = 4)
    {
        posicionSecuencia = 0;
        imagen.SetActive(true);
        secuencia = new InputBoton[cantidad];
        for(int i = 0; i < cantidad; i++)
        {
            InputBoton botonAleatorio = (InputBoton)UnityEngine.Random.Range(1, Enum.GetValues(typeof(InputBoton)).Length);
            secuencia[i] = botonAleatorio;
        }
        botonEsperado = secuencia[0];
        imagenSimbolo.sprite = imagenSecuencia[(int)botonEsperado - 1];
    }
}
