using TMPro; // Necesario para los textos bonitos
using UnityEngine;

public class UIReceta : MonoBehaviour
{
    public TextMeshProUGUI textoPantalla;

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
            }
            else if (i == pasoActual)
            {
                // Ingrediente que toca AHORA: Amarillo y grande
                texto += $"<color=#FFFF00><b>{receta[i]}</b></color> - ";
            }
            else
            {
                // Ingredientes que faltan: Blancos normales
                texto += $"{receta[i]} - ";
            }
        }

        textoPantalla.text = texto;
    }
}