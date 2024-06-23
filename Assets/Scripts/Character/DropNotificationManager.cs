using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropNotificationManager : UnitySingleton<DropNotificationManager>
{
    [SerializeField] private RectTransform notificationPanel;
    [SerializeField] private GameObject notificationPrefab;
    private List<GameObject> notifications = new List<GameObject>();

    private const float notificationHeight = 30f;
    private const float notificationSpacing = 10f;
    private const float notificationDuration = 3f;

    public void ShowDropNotification(string rarity, string grade)
    {
        GameObject notification = Instantiate(notificationPrefab, notificationPanel);
        Text notificationText = notification.GetComponent<Text>();
        string rarityColor = ColorUtility.ToHtmlStringRGBA(GetRarityColor(rarity));
        notificationText.text = $"몬스터가<color=#{rarityColor}> {rarity}</color> {grade} 무기 드랍";
        notifications.Add(notification);

        // 새 알림 위치 조정
        for (int i = 0; i < notifications.Count; i++)
        {
            float targetY = -i * (notificationHeight + notificationSpacing);
            notifications[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, targetY);
        }

        // 3초 후 알림 제거
        DOVirtual.DelayedCall(notificationDuration, () =>
        {
            notifications.Remove(notification);
            Destroy(notification);

            // 알림 제거 후 위치 조정
            for (int i = 0; i < notifications.Count; i++)
            {
                float targetY = -i * (notificationHeight + notificationSpacing);
                notifications[i].GetComponent<RectTransform>().DOAnchorPosY(targetY, 0.2f);
            }
        });
    }

    private Color GetRarityColor(string rarity)
    {
        return rarity switch
        {
            "일반" => new Color(0.75f, 0.75f, 0.75f, 1f), // 밝은 회색
            "고급" => new Color(0f, 1f, 0f, 1f), // 초록
            "매직" => new Color(0f, 0f, 1f, 1f), // 파랑
            "유물" => new Color(1f, 0.65f, 0f, 1f), // 주황
            "영웅" => new Color(1f, 0f, 1f, 1f), // 마젠타
            "에픽" => new Color(1f, 1f, 0f, 1f), // 노랑
            "고대" => new Color(0.55f, 0f, 0f, 1f), // 빨강
            "신화" => new Color(0.53f, 0.81f, 0.92f, 1f), // 밝은 파랑
            _ => new Color(0f, 0f, 0f, 1f), // 기본 검정색
        };
    }
}
