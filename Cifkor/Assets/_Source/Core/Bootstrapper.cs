using UnityEngine;
using Zenject;

namespace Core
{
    public class Bootstrapper : MonoBehaviour
    {
        private Game _game;

        [Inject]
        private void Construct(Game game)
        {
            _game = game;
        }

        private void Awake()
        {
            _game.StartGame();
        }
    }
}