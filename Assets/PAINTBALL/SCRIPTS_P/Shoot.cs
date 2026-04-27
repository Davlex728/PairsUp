using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public static Shoot Instance;

    //[SerializeField] private float speed = 8f;
    [SerializeField] private float fireRate = 0.7f;
    [SerializeField] private int bulletTime = 3;

    private float contador;
    private PlayerInputHandler mandoDisparo;

    private void Awake()
    {
        Instance = this;
        contador = fireRate;
    }

    public void ConectarMando(PlayerInputHandler mando)
    {
        mandoDisparo = mando;
    }

    private void FixedUpdate()
    {
        contador += Time.fixedDeltaTime;
    }

    private void Update()
    {
        //Debug.Log(contador);
        if (mandoDisparo == null) return;

        if (mandoDisparo.isShooting )
        {
            if (contador >= fireRate)
            {
                Disparar();
                contador = 0;
            }
            
        }
    }

    private void Disparar()
    {
        Debug.Log("Disparo");
        GameObject bala = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet rb = bala.GetComponent<Bullet>();
        rb.Disparo(firePoint.up); // antes era transform.up
        Destroy(bala, bulletTime);
    }
   
}
