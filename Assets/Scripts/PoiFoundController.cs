using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PoiFoundController : MonoBehaviour
{
    public bool isShown = false;

    private CanvasGroup canvasGroup;
    private DataStorage dataStorage;
    private GameManager gameManager;

    private TextMeshProUGUI text;
    private Image img;

    public bool isShowing = false;
    private float showingStartedAt = -100;
    private float showingDuration = 0.15f;

    public bool isHiding = false;
    private float hidingStartedAt = -100;
    private float hidingDuration = 0.15f;

    void Start()
    {
        dataStorage = GameObject.Find("DataStorage").GetComponent<DataStorage>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        SetCanvasIsActive(false);

        text = gameObject.transform.Find("Canvas/Description").GetComponent<TextMeshProUGUI>();
        img = gameObject.transform.Find("Canvas/ItemImage").GetComponent<Image>();
    }

    void Update()
    {
        if (isShowing) {
            float progress = (Time.time - showingStartedAt) / showingDuration;
            if (progress <= 1) {
                canvasGroup.alpha = progress;
            } else {
                isShowing = false;
                SetCanvasIsActive(true);
            }
        }

        if (isHiding) {
            float progress = (Time.time - hidingStartedAt) / hidingDuration;
            if (progress <= 1) {
                canvasGroup.alpha = 1 - progress;
            } else {
                isHiding = false;
                SetCanvasIsActive(false);
            }
        }

        if (isShown) {
            if (Input.anyKeyDown) {
                Hide();
            }
        }
    }

    public void ShowPOI(string name, Sprite image) {
        text.text = dataStorage.poiDescriptions[name];

        if (image != null) {
            img.sprite = image;
            var c = img.color;
            c.a = 1;
            img.color = c;
        } else {
            var c = img.color;
            c.a = 0;
            img.color = c;
        }

        Show();
    }

    public void Show() {
        if (!isShowing && !isShown) {
            isShowing = true;
            showingStartedAt = Time.time;
        }
    }

    public void Hide() {
        if (!isHiding && isShown) {
            isHiding = true;
            hidingStartedAt = Time.time;
        }
    }

    void SetCanvasIsActive(bool isActive) {
        canvasGroup.alpha = isActive ? 1 : 0;
        isShown = isActive;
    }
}
