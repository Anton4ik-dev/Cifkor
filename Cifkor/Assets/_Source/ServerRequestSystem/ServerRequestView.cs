using Breeds;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ServerRequestSystem
{
    public class ServerRequestView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _weatherText;
        [SerializeField] private Image _weatherIcon;
        [SerializeField] private GameObject _loadingIndicator;
        [SerializeField] private Transform _breedsParent;

        private List<BreedView> _breedViews = new List<BreedView>();
        private BreedView.Factory _breedFactory;

        [Inject]
        private void Construct(BreedView.Factory breedFactory)
        {
            _breedFactory = breedFactory;
        }

        public void UpdateWeather(string name, int temperature, string temperatureUnit, Sprite icon)
        {
            _weatherText.text = $"{name} - {temperature}{temperatureUnit}";
            _weatherIcon.sprite = icon;
        }

        public void UpdateBreed(int breedNumber, string breedName, string breedId, Action<string, BreedView> action)
        {
            if (_breedViews.Count <= breedNumber)
            {
                _breedViews.Add(_breedFactory.Create());
                _breedViews[breedNumber].transform.SetParent(_breedsParent);
            }
            _breedViews[breedNumber].Bind(breedNumber, breedName, breedId, action);
        }

        public void ShowHideLoading(bool isActive)
        {
            _loadingIndicator.SetActive(isActive);
        }
    }
}