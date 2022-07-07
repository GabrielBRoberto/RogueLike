using UnityEngine.SceneManagement;
using Roguelike.Extension;
using UnityEngine.Pool;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Roguelike
{
    public enum LevelsType
    {
        Medieval
            /*
        Cyberpunk,
        Chinese,
        Japonese,
        Modern,
        Pirate
            */
    }

    enum MenuAreas
    {
        Main,
        Play,
        InGame,
        Character,
        Settings,
        Exit
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private string ANIMATOR_CURRENT_STATE;
        private const string ANIMATOR_PLAY = "Play";
        private const string ANIMATOR_PORTALVIEW = "PortalView";
        private const string ANIMATOR_CHARACTER = "CharacterView";
        private const string ANIMATOR_PORTAL_TO_CHARACTER = "PortalToCharacter";

        private MenuAreas menuAreas;
        private LevelsType levelsType;
        private Animator animator;
        private float delay;
        private bool loaded = false;
        public int cost;

        [Header("Buttons")]
        [SerializeField]
        private GameObject portalButtons;
        [SerializeField]
        private GameObject characterButtons;
        [SerializeField]
        private Button[] upgradeButtons;

        [Header("Text")]
        [SerializeField]
        private TMP_Text upgradeCostText;

        [Header("PlayerStats")]
        [SerializeField]
        private PlayerStats playerStats;

        [Header("EnemyAttack")]
        public ObjectPool<GameObject> fireballPool;
        [SerializeField]
        private GameObject fireballPrefab;

        private void Awake()
        {
            menuAreas = new MenuAreas();
            levelsType = new LevelsType();

            animator = Camera.main.GetComponentInParent<Animator>();

            fireballPool = new ObjectPool<GameObject>(createFireball, takeFireball, releaseFireball);

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
            else if (Instance != null)
            {
                Destroy(gameObject);
            }

            if (portalButtons == null)
            {
                portalButtons = GameObject.Find("Canvas/Portal");
                portalButtons.SetActive(false);
            }
            if (characterButtons == null)
            {
                characterButtons = GameObject.Find("Canvas/Character");
                portalButtons.SetActive(false);
            }
        }

        private void Update()
        {
            menuAreasSwitch();

            if (menuAreas != MenuAreas.Main)
            {
                portalButtons.SetActive(false);
            }
            if (menuAreas != MenuAreas.Character)
            {
                characterButtons.SetActive(false);
            }

            upgradeCostText.text = "Cost per Click: " + cost;

            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                if (playerStats.money < cost)
                {
                    upgradeButtons[i].interactable = false;
                }
                else
                {
                    upgradeButtons[i].interactable = true;
                }
            }
        }

        #region Menu Areas

        private void menuAreasSwitch()
        {
            switch (menuAreas)
            {
                case MenuAreas.Main:
                    Main();
                    break;
                case MenuAreas.Play:
                    Play();
                    break;
                case MenuAreas.InGame:
                    InGame();
                    break;
                case MenuAreas.Character:
                    Character();
                    break;
                case MenuAreas.Settings:
                    Settings();
                    break;
                case MenuAreas.Exit:
                    Exit();
                    break;
            }
        }

        private void Main()
        {
            ChangeAnimationState(ANIMATOR_PORTALVIEW);

            float delay = animator.GetCurrentAnimatorStateInfo(0).length;
            Invoke("ChangeButtons", delay);
        }
        private void Play()
        {
            ChangeAnimationState(ANIMATOR_PLAY);

            delay = animator.GetCurrentAnimatorStateInfo(0).length;
            Invoke("changeToInGame", delay);
        }
        private void InGame()
        {
            levelsType = Extensions.GetRandomEnum<LevelsType>();
            Debug.Log(levelsType);

            if (!loaded)
            {
                SceneManager.LoadScene("Game");
                loaded = true;
            }
        }
        private void Character()
        {
            ChangeAnimationState(ANIMATOR_CHARACTER);

            delay = animator.GetCurrentAnimatorStateInfo(0).length;
            Invoke("ChangeButtons", delay);
        }
        private void Settings()
        {

        }
        private void Exit()
        {
            Application.Quit();
        }

        #endregion
        #region Triggers Change menuAreas

        public void changeToMenu()
        {
            menuAreas = MenuAreas.Main;
        }
        public void changeToPlay()
        {
            menuAreas = MenuAreas.Play;
        }
        public void changeToInGame()
        {
            menuAreas = MenuAreas.InGame;
        }
        public void changeToCharacter()
        {
            menuAreas = MenuAreas.Character;
        }
        public void changeToSettings()
        {
            menuAreas = MenuAreas.Settings;
        }
        public void changeToExit()
        {
            menuAreas = MenuAreas.Exit;
        }
        #endregion

        private void ChangeAnimationState(string NEW_STATE)
        {
            if (ANIMATOR_CURRENT_STATE == NEW_STATE)
            {
                return;
            }

            animator.Play(NEW_STATE);

            ANIMATOR_CURRENT_STATE = NEW_STATE;
        }

        private void ChangeButtons()
        {
            switch (menuAreas)
            {
                case MenuAreas.Main:
                    Debug.Log("1");
                    portalButtons.SetActive(true);
                    //portalButtons.SetActive(true);
                    break;
                case MenuAreas.Play:
                    break;
                case MenuAreas.InGame:
                    break;
                case MenuAreas.Character:
                    Debug.Log("2");
                    characterButtons.SetActive(true);
                    break;
                case MenuAreas.Settings:
                    break;
                case MenuAreas.Exit:
                    break;
                default:
                    break;
            }
        }

        #region Upgrades

        public void increaseHealth()
        {
            playerStats.maxHealth += 10;
        }
        public void increaseShield()
        {
            playerStats.maxShield += 10;
        }
        public void increaseSpeed()
        {
            playerStats.speed += 1;
        }
        public void increaseResistence()
        {
            if (playerStats.actualResistence < playerStats.maxResistence)
            {
                playerStats.actualResistence += 10;
            }
            if (playerStats.actualResistence > playerStats.maxResistence)
            {
                playerStats.actualResistence = playerStats.maxResistence;
                upgradeButtons[3].interactable = false;
            }
        }
        public void increaseDamage()
        {
            if (playerStats.actualDamage < playerStats.maxDamage)
            {
                playerStats.actualDamage += 10;
            }
            if (playerStats.actualDamage > playerStats.maxDamage)
            {
                playerStats.actualDamage = playerStats.maxDamage;
                upgradeButtons[4].interactable = false;
            }
        }
        public void increaseLifesteal()
        {
            if (playerStats.actualLifeSteal < playerStats.maxLifeSteal)
            {
                playerStats.actualLifeSteal += 5;
            }
            if (playerStats.actualLifeSteal > playerStats.maxLifeSteal)
            {
                playerStats.actualLifeSteal = playerStats.maxLifeSteal;
                upgradeButtons[5].interactable = false;
            }
        }
        public void increaseLuck()
        {
            if (playerStats.actualLuck < playerStats.maxLuck)
            {
                playerStats.actualLuck += 5;
            }
            if (playerStats.actualLuck > playerStats.maxLuck)
            {
                playerStats.actualLuck = playerStats.maxLuck;
                upgradeButtons[6].interactable = false;
            }
        }
        public void increaseShieldRegen()
        {
            if (playerStats.actualRegenShieldRate < playerStats.maxRegenShieldRate)
            {
                playerStats.actualRegenShieldRate += 5;
            }
            if (playerStats.actualRegenShieldRate > playerStats.maxRegenShieldRate)
            {
                playerStats.actualRegenShieldRate = playerStats.maxRegenShieldRate;
                upgradeButtons[7].interactable = false;
            }
        }

        public void increaseCost()
        {
            playerStats.money -= cost;

            cost += 10;
            
        }

        #endregion

        #region Fireball Pool

        private GameObject createFireball()
        {
            return Instantiate(fireballPrefab);
        }
        public void takeFireball(GameObject fireball)
        {
            fireball.SetActive(true);
        }
        public void releaseFireball(GameObject fireball)
        {
            fireball.GetComponent<Rigidbody>().velocity = Vector3.zero;

            fireball.SetActive(false);
        }

        #endregion
    }
}