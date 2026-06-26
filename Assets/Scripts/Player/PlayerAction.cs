using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private float weaponReloadTime = 1;
    private IPlayerInput input;
    private IWeapon weapon;
    private bool isReloaded = false;
    private bool isAttacking = false;
    private float attackTimeElapsed = 0;
    private void Awake()
    {
        input = GetComponent<IPlayerInput>();
        weapon = GetComponent<IWeapon>();
    }
    void Update()
    {
        if (weapon != null)
        {
            Reload();
            Attack();
        }
    }
    private void Reload()
    {
        if (!isReloaded)
        {
            attackTimeElapsed += Time.deltaTime;
            if (attackTimeElapsed >= weaponReloadTime)
            {
                weapon.Reload();
                isReloaded = true;
            }
        }
    }
    private void Attack()
    {
        if (input.AttackInput != 0)
        {
            if (!isAttacking)
            {
                isAttacking = true;
                isReloaded = false;
                attackTimeElapsed = 0;
                weapon.Fire();
            }
        }
        else
        {
            isAttacking = false;
        }
    }
}
