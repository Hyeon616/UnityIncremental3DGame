using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_DamageText : UIManager
{
    [SerializeField] private Text DamageText;
    [SerializeField] private float _disappearSpeed = 1f;
    [SerializeField] private float _moveDistance = 2f;
    [SerializeField] private float _fadeSpeed = 1f;

    private static readonly Color CriticalColor = Color.yellow;
    private static readonly Color NormalColor = Color.white;

    public void Setup(int damage, bool isCritical, Vector3 worldPosition, Action onComplete)
    {
        transform.position = Camera.main.WorldToScreenPoint(worldPosition);
        DamageText.text = damage.ToString();
        DamageText.color = isCritical ? CriticalColor : NormalColor;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(transform.position.y + _moveDistance, _disappearSpeed).SetEase(Ease.OutCubic));
        sequence.Join(DamageText.DOFade(0, _fadeSpeed).SetEase(Ease.InCubic));
        sequence.OnComplete(() => {
            onComplete?.Invoke();
            ResetText();
        });
    }

    private void ResetText()
    {
        DamageText.color = NormalColor;
        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

}
