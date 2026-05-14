using UnityEngine;
using System.Collections.Generic;

public class PuntuacionManager : MonoBehaviour
{
    [SerializeField] private int[] puntuacion = new int[3];
    [SerializeField] private Stack<string> escenas = new Stack<string>();
    private string[] escenasArray;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < puntuacion.Length; i++)
        {
            puntuacion[i] = 0;
        }
        RandomPila(escenas);
        escenasArray = escenas.ToArray();

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


    Stack<string> RandomPila(Stack<string> pila)
    {
        List<string> escenasList = new List<string>(pila);
        for (int i = escenasList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = escenasList[i];
            escenasList[i] = escenasList[j];
            escenasList[j] = temp;
        }
        return new Stack<string>(escenasList);
    }

    public string SiguienteEscena()
    {
        if (escenas.Count > 0)
        {
            return escenas.Pop();
        }
        escenas = escenasArray.Length > 0 ? new Stack<string>(escenasArray) : new Stack<string>();
        escenas = RandomPila(escenas);
        return SiguienteEscena();
    }
}