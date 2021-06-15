using UnityEngine;

public class FlashManager : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private bool isFlashFadingIn = false;
    private bool isFlashFadingOut = false;
    private float flashFadeStartedAt = -100;
    private float flashInDuration = 0.1f;
    private float flashOutDuration = 1.4f;

    void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (isFlashFadingIn) {
            float progress = (Time.time - flashFadeStartedAt) / flashInDuration;
            if (progress <= 1) {
                canvasGroup.alpha = progress;
            } else {
                canvasGroup.alpha = 1;
                isFlashFadingIn = false;
                isFlashFadingOut = true;
                flashFadeStartedAt = Time.time;
                Camera.main.GetComponent<SoundManager>().Play("polaroid");
            }
        } else if (isFlashFadingOut) {
            float progress = (Time.time - flashFadeStartedAt) / flashOutDuration;
            if (progress <= 1) {
                canvasGroup.alpha = 1 - progress;
            } else {
                canvasGroup.alpha = 0;
                isFlashFadingOut = false;
            }
        }
    }


    public void PhotoFlash() {
        if (isFlashFadingIn || isFlashFadingOut) {
            return;
        }

        isFlashFadingIn = true;
        flashFadeStartedAt = Time.time;
    }
}
