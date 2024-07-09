using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;

public class RegistrationManager
{
    public async UniTask Register(string username, string password, string nickname, Action<string> setFeedbackText)
    {
        try
        {
            var requestBody = new
            {
                username = username,
                password = password,
                nickname = nickname
            };

            string jsonData = JsonConvert.SerializeObject(requestBody);
            string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.Register);

            UnityEngine.Debug.Log($"Sending registration request to: {url}");
            UnityEngine.Debug.Log($"Request body: {jsonData}");

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                UnityEngine.Debug.Log("Sending web request...");
                await request.SendWebRequest();
                UnityEngine.Debug.Log($"Request completed. Result: {request.result}, Response Code: {request.responseCode}");

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    UnityEngine.Debug.LogError($"Request error: {request.error}");
                    setFeedbackText($"연결 오류: {request.error}");
                }
                else
                {
                    UnityEngine.Debug.Log($"Response: {request.downloadHandler.text}");
                    if (request.responseCode == 400)
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(request.downloadHandler.text);
                        setFeedbackText(errorResponse.error);
                    }
                    else if (request.responseCode == 201)
                    {
                        setFeedbackText("회원가입 성공!");
                    }
                    else
                    {
                        setFeedbackText($"회원가입 실패: 알 수 없는 오류. 응답 코드: {request.responseCode}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Exception in Register: {ex}");
            setFeedbackText($"오류 발생: {ex.Message}");
        }
    }

    [System.Serializable]
    private class ErrorResponse
    {
        public string error;
    }
}