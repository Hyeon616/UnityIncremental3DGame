using UnityEngine;
using UnityEngine.AI;

public abstract class CharacterViewModel : MonoBehaviour
{
    [SerializeField] private CharacterView CharacterView_CharacterView;
    [SerializeField] private NavMeshAgent NavMeshAgent_Agent;
    [SerializeField] private Transform Transform_Target;
    [SerializeField] private CharacterModel CharacterModel_CharacterModel;
    [SerializeField] private GameObject GameObject_DamageTextPrefab;
    [SerializeField] private GameObject GameObject_AttackEffectPrefab;
    [SerializeField] private GameObject GameObject_HitEffectPrefab;
    [SerializeField] private AudioSource AudioSource_Attack;
    [SerializeField] private AudioClip AudioClip_Attack;
    [SerializeField] private Canvas Canvas_DamageFont;

    private ICharacterState _currentState;
    private float _attackTimer;
    private AttackHandler _attackHandler;

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

    public GameObject DamageTextPrefab => GameObject_DamageTextPrefab;
    public GameObject AttackEffectPrefab => GameObject_AttackEffectPrefab;
    public GameObject HitEffectPrefab => GameObject_HitEffectPrefab;
    public AudioSource AttackAudioSource => AudioSource_Attack;
    public AudioClip AttackAudioClip => AudioClip_Attack;
    public abstract string TargetTag { get; }

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
        Canvas instantiatedCanvas = Instantiate(Canvas_DamageFont);
        _attackHandler = new AttackHandler(this);
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

    public virtual void ApplyDamage()
    {
        _attackHandler.ApplyDamage();
    }

    public void FullHeal()
    {
        CharacterModel.FullHeal();
    }
}
