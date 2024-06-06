using UnityEngine;

public class PlayerViewModel : CharacterViewModel
{
    [SerializeField] private Transform AttackEffectSpawnPoint; // 이펙트 스폰 위치

    public override string TargetTag => "Monster";

    public override void ApplyDamage()
    {
        base.ApplyDamage();
    }

    public void PlayAttackEffect(Transform targetTransform)
    {
        if (AttackEffectPrefab == null)
        {
            Debug.LogError("AttackEffectPrefab is not set.");
            return;
        }

        Vector3 effectPosition = AttackEffectSpawnPoint.position;
        GameObject effect = Object.Instantiate(AttackEffectPrefab, effectPosition, Quaternion.identity);

        Vector3 direction = (targetTransform.position - effectPosition).normalized;
        effect.transform.rotation = Quaternion.LookRotation(direction);

        Object.Destroy(effect, 2f);
    }

    public new void FullHeal()
    {
        CharacterModel.FullHeal();
    }

    protected override void Die()
    {
        Debug.Log("Player died.");
    }
    private void PlayAttackSound()
    {
        if (AttackAudioSource != null && AttackAudioClip != null)
        {
            AttackAudioSource.PlayOneShot(AttackAudioClip);
        }
    }
}
