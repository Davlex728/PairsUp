using UnityEngine;

public class ScreenWrap : MonoBehaviour
{

    private float screenLeft;
    private float screenRght;

    void Awake()
    {
        screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        screenRght = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > screenRght)
        {
            transform.position = new Vector3(screenLeft, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < screenLeft)
        {
            transform.position = new Vector3(screenRght, transform.position.y, transform.position.z);
        }
    }
}
