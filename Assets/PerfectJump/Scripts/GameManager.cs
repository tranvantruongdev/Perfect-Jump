using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager S_Instance { get; set; }

    public UIManager UIManagerInstance;
    public ScoreManager ScoreManagerInstance;
    public GameObject PlayerGameObject;
    public Rigidbody2D PlayerRigidbody2DInstance;
    public GameObject ObstaclePrefabGameObject;
    public Camera Camera;
    public Animator PerfectAnimatorInstance;

    [Header("Game settings")]
    [Space(5)]
    public float MinYObstaclePositionConfig = -4f;
    [Space(5)]
    public float MaxYObstaclePositionConfig = 2.5f;
    [Space(5)]
    public Color[] ObstacleColorsConfig;

    readonly float _xDistanceBetweenObstaclesConfig = 2.5f; //fixed because of first jump, if you change this then you need to change jump force too
    GameObject _lastObstacleGameObject;
    GameObject _newObstacleGameObject;
    Vector3 _screenSizeVector3;
    int _obstacleIndexNumber;

    public bool _isInAir;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (S_Instance == null)
            S_Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _screenSizeVector3 = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        //Debug.Log(screenSize);

        Physics2D.gravity = new Vector2(0, -9.81f);
        Application.targetFrameRate = 60;
        EnsureDefaultObstacleColors();
        CreateNewScene();
    }

    void Update()
    {
        if (UIManagerInstance.GameStateEnum == GameStateEnum.PLAYING)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (UIManagerInstance.IsButtonClicked())
                    return;

                if (!_isInAir)
                {
                    PlayerRigidbody2DInstance.linearVelocity = Vector2.zero;
                    PlayerRigidbody2DInstance.AddForce(new Vector2(95, 440));
                    _isInAir = true;
                }
            }

            if (_lastObstacleGameObject.transform.position.x < Camera.transform.position.x + (2 * _screenSizeVector3.x))
                CreateNewObstacleObject();
        }
    }

    //create new scene
    public void CreateNewScene()
    {
        _obstacleIndexNumber = 0;
        //reset camera position
        Camera.transform.position = new Vector3(0, 0, -10);

        //first obstacle
        _lastObstacleGameObject = Instantiate(ObstaclePrefabGameObject);
        _lastObstacleGameObject.transform.position = new Vector2(-1.5f, -1.5f);
        _lastObstacleGameObject.GetComponent<SpriteRenderer>().color = GetRandomColorFromConfig();
        _lastObstacleGameObject.GetComponent<Obstacle>().IndexCounter = _obstacleIndexNumber;

        //player on the first obstacle
        PlayerGameObject.transform.SetPositionAndRotation(new Vector2(-1.5f, -.8f), new Quaternion(0, 0, 0, 0));
        PlayerRigidbody2DInstance.bodyType = RigidbodyType2D.Dynamic;
        PlayerGameObject.GetComponent<SpriteRenderer>().enabled = true;
        PlayerGameObject.GetComponent<TrailRenderer>().enabled = true;
        PlayerRigidbody2DInstance.gravityScale = 3f;

        _obstacleIndexNumber++;

        //second obstacle
        _lastObstacleGameObject = Instantiate(ObstaclePrefabGameObject);
        _lastObstacleGameObject.transform.position = new Vector2(1f, -.5f);
        _lastObstacleGameObject.GetComponent<SpriteRenderer>().color = GetRandomColorFromConfig();
        _lastObstacleGameObject.GetComponent<Obstacle>().IndexCounter = _obstacleIndexNumber;

        //third obstacle
        CreateNewObstacleObject();
    }

    //create obstacle
    public void CreateNewObstacleObject()
    {
        _obstacleIndexNumber++;

        //random y position, depend on previous obstacle
        float newObstacleY = Random.Range(_lastObstacleGameObject.transform.position.y - 1.25f, _lastObstacleGameObject.transform.position.y + 1.25f);

        //dont go over the top limit
        if (newObstacleY > MaxYObstaclePositionConfig)
            newObstacleY = MaxYObstaclePositionConfig;

        //dont go bellow the bottom limit
        if (newObstacleY < MinYObstaclePositionConfig)
            newObstacleY = MinYObstaclePositionConfig;

        //create obstacle
        _newObstacleGameObject = Instantiate(ObstaclePrefabGameObject);
        _newObstacleGameObject.transform.position = new Vector2(_lastObstacleGameObject.transform.position.x + _xDistanceBetweenObstaclesConfig, newObstacleY);
        _newObstacleGameObject.GetComponent<SpriteRenderer>().color = GetRandomColorFromConfig();
        _newObstacleGameObject.GetComponent<Obstacle>().IndexCounter = _obstacleIndexNumber;
        _lastObstacleGameObject = _newObstacleGameObject;
    }

    //restart game, reset score
    public void RestartTheGame()
    {
        if (UIManagerInstance.GameStateEnum == GameStateEnum.PAUSED)
            Time.timeScale = 1;

        Camera.transform.position = new Vector3(0, 0, -10);
        ScoreManagerInstance.ResetTheCurrentScoreValue();
        ClearTheScene();
        CreateNewScene();
        UIManagerInstance.ShowGameplayUI();
        _isInAir = false;
        AudioManager.S_Instance.PlayMusic(AudioManager.S_Instance.GameMusicAudio);
    }


    //clear all obstacles from scene
    public void ClearTheScene()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach (GameObject item in obstacles)
        {
            Destroy(item);
        }
    }

    public void PlayPerfectAnimation()
    {
        PerfectAnimatorInstance.Play("Show");
    }

    Color GetRandomColorFromConfig()
    {
        return ObstacleColorsConfig[Random.Range(0, ObstacleColorsConfig.Length)];
    }

    void EnsureDefaultObstacleColors()
    {
        if (ObstacleColorsConfig == null || ObstacleColorsConfig.Length < 10)
        {
            ObstacleColorsConfig = new Color[]
            {
                new(0.95f, 0.26f, 0.21f), // Red
                new(1.00f, 0.60f, 0.00f), // Orange
                new(1.00f, 0.92f, 0.23f), // Yellow
                new(0.40f, 0.74f, 0.20f), // Green
                new(0.00f, 0.59f, 0.53f), // Teal
                new(0.01f, 0.66f, 0.96f), // Light Blue
                new(0.25f, 0.32f, 0.71f), // Indigo
                new(0.62f, 0.36f, 0.71f), // Purple
                new(1.00f, 0.34f, 0.13f), // Deep Orange
                new(0.37f, 0.45f, 0.49f)  // Blue Grey
            };
        }
    }

    //show game over gui
    public void GameOver()
    {
        if (UIManagerInstance.GameStateEnum == GameStateEnum.PLAYING)
        {
            AudioManager.S_Instance.PlayEffectsAudio(AudioManager.S_Instance.GameOverAudio);
            UIManagerInstance.ShowGameOverUI();
            ScoreManagerInstance.UpdateTheGameOverScores();
        }
    }
}
