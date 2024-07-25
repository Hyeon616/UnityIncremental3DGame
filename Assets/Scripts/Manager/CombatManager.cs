using UnityEngine;
using System.Collections.Generic;

public class CombatManager : UnitySingleton<CombatManager>
{
    [SerializeField] private UI_DamageText damageTextPrefab;
    [SerializeField] private int damageTextPoolSize = 15;

    private ObjectPool<UI_DamageText> damageTextPool;
    private Canvas uiCanvas;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        uiCanvas = GameObject.Find("@UI_Canvas")?.GetComponent<Canvas>();
        if (uiCanvas == null)
        {
            Debug.LogError("@UI_Canvas not found!");
            return;
        }

        if (damageTextPrefab == null)
        {
            damageTextPrefab = Resources.Load<UI_DamageText>("Prefabs/UI/DamageTextUI");
            if (damageTextPrefab == null)
            {
                Debug.LogError("DamageTextUI prefab not found in Resources!");
                return;
            }
        }

        InitializeDamageTextPool();
    }

    private void InitializeDamageTextPool()
    {
        damageTextPool = new ObjectPool<UI_DamageText>(damageTextPrefab, damageTextPoolSize, uiCanvas.transform);
    }

    public void ShowDamageText(int damage, bool isCritical, Vector3 worldPosition)
    {
        if (damageTextPool == null)
        {
            Debug.LogError("DamageTextPool is not initialized!");
            return;
        }

        UI_DamageText damageText = damageTextPool.GetObject();
        if (damageText != null)
        {
            damageText.gameObject.SetActive(true);
            damageText.Setup(damage, isCritical, worldPosition, () =>
            {
                if (damageText != null && damageText.gameObject != null)
                {
                    damageText.gameObject.SetActive(false);
                }
            });
        }
        else
        {
            Debug.LogError("Failed to get UI_DamageText from pool");
        }
    }

    public int CalculateDamage(int baseDamage, float criticalChance, float criticalDamage)
    {
        bool isCritical = Random.value < criticalChance;
        float damageMultiplier = Random.Range(0.9f, 1.1f);
        int calculatedDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);

        if (isCritical)
        {
            calculatedDamage = Mathf.RoundToInt(calculatedDamage * criticalDamage);
        }

        return calculatedDamage;
    }

}