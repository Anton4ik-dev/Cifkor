using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Breeds
{
    public class BreedView : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private TextMeshProUGUI _breedText;
        [SerializeField] private GameObject _loadingIndicator;

        private string _breedId = "";

        private event Action<string, BreedView> OnClick;

        public void Bind(int breedNumber, string breedName, string breedId, Action<string, BreedView> action)
        {
            _breedId = breedId;
            _breedText.text = $"{breedNumber + 1} - {breedName}";
            OnClick += action;
        }

        public void ShowHideLoading(bool isActive)
        {
            _loadingIndicator.SetActive(isActive);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClick?.Invoke(_breedId, this);
        }

        private void OnDisable()
        {
            OnClick = null;
        }

        public class Factory : PlaceholderFactory<BreedView>
        {
        }
    }
}