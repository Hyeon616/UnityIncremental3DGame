using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class UI_Authentication : MonoBehaviour
{
    [Header("AuthenticationTab")]
    [SerializeField] private Button LoginTabButton;
    [SerializeField] private Button RegisterTabButton;

    [Header("AuthenticationUI")]
    [SerializeField] private GameObject LoginTabUI;
    [SerializeField] private GameObject RegisterTabUI;
    [SerializeField] private TMP_Text FeedbackText;

    [Header("LoginUI")]
    [SerializeField] private TMP_InputField LoginUserNameInputField;
    [SerializeField] private TMP_InputField LoginPasswordInputField;
    [SerializeField] private Button LoginButton;

    [Header("RegisterUI")]
    [SerializeField] private TMP_InputField RegisterUserNameInputField;
    [SerializeField] private TMP_InputField RegisterPasswordInputField;
    [SerializeField] private TMP_InputField RegisterNickNameInputField;
    [SerializeField] private Button RegisterButton;


    private Color32 inactiveColor = new Color32(142, 191, 224, 177); // #8EBFE0, alpha 177
    private Color32 activeColor = new Color32(170, 181, 221, 255); // #AAB5DD, alpha 255

    private LoginManager loginManager = new LoginManager();
    private RegistrationManager registrationManager = new RegistrationManager();

    private void Start()
    {
        ShowLoginTabUI();

    }

    private void OnEnable()
    {
        // 버튼 클릭 이벤트 추가
        LoginTabButton.onClick.AddListener(ShowLoginTabUI);
        RegisterTabButton.onClick.AddListener(ShowRegisterTabUI);
        LoginButton.onClick.AddListener(Login);
        RegisterButton.onClick.AddListener(Register);
    }

    private void OnDisable()
    {
        LoginTabButton.onClick.RemoveListener(ShowLoginTabUI);
        RegisterTabButton.onClick.RemoveListener(ShowRegisterTabUI);
        LoginButton.onClick.RemoveListener(Login);
        RegisterButton.onClick.RemoveListener(Register);
    }


    private void ShowLoginTabUI()
    {
        SetPanelState(LoginTabUI, LoginTabButton, true);
        SetPanelState(RegisterTabUI, RegisterTabButton, false);
    }

    private void ShowRegisterTabUI()
    {
        SetPanelState(LoginTabUI, LoginTabButton, false);
        SetPanelState(RegisterTabUI, RegisterTabButton, true);
    }

    private void SetPanelState(GameObject panel, Button button, bool isActive)
    {
        panel.SetActive(isActive);
        button.GetComponent<Image>().color = isActive ? activeColor : inactiveColor;
    }

    private async void Login()
    {
        string username = LoginUserNameInputField.text;
        string password = LoginPasswordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            SetFeedbackText("아이디와 패스워드를 입력하세요.");
            return;
        }

        SetFeedbackText("로그인 중...");
        Debug.Log("로그인 중");

        bool success = false;

        try
        {
            Debug.Log($"Sending login request for username: {username}");
            // 로그인 요청 코드
            
            success = await loginManager.Login(username, password, SetFeedbackText);
            Debug.Log($"로그인 시도 결과: {success}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Login failed with exception: {ex.Message}");
            SetFeedbackText("로그인에 실패했습니다.");
        }

        if (success)
        {
            Debug.Log("로그인 성공, UI 전환");
            UIManager.Instance.HideUI("AuthenticationUI");
            UIManager.Instance.ShowUI("LoadingUI");
            await GameManager.Instance.InitializeGame();
            UIManager.Instance.HideUI("LoadingUI");
        }
        else
        {
            Debug.LogError("로그인 실패");
            SetFeedbackText("로그인에 실패했습니다.");
        }
    }

    private async void Register()
    {
        string username = RegisterUserNameInputField.text;
        string password = RegisterPasswordInputField.text;
        string nickname = RegisterNickNameInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nickname))
        {
            SetFeedbackText("모든 필드를 입력하세요.");
            return;
        }

        SetFeedbackText("회원가입 중...");
        await registrationManager.Register(username, password, nickname, SetFeedbackText);
    }

    private void SetFeedbackText(string message)
    {
        FeedbackText.text = message;
        HideFeedbackText();
    }

    private async void HideFeedbackText()
    {
        await UniTask.Delay(3000);
        FeedbackText.text = "";
    }

    
}
