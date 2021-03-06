using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{

    // Singleton
    public static PlayerShoot current;

    // Input
    private GameplayInput gameInput;

    // Weapon Vars
    public string weaponName;
    public float weaponDamage;
    public int weaponFireMode;
    public float weaponFireRate;
    public int weaponAmmoType;

    // Bullet Vars
    public GameObject bullet;
    public Transform bulletSpawn;
    public ObjectPool bulletPool;
    public int bulletPoolAmount;
    public float shootForce;

    // Shoot Vars
    private float shotTime;
    private bool shotReady = true;

    // Audio Vars
    public bool audioManagerReady = false;

    // Start is called before the first frame update
    void Start()
    {
        // Singleton
        current = this;

        // Input
        gameInput = GetComponentInParent<GameplayInput>();

        // UpdateWeapon() to be invoked when switching weapons
        GameEvents.current.OnSwitchWeapon += UpdateWeapon;

        // UpdateWeapon() to be invoked when WeaponManager has finished initializing
        GameEvents.current.OnWeaponManagerReady += UpdateWeapon;

        // AudioManagerReady() to be invoked when AudioManager has finished initializing
        GameEvents.current.OnAudioManagerReady += SetAudioManagerReady;

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
                if (gameInput.leftClick && shotTime <= 0)
                {
                    Shoot(weaponAmmoType);
                    shotTime = weaponFireRate;
                }
                else
                {
                    shotTime -= Time.deltaTime;
                }
        
                if (Input.GetAxisRaw("Shoot") != 0)
                {
                    if (shotReady == true)
                    {
                        Shoot(weaponAmmoType);
                    }
                    shotReady = false;
                }
                else
                {
                    shotReady = true;
                }
                break;
        
            case 1:
                if ((gameInput.leftClickHold || Input.GetAxisRaw("Shoot") > 0) && shotTime <= 0)
                {
                    Shoot(weaponAmmoType);
                    shotTime = weaponFireRate;
                }
                else
                {
                    shotTime -= Time.deltaTime;
                }
                break;
        }

        if (gameInput.rightClick)
        {
            Grappler.current.ShootGrapple();
        }

    }

    // Fires weapon
    public void Shoot(int ammoType)
    {
        if (ammoType == 2)
        {
            for (int i = 0; i < 8; i++)
            {
                Shoot(0);
            }
        }
        //Debug.Log("Shooting with ammo type: " + ammoType);
        GameObject _bullet = bulletPool.Instantiate(bulletSpawn.position + new Vector3(Random.Range(-0.25f, 0.25f) * Mathf.Cos(transform.rotation.y), Random.Range(-0.15f, 0.15f), Random.Range(-0.25f, 0.25f) * Mathf.Sin(transform.rotation.y)), Quaternion.Euler(Vector3.zero));

        // Get Bullet component of bullet prefab, set damage of bullet
        Bullet _bulletObject = _bullet.GetComponent<Bullet>();
        _bulletObject.InitializeBullet(weaponDamage, ammoType);

        if (_bulletObject._damage != weaponDamage)
        {
            _bulletObject._damage = weaponDamage;
        }

        // Get Rigidbody component of bullet prefab, add shooting force
        Rigidbody rb = _bullet.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * (shootForce + Player.current.rb.velocity.magnitude), ForceMode.Impulse);

        // Play shooting sound
        AudioManager.current.PlayRandomClip(GameAssets.current.shootAudioClips);
    }

    // Updates weapon information
    private void UpdateWeapon()
    {
        weaponName = WeaponManager.current.currentWeaponClass.Item1._name;
        weaponDamage = WeaponManager.current.currentWeaponClass.Item1._damage;
        weaponFireMode = WeaponManager.current.currentWeaponClass.Item1._fireMode;
        weaponFireRate = 1.0f / WeaponManager.current.currentWeaponClass.Item1._fireRate;
        weaponAmmoType = WeaponManager.current.currentWeaponClass.Item1._ammoType;
        Debug.Log("AMMO TYPE: " + weaponAmmoType);
    }

    private void SetAudioManagerReady()
    {
        audioManagerReady = true;
    }

    public ref ObjectPool GetBulletPool()
    {
        return ref bulletPool;
    }

}
