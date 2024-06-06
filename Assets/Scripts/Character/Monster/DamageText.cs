using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [SerializeField] private Text damageText;
    [SerializeField] private float disappearSpeed = 2f;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float fadeSpeed = 2f;

    private Color textColor;

    private void Awake()
    {
        // Initialize the text color
        damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, 1f);
    }

    public void Setup(int damageAmount)
    {
        damageText.text = damageAmount.ToString();

        // Move up and fade out animation using DOTween
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(transform.position.y + moveDistance, disappearSpeed).SetEase(Ease.OutCubic));
        sequence.Join(damageText.DOFade(0, fadeSpeed).SetEase(Ease.InCubic));
        sequence.OnComplete(() => Destroy(gameObject));
    }

}
