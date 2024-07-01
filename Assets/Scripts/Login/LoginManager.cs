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
                    Debug.LogError($"Error: {request.error}");
                    Debug.LogError($"Response: {request.downloadHandler.text}");
                    setFeedbackText(request.error);
                    return false;
                }
                else
                {
                    var responseJson = request.downloadHandler.text;
                    Debug.Log($"Server response : {responseJson}!");
                    if (request.responseCode == 400)
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseJson);
                        setFeedbackText(errorResponse.error);
                        return false;
                    }
                    else if (request.responseCode == 200)
                    {
                        var response = JsonConvert.DeserializeObject<LoginResponse>(responseJson);
                        if (response.token != null && response.userId != 0)
                        {
                            GameManager.Instance.SetAuthToken(response.token);
                            GameManager.Instance.SetUserId(response.userId);
                            setFeedbackText("로그인 성공!");
                            return true;
                        }
                        else
                        {
                            Debug.LogError("Invalid response data");
                            setFeedbackText("로그인 실패: 서버 응답 오류");
                            return false;
                        }
                    }
                    else
                    {
                        Debug.LogError($"Unknown error occurred. Status code: {request.responseCode}");
                        setFeedbackText("로그인 실패: 알 수 없는 오류.");
                        return false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception occurred: {ex.Message}");
            setFeedbackText($"오류 발생: {ex.Message}");
            return false;
        }
    }

    [Serializable]
    private class LoginResponse
    {
        public string token;
        public int userId;
    }

    [Serializable]
    private class ErrorResponse
    {
        public string error;
    }
}