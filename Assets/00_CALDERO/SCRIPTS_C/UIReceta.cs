using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIReceta : MonoBehaviour
{
    public TextMeshProUGUI textoPuntos;
    [SerializeField] private Image imagenIngrediente;
    [SerializeField] private Sprite[] spritesIngredientes;

    public void ActualizarPuntos(int puntos)
    {
        if (textoPuntos != null)
            textoPuntos.text = puntos.ToString();
    }

    public void MostrarIngredienteObjetivo(TipoIngrediente tipo)
    {
        if (imagenIngrediente != null && spritesIngredientes.Length > (int)tipo)
            imagenIngrediente.sprite = spritesIngredientes[(int)tipo];
    }
}