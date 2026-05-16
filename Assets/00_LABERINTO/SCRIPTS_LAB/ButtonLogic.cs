using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class ButtonLogic : MonoBehaviour
{
    [Header("Secuencia")]
    public InputBoton[] secuencia;
    private int posicionSecuencia = 0;
    private InputBoton botonEsperado;
 
    [Header("UI")]
    public Image imagenSimbolo;
    public Sprite[] imagenSecuencia;
    public GameObject imagen;
 
    [Header("Estado")]
    private bool esperando = false;
    private bool bloqueado = false;      
    private bool secuenciaCompletada = false;
 
    
    public Action<ButtonLogic> OnSecuenciaCompletada;

    public Color[] coloresEquipo = new Color[] { Color.blue, Color.red, Color.yellow };


    [HideInInspector] public List<GameObject> pareja;
 
    private void Start()
    {
        imagen.SetActive(false);
        Invoke(nameof(GenerarSecuencia), 20f);
    }
 
    
    public void Bloquear()
    {
        bloqueado = true;
        imagen.SetActive(false);
    }
 
    public void ComprobarInput(InputBoton botonPulsado)
    {
        if (esperando || bloqueado || secuenciaCompletada) return;
 
        if (botonPulsado == botonEsperado)
        {
            imagen.SetActive(false);
            posicionSecuencia++;
 
            if (posicionSecuencia == secuencia.Length)
            {
                secuenciaCompletada = true;
                imagen.SetActive(false);
                posicionSecuencia = 0;
                StartCoroutine(TimeShader(7f, pareja));
                OnSecuenciaCompletada?.Invoke(this);
                return;
            }
 
            
            botonEsperado = secuencia[posicionSecuencia];
            imagenSimbolo.sprite = imagenSecuencia[(int)botonEsperado - 1];
            StartCoroutine(MostrarSiguiente(1f));
        }
        else
        {
            
            imagen.SetActive(false);
            posicionSecuencia = 0;
            botonEsperado = secuencia[0];
            StartCoroutine(MostrarSiguiente(0.5f)); 
        }
    }
 
    public void GenerarSecuencia()
    {
        if (bloqueado) return;
 
        posicionSecuencia = 0;
        secuenciaCompletada = false;
        secuencia = new InputBoton[4];
 
        for (int i = 0; i < 4; i++)
        {
            secuencia[i] = (InputBoton)UnityEngine.Random.Range(1, Enum.GetValues(typeof(InputBoton)).Length);
        }
 
        botonEsperado = secuencia[0];
        imagenSimbolo.sprite = imagenSecuencia[(int)botonEsperado - 1];
        imagen.SetActive(true);
    }
 
    IEnumerator MostrarSiguiente(float delay)
    {
        esperando = true;
        yield return new WaitForSeconds(delay);
        imagen.SetActive(true);
        esperando = false;
    }
 
    IEnumerator TimeShader(float segundos, List<GameObject> objetivos)
    {
        if (objetivos == null || objetivos.Count < 2) yield break;
 
        objetivos[0].TryGetComponent<MazeMovement>(out var mov1);
        objetivos[1].TryGetComponent<MazeMovement>(out var mov2);
 
        if (mov1 != null) mov1.shaderRadius = 1f;
        if (mov2 != null) mov2.shaderRadius = 1f;
 
        yield return new WaitForSeconds(segundos);
 
        if (mov1 != null) mov1.shaderRadius = 0.4f;
        if (mov2 != null) mov2.shaderRadius = 0.4f;
    }
}
