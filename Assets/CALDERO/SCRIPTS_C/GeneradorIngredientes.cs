using UnityEngine;

public class GeneradorIngredientes : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject[] prefabsIngredientes; // Lista de cosas que pueden caer(escalable a mas tipos/menos pra dificultad)
    public float tiempoEntreSpawns = 2f;     // Cada cuántos segundos cae uno
    public float limiteAnchoX = 7f;          // Rango horizontal  de spawn (ajustable para que no caigan por fuera de la pantalla)

    private float temporizador;

    private void Update()
    {
        temporizador += Time.deltaTime;

        if (temporizador >= tiempoEntreSpawns)
        {
            SpawnearObjeto();
            temporizador = 0f;
        }
    }

    private void SpawnearObjeto()
    {
        if (prefabsIngredientes.Length == 0) return;

        //  posición X aleatoria entre 
        float posicionX = Random.Range(-limiteAnchoX, limiteAnchoX);
        Vector3 posicionSpawn = new Vector3(posicionX, transform.position.y, 0f);

        //  ingrediente aleatorio de la lista
        int indiceAleatorio = Random.Range(0, prefabsIngredientes.Length);

        // instantiate del ingrediente seleccionado de antes
        Instantiate(prefabsIngredientes[indiceAleatorio], posicionSpawn, Quaternion.identity);
    }
}