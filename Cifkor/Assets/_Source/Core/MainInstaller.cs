using ServerRequestSystem;
using UnityEngine;
using Zenject;

namespace Core
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private ServerRequestView _serverRequestView;

        private void InstallServerRequestSystem()
        {
            Container
                .Bind<ServerRequestView>()
                .FromInstance(_serverRequestView)
                .AsSingle()
                .NonLazy();
            Container
                .Bind<ServerRequestController>()
                .AsSingle()
                .NonLazy();
            Container.Bind<ServerRequestModel>()
                .AsSingle()
                .NonLazy();
        }

        public override void InstallBindings()
        {
            InstallServerRequestSystem();
        }
    }
}