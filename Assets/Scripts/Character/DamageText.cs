using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [SerializeField] private Text Text_DamageText;
    [SerializeField] private float _disappearSpeed = 2f;
    [SerializeField] private float _moveDistance = 2f;
    [SerializeField] private float _fadeSpeed = 2f;

    private static readonly Color CriticalColor = Color.yellow;
    private static readonly Color MonsterDamageColor = Color.red;
    private Color _originalColor;

    private void Awake()
    {
        _originalColor = Text_DamageText.color;
    }

    public void Setup(int damageAmount, bool isCritical, bool isMonsterDamage)
    {
        Text_DamageText.text = damageAmount.ToString();

        if (isCritical)
        {
            Text_DamageText.color = CriticalColor;
        }
        else if (isMonsterDamage)
        {
            Text_DamageText.color = MonsterDamageColor;
        }
        else
        {
            Text_DamageText.color = _originalColor;
        }

        //Sequence sequence = DOTween.Sequence();
        //sequence.Append(transform.DOMoveY(transform.position.y + _moveDistance, _disappearSpeed).SetEase(Ease.OutCubic));
        //sequence.Join(Text_DamageText.DOFade(0, _fadeSpeed).SetEase(Ease.InCubic));
        //sequence.OnComplete(() => Destroy(gameObject));
    }
}
