using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RegistrationManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private APISettings apiSettings;
    [SerializeField] private Button registerButton;

    private void OnEnable()
    {
        registerButton.onClick.AddListener(Register);
    }

    private void OnDisable()
    {
        registerButton.onClick.RemoveListener(Register);
    }

    public async void Register()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        string nickname = nicknameInputField.text;

        if (string.IsNullOrEmpty(username))
        {
            feedbackText.text = "아이디를 입력해주세요.";
            HideFeedbackText();
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            feedbackText.text = "비밀번호를 입력해주세요.";
            HideFeedbackText();
            return;
        }

        if (string.IsNullOrEmpty(nickname))
        {
            feedbackText.text = "닉네임을 입력해주세요.";
            HideFeedbackText();
            return;
        }

        var requestBody = new
        {
            username = username,
            password = password,
            nickname = nickname
        };

        string jsonData = JsonConvert.SerializeObject(requestBody);
        using (UnityWebRequest request = new UnityWebRequest(apiSettings.GetUrl(APISettings.Endpoint.Register), "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                feedbackText.text = request.error;
                Debug.LogError($"Error: {request.error}");
            }
            else
            {
                if (request.responseCode == 400)
                {
                    var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(request.downloadHandler.text);
                    feedbackText.text = errorResponse.error;
                }
                else if (request.responseCode == 201)
                {
                    feedbackText.text = "회원가입 성공!";
                }
                else
                {
                    feedbackText.text = "회원가입 실패: 알 수 없는 오류.";
                }
            }

            HideFeedbackText();
        }
    }

    private async void HideFeedbackText()
    {
        await UniTask.Delay(3000);
        feedbackText.text = "";
    }

    [System.Serializable]
    private class ErrorResponse
    {
        public string error;
    }
}
