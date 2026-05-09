using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Silah / Item Objeleri")]
    public GameObject meleeItem;      // MeleeItem (Player'ın child'ı)
    public GameObject rangedWeapon;   // RangedWeapon (Player'ın child'ı)

    void Awake()
    {
        // Başlangıç: Melee açık, Ranged kapalı
        if (meleeItem != null) meleeItem.SetActive(true);
        if (rangedWeapon != null) rangedWeapon.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (meleeItem != null) meleeItem.SetActive(true);
            if (rangedWeapon != null) rangedWeapon.SetActive(false);
            Debug.Log("🔪 Melee Aktif");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (meleeItem != null) meleeItem.SetActive(false);
            if (rangedWeapon != null) rangedWeapon.SetActive(true);
            Debug.Log("🔫 Ranged Aktif");
        }
    }
}