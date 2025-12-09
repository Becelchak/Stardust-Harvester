using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shooter.Gameplay;

namespace Shooter.UI
{
    public class UI_HUD : MonoBehaviour
    {
        public Image m_DamageOverlay;
        public Text[] m_PlayerTexts_1;
        public Text m_GemCountText;
        public Text m_GunNameText;
        public Image m_AimTargetImage;
        public RectTransform m_MainCanvas;

        public Image m_WeaponPowerTime;
        public Image m_PlayerHealth;

        [Space]
        public Image m_BossHealthBase;
        public Image m_BossHealth;

        [Space]
        public Image m_PowerBase;
        public Image m_PowerBar;
        public Text m_PowerNameText;
        public Text m_PowerAmountText;

        public string[] m_WeaponNames = new string[4] { "PISTOL", "SHOTGUN", "MACHINGUN", "PLASMA GUN" };

        public static UI_HUD m_Main;

        void Awake()
        {
            m_Main = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            //Cursor.visible = false;
            m_BossHealthBase.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            m_GemCountText.text = PlayerControl.MainPlayerController.m_GemCount.ToString();

            if (PlayerChar.m_Current.m_TempTarget != null)
            {
                m_AimTargetImage.gameObject.SetActive(true);
                Vector3 v = CameraControl.m_Current.m_MyCamera.WorldToScreenPoint(PlayerChar.m_Current.m_TempTarget.m_TargetCenter.position);
                v.x = v.x / (float)Screen.width;
                v.y = v.y / (float)Screen.height;

                v.x = m_MainCanvas.sizeDelta.x * v.x;
                v.y = m_MainCanvas.sizeDelta.y * v.y;

                m_AimTargetImage.rectTransform.anchoredPosition = Helper.ToVector2(v);



            }
            else
            {
                m_AimTargetImage.gameObject.SetActive(false);
            }


            m_WeaponPowerTime.fillAmount = PlayerChar.m_Current.m_WpnPowerTime / 16f;

            DamageControl damage = PlayerChar.m_Current.GetComponent<DamageControl>();
            m_PlayerHealth.fillAmount = damage.Damage / damage.MaxDamage;


            m_GunNameText.text = m_WeaponNames[PlayerChar.m_Current.m_WeaponNum];

            if (GameControl.m_Current.m_LevelBoss != null)
            {
                damage = GameControl.m_Current.m_LevelBoss.GetComponent<DamageControl>();
                m_BossHealth.fillAmount = damage.Damage / damage.MaxDamage;
            }

            PlayerPowers p = PlayerChar.m_Current.GetComponent<PlayerPowers>();
            if (p.m_HavePower)
            {
                m_PowerBase.gameObject.SetActive(true);
                switch (p.m_PowerNum)
                {
                    case 0:
                        m_PowerNameText.text = "Grenade";
                        m_PowerAmountText.gameObject.SetActive(true);
                        m_PowerAmountText.text = p.m_AmmoCount.ToString();
                        m_PowerBar.gameObject.SetActive(false);
                        break;
                    case 1:
                        m_PowerNameText.text = "Bomb";
                        m_PowerAmountText.gameObject.SetActive(true);
                        m_PowerAmountText.text = p.m_AmmoCount.ToString();
                        m_PowerBar.gameObject.SetActive(false);
                        break;
                }
            }
            else
            {
                m_PowerBase.gameObject.SetActive(false);
            }

        }

        public void ShowBossHealth()
        {
            m_BossHealthBase.gameObject.SetActive(true);
        }
        public void HideBossHealth()
        {
            m_BossHealthBase.gameObject.SetActive(false);
        }

    }
}
