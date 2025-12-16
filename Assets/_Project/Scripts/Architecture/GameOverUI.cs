using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Button _restartButton;
        
        private IGameManager _gameManager;

        private void Start() {
            _gameManager = GameManager.Instance;
            _gameManager.OnGameOver += OnGameOver;
            _restartButton.onClick.AddListener(_gameManager.RestartGame);
            
            gameObject.SetActive(false);
        }

        private void OnDestroy() {
            if (_gameManager != null) {
                _gameManager.OnGameOver -= OnGameOver;
                _restartButton.onClick.RemoveListener(_gameManager.RestartGame);
            }
        }

        private void OnGameOver() {
            gameObject.SetActive(true);
            _scoreText.text = $"Score: {_gameManager.Score}";
        }
    }
}
