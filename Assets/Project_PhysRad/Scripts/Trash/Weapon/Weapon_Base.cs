using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Shooter.Gameplay
{
    public class Weapon_Base : MonoBehaviour
    {

        public int WpnID = 0;
        public string Title = "SMG 1";
        public bool AutoFire = true;
        public float FireDelay = 0.2f;
        public float BurstDelay = .5f;
        public int BurstFireCount = 3;
        public float RecoilSpeed = 5;
        [Space]
        public int InitAmmo = 80;
        public int MaxAmmo = 80;
        public int AddAmmo = 10;
        public int PowerWeaponMaxAmmo = 80;
        [Space]
        public Sprite WeaponIcon;
        public float Damage = 1;
        public float ProjectileSpeed = 200;
        public float Range = 100;

        public bool InfiniteAmmo = false;
        [Space]
        public GameObject WeaponModelPrefab;
        public GameObject BulletPrefab;
        public GameObject EffectPrefab;
        [Space]
        [HideInInspector]
        public bool WeaponEnable = true;
        [HideInInspector]
        public int AmmoCount = 50;

        public Transform m_FirePoint;
        public Transform m_ParticlePoint;
        public GameObject m_Owner;


        [HideInInspector]
        public PlayerControl Owner;

        [HideInInspector]
        public float FireDelayTimer = 0;
        [HideInInspector]
        public float BurstDelayTimer = 0;
        [HideInInspector]
        public int BurstFireCounter = 0;
        [HideInInspector]
        public float RecoilTimer = 0;


        [HideInInspector]
        public bool Input_FireHold = false;
        [HideInInspector]
        public Vector3 Forward;

        [HideInInspector]
        public int m_PowerLevel = 0;

        //[SerializeField, Space]
        //private Content m_Contents;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            FireDelayTimer -= Time.deltaTime;
            if (FireDelayTimer <= 0)
                FireDelayTimer = 0;


            BurstDelayTimer -= Time.deltaTime;
            if (BurstDelayTimer <= 0)
                BurstDelayTimer = 0;

            RecoilTimer -= RecoilSpeed * Time.deltaTime;
            if (RecoilTimer <= 0)
                RecoilTimer = 0;

            if (Input_FireHold)
            {
                if (FireDelayTimer == 0)
                {
                    if (BurstDelayTimer == 0)
                    {
                        if (AmmoCount > 0 || InfiniteAmmo)
                        {
                            if (Owner == PlayerControl.MainPlayerController)
                            {
                                CameraControl.m_Current.StartShake(.2f, 1f);
                            }
                            FireWeapon();
                            AmmoCount -= 1;
                            RecoilTimer = 1;
                            BurstFireCounter++;
                            if (BurstFireCounter >= BurstFireCount)
                            {
                                BurstDelayTimer = BurstDelay;
                                BurstFireCounter = 0;
                            }
                        }
                        else
                        {
                            //SoundGallery.PlaySound("EmptyFire1");
                        }
                        FireDelayTimer = FireDelay;
                    }
                }
            }

            Input_FireHold = false;
        }

        public virtual void FireWeapon()
        {
            GameObject obj;
            for (int i = -1; i < 2; i++)
            {
                obj = Instantiate(BulletPrefab);
                //obj.transform.position = CurrentWeaponModel.transform.position;
                obj.transform.position = m_FirePoint.position;
                obj.transform.forward = Quaternion.Euler(0, i * 10, 0) * m_FirePoint.forward;
                Projectile_Base proj = obj.GetComponent<Projectile_Base>();
                proj.Creator = m_Owner;
                //proj.Range = Range;
                proj.Speed = ProjectileSpeed;
                //proj.Damage = Damage;
                Destroy(obj, 5);
            }

            obj = Instantiate(EffectPrefab);
            obj.transform.position = m_FirePoint.position;
            obj.transform.forward = m_FirePoint.forward;
            Destroy(obj, 3);

            //m_Contents.CreateNoise(MyCharacter.WeaponFirePoint.position, 10, 1);
        }
    }
}