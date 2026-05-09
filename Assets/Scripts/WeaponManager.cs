using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Item / Silah Objeleri")]
    public GameObject meleeItem;      // MeleeItem (PlayerMelee burada)
    public GameObject rangedWeapon;   // RangedWeapon (PlayerShooting burada)

    private int activeWeapon = 1;

    void Awake()
    {
        // Başlangıç: Melee açık, Ranged kapalı
        if (meleeItem != null) meleeItem.SetActive(true);
        if (rangedWeapon != null) rangedWeapon.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && activeWeapon != 1)
            SwitchWeapon(1);

        if (Input.GetKeyDown(KeyCode.Alpha2) && activeWeapon != 2)
            SwitchWeapon(2);
    }

    void SwitchWeapon(int id)
    {
        activeWeapon = id;

        if (id == 1)
        {
            // 🔪 MELEE: MeleeItem açık, RangedWeapon kapalı
            if (meleeItem != null) meleeItem.SetActive(true);
            if (rangedWeapon != null) rangedWeapon.SetActive(false);

            Debug.Log("🔪 Melee Item Aktif");
        }
        else if (id == 2)
        {
            // 🔫 RANGED: MeleeItem kapalı, RangedWeapon açık
            if (meleeItem != null) meleeItem.SetActive(false);
            if (rangedWeapon != null) rangedWeapon.SetActive(true);

            Debug.Log("🔫 Ranged Silah Aktif");
        }
    }
}