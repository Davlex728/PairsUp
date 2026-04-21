using UnityEngine;

public class PlataformeoManager : MonoBehaviour
{
    // Igual que CalderoManager pero par = Platformero, impar = Mano Constructora

    [Header("Prefabs Sprites")]
    public GameObject prefabPlatformero;    // El que corre y salta (par)
    public GameObject prefabMano;           // El cursor constructor (impar)

    [Header("Animators Platformero")]
    public RuntimeAnimatorController[] animatorsPlatformero; // P1, P3, P5...

    [Header("Animators Mano")]
    public RuntimeAnimatorController[] animatorsMano;        // P2, P4, P6...

    [Header("Spawn Points")]
    public Transform[] spawnPointsPlatformero; // Dónde aparece el corredor (inicio del nivel)
    public Transform[] spawnPointsMano;        // Dónde aparece la mano (libre por el nivel)

    private void Start()
    {
        PersistentPlayer[] jugadoresConectados = FindObjectsByType<PersistentPlayer>(FindObjectsSortMode.None);

        if (jugadoresConectados.Length == 0)
        {
            Debug.LogWarning("[PlataformeoManager] No hay jugadores. ¿Has pasado por el lobby?");
            return;
        }

        int contadorJugadores = 0;

        foreach (PersistentPlayer mandoFantasma in jugadoresConectados)
        {
            PlayerInputHandler lectorBotones = mandoFantasma.GetComponent<PlayerInputHandler>();

            // Par = Platformero | Impar = Mano constructora

            if (contadorJugadores % 2 == 0)
                SpawnearPlatformero(contadorJugadores, lectorBotones);
            else
                SpawnearMano(contadorJugadores, lectorBotones);

            contadorJugadores++;
        }
    }

    private void SpawnearPlatformero(int idJugador, PlayerInputHandler mando)
    {
        // ID 0 = Spawn 0 | ID 2 = Spawn 1 | ID 4 = Spawn 2

        int indexSpawn = idJugador / 2;
        Transform puntoSpawn = spawnPointsPlatformero[indexSpawn];

        GameObject sprite = Instantiate(prefabPlatformero, puntoSpawn.position, Quaternion.identity);

        if (sprite.TryGetComponent(out SpritePlatformero scriptPlatformero))
        {
            scriptPlatformero.ConectarMando(mando);
        }

        if (sprite.TryGetComponent(out Animator animator))
        {
            if (animatorsPlatformero.Length > indexSpawn && animatorsPlatformero[indexSpawn] != null)
                animator.runtimeAnimatorController = animatorsPlatformero[indexSpawn];
        }
    }

    private void SpawnearMano(int idJugador, PlayerInputHandler mando)
    {
        // ID 1 = Spawn 0 | ID 3 = Spawn 1 | ID 5 = Spawn 2

        int indexSpawn = idJugador / 2;
        Transform puntoSpawn = spawnPointsMano[indexSpawn];

        GameObject avatar = Instantiate(prefabMano, puntoSpawn.position, Quaternion.identity);

        if (avatar.TryGetComponent(out SpriteManoBob scriptMano))
        {
            scriptMano.ConectarMando(mando);
        }

        if (avatar.TryGetComponent(out Animator animator))
        {
            if (animatorsMano.Length > indexSpawn && animatorsMano[indexSpawn] != null)
                animator.runtimeAnimatorController = animatorsMano[indexSpawn];
        }
    }
}