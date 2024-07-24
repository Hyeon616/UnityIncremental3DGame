using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_DamageText : MonoBehaviour
{
    [SerializeField] private Text DamageText;
    [SerializeField] private float _disappearSpeed = 2f;
    [SerializeField] private float _moveDistance = 2f;
    [SerializeField] private float _fadeSpeed = 1.3f;

    private static readonly Color CriticalColor = Color.yellow;
    private static readonly Color MonsterDamageColor = Color.red;
    private Color _originalColor;
    private Sequence _currentSequence;

    private static ObjectPool<UI_DamageText> damageTextPool;
    private const int DAMAGE_TEXT_POOL_SIZE = 15;
    private void Awake()
    {
        _originalColor = DamageText.color;
    }

    public static void Initialize()
    {
        if (damageTextPool == null)
        {
            GameObject prefab = GameManager.Instance.UIPrefabs["DamageTextUI"];
            if (prefab == null)
            {
                Debug.LogError("DamageTextUI prefab not found");
                return;
            }

            UI_DamageText damageTextPrefab = prefab.GetComponent<UI_DamageText>();
            if (damageTextPrefab == null)
            {
                Debug.LogError("UI_DamageText component not found on DamageTextUI prefab");
                return;
            }

            Transform canvas = GameObject.Find("@UI_Canvas").transform;
            damageTextPool = new ObjectPool<UI_DamageText>(damageTextPrefab, DAMAGE_TEXT_POOL_SIZE, canvas);
        }
    }

    public void Setup(int damageAmount, bool isCritical, bool isMonsterDamage, Vector3 position)
    {
        if (_currentSequence != null)
        {
            _currentSequence.Kill();
        }

        gameObject.SetActive(true);
        transform.position = Camera.main.WorldToScreenPoint(position);
        DamageText.text = damageAmount.ToString();
        DamageText.color = isCritical ? CriticalColor : (isMonsterDamage ? MonsterDamageColor : _originalColor);

        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;

        _currentSequence = DOTween.Sequence();
        _currentSequence.Append(transform.DOMoveY(transform.position.y + _moveDistance, _disappearSpeed).SetEase(Ease.OutCubic));
        _currentSequence.Join(canvasGroup.DOFade(0, _fadeSpeed).SetEase(Ease.InCubic));
        _currentSequence.OnComplete(() => gameObject.SetActive(false));
    }

    public static void Create(int damage, bool isCritical, Vector3 position)
    {
        if (damageTextPool == null)
        {
            Initialize();
        }

        UI_DamageText damageText = damageTextPool.GetObject();
        if (damageText != null)
        {
            damageText.Setup(damage, isCritical, false, position);
        }
        else
        {
            Debug.LogError("Failed to get UI_DamageText from pool");
        }
    }


}
