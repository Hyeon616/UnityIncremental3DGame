using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System;

public class LoginManager
{
    public async UniTask<bool> Login(string username, string password, Action<string> setFeedbackText)
    {
        try
        {
            var requestBody = new
            {
                username = username,
                password = password
            };

            string jsonData = JsonConvert.SerializeObject(requestBody);
            using (UnityWebRequest request = new UnityWebRequest(ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.Login), "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    setFeedbackText(request.error);
                    return false;
                }
                else
                {
                    if (request.responseCode == 400)
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(request.downloadHandler.text);
                        setFeedbackText(errorResponse.error);
                        return false;
                    }
                    else if (request.responseCode == 200)
                    {
                        var response = JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text);
                        PlayerPrefs.SetString("authToken", response.token);
                        PlayerPrefs.SetInt("UserId", response.userId);
                        Debug.Log("토큰 저장: " + response.token);
                        setFeedbackText("로그인 성공!");
                        return true;
                    }
                    else
                    {
                        setFeedbackText("로그인 실패: 알 수 없는 오류.");
                        return false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            setFeedbackText($"오류 발생: {ex.Message}");
            return false;
        }
    }

    [System.Serializable]
    private class LoginResponse
    {
        public string token;
        public int userId;
    }

    [System.Serializable]
    private class ErrorResponse
    {
        public string error;
    }
}