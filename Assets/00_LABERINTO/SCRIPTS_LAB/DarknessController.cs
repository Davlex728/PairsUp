using UnityEngine;

public class DarknessController : MonoBehaviour
{
    public Material darknessMaterial;
    public MazeGameManager mazeGameManager;

    void Update()
    {
        Vector4[] posiciones = new Vector4[7];
        float[] radios = new float[7];
        int index = 0;

        CentroCirculo centro = FindObjectOfType<CentroCirculo>();
        if (centro != null && index < 7)
        {
            posiciones[index] = new Vector4(centro.transform.position.x, centro.transform.position.y, 0, 0);
            radios[index] = centro.shaderRadius;
            index++;
        }

        for (int i = 0; i < mazeGameManager.jugadoresVivos.Count; i++)
        {
            if (mazeGameManager.jugadoresVivos[i] != null && index < 7)
            {
                Vector3 pos = mazeGameManager.jugadoresVivos[i].transform.position;
                posiciones[index] = new Vector4(pos.x, pos.y, 0, 0);

                float radio = 0.4f;
                if (mazeGameManager.jugadoresVivos[i].TryGetComponent<MazeMovement>(out MazeMovement movimiento))
                {
                    radio = movimiento.shaderRadius;
                }
                radios[index] = radio;

                index++;
            }
        }


        darknessMaterial.SetVectorArray("_PlayerPositions", posiciones);
        darknessMaterial.SetFloatArray("_PlayerRadii", radios);
    }
}