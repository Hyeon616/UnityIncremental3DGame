using UnityEngine;

[CreateAssetMenu(fileName = "APISettings", menuName = "Settings/APISettings")]
public class APISettings : ScriptableObject
{
    public string baseUrl;
    public string registerEndpoint = "register";
    public string loginEndpoint = "login";
    public string weaponsEndpoint = "weapons";
    public string drawWeaponEndpoint = "drawWeapon";

    public string RegisterUrl => $"{baseUrl}/{registerEndpoint}";
    public string LoginUrl => $"{baseUrl}/{loginEndpoint}";
    public string WeaponsUrl => $"{baseUrl}/{weaponsEndpoint}";
    public string DrawWeaponUrl => $"{baseUrl}/{drawWeaponEndpoint}";
}
