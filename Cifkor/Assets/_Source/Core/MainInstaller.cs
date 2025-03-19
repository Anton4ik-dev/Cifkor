using Breeds;
using ServerRequestSystem;
using UnityEngine;
using Zenject;

namespace Core
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private ServerRequestView _serverRequestView;
        [SerializeField] private GameView _gameView;
        [SerializeField] private BreedPopUpView _breedPopUpView;
        [SerializeField] private Game _game;
        [SerializeField] private GameObject _breeViewPrefab;

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

        private void InstallCore()
        {
            Container
                .Bind<GameView>()
                .FromInstance(_gameView)
                .AsSingle()
                .NonLazy();
            Container
                .Bind<Game>()
                .FromInstance(_game)
                .AsSingle()
                .NonLazy();
        }

        private void InstallBreed()
        {
            Container
                .Bind<BreedPopUpView>()
                .FromInstance(_breedPopUpView)
                .AsSingle()
                .NonLazy();
            Container
                .BindFactory<BreedView, BreedView.Factory>()
                .FromComponentInNewPrefab(_breeViewPrefab);
        }

        public override void InstallBindings()
        {
            InstallServerRequestSystem();
            InstallBreed();
            InstallCore();
        }
    }
}