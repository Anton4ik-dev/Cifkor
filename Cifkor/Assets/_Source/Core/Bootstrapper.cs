using ServerRequestSystem;
using UnityEngine;
using Zenject;

namespace Core
{
    public class Bootstrapper : MonoBehaviour
    {
        private ServerRequestController _serverRequestController;

        [Inject]
        private void Construct(ServerRequestController serverRequestController)
        {
            _serverRequestController = serverRequestController;
        }

        private void Awake()
        {
            _serverRequestController.SetWeatherTabActive(true);
        }
    }
}