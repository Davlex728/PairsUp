using UnityEngine;

public class Rloj : MonoBehaviour
{
    float movezzzzEnSilencio = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //que se columpie de un lado a otro levemente 2D, eje X y Y, de dere
        transform.position = new Vector3(transform.position.x + Mathf.Sin(Time.time * movezzzzEnSilencio) * 0.07f, transform.position.y + Mathf.Cos(Time.time * movezzzzEnSilencio) * 0.03f, transform.position.z);
    }
}
