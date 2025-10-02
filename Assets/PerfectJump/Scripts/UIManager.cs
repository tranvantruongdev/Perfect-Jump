using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [Header("GUI Components")]
    public GameObject MainMenuGuiGameObject;
    public GameObject PauseGuiGameObject, GameplayGuiGameObject, GameOverGuiGameObject;

    public GameStateEnum GameStateEnum;

    bool _isClicked;
    [SerializeField] float _minMenuStartDelayConfig = 0.2f;
    float _menuEnterTimeCounter;
    Coroutine _clickedResetCoroutine;

    // Use this for initialization
    void Start()
    {
        MainMenuGuiGameObject.SetActive(true);
        PauseGuiGameObject.SetActive(false);
        GameplayGuiGameObject.SetActive(false);
        GameOverGuiGameObject.SetActive(false);
        GameStateEnum = GameStateEnum.MENU;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameStateEnum == GameStateEnum.MENU && !_isClicked)
        {
            if (IsButtonClicked())
                return;

            AudioManager.S_Instance.PlayEffectsAudio(AudioManager.S_Instance.ButtonClickAudio);
            ShowGameplayUI();
            AudioManager.S_Instance.PlayMusic(AudioManager.S_Instance.GameMusicAudio);
        }
        else if (Input.GetMouseButtonUp(0) && _isClicked && GameStateEnum == GameStateEnum.MENU)
            _isClicked = false;
    }

    //show main menu
    public void ShowMainMenuUI()
    {
        _isClicked = true;
        MainMenuGuiGameObject.SetActive(true);
        PauseGuiGameObject.SetActive(false);
        GameplayGuiGameObject.SetActive(false);
        GameOverGuiGameObject.SetActive(false);
        GameStateEnum = GameStateEnum.MENU;
        AudioManager.S_Instance.PlayEffectsAudio(AudioManager.S_Instance.ButtonClickAudio);
        GameManager.S_Instance.ClearTheScene();

        // Always resume time when returning to main menu to avoid being stuck paused
        Time.timeScale = 1;

        GameManager.S_Instance.CreateNewScene();

        // Ensure we only re-enable clicks after the user has released the button once
        if (_clickedResetCoroutine != null)
            StopCoroutine(_clickedResetCoroutine);
        _menuEnterTimeCounter = Time.unscaledTime;
        _clickedResetCoroutine = StartCoroutine(EnableClickAfterPointerRelease());
    }

    IEnumerator EnableClickAfterPointerRelease()
    {
        // Wait until the user releases the button and the minimum delay has passed
        while (Input.GetMouseButton(0) || (Time.unscaledTime - _menuEnterTimeCounter) < _minMenuStartDelayConfig)
            yield return null;
        _isClicked = false;
        _clickedResetCoroutine = null;
    }

    //show pause menu
    public void ShowPauseMenuUI()
    {
        if (GameStateEnum == GameStateEnum.PAUSED)
            return;

        PauseGuiGameObject.SetActive(true);
        Time.timeScale = 0;
        GameStateEnum = GameStateEnum.PAUSED;
        AudioManager.S_Instance.PlayEffectsAudio(AudioManager.S_Instance.ButtonClickAudio);
    }

    //hide pause menu
    public void HidePauseMenuUI()
    {
        PauseGuiGameObject.SetActive(false);
        Time.timeScale = 1;
        GameStateEnum = GameStateEnum.PLAYING;
        AudioManager.S_Instance.PlayEffectsAudio(AudioManager.S_Instance.ButtonClickAudio);
    }

    //show gameplay gui
    public void ShowGameplayUI()
    {
        MainMenuGuiGameObject.SetActive(false);
        PauseGuiGameObject.SetActive(false);
        GameplayGuiGameObject.SetActive(true);
        GameOverGuiGameObject.SetActive(false);
        // Reset score when starting a new game session
        if (GameManager.S_Instance != null && GameManager.S_Instance.ScoreManagerInstance != null)
            GameManager.S_Instance.ScoreManagerInstance.ResetTheCurrentScoreValue();
        GameStateEnum = GameStateEnum.PLAYING;
        AudioManager.S_Instance.PlayEffectsAudio(AudioManager.S_Instance.ButtonClickAudio);
    }

    //show game over gui
    public void ShowGameOverUI()
    {
        MainMenuGuiGameObject.SetActive(false);
        PauseGuiGameObject.SetActive(false);
        GameplayGuiGameObject.SetActive(false);
        GameOverGuiGameObject.SetActive(true);
        GameStateEnum = GameStateEnum.GAMEOVER;
        AudioManager.S_Instance.PlayMusic(AudioManager.S_Instance.MenuMusicAudio);
    }

    //check if user click any menu button
    public bool IsButtonClicked()
    {
        bool tmp = false;

        PointerEventData eventData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            tmp |= result.gameObject.GetComponent<Button>() != null;
        }

        return tmp;
    }
}
