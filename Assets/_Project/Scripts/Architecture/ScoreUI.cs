using TMPro;
using UnityEngine;

namespace Architecture
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        
        private IGameManager _gameManager;
        
        private void Start() {
            _gameManager = GameManager.Instance;
            _gameManager.OnScoreChanged += ChangeScore;
        }

        private void OnDestroy() {
            if (_gameManager != null) {
                _gameManager.OnScoreChanged -= ChangeScore;
            }
        }
        
        private void ChangeScore(int score) {
            _scoreText.text = $"Score: {score}";
        }
    }
}