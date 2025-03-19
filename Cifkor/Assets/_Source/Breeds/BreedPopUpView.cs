using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Breeds
{
    public class BreedPopUpView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _breedNameText;
        [SerializeField] private TextMeshProUGUI _breedDescriptionText;
        [SerializeField] private Button _closeButton;

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
            _closeButton.onClick.AddListener(() => ShowHidePopUp(false));
        }

        private void Expose()
        {
            _closeButton.onClick.RemoveListener(() => ShowHidePopUp(false));
        }

        public void ShowHidePopUp(bool isActive) => gameObject.SetActive(isActive);

        public void ShowPopUp(string breedName, string breedDescription)
        {
            ShowHidePopUp(true);
            _breedNameText.text = breedName;
            _breedDescriptionText.text = breedDescription;
        }
    }
}