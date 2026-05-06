using TMPro; // Necesario para los textos bonitos
using UnityEngine;
using UnityEngine.UI;

public class UIReceta : MonoBehaviour
{
    public TextMeshProUGUI textoPantalla;
    [SerializeField] private GameObject[] ingredientesUI; // Array de objetos UI para cada ingrediente (5 en total)
    [SerializeField] private Sprite[] spritesIngredientes; //0 Uzi, 1 Cash, 2 Pearl, 3 Gold
    [SerializeField] private GameObject[] tachadosUI; // Array de objetos UI para mostrar el tachado (5 en total)
    public void MostrarReceta(TipoIngrediente[] receta)
    {
       for (int i = 0; i < receta.Length; i++)
        {
            Debug.Log($"Receta[{i}] = {receta[i]}");
            ingredientesUI[i].GetComponent<UnityEngine.UI.Image>().sprite = spritesIngredientes[(int)receta[i]];
            ingredientesUI[i].SetActive(true); // Asegúrate de que el UI del ingrediente esté activo
        }
    }
    // Esta función la llamará la Cesta cada vez que coja algo
    public void ActualizarTexto(TipoIngrediente[] receta, int pasoActual, int puntuacion)
    {
        string texto = $"POCIONES: {puntuacion}\nReceta: ";

        for (int i = 0; i < receta.Length; i++)
        {
            if (i < pasoActual)
            {
                // Ingredientes ya conseguidos: Verdes y tachados
                texto += $"<color=#00FF00><s>{receta[i]}</s></color> - ";
                tachadosUI[i].SetActive(true); // Muestra el tachado para los ingredientes conseguidos
            }
            else if (i == pasoActual)
            {
                // Ingrediente que toca AHORA: Amarillo y grande
                texto += $"<color=#FFFF00><b>{receta[i]}</b></color> - ";
                tachadosUI[i].SetActive(false); // Asegúrate de que el tachado esté oculto para el ingrediente actual
            }
            else
            {
                // Ingredientes que faltan: Blancos normales
                texto += $"{receta[i]} - ";
                tachadosUI[i].SetActive(false);
            }
        }

        textoPantalla.text = texto;
    }
}