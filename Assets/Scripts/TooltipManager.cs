using UnityEngine;
using TMPro;
using System.Linq;

public class TooltipManager : MonoBehaviour
{
    private bool isShowingItem = false;
    private bool isShowingConnection = false;

    private TextMeshProUGUI text;

    private DataStorage dataStorage;

    void Start()
    {
        dataStorage = GameObject.Find("DataStorage").GetComponent<DataStorage>();
        text = gameObject.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        Hide();
    }

    void Update()
    {
        if (isShowingItem || isShowingConnection) {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameObject.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }
    }

    public void ShowPhoto(string name) {
        isShowingItem = true;
        text.text = dataStorage.evidenceDescriptions[name];
    }

    public void ShowThread(Connection connection) {
        var choiceDesc = connection.options.Where(x => x.Value == true).Select(x => x.Key).First().choiceDescription;
        isShowingConnection = true;
        if (connection.commonDescription != "") {
            text.text = connection.commonDescription + "\n" + choiceDesc;
        } else {
            text.text = choiceDesc;
        }
    }

    public void Hide() {
        isShowingItem = false;
        isShowingConnection = false;
        gameObject.transform.position = new Vector3(1000, 0, 0);
    }

    public bool IsShown() {
        return isShowingItem || isShowingConnection;
    }
}
