using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [SerializeField] private Text Text_DamageText;
    [SerializeField] private float _disappearSpeed = 2f;
    [SerializeField] private float _moveDistance = 2f;
    [SerializeField] private float _fadeSpeed = 2f;

    private Color _textColor;
    private static readonly Color _criticalColor = new Color(1f, 1f, 0f);

    private void Awake()
    {
        // Initialize the text color
        Text_DamageText.color = new Color(Text_DamageText.color.r, Text_DamageText.color.g, Text_DamageText.color.b, 1f);
    }

    public void Setup(int damageAmount, bool isCritical)
    {
        Text_DamageText.text = damageAmount.ToString();
        _textColor = isCritical ? _criticalColor : Color.white;
        Text_DamageText.color = _textColor;

        // Move up and fade out animation using DOTween
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(transform.position.y + _moveDistance, _disappearSpeed).SetEase(Ease.OutCubic));
        sequence.Join(Text_DamageText.DOFade(0, _fadeSpeed).SetEase(Ease.InCubic));
        sequence.OnComplete(() => Destroy(gameObject));
    }

}
