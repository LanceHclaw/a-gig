using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EvidenceManager : MonoBehaviour
{
    public GameObject polaroidPhotoPrefab;
    public GameObject threadPrefab;
    public GameObject threadDescriptionTextPrefab;

    private bool isInit = false;

    private float xmin = -5.6f;
    private float ymin = -3.25f;
    private float xmax = 5.6f;
    private float ymax = 3.25f;

    private DataStorage dataStorage;
    private GameSceneManager gameSceneManager;
    private JournalManager journalManager;
    private TooltipManager tooltipManager;
    private GameObject choiceDialog;
    private FlashManager flashManager;
    private EvidenceCollectedController evidenceCollectedController;

    private Vector3 prevMousePosition = new Vector3(0, 0, 0);
    private float prevMousePositionRecordedAt = -100;
    private float stationaryDuration = 0.2f;
    private string currentlyShowingTooltipFor = "none";

    private GameObject photos;
    private GameObject threads;
    private GameObject threadDescriptions;

    private bool isHoldingPhoto = false;
    private bool isHoldingThread = false;
    private Vector3 currentlyHoldingInitialScale;
    private GameObject currentlyHolding = null;
    private string currentlyHoldingName = null;
    private GameObject currentThread = null;
    private string currentThreadComingFrom = null;

    private float threadDeletionThreshold = 0.1f;

    private bool isThreadConnectionPending = false;
    private GameObject connectionPendingTo;
    private string connectionPendingToName;
    private string connectionPendingDescriptionKey;
    private int connectionPendingDescriptionIdx;

    void Start()
    {
        Init();
    }

    void Init() {
        if (isInit) {
            return;
        }
        isInit = true;

        dataStorage = GameObject.Find("DataStorage").GetComponent<DataStorage>();
        gameSceneManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        journalManager = GameObject.Find("UI/Journal").GetComponent<JournalManager>();
        tooltipManager = GameObject.Find("UI/Journal/PopupCanvas/Tooltip").GetComponent<TooltipManager>();
        choiceDialog = GameObject.Find("UI/Journal/PopupCanvas/ChoiceDialog");
        flashManager = GameObject.Find("UI/WhiteScreen").GetComponent<FlashManager>();
        evidenceCollectedController = GameObject.Find("UI/EvidenceCollected").GetComponent<EvidenceCollectedController>();

        photos = gameObject.transform.Find("Photos").gameObject;
        threads = gameObject.transform.Find("Threads").gameObject;
        threadDescriptions = gameObject.transform.Find("ThreadDescriptions").gameObject;
    }

    void Update()
    {
        if (!gameSceneManager.isJournalOpen || journalManager.currentPage != journalManager.evidencePageNumber) {
            return;
        }

        if (isThreadConnectionPending) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            var (photo, thread, name) = GetObjectUnderCursor(true);
            if (photo) {
                currentlyHolding = photo;
                currentlyHoldingInitialScale = currentlyHolding.transform.localScale;
                currentlyHoldingName = name;
                isHoldingPhoto = true;

                currentlyHolding.transform.localScale = currentlyHoldingInitialScale * 1.1f;
            } else if (thread) {
                currentThread = thread;
                currentThreadComingFrom = name;
                isHoldingThread = true;
            } else {
                DeleteThreadIfClicked();
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            if (isHoldingPhoto && currentlyHolding != null) {
                currentlyHolding.transform.localScale = currentlyHoldingInitialScale;
                UpdateThreadsFor(currentlyHoldingName, currentlyHolding);
            }
            if (isHoldingThread && currentThread != null) {
                TryConnectThread();
            }

            isHoldingPhoto = false;
            isHoldingThread = false;
        }

        if (Input.GetMouseButton(0)) {
            if (isHoldingPhoto && currentlyHolding != null) {
                MoveHeldPhoto();
            }

            if (isHoldingThread && currentThread != null) {
                UpdateThreadPosition();
            }

            tooltipManager.Hide();
        } else {
            var (type, picName, (threadKey, threadIdx, threadOpt)) = GetObjectNameUnderCursor();
            if (IsMouseStationary() && !tooltipManager.IsShown()) {
                if (type == "photo") {
                    tooltipManager.ShowPhoto(picName);
                }
                if (type == "thread") {
                    tooltipManager.ShowThread(threadKey, threadIdx, threadOpt);
                }
            }

            if (tooltipManager.IsShown()) {
                if (type == "none" || (currentlyShowingTooltipFor != "none" && type != currentlyShowingTooltipFor)) {
                    tooltipManager.Hide();
                }
            }
            currentlyShowingTooltipFor = type;
        }
    }

    public void AddEvidence(string name) {
        Init();

        var pictureInstance = Instantiate(polaroidPhotoPrefab);
        pictureInstance.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = name;
        pictureInstance.transform.Find("ItemImage").GetComponent<Image>().sprite = dataStorage.spriteByName[name];
        pictureInstance.transform.SetParent(photos.transform, false);

        pictureInstance.transform.localPosition = new Vector3(
            // xmin + polaroidPhotoPrefab.GetComponent<RectTransform>().rect.width / 2 + (collectedEvidence.Count * 2.1f),
            // ymax - polaroidPhotoPrefab.GetComponent<RectTransform>().rect.height / 2 - 2 - (collectedEvidence.Count * 0.1f),
            xmin + polaroidPhotoPrefab.GetComponent<RectTransform>().rect.width / 2 + 1.2f + (dataStorage.collectedEvidence.Count * 0.1f),
            ymax - polaroidPhotoPrefab.GetComponent<RectTransform>().rect.height / 2 - (dataStorage.collectedEvidence.Count * 0.1f),
            0
        );

        dataStorage.collectedEvidence.Add((name, pictureInstance));
    }

    (GameObject, GameObject, string) GetObjectUnderCursor(bool createThreads) {
        for (int i = dataStorage.collectedEvidence.Count - 1; i >= 0; --i) {
            var (name, photo) = dataStorage.collectedEvidence[i];

            var photoPos = photo.transform.position;
            var photoRect = photo.GetComponent<RectTransform>().rect;
            var topLeft = photoPos - new Vector3(photoRect.width / 2, photoRect.height / 2, 0);
            var bottomRight = photoPos + new Vector3(photoRect.width / 2, photoRect.height / 2, 0);

            var pinPos = photo.transform.Find("PinArea").transform.position;
            var pinRect = photo.transform.Find("PinArea").GetComponent<RectTransform>().rect;
            var pinTopLeft = pinPos - new Vector3(pinRect.width / 2, pinRect.height / 2, 0);
            var pinBottomRight = pinPos + new Vector3(pinRect.width / 2, pinRect.height / 2, 0);

            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (pinTopLeft.x <= mousePos.x && mousePos.x <= pinBottomRight.x && pinTopLeft.y <= mousePos.y && mousePos.y <= pinBottomRight.y) {
                if (!createThreads) {
                    return (photo, null, name);
                }

                var newThread = Instantiate(threadPrefab);
                newThread.GetComponent<LineRenderer>().SetPosition(0, pinPos);
                newThread.transform.SetParent(threads.transform, true);

                return (null, newThread, name);
            }

            if (topLeft.x <= mousePos.x && mousePos.x <= bottomRight.x && topLeft.y <= mousePos.y && mousePos.y <= bottomRight.y) {
                return (photo, null, name);
            }
        }

        return (null, null, null);
    }

    void MoveHeldPhoto() {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var photoWidth = currentlyHolding.GetComponent<RectTransform>().rect.width;
        var photoHeight = currentlyHolding.GetComponent<RectTransform>().rect.height;

        currentlyHolding.transform.position = new Vector3(
            Mathf.Clamp(mousePos.x, xmin + photoWidth / 2, xmax - photoWidth / 2),
            Mathf.Clamp(mousePos.y, ymin + photoHeight / 2, ymax - photoHeight / 2),
            currentlyHolding.transform.position.z
        );

        UpdateThreadsFor(currentlyHoldingName, currentlyHolding);
    }

    void UpdateThreadPosition() {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        currentThread.GetComponent<LineRenderer>().SetPosition(1, new Vector3(
            Mathf.Clamp(mousePos.x, xmin, xmax),
            Mathf.Clamp(mousePos.y, ymin, ymax),
            currentThread.transform.position.z
        ));
    }

    void TryConnectThread() {
        var (photo, _, name) = GetObjectUnderCursor(false);
        if (photo && currentThreadComingFrom != name) {
            foreach (var (from, to, _, _, _, _, _) in dataStorage.connectedThreads) {
                if (from == currentThreadComingFrom && to == name) {
                    Destroy(currentThread);
                    return;
                }
            }

            var doesDescriptionExist = false;
            connectionPendingTo = photo;
            connectionPendingToName = name;
            if (dataStorage.connectionDescriptions.ContainsKey(currentThreadComingFrom)) {
                for (int i = 0; i < dataStorage.connectionDescriptions[currentThreadComingFrom].Count; ++i) {
                    var (_to, _) = dataStorage.connectionDescriptions[currentThreadComingFrom][i];
                    if (_to == connectionPendingToName) {
                        connectionPendingDescriptionKey = currentThreadComingFrom;
                        connectionPendingDescriptionIdx = i;
                        doesDescriptionExist = true;
                        break;
                    }
                }
            }
            if (dataStorage.connectionDescriptions.ContainsKey(name)) {
                for (int i = 0; i < dataStorage.connectionDescriptions[name].Count; ++i) {
                    var (_to, _) = dataStorage.connectionDescriptions[name][i];
                    if (_to == currentThreadComingFrom) {
                        connectionPendingDescriptionKey = name;
                        connectionPendingDescriptionIdx = i;
                        doesDescriptionExist = true;
                        break;
                    }
                }
            }

            if (doesDescriptionExist) {
                isThreadConnectionPending = true;
                var cdm = choiceDialog.GetComponent<ChoiceDialogManager>();
                cdm.description = dataStorage.connectionDescriptions[connectionPendingDescriptionKey][connectionPendingDescriptionIdx].Item2.Item1;
                cdm.leftName = dataStorage.connectionDescriptions[connectionPendingDescriptionKey][connectionPendingDescriptionIdx].Item1;
                cdm.leftImage = dataStorage.spriteByName[cdm.leftName];
                cdm.rightName = connectionPendingDescriptionKey;
                cdm.rightImage = dataStorage.spriteByName[cdm.rightName];
                cdm.options = new List<string>();
                foreach (var (option, _) in dataStorage.connectionDescriptions[connectionPendingDescriptionKey][connectionPendingDescriptionIdx].Item2.Item2) {
                    cdm.options.Add(option);
                }
                choiceDialog.SetActive(true);
                cdm.Reset();
            } else {
                FinishConnectingThread(-1, false);
            }

            return;
        }

        Destroy(currentThread);
    }

    public void FinishConnectingThread(int optionNumber, bool doesMakeSense = true) {
        currentThread.GetComponent<LineRenderer>().SetPosition(1, connectionPendingTo.transform.Find("PinArea").position);

        bool shouldSwapDirection = SwapThreadPositionsIfNeeded(currentThread);

        var threadDescription = Instantiate(threadDescriptionTextPrefab);
        threadDescription.transform.SetParent(threadDescriptions.transform, true);

        string threadDescriptionText = "I see no connection";
        if (doesMakeSense) {
            threadDescriptionText = dataStorage.connectionDescriptions[connectionPendingDescriptionKey][connectionPendingDescriptionIdx].Item2.Item2[optionNumber].Item2;
        }
        threadDescription.GetComponent<TextMeshProUGUI>().text = "\n" + threadDescriptionText;

        AlignTextToThread(threadDescription, currentThread);

        if (shouldSwapDirection) {
            dataStorage.connectedThreads.Add((connectionPendingToName, currentThreadComingFrom, currentThread, threadDescription, doesMakeSense ? connectionPendingDescriptionKey : null, connectionPendingDescriptionIdx, optionNumber));
        } else {
            dataStorage.connectedThreads.Add((currentThreadComingFrom, connectionPendingToName, currentThread, threadDescription, doesMakeSense ? connectionPendingDescriptionKey : null, connectionPendingDescriptionIdx, optionNumber));
        }

        isThreadConnectionPending = false;

        if ((connectionPendingToName == "Knife" && currentThreadComingFrom == ".44 Magnum") || (connectionPendingToName == ".44 Magnum" && currentThreadComingFrom == "Knife")) {
            flashManager.PhotoFlash();
            evidenceCollectedController.ShowPhoto("Bullet in the Body");
            AddEvidence("Bullet in the Body");
        }
    }

    public void CancelConnectingThread() {
        Destroy(currentThread);
        isThreadConnectionPending = false;
    }

    void UpdateThreadsFor(string name, GameObject photo) {
        var newPosition = photo.transform.Find("PinArea").position;

        for (int i = 0; i < dataStorage.connectedThreads.Count; ++i) {
            var (from, to, thread, threadDescription, key, idx, opt) = dataStorage.connectedThreads[i];
            if (from == name || to == name) {
                if (from == name) {
                    thread.GetComponent<LineRenderer>().SetPosition(0, newPosition);
                }
                if (to == name) {
                    thread.GetComponent<LineRenderer>().SetPosition(1, newPosition);
                }

                bool shouldSwapDirection = SwapThreadPositionsIfNeeded(thread);
                if (shouldSwapDirection) {
                    dataStorage.connectedThreads[i] = (to, from, thread, threadDescription, key, idx, opt);
                }

                AlignTextToThread(threadDescription, thread);
            }
        }
    }

    bool SwapThreadPositionsIfNeeded(GameObject thread) {
        var lr = thread.GetComponent<LineRenderer>();

        bool shouldSwapDirection = lr.GetPosition(0).x >= lr.GetPosition(1).x;

        if (shouldSwapDirection) {
            var tmp = lr.GetPosition(0);
            lr.SetPosition(0, lr.GetPosition(1));
            lr.SetPosition(1, tmp);
        }

        return shouldSwapDirection;
    }

    void DeleteThreadIfClicked() {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        for (int i = 0; i < dataStorage.connectedThreads.Count; ++i) {
            var (from, to, thread, threadDescription, _, _, _) = dataStorage.connectedThreads[i];
            if (IsThreadInRange(mousePos, thread)) {
                Destroy(thread);
                Destroy(threadDescription);
                dataStorage.connectedThreads.RemoveAt(i);
                break;
            }
        }
    }

    bool IsThreadInRange(Vector2 point, GameObject thread) {
        var P1 = thread.GetComponent<LineRenderer>().GetPosition(0);
        var P2 = thread.GetComponent<LineRenderer>().GetPosition(1);

        var distance = (
            Mathf.Abs((P2.y - P1.y) * point.x - (P2.x - P1.x) * point.y + P2.x * P1.y - P2.y * P1.x)
            /
            Mathf.Sqrt(Mathf.Pow(P2.y - P1.y, 2) + Mathf.Pow(P2.x - P1.x, 2))
        );

        return distance <= threadDeletionThreshold;
    }

    void AlignTextToThread(GameObject text, GameObject thread) {
        var lr = thread.GetComponent<LineRenderer>();
        var p1 = lr.GetPosition(0);
        var p2 = lr.GetPosition(1);

        var threadLength = Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));

        var tRect = text.GetComponent<RectTransform>();
        tRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, threadLength / tRect.localScale.x);

        text.transform.position = new Vector3(
            p1.x + (p2.x - p1.x) / 2,
            p1.y + (p2.y - p1.y) / 2,
            text.transform.position.z
        );

        text.transform.eulerAngles = new Vector3(
            0,
            0,
            Mathf.Atan2(p2.y - p1.y, p2.x - p1.x) / Mathf.PI * 180
        );
    }

    bool IsMouseStationary() {
        if (Time.time - prevMousePositionRecordedAt > stationaryDuration) {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if ((mousePos - prevMousePosition).magnitude < 0.1) {
                return true;
            }

            prevMousePositionRecordedAt = Time.time;
            prevMousePosition = mousePos;
        }

        return false;
    }

    (string, string, (string, int, int)) GetObjectNameUnderCursor() {
        var (photo, _, name) = GetObjectUnderCursor(false);
        if (photo) {
            return ("photo", name, ("", 0, 0));
        }

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (var (_, _, thread, _, key, idx, opt) in dataStorage.connectedThreads) {
            if (key != null && IsThreadInRange(mousePos, thread)) {
                return ("thread", "", (key, idx, opt));
            }
        }

        return ("none", "", ("", 0, 0));
    }
}
