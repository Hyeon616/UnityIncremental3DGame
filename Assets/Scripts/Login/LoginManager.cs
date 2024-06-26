using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Newtonsoft.Json;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Button loginButton;
    [SerializeField] private APISettings apiSettings;

    private void OnEnable()
    {
        loginButton.onClick.AddListener(Login);
    }

    private void OnDisable()
    {
        loginButton.onClick.RemoveListener(Login);
    }

    public async void Login()
    {
        try
        {
            string username = usernameInputField.text;
            string password = passwordInputField.text;

            if (string.IsNullOrEmpty(username))
            {
                feedbackText.text = "아이디를 입력하세요.";
                HideFeedbackText();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                feedbackText.text = "패스워드를 입력하세요.";
                HideFeedbackText();
                return;
            }

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
                    feedbackText.text = request.error;
                }
                else
                {
                    if (request.responseCode == 400)
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(request.downloadHandler.text);
                        feedbackText.text = errorResponse.error;
                    }
                    else if (request.responseCode == 200)
                    {
                        var response = JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text);
                        PlayerPrefs.SetString("authToken", response.token);
                        PlayerPrefs.SetInt("UserId", response.userId);
                        Debug.Log("토큰 저장: " + response.token);
                        feedbackText.text = "로그인 성공!";
                        await UIManager.Instance.ShowLoadingUI();
                        await GameManager.Instance.InitializeGameData();
                        UIManager.Instance.HideLoadingUI();

                    }
                    else
                    {
                        feedbackText.text = "로그인 실패: 알 수 없는 오류.";
                    }
                }

                HideFeedbackText();
            }
        }
        catch (System.Exception ex)
        {
            feedbackText.text = $"오류 발생: {ex.Message}";
            HideFeedbackText();
        }
    }

    private async void HideFeedbackText()
    {
        await UniTask.Delay(3000);
        feedbackText.text = "";
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
