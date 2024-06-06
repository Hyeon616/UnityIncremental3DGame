using UnityEngine;

public class AttackHandler
{
    private CharacterViewModel _characterViewModel;
    private Canvas _canvas;

    public AttackHandler(CharacterViewModel characterViewModel)
    {
        _characterViewModel = characterViewModel;
        _canvas = GameObject.FindObjectOfType<Canvas>();
    }

    public void ApplyDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_characterViewModel.transform.position, _characterViewModel.CharacterModel.AttackRange);
        Collider closestCollider = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(_characterViewModel.TargetTag))
            {
                float distance = Vector3.Distance(_characterViewModel.transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = hitCollider;
                }
            }
        }

        if (closestCollider != null)
        {
            IDamageable damageable = closestCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                int damage = _characterViewModel.CharacterModel.GetRandomAttackPower();
                bool isCritical = _characterViewModel.CharacterModel.IsCriticalHit();
                bool isMonsterDamage = _characterViewModel is MonsterViewModel;

                damageable.TakeDamage(damage);
                ShowDamageText(closestCollider.transform, damage, isCritical, isMonsterDamage);
                ShowHitEffect(closestCollider.transform);

                if (_characterViewModel is PlayerViewModel playerViewModel)
                {
                    playerViewModel.PlayAttackEffect(closestCollider.transform);
                    PlayAttackSound();
                }

            }
        }
    }

    private void ShowDamageText(Transform targetTransform, int damage, bool isCritical, bool isMonsterDamage)
    {
        if (_characterViewModel.DamageTextPrefab != null)
        {
            // 화면 좌표로 변환
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);

            // 오프셋을 추가하여 텍스트가 화면에 걸쳐서 출력되지 않도록 합니다.
            Vector3 randomOffset = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0);
            screenPosition += randomOffset;

            // 오프셋이 적용된 좌표가 화면 안에 위치하는지 확인합니다.
            screenPosition.x = Mathf.Clamp(screenPosition.x, 50, Screen.width - 50);
            screenPosition.y = Mathf.Clamp(screenPosition.y, 50, Screen.height - 50);

            GameObject damageText = Object.Instantiate(_characterViewModel.DamageTextPrefab, _canvas.transform);
            damageText.transform.position = screenPosition;

            DamageText damageTextComponent = damageText.GetComponent<DamageText>();
            if (damageTextComponent != null)
            {
                damageTextComponent.Setup(damage, isCritical, isMonsterDamage);
            }
        }
    }

    private void ShowHitEffect(Transform targetTransform)
    {
        if (_characterViewModel.HitEffectPrefab != null)
        {

            GameObject hitEffect = Object.Instantiate(_characterViewModel.HitEffectPrefab, targetTransform.position, Quaternion.identity);
            Object.Destroy(hitEffect, 2f);
        }
    }

    private void PlayAttackSound()
    {
        if (_characterViewModel.AttackAudioSource != null && _characterViewModel.AttackAudioClip != null)
        {
            _characterViewModel.AttackAudioSource.PlayOneShot(_characterViewModel.AttackAudioClip);
        }
    }

}
