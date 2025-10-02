using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text _currentScoreText, _highScoreText, _currentScoreGameOverText, _highScoreGameOverText;

    int _currentScoreCounter, _highScoreCounter;
    // Start is called before the first frame update

    //init and load highscore
    void Start()
    {
        if (!PlayerPrefs.HasKey("HighScore"))
            PlayerPrefs.SetInt("HighScore", 0);

        _highScoreCounter = PlayerPrefs.GetInt("HighScore");

        UpdateTheHighScore();
        ResetTheCurrentScoreValue();
    }

    //save and update highscore
    void UpdateTheHighScore()
    {
        if (_currentScoreCounter > _highScoreCounter)
            _highScoreCounter = _currentScoreCounter;

        _highScoreText.text = _highScoreCounter.ToString();
        PlayerPrefs.SetInt("HighScore", _highScoreCounter);
    }

    //update currentscore
    public void UpdateScoreValue(int value)
    {
        _currentScoreCounter += value;
        _currentScoreText.text = _currentScoreCounter.ToString();
    }

    //reset current score
    public void ResetTheCurrentScoreValue()
    {
        _currentScoreCounter = 0;
        UpdateScoreValue(0);
    }

    //update gameover scores
    public void UpdateTheGameOverScores()
    {
        UpdateTheHighScore();

        _currentScoreGameOverText.text = _currentScoreCounter.ToString();
        _highScoreGameOverText.text = _highScoreCounter.ToString();
    }
}
