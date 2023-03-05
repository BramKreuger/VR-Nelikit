using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    // Other managers
    private IntroductionManager introManager;
    private PlayPhaseManager playPhaseManager;

    // GameState enums 

    public enum GameState
    {
        introduction,
        selection,
        playing
    }

    [System.NonSerialized] public GameState currentGameState = GameState.introduction;

    public enum Role
    {
        none,
        thrower,
        catcher
    }

    [System.NonSerialized] public Role currentRole = Role.catcher;

    [System.NonSerialized] public bool paused = false;   

    public enum UiScreen
    {
        none,
        paused,
        tutorial,
        language
    }

    [System.NonSerialized] public UiScreen currentUiScreen = UiScreen.none;

    public enum Language
    {
        english,
        penan
    }

    [System.NonSerialized] public Language currentLanguage = Language.english;

    public enum DifficultySpeed
    {
        easy,
        medium,
        hard
    }

    [System.NonSerialized] public DifficultySpeed currentDifficultySpeed = DifficultySpeed.medium;

    public enum DifficultySize
    {
        large,
        medium,
        small
    }

    [System.NonSerialized] public DifficultySize currentDifficultySize = DifficultySize.medium;

    [Header("UI GameObjects")]

    public GameObject restartButton;

    public GameObject selectButton;

    public GameObject startButton;

    private Button startButton_button;

    public GameObject pauseButton;

    public GameObject pauseUI;

    public GameObject roleUI;

    public TextMeshProUGUI gameStateText;

    void Start()
    {
        introManager = gameObject.GetComponent<IntroductionManager>();
        playPhaseManager = gameObject.GetComponent<PlayPhaseManager>();
        startButton_button = startButton.GetComponent<Button>();
    }

    private void Update()
    {
        
    }

    /// <summary>
    /// This will change the game state. There are three: intro (0), selection (1), playing(2). Mainly used by buttons now...
    /// </summary>
    /// <param name="state">this parameter correlates an int to the enum state.</param>
    public void ChangeGameState(int state)
    {       
        switch ((GameState)state)
        {           
            case GameState.introduction:
                Debug.Log("Start introduction");
                SceneManager.LoadScene(0);
                break;
            case GameState.selection:
                if(currentGameState == GameState.introduction) // If we are in the intro and we're skipping it.
                {
                    currentGameState = (GameState)state;
                    //introManager.EndIntro();
                }
                currentGameState = (GameState)state; // Change currentGameState
                Debug.Log("Start selection");
                selectButton.SetActive(false);
                pauseButton.SetActive(true);
                roleUI.SetActive(true);
                startButton.SetActive(true);
                break;
                case GameState.playing:
                if (currentRole != Role.none) // You need to choose a role first
                {
                    currentGameState = (GameState)state; // Change currentGameState
                    Debug.Log("Start playing");
                    selectButton.SetActive(false);
                    pauseButton.SetActive(true);
                    roleUI.SetActive(false);
                    startButton.SetActive(false);

                    // Move over to the playPhaseManager and start the game
                    //playPhaseManager.StartGame(Role.catcher);
                }
                break;
        }        
        UpdateDebugText(" ");
    }

    public void ChangeUIScreen(int state)
    {
        switch ((UiScreen)state)
        {
            case UiScreen.none:                
                Debug.Log("UI none");
                UpdateDebugText(" ");
                break;
            case UiScreen.paused:
                Debug.Log("UI Paused");
                UpdateDebugText("UI: Paused");
                break;
            case UiScreen.language:
                currentUiScreen = UiScreen.language;                
                Debug.Log("UI Language");
                UpdateDebugText("UI: Language");
                break;
            case UiScreen.tutorial:
                currentUiScreen = UiScreen.language;
                Debug.Log("UI Tutorial");
                UpdateDebugText("UI: Tutorial");
                break;
        }        
    }

    public void ChangeRole(int state)
    {
        switch ((Role)state)
        {
            case Role.none:
                currentRole = Role.none;
                Debug.Log("Role: None");
                UpdateDebugText(" ");
                startButton_button.interactable = false;
                break;
            case Role.catcher:
                currentRole = Role.catcher;
                Debug.Log("Role: catcher");
                UpdateDebugText(" ");
                startButton_button.interactable = true;
                break;
            case Role.thrower:
                currentRole = Role.thrower;
                Debug.Log("Role: thrower");
                UpdateDebugText(" ");
                startButton_button.interactable = true;
                break;

        }
    }

    public void PauzeGame()
    {
        paused = true;
        currentUiScreen = UiScreen.paused;
        pauseUI.SetActive(true);
        restartButton.SetActive(true);
        startButton.SetActive(false);
        UpdateDebugText("UI: paused");
    }

    public void ResumeGame()
    {
        paused = false;
        currentUiScreen = UiScreen.none;
        pauseUI.SetActive(false);
        restartButton.SetActive(false);
        startButton.SetActive(true);
        UpdateDebugText(" ");        
    }

    /// <summary>
    /// Use this function to update the debug text shown in the game. It will show the current game state and the role (if applicable).
    /// </summary>
    /// <param name="addString">This adds a arbitrary string at the end, seperated with a new line.</param>
    private void UpdateDebugText(string addString)
    {
        string roleString = "";

        if(currentRole != Role.none)
        {
            roleString = "\nRole: " + currentRole.ToString();
        }
        else
        {
            roleString = "";
        }
        gameStateText.text = "Game state: " + currentGameState.ToString() + roleString + "\n" + addString;
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
