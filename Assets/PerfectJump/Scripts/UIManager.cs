using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [Header("GUI Components")]
    public GameObject mainMenuGui;
    public GameObject pauseGui, gameplayGui, gameOverGui;

    public GameStateEnum gameState;

    bool clicked;
    [SerializeField] float minMenuStartDelay = 0.2f;
    float menuEnterTime;
    Coroutine clickedResetRoutine;

    // Use this for initialization
    void Start()
    {
        mainMenuGui.SetActive(true);
        pauseGui.SetActive(false);
        gameplayGui.SetActive(false);
        gameOverGui.SetActive(false);
        gameState = GameStateEnum.MENU;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && gameState == GameStateEnum.MENU && !clicked)
        {
            if (IsButton())
                return;

            AudioManager.Instance.PlayEffectsAudio(AudioManager.Instance.ButtonClickAudio);
            ShowGameplay();
            AudioManager.Instance.PlayMusic(AudioManager.Instance.GameMusicAudio);
        }
        else if (Input.GetMouseButtonUp(0) && clicked && gameState == GameStateEnum.MENU)
            clicked = false;
    }

    //show main menu
    public void ShowMainMenu()
    {
        clicked = true;
        mainMenuGui.SetActive(true);
        pauseGui.SetActive(false);
        gameplayGui.SetActive(false);
        gameOverGui.SetActive(false);
        gameState = GameStateEnum.MENU;
        AudioManager.Instance.PlayEffectsAudio(AudioManager.Instance.ButtonClickAudio);
        GameManager.Instance.ClearScene();

        // Always resume time when returning to main menu to avoid being stuck paused
        Time.timeScale = 1;

        GameManager.Instance.CreateScene();

        // Ensure we only re-enable clicks after the user has released the button once
        if (clickedResetRoutine != null)
            StopCoroutine(clickedResetRoutine);
        menuEnterTime = Time.unscaledTime;
        clickedResetRoutine = StartCoroutine(EnableClickAfterPointerRelease());
    }

    IEnumerator EnableClickAfterPointerRelease()
    {
        // Wait until the user releases the button and the minimum delay has passed
        while (Input.GetMouseButton(0) || (Time.unscaledTime - menuEnterTime) < minMenuStartDelay)
            yield return null;
        clicked = false;
        clickedResetRoutine = null;
    }

    //show pause menu
    public void ShowPauseMenu()
    {
        if (gameState == GameStateEnum.PAUSED)
            return;

        pauseGui.SetActive(true);
        Time.timeScale = 0;
        gameState = GameStateEnum.PAUSED;
        AudioManager.Instance.PlayEffectsAudio(AudioManager.Instance.ButtonClickAudio);
    }

    //hide pause menu
    public void HidePauseMenu()
    {
        pauseGui.SetActive(false);
        Time.timeScale = 1;
        gameState = GameStateEnum.PLAYING;
        AudioManager.Instance.PlayEffectsAudio(AudioManager.Instance.ButtonClickAudio);
    }

    //show gameplay gui
    public void ShowGameplay()
    {
        mainMenuGui.SetActive(false);
        pauseGui.SetActive(false);
        gameplayGui.SetActive(true);
        gameOverGui.SetActive(false);
        // Reset score when starting a new game session
        if (GameManager.Instance != null && GameManager.Instance.scoreManager != null)
            GameManager.Instance.scoreManager.ResetCurrentScore();
        gameState = GameStateEnum.PLAYING;
        AudioManager.Instance.PlayEffectsAudio(AudioManager.Instance.ButtonClickAudio);
    }

    //show game over gui
    public void ShowGameOver()
    {
        mainMenuGui.SetActive(false);
        pauseGui.SetActive(false);
        gameplayGui.SetActive(false);
        gameOverGui.SetActive(true);
        gameState = GameStateEnum.GAMEOVER;
        AudioManager.Instance.PlayMusic(AudioManager.Instance.MenuMusicAudio);
    }

    //check if user click any menu button
    public bool IsButton()
    {
        bool temp = false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult item in results)
        {
            temp |= item.gameObject.GetComponent<Button>() != null;
        }

        return temp;
    }
}
