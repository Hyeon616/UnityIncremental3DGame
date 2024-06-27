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
            using (UnityWebRequest request = new UnityWebRequest(ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.Register), "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    setFeedbackText(request.error);
                }
                else
                {
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
                        setFeedbackText("회원가입 실패: 알 수 없는 오류.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            setFeedbackText($"오류 발생: {ex.Message}");
        }
    }

    [System.Serializable]
    private class ErrorResponse
    {
        public string error;
    }
}