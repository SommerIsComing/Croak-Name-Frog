using UnityEngine;
using System.Collections;

public class ShowCollected : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
