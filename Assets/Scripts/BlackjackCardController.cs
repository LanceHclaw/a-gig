using UnityEngine;
using UnityEngine.UI;

public class BlackjackCardController : MonoBehaviour
{
    public int numericValue;

    private BlackjackManager blackjackManager;

    private Vector3 initialScale;

    private Vector3 initialPosition;
    private Vector3 oldPosition;
    private Vector3 slotPosition;

    private bool isHeld = false;

    private bool isInSlot = false;
    private int slotIdx = -1;

    private bool isMovingBack = false;
    private float movingBackStartedAt = -100;
    private float movingBackDuration = 0.1f;

    private bool isMovingToSlot = false;
    private float movingToSlotStartedAt = -100;
    private float movingToSlotDuration = 0.3f;

    void Start()
    {
        blackjackManager = GameObject.Find("Rooms/Blackjack").GetComponent<BlackjackManager>();
        initialPosition = gameObject.transform.position;
        initialScale = gameObject.transform.localScale;
    }

    void Update()
    {
        if (isMovingToSlot) {
            float progress = (Time.time - movingToSlotStartedAt) / movingToSlotDuration;
            if (progress <= 1) {
                gameObject.transform.position = Vector3.Lerp(oldPosition, slotPosition, progress);
            } else {
                gameObject.transform.position = slotPosition;
                isMovingToSlot = false;
            }
            return;
        }

        if (isMovingBack) {
            float progress = (Time.time - movingBackStartedAt) / movingBackDuration;
            if (progress <= 1) {
                gameObject.transform.position = Vector3.Lerp(oldPosition, initialPosition, progress);
            } else {
                gameObject.transform.position = initialPosition;
                isMovingBack = false;
            }
            return;
        }

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)) {
            var crect = gameObject.GetComponent<RectTransform>().rect;
            var topLeft = gameObject.transform.position - new Vector3(crect.width / 2, crect.height / 2, 0);
            var bottomRight = gameObject.transform.position + new Vector3(crect.width / 2, crect.height / 2, 0);

            if (topLeft.x <= mousePos.x && mousePos.x <= bottomRight.x && topLeft.y <= mousePos.y && mousePos.y <= bottomRight.y) { 
                gameObject.transform.localScale = initialScale * 1.05f;
                isHeld = true;
            }
        }
        if (Input.GetMouseButtonUp(0) && isHeld) {
            gameObject.transform.localScale = initialScale;
            isHeld = false;

            var (isSlotAligned, idx, sPosition) = blackjackManager.AlignToSlotIfExists(numericValue, gameObject.transform.position);
            if (isSlotAligned) {
                if (isInSlot)
                {
                    blackjackManager.RemoveFromSlot(slotIdx);
                    isInSlot = false;
                }
                isInSlot = true;
                slotIdx = idx;
                isMovingToSlot = true;
                movingToSlotStartedAt = Time.time;
                oldPosition = gameObject.transform.position;
                slotPosition = new Vector3(sPosition.x, sPosition.y, gameObject.transform.position.z);
            } else {
                isMovingBack = true;
                movingBackStartedAt = Time.time;
                oldPosition = gameObject.transform.position;
                if (isInSlot) {
                    blackjackManager.RemoveFromSlot(slotIdx);
                    isInSlot = false;
                }
            }
        }

        if (Input.GetMouseButton(0) && isHeld) {
            gameObject.transform.position = new Vector3(mousePos.x, mousePos.y, gameObject.transform.position.z);
        }
    }
}
