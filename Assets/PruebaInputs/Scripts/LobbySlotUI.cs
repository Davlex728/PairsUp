using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySlotUI : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject readyPanel;

    [Header("Refs dentro del panel listo")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image playerColorAccent;

    [Header("Color de acento por jugador")]
    [SerializeField]
    private Color[] colors =
    {
        new Color(0.95f, 0.22f, 0.22f), // Rojo     — J1
        new Color(0.22f, 0.52f, 0.95f), // Azul     — J2
        new Color(0.22f, 0.85f, 0.38f), // Verde    — J3
        new Color(0.95f, 0.85f, 0.15f), // Amarillo — J4
        new Color(0.95f, 0.52f, 0.08f), // Naranja  — J5
        new Color(0.75f, 0.22f, 0.95f), // Morado   — J6
    };

    private void Awake() => SetWaiting();

    public void SetWaiting()
    {
        if (waitingPanel) waitingPanel.SetActive(true);
        if (readyPanel) readyPanel.SetActive(false);
    }

    public void SetReady(int playerIndex)
    {
        Debug.Log($"[SlotUI] SetReady en {gameObject.name}, index {playerIndex}");

        if (waitingPanel) waitingPanel.SetActive(false);
        if (readyPanel) readyPanel.SetActive(true);

        if (playerNameText)
            playerNameText.text = $"JUGADOR {playerIndex + 1}";

        if (playerColorAccent && playerIndex < colors.Length)
            playerColorAccent.color = colors[playerIndex];
    }
}