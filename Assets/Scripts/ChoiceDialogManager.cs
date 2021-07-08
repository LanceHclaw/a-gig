using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ChoiceDialogManager : MonoBehaviour
{
    public string description = "";

    public List<Option> options;

    public Sprite leftImage;
    public Sprite rightImage;
    public string leftName;
    public string rightName;

    private GameObject leftItem;
    private GameObject rightItem;

    private EvidenceManager evidenceManager;

    private GameObject descriptionContainer;
    private GameObject choicesContainer;

    private MainQuestManager mqManager;

    private int selectedOption = 0;

    void Start()
    {
        Reset();
    }

    public void Reset() {
        mqManager = GameObject.Find("GameManager").GetComponent<MainQuestManager>();

        evidenceManager = GameObject.Find("UI/Journal/EvidenceCanvas/Evidence").GetComponent<EvidenceManager>();
        descriptionContainer = gameObject.transform.Find("Description").gameObject;
        choicesContainer = gameObject.transform.Find("Choices").gameObject;

        leftItem = gameObject.transform.Find("LeftPhoto").gameObject;
        leftItem.transform.Find("ItemImage").GetComponent<Image>().sprite = leftImage;
        leftItem.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = leftName;
        rightItem = gameObject.transform.Find("RightPhoto").gameObject;
        rightItem.transform.Find("ItemImage").GetComponent<Image>().sprite = rightImage;
        rightItem.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = rightName;

        if (description != "") {
            descriptionContainer.SetActive(true);
            descriptionContainer.GetComponent<TextMeshProUGUI>().text = description;
            choicesContainer.transform.localPosition = new Vector3(0, -0.08f, 0);
        } else {
            descriptionContainer.SetActive(false);
            choicesContainer.transform.localPosition = new Vector3(0, 0.6f, 0);
        }

        for (var i = 0; i < choicesContainer.transform.childCount; ++i) {
            choicesContainer.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (var i = 0; i < options.Count; ++i) {
            var option = choicesContainer.transform.GetChild(i).gameObject;
            option.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = options[i].choiceDescription;
            option.SetActive(true);
        }

        selectedOption = 0;
        UpdateAlphas();
    }

    public void Setup(Connection connection, Evidence leftEv, Evidence rightEv)
    {
        description = connection.commonDescription;

        leftName = leftEv.name;
        leftImage = leftEv.sprite;

        rightName = rightEv.name;
        rightImage = rightEv.sprite;

        options = connection.options.Select(x => x.Key).ToList();
    }

    public void OnClick(int option) {
        selectedOption = option;
        UpdateAlphas();
    }

    public void OnConfirm() {
        evidenceManager.FinishConnectingThread(selectedOption, option: options[selectedOption]);
        gameObject.SetActive(false);
    }

    public void OnCancel() {
        evidenceManager.CancelConnectingThread();
        gameObject.SetActive(false);
    }

    void UpdateAlphas() {
        for (var i = 0; i < choicesContainer.transform.childCount; ++i) {
            UpdateButtonNormalAlpha(i, i == selectedOption ? 1 : 0.4f);
        }
    }

    void UpdateButtonNormalAlpha(int i, float alpha) {
        var c = choicesContainer.transform.GetChild(i).GetComponent<Button>().colors;
        var cn = c.normalColor;
        cn.a = alpha;
        c.normalColor = cn;
        choicesContainer.transform.GetChild(i).GetComponent<Button>().colors = c;
    }
}
