using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Animator animator;
    private GameObject hitBox;

    [SerializeField] private float attackDuration = 0.5f;

    private void Awake()
    {
        hitBox = GameObject.FindGameObjectWithTag("AttackZone");
        hitBox.SetActive(false);
    }

    IEnumerator Attack()
    {
        hitBox.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        hitBox.SetActive(false);
    }
}
