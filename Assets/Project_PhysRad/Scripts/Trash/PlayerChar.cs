using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class PlayerChar : MonoBehaviour
    {
        [HideInInspector]
        public DamageControl m_DamageControl;
        public static PlayerChar m_Current;

        [SerializeField]
        private Transform m_TurnBase;
        [SerializeField]
        private Transform m_AimBase;

        [HideInInspector]
        public CameraControl m_MyCamera;

        //-------------------------------
        [HideInInspector]
        public bool m_InControl = false;
        [HideInInspector]
        public bool m_CanShoot = false;

        public Transform[] m_WeaponHands;
        public Transform m_FirePoint;
        public GameObject m_WeaponPowerParticle;
        public GameObject m_DeathParticle;
        //public WeaponControl m_MyWeaponControl;

        Vector3 m_MovementInput;
        Vector3 m_DashDirection;
        public AnimationCurve m_DashCurve;
        public GameObject m_DashParticle;

        bool m_Input_Fire;
        bool m_Input_Fire2;
        bool m_Input_LockAim;

        public GameObject m_HitParticlePrefab;

        [HideInInspector]
        public int m_CashAmount = 0;
        [HideInInspector]
        public int m_ResourceAmount = 0;
        [HideInInspector]
        public bool m_IsDead = false;

        public Weapon_Base[] m_Weapons;
        [HideInInspector]
        public int m_WeaponNum = 0;

        public bool m_HaveKey = false;

        public TargetObject m_TempTarget;
        public TargetObject m_LockedTarget;

        [HideInInspector]
        public int m_WpnPowerLevel = 0;
        [HideInInspector]
        public float m_WpnPowerTime = 0;

        public Animator m_Animator;

        public GameObject m_GrenadePrefab1;

        [HideInInspector]
        public PlayerPowers m_PlayerPowers;

        public GameObject m_ShieldObject;

        void Awake()
        {
            m_Current = this;
            m_PlayerPowers = GetComponent<PlayerPowers>();
        }

        void Start()
        {
            m_DamageControl = GetComponent<DamageControl>();
            m_DamageControl.OnDamaged.AddListener(HandleDamage);
            m_InControl = true;


            //m_MyWeaponControl.m_MyPlayer = this;

            m_CashAmount = 0;
            m_ResourceAmount = 0;

            m_ShieldObject.transform.SetParent(null);
            m_WeaponPowerParticle.SetActive(false);
            //Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        void Update()
        {
            m_Input_Fire = false;
            m_Input_Fire2 = false;
            m_Input_LockAim = false;
            //input
            if (m_InControl)
            {
                m_Input_Fire = PlayerControl.MainPlayerController.Input_FireHold;

                //if (Input.GetMouseButton(1))
                //{
                //    m_Input_Fire2 = true;
                //}

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    // m_Input_LockAim = true;
                    if (m_PlayerPowers.m_HavePower)
                    {
                        switch (m_PlayerPowers.m_PowerNum)
                        {
                            case 0:
                                if (m_PlayerPowers.m_AmmoCount > 0)
                                {
                                    ThrowGrenade();
                                    m_PlayerPowers.m_AmmoCount--;
                                }
                                break;
                        }
                    }
                }

                //if (Input.GetKeyDown(KeyCode.C))
                //{
                //    CheckMelleeAttack();
                //}

                //if (Input.GetKeyDown(KeyCode.V))
                //{
                //    SetWeaponPowerLevel(1);
                //}

                //if (Input.GetMouseButtonDown(1))
                //{
                //    ThrowGrenade();
                //}




                m_MovementInput = PlayerControl.MainPlayerController.m_Input_Movement;

                //if (Input.GetMouseButtonDown(1))
                //{
                //    StartDash();
                //}

                Vector3 axis = Vector3.Cross(Vector3.up, m_MovementInput);
                Quaternion newRotation = Quaternion.AngleAxis(20, axis);

                //find target
                List<TargetObject> targets = TargetsControl.m_Main.m_Targets;

                TargetObject bestTarget = null;
                float minAngle = 40;
                foreach (TargetObject target in targets)
                {
                    if (target == null)
                        continue;

                    Vector3 targetPos = target.m_TargetCenter.position;
                    Vector3 dir = targetPos - transform.position;
                    dir.y = 0;
                    float delta = Vector3.Angle(m_TurnBase.forward, dir);
                    float distance = dir.magnitude;

                    if (distance > 30)
                        continue;

                    if (delta < minAngle)
                    {
                        bestTarget = target;
                        minAngle = delta;
                    }
                }

                if (bestTarget != null)
                {
                    Vector3 targetPos = bestTarget.m_TargetCenter.position;
                    Vector3 targetDir = targetPos - m_FirePoint.position;
                    targetDir.y = 0;
                    //m_AimBase.forward = targetDir;
                    m_AimBase.rotation = Quaternion.Lerp(m_AimBase.rotation, Quaternion.LookRotation(targetDir), 20 * Time.deltaTime);
                    m_TempTarget = bestTarget;
                }
                else
                {
                    m_TempTarget = null;
                    m_AimBase.localRotation = Quaternion.Lerp(m_AimBase.localRotation, Quaternion.identity, 20 * Time.deltaTime);
                }

                //Vector3 dir = PlayerControl.MainPlayerController.AimPosition - transform.position;
                //dir.y = 0;
                //m_AimBase.forward = dir;

                //if (m_Input_LockAim)
                //{
                //    if (m_LockedTarget==null)
                //    {
                //        m_LockedTarget = bestTarget;
                //    }
                //    //m_TargetEnemy = 
                //    if (m_LockedTarget != null)
                //    {
                //        Vector3 faceDirection = m_LockedTarget.transform.position - transform.position;
                //        faceDirection.y = 0;
                //        faceDirection.Normalize();
                //        m_GunBase.rotation = Quaternion.Lerp(m_GunBase.rotation, Quaternion.LookRotation(faceDirection), 20 * Time.deltaTime);
                //    }
                //}
                //else
                //{
                //m_LockedTarget = null;
                if (m_MovementInput != Vector3.zero)
                {
                    Vector3 faceDirection = m_MovementInput;
                    faceDirection.y = 0;
                    faceDirection.Normalize();
                    m_TurnBase.rotation = Quaternion.Lerp(m_TurnBase.rotation, Quaternion.LookRotation(faceDirection), 10 * Time.deltaTime);
                }
                //}

                m_Weapons[m_WeaponNum].Input_FireHold = m_Input_Fire;

                //m_Weapon2.Input_FireHold = m_Input_Fire2;
            }

            if (m_WpnPowerLevel == 1)
            {
                m_WpnPowerTime -= Time.deltaTime;
                if (m_WpnPowerTime <= 0)
                {
                    m_WeaponPowerParticle.SetActive(false);
                    SetWeaponPowerLevel(0);
                }
            }

            //animation parameters
            Vector3 vSpeed = GetComponent<Rigidbody>().linearVelocity;
            vSpeed.y = 0;
            float runSpeed = Mathf.Clamp(vSpeed.magnitude / 10f, 0, 1);
            m_Animator.SetFloat("RunSpeed", runSpeed);

            //shield
            m_ShieldObject.transform.position = transform.position + new Vector3(0, 1, 0);

            if (!m_IsDead)
            {
                if (m_DamageControl.Damage <= 0)
                {
                    //die
                    m_IsDead = true;
                    GameObject obj = Instantiate(m_DeathParticle);
                    obj.transform.position = transform.position + new Vector3(0, 1, 0);
                    //obj.transform.forward = m_DashDirection;
                    Destroy(obj, 3);
                    gameObject.SetActive(false);
                }
            }
        }

        void FixedUpdate()
        {
            Rigidbody rigidBody = GetComponent<Rigidbody>();

            Vector3 totalVelocity = rigidBody.linearVelocity;
            if (m_MovementInput != Vector3.zero)
            {
                totalVelocity += 5 * m_MovementInput;
                totalVelocity.y = 0;
                totalVelocity = Vector3.ClampMagnitude(totalVelocity, 11);
                totalVelocity.y = rigidBody.linearVelocity.y;
                rigidBody.linearVelocity = totalVelocity;
            }
            else
            {
                totalVelocity -= .4f * totalVelocity;
                totalVelocity.y = rigidBody.linearVelocity.y;
                rigidBody.linearVelocity = totalVelocity;
            }

        }

        public void HandleDamage()
        {

            CameraControl.m_Current.StartShake(.2f, .1f);
        }

        void LateUpdate()
        {
            float recoil = m_Weapons[m_WeaponNum].RecoilTimer;
            m_WeaponHands[0].localRotation *= Quaternion.Euler(0, -4 * recoil, 0);
            m_WeaponHands[1].localRotation *= Quaternion.Euler(0, -4 * recoil, 0);
            m_WeaponHands[0].localPosition += new Vector3(0, 0, -.5f * recoil);
        }
        public void CheckMelleeAttack()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position + 2 * m_AimBase.forward, 1);
            foreach (Collider col in colls)
            {
                if (col.gameObject.tag == "Enemy")
                {
                    DamageControl d = col.gameObject.GetComponent<DamageControl>();
                    if (d != null)
                    {
                        Vector3 dir = col.gameObject.transform.position - transform.position;
                        dir.Normalize();
                        d.ApplyDamage(5, dir, 1);
                    }
                }
                else if (col.gameObject.tag == "Block")
                {
                    //Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
                    //if (rb != null)
                    //{
                    //    Vector3 dir = col.gameObject.transform.position - transform.position;
                    //    dir.Normalize();
                    //    rb.AddForceAtPosition(3000 * dir, col.gameObject.transform.position);
                    //}

                    DamageControl d = col.gameObject.GetComponent<DamageControl>();
                    if (d != null)
                    {
                        //float lerp = Vector3.Distance(col.bounds.center, transform.position) / Radius;
                        d.ApplyDamage(1, transform.forward, 1);
                    }
                }
            }
        }

        public void AddAmmo(int count)
        {

        }

        public void SetWeaponPowerLevel(int level)
        {
            m_WpnPowerLevel = level;
            if (level == 1)
            {
                m_WpnPowerTime = 16;
                m_WeaponPowerParticle.SetActive(true);
            }

            foreach (Weapon_Base w in m_Weapons)
            {
                w.m_PowerLevel = level;
            }
        }

        public void SetWeapon(int num)
        {
            foreach (Weapon_Base w in m_Weapons)
            {
                w.Input_FireHold = false;
            }
            m_WeaponNum = num;
        }

        public void ThrowGrenade()
        {
            Vector3 start = transform.position;
            Vector3 end = PlayerControl.MainPlayerController.AimPosition + new Vector3(0, 1, 0);
            GameObject obj = Instantiate(m_GrenadePrefab1);
            obj.transform.position = transform.position;
            PlayerGrenade g = obj.GetComponent<PlayerGrenade>();
            g.m_StartPosition = start;
            g.m_TargetPosition = end;
            //Destroy(obj, 3);
        }

        public void HandlePickup(string itemType, int count)
        {
            if (itemType == "Gem")
            {
                PlayerControl.MainPlayerController.m_GemCount++;
            }
            else if (itemType == "WeaponPower")
            {
                SetWeaponPowerLevel(1);
            }
            else if (itemType == "Weapon_Pistol")
            {
                SetWeapon(0);
            }
            else if (itemType == "Weapon_Shotgun")
            {
                SetWeapon(1);
            }
            else if (itemType == "Power_Grenade")
            {
                m_PlayerPowers.SetNewPower(0);
            }
            else if (itemType == "Health")
            {
                m_DamageControl.AddHealth(count);
            }
            else if (itemType == "Key")
            {
                m_HaveKey = true;
            }
        }

        public void StartDash()
        {
            m_DashDirection = m_MovementInput;
            if (m_DashDirection != Vector3.zero)
            {
                StartCoroutine(Co_Dash());
            }
        }

        IEnumerator Co_Dash()
        {
            GameObject obj = Instantiate(m_DashParticle);
            obj.transform.position = transform.position + new Vector3(0, 1, 0);
            obj.transform.forward = m_DashDirection;
            Destroy(obj, 3);

            GetComponent<Collider>().enabled = false;
            m_Animator.SetBool("Dashing", true);
            GetComponent<Rigidbody>().isKinematic = true;
            float lerp = 0;
            Vector3 startPos = transform.position;
            Vector3 endPos = transform.position + 6 * m_DashDirection;
            while (lerp < 1)
            {
                transform.position = Vector3.Lerp(startPos, endPos, m_DashCurve.Evaluate(lerp));
                lerp += 4 * Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;
            GetComponent<Rigidbody>().isKinematic = false;
            m_DashDirection = Vector3.zero;
            m_Animator.SetBool("Dashing", false);
            GetComponent<Collider>().enabled = true;
        }
        public void Hit()
        {
            //m_MyCamera.StartShake(.5f, .5f);
            //GameObject obj = Instantiate(m_HitParticlePrefab);
            //obj.transform.position = transform.position;
            //Destroy(obj, 1);
        }
    }
}