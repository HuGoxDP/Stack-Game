using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Architecture
{
    public interface IGameManager
    {
        event Action OnGameOver;
        event Action<int> OnScoreChanged;
        int Score { get; }
        bool IsGameOver { get; }
        
        void AddScore(int points = 1);
        void TriggerGameOver();
        void RestartGame();
    }

    public class GameManager : MonoBehaviour, IGameManager
    {
        public static GameManager Instance
        {
            get
            {
                if (_instance == null) {
                    _instance = FindAnyObjectByType<GameManager>();
                    if (_instance == null) {
                        var go = new GameObject("GameManager" + "Auto-Generated");
                        _instance = go.AddComponent<GameManager>();
                        DontDestroyOnLoad(go);
                    }
                }

                return _instance;
            }
        }

        public event Action<int> OnScoreChanged;
        public event Action OnGameOver;

        private static GameManager _instance;

        public int Score { get; private set; }
        public bool IsGameOver { get; private set; }


        private void Awake() {
            InitializeSingleton();
        }

        private void InitializeSingleton() {
            if (!Application.isPlaying) {
                return;
            }

            if (_instance == null) {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this) {
                Destroy(gameObject);
            }
        }

        public void AddScore(int points = 1) {
            if (IsGameOver) {
                return;
            }

            Score += points;
            OnScoreChanged?.Invoke(Score);
        }

        public void TriggerGameOver() {
            if (IsGameOver) {
                return;
            }

            IsGameOver = true;
            OnGameOver?.Invoke();
        }

        public void RestartGame() {
            Score = 0;
            IsGameOver = false;
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }
}