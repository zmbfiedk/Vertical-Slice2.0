using System.Collections;
using UnityEngine;
using TMPro;

public class CoinPopupUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Animation Settings")]
    [SerializeField] private Vector2 offScreenPos = new Vector2(0, -600);
    [SerializeField] private Vector2 onScreenPos = new Vector2(0, -200);
    [SerializeField] private float slideTime = 0.4f;
    [SerializeField] private float stayTime = 2f;

    private Coroutine popupRoutine;

    void Awake()
    {
        panel.anchoredPosition = offScreenPos;
        gameObject.SetActive(false);
    }

    public void ShowCoins(int coinAmount)
    {
        if (popupRoutine != null)
            StopCoroutine(popupRoutine);

        gameObject.SetActive(true);
        coinText.text = $"Coins: {coinAmount}";

        popupRoutine = StartCoroutine(PopupSequence());
    }

    private IEnumerator PopupSequence()
    {
        // Slide in
        yield return Slide(panel, offScreenPos, onScreenPos);

        // Stay on screen
        yield return new WaitForSeconds(stayTime);

        // Slide out
        yield return Slide(panel, onScreenPos, offScreenPos);

        gameObject.SetActive(false);
    }

    private IEnumerator Slide(RectTransform rect, Vector2 from, Vector2 to)
    {
        float elapsed = 0f;

        while (elapsed < slideTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideTime;
            rect.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        rect.anchoredPosition = to;
    }
}