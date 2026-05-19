using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private GameObject hitBox;

    [SerializeField] private float attackDuration = 0.5f;

    private void Start()
    {
        hitBox = GameObject.FindGameObjectWithTag("AttackZone");
        hitBox.SetActive(false);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        hitBox.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        hitBox.SetActive(false);
    }
}
