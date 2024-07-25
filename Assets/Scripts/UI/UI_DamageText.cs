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

    private Sequence _currentSequence;

    private void Awake()
    {
        if (DamageText == null)
        {
            DamageText = GetComponent<Text>();
        }
    }

    public void Setup(int damageAmount, bool isCritical, Vector3 worldPosition, Action onComplete)
    {
       
        transform.position = Camera.main.WorldToScreenPoint(worldPosition);
        DamageText.text = damageAmount.ToString();
        DamageText.color = isCritical ? CriticalColor : NormalColor;

        if (_currentSequence != null)
        {
            _currentSequence.Kill();
        }

        _currentSequence = DOTween.Sequence();
        _currentSequence.Append(transform.DOMoveY(transform.position.y + _moveDistance, _disappearSpeed).SetEase(Ease.OutCubic));
        _currentSequence.Join(DamageText.DOFade(0, _fadeSpeed).SetEase(Ease.InCubic));
        _currentSequence.OnComplete(() =>
        {
            onComplete?.Invoke();
            _currentSequence = null;
        });
    }


    private void OnDisable()
    {
        if (_currentSequence != null)
        {
            _currentSequence.Kill();
            _currentSequence = null;
        }

        if (DamageText != null)
        {
            DamageText.color = NormalColor;
            DamageText.text = "";
        }

        transform.localPosition = Vector3.zero;
    }

}
