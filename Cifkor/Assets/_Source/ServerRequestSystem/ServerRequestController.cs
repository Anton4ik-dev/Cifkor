using Breeds;
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
        private static string BREEDS_URL = "https://dogapi.dog/api/v2/breeds";

        private ServerRequestView _view;
        private BreedPopUpView _breedPopUpView;
        private ServerRequestModel _model;
        private bool _isWeather = false;
        private CancellationTokenSource _weatherTimerTokenSource;

        [Inject]
        public ServerRequestController(ServerRequestView view, ServerRequestModel model, BreedPopUpView breedPopUpView)
        {
            _view = view;
            _model = model;
            _breedPopUpView = breedPopUpView;
        }

        private void StartRequestByBreedId(string id, BreedView breedView)
        {
            StopRequests();
            SendRequestByBreedId(id, breedView).Forget();
        }

        private async UniTask StartWeatherRequestTimer()
        {
            _weatherTimerTokenSource = new CancellationTokenSource();
            CancellationToken ct = _weatherTimerTokenSource.Token;

            while (!ct.IsCancellationRequested)
            {
                await SendRequest(ct);

                await UniTask.Delay(5000, cancellationToken: ct);
            }
        }

        private void StopRequests()
        {
            _model.CancelRequests();
            _weatherTimerTokenSource?.Cancel();
        }

        private async UniTask SendRequest(CancellationToken ct)
        {
            try
            {
                if (_isWeather)
                    await SendWeatherRequest(ct);
                else
                    await SendBreedsRequest(ct);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Request was canceled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Request failed: {ex?.GetType().Name} - {ex?.Message ?? "No message"}");
            }
            finally
            {
                if (!_isWeather)
                    _view.ShowHideLoading(false);
            }
        }

        private async UniTask SendRequestByBreedId(string id, BreedView breedView)
        {
            try
            {
                breedView.ShowHideLoading(true);
                BreedData breed = null;

                await _model.AddRequest(async (ct) =>
                {
                    breed = await _model.GetDogBreedById(BREEDS_URL, ct, id);
                });

                _breedPopUpView.ShowPopUp(breed.attributes.name, breed.attributes.description);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Request was canceled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Request failed: {ex?.GetType().Name} - {ex?.Message ?? "No message"}");
            }
            finally
            {
                breedView.ShowHideLoading(false);
            }
        }

        private async UniTask SendWeatherRequest(CancellationToken ct)
        {
            WeatherData weatherData = new WeatherData();
            Sprite icon = null;

            await _model.AddRequest(async (ct) =>
            {
                weatherData = await _model.GetWeather(WEATHER_URL, ct);
            });

            await _model.AddRequest(async (ct) =>
            {
                icon = await _model.GetIcon(weatherData.icon, ct);
            });

            _view.UpdateWeather(weatherData.name, weatherData.temperature, weatherData.temperatureUnit, icon);
        }

        private async UniTask SendBreedsRequest(CancellationToken ct)
        {
            BreedData[] breeds = null;
            _view.ShowHideLoading(true);

            await _model.AddRequest(async (ct) =>
            {
                breeds = await _model.GetDogBreeds(BREEDS_URL, ct);
            });

            for (int i = 0; i < breeds.Length; i++)
            {
                _view.UpdateBreed(i, breeds[i].attributes.name, breeds[i].id, StartRequestByBreedId);
            }
        }

        public void ChangeActiveTab(bool isWeather)
        {
            StopRequests();

            _isWeather = isWeather;

            if (_isWeather)
            {
                _breedPopUpView.ShowHidePopUp(false);
                StartWeatherRequestTimer().Forget();
            }
            else
            {
                SendRequest(default).Forget();
            }
        }

        public void Destroy()
        {
            StopRequests();
        }
    }
}