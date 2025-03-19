using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weather;

namespace ServerRequestSystem
{
    public class ServerRequestView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _weatherText;
        [SerializeField] private Image _weatherIcon;

        public void UpdateWeather(string name, int temperature, string temperatureUnit, Sprite icon)
        {
            _weatherText.text = $"{name} - {temperature}{temperatureUnit}";
            _weatherIcon.sprite = icon;
        }
    }
}