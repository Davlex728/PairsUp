using UnityEngine;

// lista de todos los ingredientes se pueden añadir o quitar para ajustar dificultad o variedad, pero es importante que el nombre del enum coincida con el nombre del prefab para que se puedan identificar correctamente
public enum TipoIngrediente
{
    Manzana,
    Pera,
    Uva,
    Anchoa
}

public class Ingrediente : MonoBehaviour
{
    [Header("Tipo")]
    public TipoIngrediente miTipo;

    private void Update()
    {
        // Se destruye si nadie lo coge
        if (transform.position.y < -8f)
        {
            Destroy(gameObject);
        }
    }
}