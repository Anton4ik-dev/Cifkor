using ServerRequestSystem;
using UnityEngine;
using Zenject;

namespace Core
{
    public class Game : MonoBehaviour
    {
        private ServerRequestController _serverRequestController;
        private GameView _gameView;

        [Inject]
        private void Construct(ServerRequestController serverRequestController, GameView gameView)
        {
            _serverRequestController = serverRequestController;
            _gameView = gameView;
            _gameView.OnClick += ChangeActiveTab;
        }

        private void ChangeActiveTab(bool isWeather)
        {
            _serverRequestController.ChangeActiveTab(isWeather);
        }

        private void OnDestroy()
        {
            _gameView.OnClick -= ChangeActiveTab;
            _serverRequestController.Destroy();
        }

        public void StartGame()
        {
            ChangeActiveTab(true);
        }
    }
}