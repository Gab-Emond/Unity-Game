using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "NinjaGame/WeaponDataSO", order = 0)]
public class WeaponDataSO : ScriptableObject {
    
    public string weaponName;
    public int weaponDamage;
    
    public GameObject weaponPrefab;
}