using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _counters;
    [SerializeField] private GameObject _scoreBoard;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private DepthMonitor _depthMonitor;

    private void OnEnable()
    {
        Player.OnPlayerDeath += DisplayScore;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= DisplayScore;
    }

    private void DisplayScore()
    {
        _counters.SetActive(false);

        _scoreText.text = _depthMonitor.ScaledCurrentDepth.ToString("0.0") + "m";
        _scoreBoard.SetActive(true);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
