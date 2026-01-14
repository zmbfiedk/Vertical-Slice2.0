using System.Collections;
using UnityEngine;

public class ChestBehaiviour : MonoBehaviour
{
    GameObject CoinPrefab;
    private void OnEnable()
    {
        StartCoroutine(OpenChest());
    }

    private void OnDisable()
    {

    }

    IEnumerator OpenChest()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("OpenChest");
        yield return new WaitForSeconds(2f);
        GameObject.Instantiate(CoinPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
