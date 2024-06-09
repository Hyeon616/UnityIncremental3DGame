using UnityEngine;

[CreateAssetMenu(fileName = "APISettings", menuName = "Settings/APISettings")]
public class APISettings : ScriptableObject
{
    public string baseUrl;
    public string registerEndpoint = "register";
    public string loginEndpoint = "login";
    public string weaponsEndpoint = "weapons";
    public string drawWeaponEndpoint = "drawWeapon";
    public string synthesizeWeaponEndpoint = "synthesizeWeapon";
    public string synthesizeAllWeaponsEndpoint = "synthesizeAllWeapons";
    public string getWeaponsByRarityEndpoint = "weaponsByRarity";
    public string RegisterUrl => $"{baseUrl}/{registerEndpoint}";
    public string LoginUrl => $"{baseUrl}/{loginEndpoint}";
    public string WeaponsUrl => $"{baseUrl}/{weaponsEndpoint}";
    public string DrawWeaponUrl => $"{baseUrl}/{drawWeaponEndpoint}";
    public string SynthesizeWeaponUrl => $"{baseUrl}/{synthesizeWeaponEndpoint}";
    public string SynthesizeAllWeaponsUrl => $"{baseUrl}/{synthesizeAllWeaponsEndpoint}";
    public string GetWeaponsByRarityUrl => $"{baseUrl}/{getWeaponsByRarityEndpoint}";
}
