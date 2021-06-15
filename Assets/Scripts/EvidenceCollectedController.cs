using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EvidenceCollectedController : MonoBehaviour
{
    public bool isShown = false;

    private CanvasGroup canvasGroup;
    private DataStorage dataStorage;
    private GameManager gameManager;

    private TextMeshProUGUI text;
    private Image img;
    private TextMeshProUGUI title;

    public bool showPending = false;
    private float showPendingStartedAt = -100;
    private float showPendingDuration;
    private string pendingName;

    public bool isHiding = false;
    private float hidingStartedAt = -100;
    private float hidingDuration = 0.3f;

    void Start()
    {
        dataStorage = GameObject.Find("DataStorage").GetComponent<DataStorage>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        SetCanvasIsActive(false);

        text = gameObject.transform.Find("Canvas/Description").GetComponent<TextMeshProUGUI>();
        title = gameObject.transform.Find("Canvas/PolaroidPhoto/Text").GetComponent<TextMeshProUGUI>();
        img = gameObject.transform.Find("Canvas/PolaroidPhoto/ItemImage").GetComponent<Image>();
    }

    void Update()
    {
        if (showPending && (Time.time - showPendingStartedAt) > showPendingDuration) {
            ShowPhoto(pendingName, 0);
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

    public void ShowPhoto(string name, float delay = 0.11f) {
        if (delay == 0) {
            showPending = false;

            SetCanvasIsActive(true);

            text.text = dataStorage.evidenceDescriptions[name];
            title.text = name;
            img.sprite = dataStorage.spriteByName[name];
        } else {
            showPending = true;
            showPendingStartedAt = Time.time;
            showPendingDuration = delay;
            pendingName = name;
        }
    }

    public void Hide() {
        if (!isHiding) {
            isHiding = true;
            hidingStartedAt = Time.time;
        }
    }

    void SetCanvasIsActive(bool isActive) {
        canvasGroup.alpha = isActive ? 1 : 0;
        canvasGroup.interactable = isActive;
        canvasGroup.blocksRaycasts = isActive;
        isShown = isActive;
        isHiding = false;
    }
}
