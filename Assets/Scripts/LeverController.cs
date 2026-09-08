using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// Makes the lever graphic physically travel downward (not just swap sprite in place) when tapped, then springs back up. The "down" sprite only shows once it has actually reached the bottom of its travel,
// so it reads as the lever being pulled rather than a sprite flicker. 


[RequireComponent(typeof(Image))]
public class LeverController : MonoBehaviour, IPointerClickHandler
{
    [Header("Lever Sprites")]
    [SerializeField] private Sprite leverUpSprite;   // slot-machine2.png
    [SerializeField] private Sprite leverDownSprite;  // slot-machine3.png

    [Header("Travel")]
    [Tooltip("How far (and which direction) the lever moves from its resting position when pulled. Negative Y = downward.")]
    [SerializeField] private Vector2 pulledOffset = new Vector2(0f, -60f);
    [SerializeField] private float pullDuration = 0.16f;
    [SerializeField] private float holdAtBottom = 0.1f;
    [SerializeField] private float springBackDuration = 0.35f;

    [Header("References")]
    [SerializeField] private SlotMachine slotMachine;
    [SerializeField] private GameFlowController gameFlow;

    private Image leverImage;
    private RectTransform leverRect;
    private Vector2 restPosition;

    private void Awake()
    {
        leverImage = GetComponent<Image>();
        leverRect = (RectTransform)transform;
        restPosition = leverRect.anchoredPosition;
        leverImage.sprite = leverUpSprite;
    }

    private void OnEnable()
    {
        slotMachine.onSpinResolved.AddListener(HandleSpinResolved);
    }

    private void OnDisable()
    {
        slotMachine.onSpinResolved.RemoveListener(HandleSpinResolved);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slotMachine.SpinInProgress) return;

        StopAllCoroutines();
        StartCoroutine(PullRoutine());
    }

    private IEnumerator PullRoutine()
    {
        // Travel down while still showing the "up" sprite - the motion itself
        // reads as the pull starting.
        yield return MoveTo(restPosition + pulledOffset, pullDuration);

        // Only switch to the "pulled" sprite once it's actually at the bottom.
        leverImage.sprite = leverDownSprite;
        gameFlow.RequestSpin();

        yield return new WaitForSeconds(holdAtBottom);

        leverImage.sprite = leverUpSprite;
        yield return MoveTo(restPosition, springBackDuration);
    }

    private IEnumerator MoveTo(Vector2 target, float duration)
    {
        Vector2 start = leverRect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 2f); // ease-out
            leverRect.anchoredPosition = Vector2.LerpUnclamped(start, target, eased);
            yield return null;
        }

        leverRect.anchoredPosition = target;
    }

    private void HandleSpinResolved(bool won, SlotSymbol matched, float multiplier)
    {
        // Safety net: if a spin was triggered by the Spin button rather than the lever, makes sure the lever is visually reset.
        StopAllCoroutines();
        leverImage.sprite = leverUpSprite;
        leverRect.anchoredPosition = restPosition;
    }
}
