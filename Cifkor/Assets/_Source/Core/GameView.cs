using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] private Button _changePanelButton;
        [SerializeField] private GameObject _weatherPanel;
        [SerializeField] private GameObject _breedsPanel;

        private bool _isWeather = true;

        public event Action<bool> OnClick;

        private void Start()
        {
            Bind();
        }

        private void OnDestroy()
        {
            Expose();
        }

        private void Bind()
        {
            _changePanelButton.onClick.AddListener(ChangePanel);
        }

        private void Expose()
        {
            _changePanelButton.onClick.RemoveListener(ChangePanel);
        }

        private void ChangePanel()
        {
            _isWeather = !_isWeather;

            if (_isWeather)
            {
                _breedsPanel.SetActive(false);
                _weatherPanel.SetActive(true);
            }
            else
            {
                _breedsPanel.SetActive(true);
                _weatherPanel.SetActive(false);
            }

            OnClick?.Invoke(_isWeather);
        }
    }
}