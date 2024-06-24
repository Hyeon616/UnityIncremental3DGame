using System.Collections.Generic;
using UnityEngine;

public class AttackHandler
{
   
    private void ShowDamageText(Transform targetTransform, int damage, bool isCritical, bool isMonsterDamage)
    {
        //if (_characterViewModel.DamageTextPrefab != null)
        //{
        //    // 화면 좌표로 변환
        //    Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);

        //    // 오프셋을 추가하여 텍스트가 화면에 걸쳐서 출력되지 않도록 합니다.
        //    Vector3 randomOffset = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0);
        //    screenPosition += randomOffset;

        //    // 오프셋이 적용된 좌표가 화면 안에 위치하는지 확인합니다.
        //    screenPosition.x = Mathf.Clamp(screenPosition.x, 50, Screen.width - 50);
        //    screenPosition.y = Mathf.Clamp(screenPosition.y, 50, Screen.height - 50);

        //    //GameObject damageText = Object.Instantiate(_characterViewModel.DamageTextPrefab, SingletonDamageFontCanvas.Instance.transform);
        //    //damageText.transform.position = screenPosition;

        //    //DamageText damageTextComponent = damageText.GetComponent<DamageText>();
        //    //if (damageTextComponent != null)
        //    //{
        //    //    damageTextComponent.Setup(damage, isCritical, isMonsterDamage);
                
        //    //}
        //}
    }

    private void ShowHitEffect(Transform targetTransform)
    {
        //if (_characterViewModel.HitEffectPrefab != null)
        //{
        //    GameObject hitEffect = Object.Instantiate(_characterViewModel.HitEffectPrefab, targetTransform.position, Quaternion.Euler(0, 180, 0));
        //    hitEffect.transform.LookAt(_characterViewModel.transform);
        //    Object.Destroy(hitEffect, 2f);
        //}
    }

    private void PlayAttackSound()
    {
        //if (_characterViewModel.AttackAudioSource != null && _characterViewModel.AttackAudioClip != null)
        //{
        //    _characterViewModel.AttackAudioSource.PlayOneShot(_characterViewModel.AttackAudioClip);
        //}

    }
}
