using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    PlayerInput playerInput;
    public GameObject BulletPrefab;
    public Transform firePoint;
    public static Shoot Instance;
    private GameObject buller;
    [SerializeField] public float speed = 8;
    public float contador;
    [SerializeField] private float fireRate = 0.7f;
    [SerializeField] private int bulletTime = 3;
    
    private void Awake()
    {
        Instance = this;
        contador = fireRate;
    }

    private void FixedUpdate()
    {
        
        contador += Time.fixedDeltaTime;
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if ( contador >= fireRate)
        {
            Debug.Log("Disparo");
            buller = Instantiate(BulletPrefab, firePoint.position, firePoint.rotation);
            
            Bullet rb = buller.GetComponent<Bullet>();
            
            rb.Disparo(transform.up);
            contador = 0;
           
            Destroy(buller, bulletTime);
            
        }
    }
}
