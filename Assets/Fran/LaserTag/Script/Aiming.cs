using UnityEngine;

public class Aiming : MonoBehaviour
{
    private PlayerInputHandler mandoApuntado;

    private void Start()
    {
        Debug.Log($"[Aiming] mandoApuntado = {mandoApuntado}");
    }
    
    public void ConectarMando(PlayerInputHandler mando)
    {
        mandoApuntado = mando;
    }

    private void Update()
    {
        if (mandoApuntado == null) return;

        Vector2 lookInput = mandoApuntado.lookInput;
        if (lookInput == Vector2.zero) return;

        float angle = Mathf.Atan2(-lookInput.x, lookInput.y) * Mathf.Rad2Deg;
        // Rota solo este objeto, independiente del padre
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
