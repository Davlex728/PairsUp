using UnityEngine;

public class DarknessController : MonoBehaviour
{
    public Material darknessMaterial;
    public MazeGameManager mazeGameManager;

    void Update()
    {
        Vector4[] posiciones = new Vector4[6];
        float[] radios = new float[6];
        int index = 0;
        
        // Primero el centro
        CentroCirculo centro = FindObjectOfType<CentroCirculo>();
        if (centro != null && index < 6)
        {
            posiciones[index] = new Vector4(centro.transform.position.x, centro.transform.position.y, 0, 0);
            radios[index] = centro.shaderRadius;
            index++;
        }
        
        // Luego los jugadores
        for (int i = 0; i < mazeGameManager.jugadoresVivos.Count; i++)
        {
            if (mazeGameManager.jugadoresVivos[i] != null && index < 6)
            {
                Vector3 pos = mazeGameManager.jugadoresVivos[i].transform.position;
                posiciones[index] = new Vector4(pos.x, pos.y, 0, 0);
                
                if(mazeGameManager.jugadoresVivos[i].TryGetComponent<MazeMovement>(out MazeMovement movimiento))
                {
                    radios[index] = movimiento.shaderRadius;
                }
                index++;
            }
        }

        darknessMaterial.SetVectorArray("_PlayerPositions", posiciones);
        darknessMaterial.SetFloatArray("_PlayerRadii", radios);
    }
}