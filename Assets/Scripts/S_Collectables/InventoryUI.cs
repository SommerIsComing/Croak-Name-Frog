using UnityEngine;
using TMPro;
using System.Collections;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private GameObject coinImage;
    [SerializeField] private float showTime = 2f;

    private Coroutine hideRoutine;

    private void Start()
    {
        HideUI();
    }

    public void UpdateCoinCount(PlayerInventory playerInventory)
    {
        coinText.text = playerInventory.NumberOfCoins.ToString();
        ShowUI();
    }

    private void ShowUI()
    {
        gameObject.SetActive(true);
        coinImage.SetActive(true);

        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
        }

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(showTime);
        HideUI();
    }

    private void HideUI()
    {
        gameObject.SetActive(false);
        coinImage.SetActive(false);
    }
}
