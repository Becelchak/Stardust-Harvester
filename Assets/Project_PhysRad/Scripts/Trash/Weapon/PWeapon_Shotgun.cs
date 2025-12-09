using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class PWeapon_Shotgun : Weapon_Base
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (m_PowerLevel == 0)
            {
                FireDelay = .4f;
            }
            else if (m_PowerLevel == 1)
            {
                FireDelay = .3f;
            }


            FireDelayTimer -= Time.deltaTime;
            if (FireDelayTimer <= 0)
                FireDelayTimer = 0;

            RecoilTimer -= 10 * Time.deltaTime;
            if (RecoilTimer <= 0)
                RecoilTimer = 0;

            if (Input_FireHold)
            {
                if (FireDelayTimer == 0)
                {

                    FireWeapon();
                    FireDelayTimer = FireDelay;
                    RecoilTimer = 1f;
                }

            }

            //Input_FireHold = false;
        }

        public override void FireWeapon()
        {

            GameObject obj;

            if (m_PowerLevel == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    obj = Instantiate(BulletPrefab);
                    obj.transform.position = m_FirePoint.position;
                    obj.transform.forward = Quaternion.Euler(0,-6+ i * 6, 0) * m_FirePoint.forward;
                    Projectile_Base proj = obj.GetComponent<Projectile_Base>();
                    proj.Creator = m_Owner;
                    proj.Speed = ProjectileSpeed;
                    //proj.Damage = Damage;
                    proj.m_Range = Range;
                    Destroy(obj, 5);
                }

            }
            else if (m_PowerLevel == 1)
            {
                for (int i = 0; i < 6; i++)
                {
                    obj = Instantiate(BulletPrefab);
                    obj.transform.position = m_FirePoint.position;
                    obj.transform.forward = Quaternion.Euler(0, -30+i * 10, 0) * m_FirePoint.forward;
                    Projectile_Base proj = obj.GetComponent<Projectile_Base>();
                    proj.Creator = m_Owner;
                    proj.Speed = ProjectileSpeed;
                    //proj.Damage = Damage;
                    proj.m_Range = Range;
                    Destroy(obj, 5);
                }
            }

            obj = Instantiate(EffectPrefab);
            obj.transform.SetParent(m_ParticlePoint);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.forward = m_ParticlePoint.forward;
            Destroy(obj, 3);
        }
    }
}