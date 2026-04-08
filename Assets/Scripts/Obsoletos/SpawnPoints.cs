using UnityEngine;
using UnityEngine.InputSystem;
//No hacer prefab hacer un manager de spwans en cada minijuego
public class MinigameSetup : MonoBehaviour
{
    [Header("Assign spawnpoint for THIS minigame")]
    public Transform[] minigameSpawnPoints;

    private void Start()
    {
        // este script se hizo por la mañana y viendo un video no critiquen
        PlayerInput[] players = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);

        foreach (PlayerInput player in players)
        {
            int idJugador = player.playerIndex;

            // Verificaque haya un punto de aparición para este jugador
            if (idJugador < minigameSpawnPoints.Length)
            {
                // Teletransportamos al jugador al punto 
                player.transform.position = minigameSpawnPoints[idJugador].position;

                // reiniciar su velocidad si se quedó moviéndose del mapa anterior
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }
            }
        }
    }
}
