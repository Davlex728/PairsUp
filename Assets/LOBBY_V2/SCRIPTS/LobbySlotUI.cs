using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Slot visual en la lobby. SlotId va del 1 al 6.
/// Columnas de equipo: 1-2 → equipo 0, 3-4 → equipo 1, 5-6 → equipo 2.
/// </summary>
public class LobbySlotUI : MonoBehaviour
{
    [Header("ID del Slot (1-6)")]
    [SerializeField] private int slotId;
    public int SlotId => slotId;

    [Header("Paneles")]
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject hoverPanel;
    [SerializeField] private GameObject occupiedPanel;

    [Header("Refs dentro del panel ocupado")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image playerColorAccent;

    [Header("Color de acento por slot (1-6)")]
    [SerializeField]
    private Color[] colors =
    {
        new Color(0.95f, 0.22f, 0.22f), // Slot 1 — Rojo
        new Color(0.22f, 0.52f, 0.95f), // Slot 2 — Azul
        new Color(0.22f, 0.85f, 0.38f), // Slot 3 — Verde
        new Color(0.95f, 0.85f, 0.15f), // Slot 4 — Amarillo
        new Color(0.95f, 0.52f, 0.08f), // Slot 5 — Naranja
        new Color(0.75f, 0.22f, 0.95f), // Slot 6 — Morado
    };

    private bool isOccupied = false;
    private int occupiedByPlayer = -1;

    private void Awake() => SetWaiting();

    // ── Estados públicos ────────────────────────────────────────────────────

    public void SetWaiting()
    {
        isOccupied = false;
        occupiedByPlayer = -1;
        ShowOnly(waitingPanel);
    }

    public void SetHoverOn(int playerSlotId)
    {
        if (isOccupied) return;
        if (hoverPanel)
        {
            var img = hoverPanel.GetComponentInChildren<Image>();
            int colorIndex = playerSlotId - 1; // slots 1-6 → índice 0-5
            if (img && colorIndex >= 0 && colorIndex < colors.Length)
                img.color = colors[colorIndex];
        }
        ShowOnly(hoverPanel);
    }

    public void SetHoverOff()
    {
        if (isOccupied) return;
        ShowOnly(waitingPanel);
    }

    /// <summary>Intenta ocupar el slot. Devuelve false si ya está ocupado.</summary>
    public bool TryOccupy(int playerSlotId)
    {
        if (isOccupied) return false;

        isOccupied = true;
        occupiedByPlayer = playerSlotId;

        ShowOnly(occupiedPanel);

        if (playerNameText)
            playerNameText.text = $"J{playerSlotId}";

        int colorIndex = playerSlotId - 1;
        if (playerColorAccent && colorIndex >= 0 && colorIndex < colors.Length)
            playerColorAccent.color = colors[colorIndex];

        return true;
    }

    public bool IsOccupied => isOccupied;
    public int OccupiedBy => occupiedByPlayer;

    // ── Util ────────────────────────────────────────────────────────────────
    private void ShowOnly(GameObject active)
    {
        if (waitingPanel) waitingPanel.SetActive(waitingPanel == active);
        if (hoverPanel) hoverPanel.SetActive(hoverPanel == active);
        if (occupiedPanel) occupiedPanel.SetActive(occupiedPanel == active);
    }
}