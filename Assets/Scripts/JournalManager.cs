using UnityEngine;
using UnityEngine.UI;

public class JournalManager : MonoBehaviour
{
    public int currentPage = 0;
    private int numberOfPages;

    private GameManager gameManager;
    private GameSceneManager gameSceneManager;

    private Vector3 evidenceScale = new Vector3(1.3f, 1.3f, 1);
    private Vector3 evidencePosition = new Vector3(0, -0.95f, 0);
    public int evidencePageNumber = 2;
    private bool isEvidenceSelected;
    private bool isZoomingInEvidence = false;
    private bool isZoomingOutEvidence = false;
    private float evidenceZoomStartedAt = -100;
    private float evidenceZoomDuration = 0.3f;

    private bool isSecretRoomShown = false;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameSceneManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();

        numberOfPages = gameObject.transform.childCount - 2;

        for (var i = 0; i < numberOfPages; ++i) {
            if (gameObject.transform.GetChild(i).gameObject.activeInHierarchy) {
                currentPage = i;
                break;
            }
        }

        isEvidenceSelected = currentPage == evidencePageNumber;
        UpdatePageVisibility();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            NextPage();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            PrevPage();
        }

        if (isZoomingInEvidence || isZoomingOutEvidence) {
            UpdateEvidenceZoom();
        }

        if (gameManager.isSecretRoomShown && !isSecretRoomShown) {
            isSecretRoomShown = true;
            gameObject.transform.Find("MapCanvas/SecretRoomImage").gameObject.SetActive(true);
            gameObject.transform.Find("MapCanvas/Map/Buttons/SecretRoomButton").GetComponent<Button>().interactable = true;
        }

        gameObject.transform.Find("Buttons/PrevButton").GetComponent<Button>().interactable = currentPage != 0;
        gameObject.transform.Find("Buttons/NextButton").GetComponent<Button>().interactable = currentPage != numberOfPages - 1;
    }

    public void NextPage() {
        if (isZoomingInEvidence || isZoomingOutEvidence || gameSceneManager.IsAnimating()) {
            return;
        }

        currentPage = Mathf.Clamp(currentPage + 1, 0, numberOfPages - 1);
        UpdatePageVisibility();
    }

    public void PrevPage() {
        if (isZoomingInEvidence || isZoomingOutEvidence || gameSceneManager.IsAnimating()) {
            return;
        }

        currentPage = Mathf.Clamp(currentPage - 1, 0, numberOfPages - 1);
        UpdatePageVisibility();
    }

    void UpdatePageVisibility() {
        for (int i = 0; i < numberOfPages; ++i) {
            gameObject.transform.GetChild(i).gameObject.SetActive(i == currentPage);
        }

        if (isEvidenceSelected || currentPage == evidencePageNumber) {
            AnimateEvidenceCanvas();
        }
    }

    void AnimateEvidenceCanvas() {
        if (isEvidenceSelected && currentPage != evidencePageNumber) {
            isZoomingOutEvidence = true;
            evidenceZoomStartedAt = Time.time;
        }
        if (!isEvidenceSelected && currentPage == evidencePageNumber) {
            isZoomingInEvidence = true;
            evidenceZoomStartedAt = Time.time;
        }
    }

    void UpdateEvidenceZoom() {
        float progress = (Time.time - evidenceZoomStartedAt) / evidenceZoomDuration;
        if (progress <= 1) {
            if (isZoomingOutEvidence) {
                progress = 1 - progress;
            }

            for (int i = 0; i < numberOfPages; ++i) {
                var obj = gameObject.transform.GetChild(i);
                obj.position = Vector3.Lerp(new Vector3(0, 0, 0), evidencePosition, progress);
                obj.localScale = Vector3.Lerp(new Vector3(1, 1, 1), evidenceScale, progress);
            }
            gameObject.transform.GetChild(gameObject.transform.childCount - 1).position = Vector3.Lerp(new Vector3(0, 0, 0), evidencePosition, progress);
            gameObject.transform.GetChild(gameObject.transform.childCount - 1).localScale = Vector3.Lerp(new Vector3(1, 1, 1), evidenceScale, progress);
        } else {
            for (int i = 0; i < numberOfPages; ++i) {
                var obj = gameObject.transform.GetChild(i);
                obj.position = isZoomingInEvidence ? evidencePosition : new Vector3(0, 0, 0);
                obj.localScale = isZoomingInEvidence ? evidenceScale : new Vector3(1, 1, 1);
            }
            gameObject.transform.GetChild(gameObject.transform.childCount - 1).position = isZoomingInEvidence ? evidencePosition : new Vector3(0, 0, 0);
            gameObject.transform.GetChild(gameObject.transform.childCount - 1).localScale = isZoomingInEvidence ? evidenceScale : new Vector3(1, 1, 1);

            isEvidenceSelected = isZoomingInEvidence;
            isZoomingInEvidence = false;
            isZoomingOutEvidence = false;
        }
    }

    public bool IsAnimating() {
        return isZoomingInEvidence || isZoomingOutEvidence;
    }
}
