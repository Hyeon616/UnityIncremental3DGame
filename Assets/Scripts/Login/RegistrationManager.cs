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

        if(string.IsNullOrEmpty(nickname))
        {
            feedbackText.text = "닉네임을 입력해주세요.";
            HideFeedbackText();
            return;
        }


        if (await IsUsernameTaken(username))
        {
            feedbackText.text = "동일한 아이디가 이미 있습니다.";
            HideFeedbackText();
            return;
        }

        if (await IsNicknameTaken(nickname))
        {
            feedbackText.text = "동일한 닉네임이 이미 있습니다.";
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
        using (UnityWebRequest request = new UnityWebRequest(apiSettings.RegisterUrl, "POST"))
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
                feedbackText.text = "Signup successful!";
            }

            HideFeedbackText();
        }
    }

    private async UniTask<bool> IsUsernameTaken(string username)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{apiSettings.baseUrl}/check-username?username={username}"))
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error checking username: {request.error}");
                return true; // 서버 에러 시 중복된 것으로 간주
            }

            string responseText = request.downloadHandler.text;
            var response = JsonConvert.DeserializeObject<Dictionary<string, bool>>(responseText);
            return response["exists"];
        }
    }

    private async UniTask<bool> IsNicknameTaken(string nickname)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{apiSettings.baseUrl}/check-nickname?nickname={nickname}"))
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error checking nickname: {request.error}");
                return true; // 서버 에러 시 중복된 것으로 간주
            }

            string responseText = request.downloadHandler.text;
            var response = JsonConvert.DeserializeObject<Dictionary<string, bool>>(responseText);
            return response["exists"];
        }
    }

    private async void HideFeedbackText()
    {
        await UniTask.Delay(3000);
        feedbackText.text = "";
    }
}
