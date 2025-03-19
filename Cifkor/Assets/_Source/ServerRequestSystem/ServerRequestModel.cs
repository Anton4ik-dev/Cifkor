using Breeds;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using Weather;
using Random = UnityEngine.Random;

namespace ServerRequestSystem
{
    public class ServerRequestModel
    {
        private const int AMPUNT_OF_BREEDS = 10;

        private Queue<Func<CancellationToken, UniTask>> _requestQueue = new Queue<Func<CancellationToken, UniTask>>();
        private bool _isProcessing = false;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private async UniTask ProcessQueue(CancellationToken ct)
        {
            _isProcessing = true;

            while (_requestQueue.Count > 0 && !ct.IsCancellationRequested)
            {
                Func<CancellationToken, UniTask> request = _requestQueue.Dequeue();
                await request(ct);
            }

            _isProcessing = false;
        }

        private async UniTask SendWebRequest(UnityWebRequest webRequest, CancellationToken ct)
        {
            await webRequest.SendWebRequest().ToUniTask(cancellationToken: ct);

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new Exception($"Request failed: {webRequest.error}");
            }
        }

        public async UniTask AddRequest(Func<CancellationToken, UniTask> request)
        {
            UniTaskCompletionSource taskCompletionSource = new UniTaskCompletionSource();

            _requestQueue.Enqueue(async (ct) =>
            {
                try
                {
                    await request(ct);
                    taskCompletionSource.TrySetResult();
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("Request was canceled");
                    taskCompletionSource.TrySetCanceled();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Request failed: {ex?.GetType().Name} - {ex?.Message ?? "No message"}");
                    taskCompletionSource.TrySetException(ex);
                }
            });

            if (!_isProcessing)
                await ProcessQueue(_cancellationTokenSource.Token);

            await taskCompletionSource.Task;
        }

        public void CancelRequests()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            _requestQueue.Clear();
        }

        public async UniTask<WeatherData> GetWeather(string url, CancellationToken ct)
        {
            await UniTask.SwitchToMainThread();

            UnityWebRequest webRequest = UnityWebRequest.Get(url);

            await SendWebRequest(webRequest, ct);

            string jsonResponse = webRequest.downloadHandler.text;
            WeatherResponse weatherResponse = JsonUtility.FromJson<WeatherResponse>(jsonResponse);

            int number = Random.Range(0, weatherResponse.properties.periods.Length);
            return weatherResponse.properties.periods[number];
        }

        public async UniTask<Sprite> GetIcon(string url, CancellationToken ct)
        {
            await UniTask.SwitchToMainThread();

            UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);

            await SendWebRequest(webRequest, ct);

            Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        public async UniTask<BreedData[]> GetDogBreeds(string url, CancellationToken ct)
        {
            await UniTask.SwitchToMainThread();

            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.SetRequestHeader("Accept", "application/json");

            await SendWebRequest(webRequest, ct);

            string jsonResponse = webRequest.downloadHandler.text;
            BreedsResponse breedsResponse = JsonUtility.FromJson<BreedsResponse>(jsonResponse);

            BreedData[] breedData = new BreedData[AMPUNT_OF_BREEDS];

            for (int i = 0; i < breedData.Length; i++)
            {
                breedData[i] = breedsResponse.data[i];
            }

            return breedData;
        }

        public async UniTask<BreedData> GetDogBreedById(string url, CancellationToken ct, string breedId)
        {
            await UniTask.SwitchToMainThread();

            UnityWebRequest webRequest = UnityWebRequest.Get(url + $"/{breedId}");
            webRequest.SetRequestHeader("Accept", "application/json");

            await SendWebRequest(webRequest, ct);

            string jsonResponse = webRequest.downloadHandler.text;
            BreedsResponseById breedData = JsonUtility.FromJson<BreedsResponseById>(jsonResponse);

            return breedData.data;
        }
    }
}