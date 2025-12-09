using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class PlayerControl : MonoBehaviour
    {

        [HideInInspector]
        public PlayerChar MyPlayerChar;
        [HideInInspector]
        public GameObject MyPlayerPlane;


        public Transform m_SpawnPoint;

        [HideInInspector]
        public int m_GemCount = 0;

        [HideInInspector]
        public PlayerControl OtherControl;

        public GameObject PlayerPrefab1;
        //public GameObject PlayerPlanePrefab1;

        public int ControllerNum = 0;
        [HideInInspector]
        public string PlayerName = "Player";


        [HideInInspector]
        public int Kills = 0;

        public bool m_IsDead = false;

        [HideInInspector]
        public Vector3 ReticlePosition;

        [HideInInspector]
        public Weapon_Base[] MainWeapons = new Weapon_Base[2];
        [HideInInspector]
        public int CurrentWeaponNum = 0;
        [HideInInspector]
        public Weapon_Base CurrentWeapon;


        [HideInInspector]
        public Transform NextSpawnPoint;

        [HideInInspector]
        public Vector3 LastDeathPosition;

        bool SelectedSpawnPoint = false;

        public static PlayerControl MainPlayerController;

        [HideInInspector]
        public int State = 0;
        [HideInInspector]
        public float StateStartTime = 0;

        [HideInInspector]
        public bool UsingPowerWeapon = false;


        //---------------input
        [HideInInspector]
        public bool InputEnable = true;
        [HideInInspector]
        public Vector3 m_Input_Movement;
        [HideInInspector]
        public Vector3 AimPosition;
        [HideInInspector]
        public Vector2 m_CameraAngle;
        [HideInInspector]
        public bool Input_Fire = false;
        [HideInInspector]
        public bool Input_FireHold = false;
        [HideInInspector]
        public bool Input_HoldAim = false;
        [HideInInspector]
        public bool Input_Grenade = false;
        [HideInInspector]
        public bool Input_Force = false;
        [HideInInspector]
        public bool Input_Dash = false;
        [HideInInspector]
        public bool Input_ChangeWeapon = false;
        [HideInInspector]
        public bool Input_Interact = false;
        [HideInInspector]
        public bool Input_Fly = false;
        [HideInInspector]
        public bool[] Input_Detonate;

        [Space]
        public Transform m_AimPointTransofrm;


        [HideInInspector]
        public bool m_IsOnFoot = false;

        //[HideInInspector]
        //public PlayerInvetory m_Inventory;

        //[SerializeField, Space]
        //private DataStorage m_DataStorage;
        //[SerializeField, Space]
        //private Content m_Contents;


        void Awake()
        {
            //GameControl.MainGameControl.PlayerControls.Insert(ControllerNum, this);
            //GameControl.MainGameControl.PlayerControls.Add(this);
            MainPlayerController = this;
            //m_Inventory = GetComponent<PlayerInvetory>();
        }

        void Start()
        {
            InputEnable = true;
            //Cursor.lockState = CursorLockMode.Locked;

            MainWeapons = new Weapon_Base[2];


            //if (m_DataStorage.Level_Weapons[0] != -1)
            //{
            //    obj = (GameObject)Instantiate(m_Contents.WeaponPrefabs[0].gameObject);
            //    w = obj.GetComponent<Weapon_Base>();
            //    MainWeapons[0] = w;
            //}
            //else
            //{
            //    obj = (GameObject)Instantiate(m_Contents.WeaponPrefabs[0].gameObject);
            //    w = obj.GetComponent<Weapon_Base>();
            //    MainWeapons[0] = w;
            //}

            //if (m_DataStorage.Level_Weapons[1] != -1)
            //{
            //    obj = (GameObject)Instantiate(m_Contents.WeaponPrefabs[0].gameObject);
            //    w = obj.GetComponent<Weapon_Base>();
            //    MainWeapons[1] = w;
            //}
            //else
            //{
            //    obj = (GameObject)Instantiate(m_Contents.WeaponPrefabs[0].gameObject);
            //    w = obj.GetComponent<Weapon_Base>();
            //    MainWeapons[1] = w;
            //}


            //for (int i = 0; i < 1; i++)
            //{
            //    MainWeapons[i].AmmoCount = MainWeapons[i].InitAmmo;
            //    //MainWeapons[i].AmmoCount = 10;
            //    MainWeapons[i].WeaponEnable = true;
            //    MainWeapons[i].Owner = this;
            //}

            State = 0;
            StateStartTime = Time.time;
            InputEnable = true;
            m_IsOnFoot = true;
            Respawn();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateInputs();

            //if (Input_Fly)
            //{
            //    if (m_IsOnFoot)
            //    {
            //        StartCoroutine(Co_EnterPlane());
            //    }
            //    else
            //    {
            //        StartCoroutine(Co_ExitPlane());
            //    }
            //}
            //m_InCarRange = false;
            //if (!m_IsDriving)
            //{
            //    if (Vector3.Distance(MyPlayerChar.transform.position, m_CurrentCar.transform.position) <= 5)
            //    {
            //        m_InCarRange = true;

            //        if (Input_Interact)
            //        {
            //            EnterVehicle();
            //        }
            //    }
            //}
            //else
            //{
            //    if (Input_Interact)
            //    {
            //        ExitVehicle();
            //    }
            //}

            //if (m_IsDead)
            //{
            //    RespawnDelay -= Time.deltaTime;

            //    if (RespawnDelay <= 1.8f)
            //    {
            //        if (!SelectedSpawnPoint)
            //        {
            //            SelectedSpawnPoint = true;
            //            FindSpawnPoint();

            //        }
            //    }
            //    if (RespawnDelay <= 0)
            //    {
            //        RespawnDelay = 0;
            //        m_IsDead = false;
            //        Respawn();
            //    }
            //}
            if (!m_IsDead)
            {
                if (PlayerChar.m_Current.m_IsDead)
                {
                    m_IsDead = true;
                    GameControl.m_Current.HandlePlayerDeath();
                }
            }
        }

        public void UpdateInputs()
        {
            m_Input_Movement = Vector3.zero;
            Input_Fire = false;
            Input_FireHold = false;
            Input_Interact = false;
            Input_HoldAim = false;
            Input_Grenade = false;
            Input_Fly = false;
            Input_Dash = false;
            Input_ChangeWeapon = false;
            Input_Detonate = new bool[4];

            Vector3 cameraForward = CameraControl.m_Current.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();
            Vector3 cameraRight = Helper.RotatedVector(90, cameraForward);

            if (Input.GetKey(KeyCode.UpArrow))
            {
                m_Input_Movement += cameraForward;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                m_Input_Movement -= cameraForward;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                m_Input_Movement -= cameraRight;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                m_Input_Movement += cameraRight;
            }

            if (Input.GetKey(KeyCode.Z))
            {
                Input_FireHold = true;
            }

            //if (Input.GetKey(KeyCode.X) || Input.GetMouseButton(0))
            //{
            //    Input_FireHold = true;

            //}

            //if (Input.GetMouseButton(1))
            //{
            //    Input_HoldAim = true;
            //}

            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //    Input_Interact = true;
            //}

            //if (Input.GetKeyDown(KeyCode.G))
            //{
            //    Input_Grenade = true;
            //}
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    Input_Dash = true;
            //}
            //if (Input.GetKeyDown(KeyCode.T))
            //{
            //    Input_Force = true;
            //}

            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    Input_ChangeWeapon = true;
            //}

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    Input_Fly = true;
            //}

            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    Input_Detonate[0] = true;
            //}
            //else if (Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    Input_Detonate[1] = true;
            //}
            //else if (Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    Input_Detonate[2] = true;
            //}
            //else if (Input.GetKeyDown(KeyCode.Alpha4))
            //{
            //    Input_Detonate[3] = true;
            //}


            //Vector3 reticleMove = Vector3.zero;
            //reticleMove.x += Input.GetAxis("Mouse X");
            //reticleMove.z += Input.GetAxis("Mouse Y");

            //m_CameraAngle.x += 500 * Time.deltaTime * reticleMove.x;

            //if (Input.GetKey(KeyCode.R))
            //{
            //    m_CameraAngle.x += 500 * Time.deltaTime;
            //}

            //AimPosition += 200 * Time.deltaTime * reticleMove;
            //AimPosition = Vector3.ClampMagnitude(AimPosition, 20);
            //AimPosition.y = 2;

            Ray ray = CameraControl.m_Current.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            float dis = 0;
            new Plane(Vector3.up, Vector3.zero).Raycast(ray, out dis);
            AimPosition = ray.origin + dis * ray.direction;
            m_AimPointTransofrm.position = AimPosition;
            ReticlePosition = m_AimPointTransofrm.position;
        }
        public void Kill()
        {
            m_IsDead = true;
            //SelectedSpawnPoint = false;
            //RespawnDelay = 2;
        }

        public void EnterVehicle()
        {
            //m_CurrentCar.m_Control = true;
            //MyPlayerChar.gameObject.SetActive(false);
            //m_IsDriving = true;
        }

        public void ExitVehicle()
        {
            //MyPlayerChar.transform.position = m_CurrentCar.transform.position + new Vector3(0, 30, 0);
            //MyPlayerChar.transform.position = m_CurrentCar.transform.position;
            //m_CurrentCar.m_Control = false;
            //MyPlayerChar.gameObject.SetActive(true);
            //m_IsDriving = false;
        }

        public void EnablePlayer(bool enable)
        {
            MyPlayerChar.gameObject.SetActive(enable);
        }
        public void Respawn()
        {
            GameObject obj = Instantiate(PlayerPrefab1);
            MyPlayerChar = obj.GetComponent<PlayerChar>();

            if (GameControl.m_Current.m_MainSaveData.m_CheckpointNumber == 0)
            {
                MyPlayerChar.transform.position = m_SpawnPoint.position + new Vector3(0, .1f, 0);
            }
            else
            {
                int num = GameControl.m_Current.m_MainSaveData.m_CheckpointNumber - 1;
                MyPlayerChar.transform.position = CheckpointControl.m_Main.m_Checkpoints[num].m_SpawnPoint.position;
            }

            //GameObject obj1 = Instantiate(PlayerPlanePrefab1);
            //MyPlayerPlane = obj1;
            //MyPlayerPlane.transform.position = new Vector3(0, 30f, 0);
            //MyPlayerPlane.gameObject.SetActive(false);

            State = 1;
            StateStartTime = Time.time;
        }

        public IEnumerator Co_ExitPlane()
        {
            MyPlayerChar.gameObject.SetActive(true);
            MyPlayerChar.GetComponent<Rigidbody>().isKinematic = true;
            MyPlayerPlane.GetComponent<Rigidbody>().isKinematic = true;

            Vector3 pos1 = MyPlayerPlane.transform.position;
            MyPlayerChar.transform.position = pos1;
            Vector3 pos2 = pos1;
            pos2.y = 0.1f;

            float counter = 0;

            while(counter<1)
            {
                MyPlayerChar.transform.position = Vector3.Lerp(pos1, pos2, counter);
                counter += 0.02f;
                yield return null;
            }

            MyPlayerChar.transform.position = pos2;
            MyPlayerChar.GetComponent<Rigidbody>().isKinematic = false;
            MyPlayerPlane.gameObject.SetActive(false);
            m_IsOnFoot = true;
            yield return null;
        }

        public IEnumerator Co_EnterPlane()
        {
            MyPlayerPlane.gameObject.SetActive(true);
            MyPlayerChar.GetComponent<Rigidbody>().isKinematic = true;
            MyPlayerPlane.GetComponent<Rigidbody>().isKinematic = true;

            Vector3 pos1 = MyPlayerChar.transform.position;
            Vector3 pos2 = pos1;
            pos2.y = 30f;
            MyPlayerPlane.transform.position = pos2;
           

            float counter = 0;

            while (counter < 1)
            {
                MyPlayerChar.transform.position = Vector3.Lerp(pos1, pos2, counter);
                counter += 0.02f;
                yield return null;
            }

            MyPlayerChar.transform.position = pos2;
            MyPlayerChar.gameObject.SetActive(false);
            MyPlayerPlane.GetComponent<Rigidbody>().isKinematic = false;
            m_IsOnFoot = false;
            yield return null;
        }

        public void FindSpawnPoint()
        {
            //PlayerControl other = GameControl.MainGameControl.PlayerControls[0];
            //if (GameControl.MainGameControl.PlayerControls[0] == this)
            //{
            //    other = GameControl.MainGameControl.PlayerControls[1];
            //}

            //while (true)
            //{
            //    int r = Random.Range(0, GameControl.MainGameControl.SpawnPoints.Length);
            //    if (other.MyPlayerChar == null || Vector3.Distance(other.MyPlayerChar.transform.position, GameControl.MainGameControl.SpawnPoints[r].transform.position) > 20)
            //    {
            //        NextSpawnPoint = GameControl.MainGameControl.SpawnPoints[r].transform;
            //        break;
            //    }
            //}
        }

        public void GiveWeapon(int num, int ammo)
        {
            MainWeapons[num].AmmoCount = ammo;
            MainWeapons[num].WeaponEnable = true;

            CurrentWeaponNum = num;
            CurrentWeapon = MainWeapons[num];
            //MyPlayerChar.ArmWeapon();
        }
    }
}