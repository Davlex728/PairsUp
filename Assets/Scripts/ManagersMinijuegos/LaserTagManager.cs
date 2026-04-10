using UnityEngine;

public class LaserTagManager : MonoBehaviour
{
    [Header("Prefab Padre (invisible, lleva movimiento)")]
    public GameObject prefabPadre;

    [Header("Prefab Hijo (el círculo visible)")]
    public GameObject prefabCirculo;
    public Vector2 offsetHijo; // posición relativa al padre, ajústala en el Inspector

    [Header("Spawn Point")]
    public Transform spawnPoint;

    private void Start()
    {
        PersistentPlayer[] jugadoresConectados = FindObjectsByType<PersistentPlayer>(FindObjectsSortMode.None);

        if (jugadoresConectados.Length < 2)
        {
            Debug.LogWarning("[LaserTagManager] Se necesitan al menos 2 jugadores.");
            return;
        }

        PlayerInputHandler mandoJ1 = jugadoresConectados[0].GetComponent<PlayerInputHandler>();
        PlayerInputHandler mandoJ2 = jugadoresConectados[1].GetComponent<PlayerInputHandler>();

        // Instanciar el padre
        GameObject padre = Instantiate(prefabPadre, spawnPoint.position, Quaternion.identity);

        // Instanciar el hijo y hacerlo hijo del padre
        GameObject hijo = Instantiate(prefabCirculo, padre.transform.position + (Vector3)offsetHijo, Quaternion.identity);
        hijo.transform.SetParent(padre.transform);

        // J1 mueve el padre
        if (padre.TryGetComponent<Movement>(out var movement))
            movement.ConectarMando(mandoJ1);

        // J2 apunta y dispara desde el hijo
        if (hijo.TryGetComponent<Aiming>(out var aiming))
            aiming.ConectarMando(mandoJ2);

        if (hijo.TryGetComponent<Shoot>(out var shoot))
            shoot.ConectarMando(mandoJ2);
    }
}
