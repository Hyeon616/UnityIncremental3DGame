using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public abstract class CharacterViewModel : MonoBehaviour
{
    [SerializeField] protected CharacterView CharacterView_CharacterView;
    [SerializeField] protected NavMeshAgent NavMeshAgent_Agent;
    [SerializeField] protected Transform Transform_Target;
    [SerializeField] protected CharacterModel CharacterModel_CharacterModel;
    [SerializeField] protected GameObject GameObject_DamageTextPrefab;

    private ICharacterState _currentState;
    private float _attackTimer;

    public ICharacterState CurrentState => _currentState;
    public CharacterView CharacterView => CharacterView_CharacterView;
    public NavMeshAgent Agent => NavMeshAgent_Agent;
    public Transform Target
    {
        get => Transform_Target;
        set => Transform_Target = value;
    }

    public CharacterModel CharacterModel => CharacterModel_CharacterModel;
    public float AttackTimer
    {
        get => _attackTimer;
        set => _attackTimer = value;
    }


    public abstract string TargetTag { get; }
    public virtual bool IsPlayer => false;

    private void Awake()
    {
        if (CharacterModel_CharacterModel == null)
        {
            CharacterModel_CharacterModel = GetComponent<CharacterModel>();
            if (CharacterModel_CharacterModel == null)
            {
                Debug.LogError("CharacterModel_CharacterModel is not set and could not be found on the GameObject.");
            }
        }
    }

    public void SetState(ICharacterState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }
        _currentState = newState;
        _currentState.Enter();
    }

    public void ChangeState(ICharacterState newState)
    {
        SetState(newState);
    }

    public virtual void Update()
    {
        _currentState?.Execute();
    }

    protected abstract void Die();
    public abstract void ApplyDamage();

    public Transform FindTarget(float detectionRange, string targetTag)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(targetTag))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = hitCollider.transform;
                }
            }
        }

        return closestTarget;
    }

    protected virtual void ShowDamage(Transform targetTransform, int damageAmount)
    {
        if (GameObject_DamageTextPrefab == null)
        {
            Debug.LogError("GameObject_DamageTextPrefab is not set.");
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in the scene.");
            return;
        }

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        Vector3 randomOffset = Random.insideUnitSphere * 50;
        randomOffset.z = 0;
        screenPosition += randomOffset;

        // Check if the screen position is within the camera view and apply offset to keep it fully visible
        float margin = 50f; // Margin from screen edges to ensure full visibility
        screenPosition.x = Mathf.Clamp(screenPosition.x, margin, Screen.width - margin);
        screenPosition.y = Mathf.Clamp(screenPosition.y, margin, Screen.height - margin);

        GameObject damageText = Instantiate(GameObject_DamageTextPrefab, canvas.transform);
        damageText.transform.position = screenPosition;

        DamageText damageTextComponent = damageText.GetComponent<DamageText>();
        if (damageTextComponent == null)
        {
            Debug.LogError("DamageText component is not found on the damageText prefab.");
            return;
        }

        damageTextComponent.Setup(damageAmount);

        // DOTween 애니메이션 추가
        damageText.transform.localScale = Vector3.zero;
        damageText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        damageText.GetComponent<CanvasGroup>().DOFade(0, 2f).SetDelay(0.5f).OnComplete(() =>
        {
            Destroy(damageText);
        });
    }

    public void OnAttackAnimationEnd()
    {
        if (_currentState is AttackingState)
        {
            _currentState.Exit();
            SetState(new IdleState(this, 100f, CharacterModel_CharacterModel.AttackRange));
        }
    }
}
