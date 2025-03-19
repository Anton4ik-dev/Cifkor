using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Weather;
using Zenject;

namespace ServerRequestSystem
{
    public class ServerRequestController
    {
        private static string WEATHER_URL = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

        private ServerRequestView _view;
        private ServerRequestModel _model;
        private bool _isOnWeatherTab = false;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        [Inject]
        public ServerRequestController(ServerRequestView view, ServerRequestModel model)
        {
            _view = view;
            _model = model;
        }

        private async UniTaskVoid StartRequestTimer()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (_isOnWeatherTab)
                {
                    await SendWeatherRequest();
                }
                await UniTask.Delay(5000, cancellationToken: _cancellationTokenSource.Token);
            }
        }

        private async UniTask SendWeatherRequest()
        {
            try
            {
                WeatherData weatherData = new WeatherData();
                Sprite icon = null;

                await _model.AddRequest(async (ct) =>
                {
                    weatherData = await _model.GetWeather(WEATHER_URL, ct);
                });

                await _model.AddRequest(async (ct) =>
                {
                    icon = await _model.GetIcon(weatherData.IconUrl, _cancellationTokenSource.Token);
                });

                _view.UpdateWeather(weatherData.Name, weatherData.Temperature, weatherData.TemperatureUnit, icon);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Request was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Request failed: {ex?.GetType().Name} - {ex?.Message ?? "No message"}");
                throw;
            }
        }

        public void SetWeatherTabActive(bool isActive)
        {
            _isOnWeatherTab = isActive;

            if (_isOnWeatherTab)
                StartRequestTimer().Forget();
            else
            {
                _model.CancelRequests();
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }
    }
}