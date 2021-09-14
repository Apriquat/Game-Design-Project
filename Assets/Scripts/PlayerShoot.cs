using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{

    // Weapon Vars
    public string weaponName;
    public float weaponDamage;
    public int weaponFireMode;
    public float weaponFireRate;

    // Bullet Vars
    public GameObject bullet;
    ObjectPool bulletPool;
    public int bulletPoolAmount;
    public float shootForce;

    // Shoot vars
    private float shotTime;
    private bool shotReady = true;

    // Start is called before the first frame update
    void Start()
    {
        // UpdateWeapon() to be invoked when switching weapons
        GameEvents.current.OnSwitchWeapon += UpdateWeapon;

        // UpdateWeapon() to be invoked when WeaponManager has finished initializing
        GameEvents.current.OnWeaponManagerReady += UpdateWeapon;

        bulletPool = ObjectPoolManager.current.CreatePool(name, bullet, bulletPoolAmount);

        Update();
    }

    // Update is called once per frame
    void Update()
    {
        GetShootInput();
    }

    // Checks for player's input before shooting
    private void GetShootInput()
    {
        // Switch on fire mode, 0 for semi auto 1 for full auto
        switch (weaponFireMode)
        {
            case 0:
                if (Input.GetMouseButtonDown(0)) { Shoot(); }
        
                if (Input.GetAxisRaw("Shoot") != 0)
                {
                    if (shotReady == true)
                    {
                        Shoot();
                    }
                    shotReady = false;
                }
                else
                {
                    shotReady = true;
                }
                break;
        
            case 1:
                if ((Input.GetMouseButton(0) || Input.GetAxisRaw("Shoot") > 0) && shotTime == 0)
                {
                    Shoot();
                    shotTime = weaponFireRate;
                }
                else
                {
                    shotTime -= Time.deltaTime;
                    shotTime = Mathf.Clamp(shotTime, 0.0f, weaponFireRate);
                }
                break;
        }

    }

    // Fires weapon
    private void Shoot()
    {
        GameObject _bullet = bulletPool.Instantiate(transform.position, Quaternion.Euler(Vector3.zero));

        // Get Bullet component of bullet prefab, set damage of bullet
        Bullet _bulletObject = _bullet.GetComponent<Bullet>();
        if (_bulletObject._damage != weaponDamage)
        {
            _bulletObject._damage = weaponDamage;
        }

        // Get Rigidbody component of bullet prefab, add shooting force
        Rigidbody rb = _bullet.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * shootForce, ForceMode.Impulse);
    }

    // Updates weapon information
    private void UpdateWeapon()
    {
        weaponName = WeaponManager.current.currentWeaponClass.Item1._name;
        weaponDamage = WeaponManager.current.currentWeaponClass.Item1._damage;
        weaponFireMode = WeaponManager.current.currentWeaponClass.Item1._fireMode;
        weaponFireRate = 1.0f / WeaponManager.current.currentWeaponClass.Item1._fireRate;
    }

}
