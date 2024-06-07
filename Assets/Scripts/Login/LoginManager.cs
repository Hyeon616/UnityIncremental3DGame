using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username))
        {
            feedbackText.text = "아이디를 입력해주세요.";
            HideFeedbackText();
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            feedbackText.text = "패스워드를 입력해주세요.";
            HideFeedbackText();
            return;
        }

        var requestBody = new
        {
            username = username,
            password = password
        };

        string jsonData = JsonConvert.SerializeObject(requestBody);

        using (UnityWebRequest request = new UnityWebRequest(apiSettings.LoginUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                feedbackText.text = request.error;
                Debug.LogError($"Login error: {request.error}");
            }
            else
            {
                var responseText = request.downloadHandler.text;

                try
                {
                    var response = JsonUtility.FromJson<LoginResponse>(responseText);
                    PlayerPrefs.SetString("authToken", response.token);
                    feedbackText.text = "Login successful!";

                    SceneManager.LoadScene("GameScene");
                }
                catch (System.Exception ex)
                {
                    feedbackText.text = "Login failed: Invalid response format";
                    Debug.LogError($"Error parsing login response: {ex.Message}");
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
    private class LoginResponse
    {
        public string token;
    }
}
