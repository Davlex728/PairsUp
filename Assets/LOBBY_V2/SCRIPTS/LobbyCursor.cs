using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Cursor libre del jugador en la lobby.
/// - Stick izquierdo → mover
/// - Grab/X encima de un slot → confirmar
/// - Jump encima de un slot confirmado → cancelar
/// SlotIds van del 1 al 6.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class LobbyCursor : MonoBehaviour
{
    [Header("Velocidad del cursor")]
    [SerializeField] private float moveSpeed = 800f;

    [Header("Visual")]
    [SerializeField] private Image avatarImage;
    [SerializeField]
    private Color[] playerColors =
    {
        new Color(0.95f, 0.22f, 0.22f), // Slot 1 — Rojo
        new Color(0.22f, 0.52f, 0.95f), // Slot 2 — Azul
        new Color(0.22f, 0.85f, 0.38f), // Slot 3 — Verde
        new Color(0.95f, 0.85f, 0.15f), // Slot 4 — Amarillo
        new Color(0.95f, 0.52f, 0.08f), // Slot 5 — Naranja
        new Color(0.75f, 0.22f, 0.95f), // Slot 6 — Morado
    };

    private int playerIndex;
    private PlayerInputHandler inputHandler;
    private RectTransform rectTransform;
    private RectTransform canvasRect;

    private LobbySlotUI hoveredSlot;
    private bool confirmed = false;

    // Para evitar que grab se dispare múltiples frames
    private bool grabWasPressed = false;
    private bool jumpWasPressed = false;

    public void Init(int index, PlayerInputHandler handler, RectTransform canvasRectTransform)
    {
        playerIndex = index;
        inputHandler = handler;
        canvasRect = canvasRectTransform;
        rectTransform = GetComponent<RectTransform>();

        rectTransform.anchoredPosition = new Vector2(
            Random.Range(-300f, 300f),
            Random.Range(-150f, 150f)
        );

        // Color según índice del jugador (0-based internamente)
        if (avatarImage && playerIndex < playerColors.Length)
            avatarImage.color = playerColors[playerIndex];
    }

    private void Update()
    {
        if (!confirmed) HandleMovement();
        if (!confirmed) HandleHover();
        HandleConfirmAndCancel();
    }

    // ── Movimiento ──────────────────────────────────────────────────────────
    private void HandleMovement()
    {
        if (inputHandler == null) return;

        Vector2 input = inputHandler.moveInput;
        Vector2 newPos = rectTransform.anchoredPosition + input * (moveSpeed * Time.deltaTime);

        Vector2 half = canvasRect.sizeDelta * 0.5f;
        newPos.x = Mathf.Clamp(newPos.x, -half.x, half.x);
        newPos.y = Mathf.Clamp(newPos.y, -half.y, half.y);

        rectTransform.anchoredPosition = newPos;
    }

    // ── Hover — usa posición mundial para compatibilidad con cualquier anchor ──
    private void HandleHover()
    {
        LobbySlotUI nearest = FindNearestSlot();

        if (nearest != hoveredSlot)
        {
            if (hoveredSlot != null) hoveredSlot.SetHoverOff();
            hoveredSlot = nearest;
            if (hoveredSlot != null) hoveredSlot.SetHoverOn(playerIndex + 1); // interno 0-based → slot 1-based
        }
    }

    // ── Confirmar (Grab) y Cancelar (Jump) ─────────────────────────────────
    private void HandleConfirmAndCancel()
    {
        // CONFIRMAR — flanco de subida de Grab
        bool grabDown = inputHandler.isGrabbing && !grabWasPressed;
        grabWasPressed = inputHandler.isGrabbing;

        if (grabDown && !confirmed && hoveredSlot != null)
        {
            bool success = hoveredSlot.TryOccupy(playerIndex + 1); // slot 1-based
            if (success)
            {
                confirmed = true;
                hoveredSlot.SetHoverOff();
                // Mueve el cursor al centro del slot
                rectTransform.position = hoveredSlot.GetComponent<RectTransform>().position;
                Debug.Log($"[LobbyCursor] Jugador {playerIndex + 1} → Slot {hoveredSlot.SlotId}");
                LobbyManager.Instance?.OnPlayerConfirmedSlot(playerIndex, hoveredSlot.SlotId);
            }
        }

        // CANCELAR — flanco de subida de Jump
        bool jumpDown = inputHandler.isJumping && !jumpWasPressed;
        jumpWasPressed = inputHandler.isJumping;

        if (jumpDown && confirmed)
        {
            // Liberar slot visualmente
            if (hoveredSlot != null) hoveredSlot.SetWaiting();

            // Buscar el slot ocupado por este jugador y liberarlo
            foreach (var slot in FindObjectsOfType<LobbySlotUI>())
            {
                if (slot.OccupiedBy == playerIndex + 1)
                {
                    slot.SetWaiting();
                    break;
                }
            }

            confirmed = false;
            hoveredSlot = null;
            Debug.Log($"[LobbyCursor] Jugador {playerIndex + 1} canceló su slot.");
            LobbyManager.Instance?.OnPlayerCancelledSlot(playerIndex);
        }
    }

    // ── Busca el slot más cercano en espacio mundial ─────────────────────────
    private LobbySlotUI FindNearestSlot()
    {
        LobbySlotUI[] slots = FindObjectsOfType<LobbySlotUI>();
        LobbySlotUI best = null;
        float bestDist = 150f; // radio en unidades de pantalla

        Vector3 cursorWorld = rectTransform.position;

        foreach (var slot in slots)
        {
            RectTransform slotRect = slot.GetComponent<RectTransform>();
            float dist = Vector3.Distance(cursorWorld, slotRect.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = slot;
            }
        }
        return best;
    }
}