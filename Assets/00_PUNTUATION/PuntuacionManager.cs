using UnityEngine;

public class PuntuacionManager : MonoBehaviour
{
    PuntuacionManager instance;
    private int[] puntuacion = new int[3];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < puntuacion.Length; i++)
        {
            puntuacion[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Método para sumar puntos a un índice específico
    public void SumarPuntuacion(int index)
    {
        if (index >= 0 && index < puntuacion.Length)
        {
            puntuacion[index] ++;
        }
    }

    // Método para obtener la puntuación de un índice específico
    public int ObtenerPuntuacion(int index)
    {
        if (index >= 0 && index < puntuacion.Length)
        {
            return puntuacion[index];
        }
        return 0;
    }
}