using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIReceta : MonoBehaviour
{
    public TextMeshProUGUI textoPuntos;
    [SerializeField] private GameObject[] ingredientesUI;
    [SerializeField] private Sprite[] spritesIngredientes;

    public void ActualizarPuntos(int puntos)
    {
        if (textoPuntos != null)
            textoPuntos.text = puntos.ToString();
    }

    public void MostrarIngredienteObjetivo(TipoIngrediente tipo)
    {
        for (int i = 0; i < ingredientesUI.Length; i++)
        {
            bool esElActivo = i == (int)tipo;
            ingredientesUI[i].SetActive(esElActivo);
            if (esElActivo)
                ingredientesUI[i].GetComponent<Image>().sprite = spritesIngredientes[(int)tipo];
        }
    }
}