using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProgressionVector;
using TMPro;
using UnityEngine.SceneManagement;

public struct EndingItem {
    public List<string> required { get; set; }
    public List<string> banned { get; set; }
    public List<string> optional { get; set; }
    public int optionalMinAmount { get; set; }
    public List<(string, string)> pairs { get; set; }
    public string epilogueText;

    public EndingItem(List<string> _required, List<string> _optional, int _optionalMinAmount, List<string> _banned, List<(string, string)> _pairs, string _epilogueText) {
        required = _required;
        banned = _banned;
        optional = _optional;
        optionalMinAmount = _optionalMinAmount;
        pairs = _pairs;
        epilogueText = _epilogueText;
    }
}

public class EpilogueManager : MonoBehaviour
{
    private string selectedText;

    /*private List<EndingItem> endings = new List<EndingItem>() {
        new EndingItem(  // Ending 1 (Correct - Murderer is Officer Davis)
            new List<string>() { "B12/2", "C13/1", "B14" },
            new List<string>() { "B4/2", "B5/2", "B9/2", "C5/2", "D5", "E7", "E9/2", "E12/2", "F11/2", "F14/1", "G8/2", "G14", "H9/2", "L14/1" },
            6,
            new List<string>() { "F7", "F14/2", "I14", "B9/1", "E12/1" },
            new List<(string, string)>(),
            "\"I looked through what I found and through what I was told when coming here. The victim was shot with a .44 caliber, which is a common weapon. The knife, however, was smartly used to hide the bullet wound presumably after the victim had died. $$The murderer is a knowledgeable person, so there are no fingerprints anywhere. However, the murderer was also looking for something here, and it were, in fact, the notes on the last case detective Allen was assigned. $$The murderer got inside the hidden room knowing about Frank Allen's blackjack hobby, which means they knew him well. Moreover, the personal gun, which was also hidden from a random visitor, was recently used, and placed back in its place. Only a close friend would know how to do all that. $$The only close friend of Mr Allen's that was near the accident is Officer Davis. Add to that my confusion about the urgency and the eagerness that Officer Davis expressed to be done with this case, I suspect, and, frankly, accuse your very own subordinate, sir. I believe, Officer Davis committed this murder and tried to cover it.\"$$Johnson: \"Arrest her!\"$$$Everyone creates their own conspiracy theories.[$Some of them[ happen to be correct.$"
        ),
        new EndingItem(  // Ending 2 (Murderer - neighbour, knife)
            new List<string>() { "B10/1", "E9/1", "E10", "F14/2", "G8/1", "I14" },
            new List<string>() { "B5/1", "C5/1", "H9/2", "L14/2" },
            2,
            new List<string>() { "C13/1", "E12/1", "E12/2", "L14/1" },
            new List<(string, string)>(),
            "\"Captain Johnson, upon my inspection I uncovered most if not all the details Officer Davis was talking about. The victim was murdered with a knife in the kitchen. The gunshot that was reported most likely was Mr Allen practicing as always, right before the murderer snuck into the room through the window. $$Mr Allen seems to have had a very strange sense of cleanliness, thus the floor was clean. The murderer, however, did not wear shoes, which explains why no dirt was found on site. Moreover, I believe, the victim dropped a cigarette when the blow was dealt. One might think the fire would clear all the evidence, but the murderer made sure it didn’t happen. Why? I think because the murderer lives in this same house and didn’t want to deal with moving places.$$All of that combined with what Officer Davis told me about personal grudges between the suspect and the victim leads me to believe it was indeed done by sir Adam Moore - the neighbour who got fed up with detective Allen’s shooting and decided to end it for good.\"[$$Johnson: \"Alright. Thanks for your help, detective. And thanks for your expediency, officer. Dismissed.\"[[$$$Everyone creates their own conspiracy theories.[$Some of them[ happen to be misled.$"
        ),
        new EndingItem(  // Ending 3 (Murderer - neightbour, pistol)
            new List<string>() { "B5/2", "B10/1", "B12/2", "E12/2", "F14/2", "G8/1", "L14/2" },
            new List<string>() { "B4/2", "B9/1", "B14", "C5/1", "C6", "D5", "E9/1", "E10", "F11/2", "G14", "H9/1", "H9/2", "I14" },
            4,
            new List<string>() { "B9/2", "C13/1" },
            new List<(string, string)>(),
            "\"Here’s a thing, Captain. I uncovered most if not all the details Officer Davis was talking about, however, there is more. The murder was committed not with a knife, but instead - with a pistol. In particular, most likely it was Mr Allen’s own AutoMag with .44 caliber bullets. The knife was stupidly used later to cover the bullet wound.$$I assume, the murderer must have been lucky to sneak into the room through the window at the right time. First of all - the lack of fingerprints must be due to the fact that the weather has been freezing and the murderer was wearing gloves. Secondly, Mr Allen was, most likely, practicing with his weapon again, which must have caused Mr Moore’s patience to end. When he got inside, Frank Allen took a break, which gave Mr Moore time to grab his weapon and deliver the lethal shot.$$That explains the panicky knife placement, the lack of footsteps, the fact that the house was not burned because of a left or dropped cigarette and it also follows what officer Davis told me about the personal grudges. So, I conclude that Mr Adam Moore indeed committed homicide against detective Allen.\"[$$Johnson: \"Hmm.. Can it really be luck? Well, it ends for him today then. Thanks for your help. I will see to your payment.\"[[$$*10pm same day, home*[[][[[[[#[[[$$Everyone creates their own conspiracy theories.[$Some of them[ can be dangerous if they go too far and take a wrong turn.$"
        ),
        new EndingItem(  // Ending 4 (Frank Allen killed himself)
            new List<string>() { "B10/1", "B12/1", "C13/2", "E10", "E12/1" },
            new List<string>() { "B4/1", "B5/1", "C5/1", "C6", "D6", "C13/2", "C13/3", "D11", "E7", "F9", "F11/2", "H9/1", "H9/2" },
            4,
            new List<string>() { "B4/2", "B5/2", "B9/2", "F7", "F14/1", "G8/2" },
            new List<(string, string)>(),
            "\"It is going to be hard to believe, Captain, but from what I discovered, it actually is a pretty sad story. I don’t believe the suspect is guilty. In fact, I don’t believe anyone is to blame for what happened, as I found out that it most likely was suicide.$$I looked through all the victim’s belongings and discovered that he had a very strange behaviour as of late. In particular, he showed a very uneven house cleaning pattern where half the rooms are a mess and the other - shining clean. $$He had been smoking a great deal lately. Both in the living room and in his bedroom, which leads me to believe that he tended towards being apathetic and unmotivated most days, spending them at home, shooting, smoking and drinking a lot, while trying to deal with his neighbour and work. He even almost burned himself alive with a cigarette once, which a burnt pillow and a pack of cigarettes in his bedroom are a good evidence.$$Finally, I discovered a letter he was trying to write to you, where he explained why he was being late on the closure of a case you assigned to him. And I think it was when he broke and decided to solve the problems another way. $$The rest of the evidence is irrelevant and could have been manipulated by the police squad that came here earlier. If anything, I can only blame the suspect for being one drop in the waterfall of problems detective Allen was trying to solve. Such is my conclusion.\"[$$Johnson: \"I.. I don’t know what to say. I sure did not expect that outcome. But I thank you for the investigation, detective. We will see to your payment.\"[[$$$Everyone creates their own conspiracy theories.[$Some of them[ can be scarier than we expect, but not necessarily truthful.$"
        ),
        new EndingItem(  // Ending 5 (Contradictions)
            new List<string>() { "B12/1", "E12/1" },
            new List<string>() { "B4/2", "B5/2", "B9/2", "B12/2", "C13/1", "E9/2", "E12/2", "F14/1", "F14/2", "I14", "L14/1", "L14/2" },
            2,
            new List<string>(),
            new List<(string, string)>() { ("B10/2", "B12/1"), ("B10/2", "C5/1"), ("B10/2", "E10"), ("C5/2", "C13/2") },
            "\"It is hard to explain, Captain, but I found evidence of both another person being there and the murder not being a murder itself...\"$You proceed to present all evidence and your conclusion with a strong belief in its correctness, overcoming all inconsistencies.[$$Johnson: \"Do you even hear yourself, detective? There was a murderer but they didn’t commit a murder? Go get some rest. I will take over.\"[[$$$Everyone creates their own conspiracy theories.[$Some of them[ make no sense, yet we firmly believe in their truthfulness.$"
        ),
        new EndingItem(  // Ending 7.1 Heart attack
            new List<string>(),
            new List<string>() { "C6", "D6", "D11", "F11/2", "H9/1", "H9/2" },
            2,
            new List<string>(),
            new List<(string, string)>(),
            "\"I believe, Captain, that the victim was not murdered, but rather died due to a sort of natural cause. What I mean by that is that I’ve found evidence of extreme amounts of smoking inside the house with little to no ventilation available. The victim must have experienced a heart attack caused by the overdose of nicotine, and as such, collapsed in the kitchen while holding a knife, which ended his life before the seizure could.\"[$$*The captain does not respond, but calls an ambulance and you are being taken to a mental hospital.*[[$$$Everyone creates their own conspiracy theories.[$Some of them[ are not welcome by others.$"
        ),
        new EndingItem(  // Ending 7.2 Slipped up
            new List<string>(),
            new List<string>() { "G8/1", "G8/2", "E7", "G14", "H9/1", "H9/2", "I11" },
            3,
            new List<string>(),
            new List<(string, string)>(),
            "\"Captain, I have a firm belief that this entire crime was not a crime but an accident. Detective Allen was obsessed with mopping the floor in the kitchen. However - ONLY in the kitchen. Since all other rooms are a mess, I am confident that he forgot that the floor was slippery due to a recent clean-up session conducted, and as such he slipped up while holding a knife and impaled himself lethally.\"$$[*The captain does not respond, but calls an ambulance and you are being taken to a mental hospital.*[[$$$Everyone creates their own conspiracy theories.[$Some of them[ are not welcome by others.$"
        ),
        new EndingItem(  // Ending 7.3 WTF?
            new List<string>(),
            new List<string>() { "B10/1", "B12/1", "C13/2", "E10", "E12/1", "B4/1", "B5/1", "C5/1", "C6", "D6", "C13/2", "C13/3", "D11", "E7", "F9", "F11/2", "H9/1", "H9/2" },
            4,
            new List<string>(),
            new List<(string, string)>(),
            "\"The case is much more complex than I thought, Captain Johnson. Frank Allen was found to have self-harming behaviours and it leads me to believe that he attempted a suicide in the middle of writing something, and shot himself with his own pistol. However, immediately after that the actual murderer rushed in and stabbed him with a knife!\"$$[*The captain does not respond, but calls an ambulance and you are being taken to a mental hospital.*[[$$$Everyone creates their own conspiracy theories.[$Some of them[ are not welcome by others.$"
        ),
        new EndingItem(
            new List<string>(),
            new List<string>(),
            0,
            new List<string>(),
            new List<(string, string)>(),
            "\"Captain, I must confess - I am so utterly confused at what happened here that I believe it was me all along who killed Mr Frank Allen. I willingly ask you to arrest me please. I can’t dig in that anymore. Just.. just let me get the hell out of here even if the destination is prison!\"[$$*The captain does not respond, but calls an ambulance and you are being taken to a mental hospital.*[[$$$Everyone creates their own conspiracy theories.[$Some of them[ are not welcome by others.$"
        )
    };*/

    private bool isFadingIn = false;
    private float fadeInStartedAt = -100;
    private float fadeInDuration = 0.3f;

    private bool isAnimatingBlood = false;
    private float bloodFadeStartedAt = -100;
    private float bloodFadeDuration = 4f;

    private bool characterAdded = false;
    private int currentCharacterIdx = 0;
    private List<float> delays = new List<float>() { 0.005f, 0.005f, 0.5f, 0.3f, 1.0f };
    private float lastCharacterAddedAt = -100;

    private CanvasGroup canvasGroup;
    private TextMeshProUGUI text;
    private GameObject dataStorage;

    void Start()
    {
        //dataStorage = GameObject.Find("DataStorage");
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        text = gameObject.transform.Find("Canvas/Text").GetComponent<TextMeshProUGUI>();

        isFadingIn = true;
        fadeInStartedAt = Time.time;

        selectedText = GlobalVersionController.GetEnding().epilogue + "\n\nEnding Key: " + GlobalVersionController.GetEnding().name;

        /*var createdConections = new List<string>();
        if (dataStorage) {
            createdConections = dataStorage.GetComponent<DataStorage>().GetFormattedCreatedConnections();
        }

        selectedText = GetMatchingEnding(createdConections).epilogueText;
        */
    }

    void Update()
    {
        if (isAnimatingBlood) {
            float progress = (Time.time - bloodFadeStartedAt) / bloodFadeDuration;
            if (progress <= 1) {
                SetBloodAlpha(progress * 0.25f);
            } else {
                isAnimatingBlood = false;
                SetBloodAlpha(0.25f);
            }
        }

        if (isFadingIn) {
            float progress = (Time.time - fadeInStartedAt) / fadeInDuration;
            if (progress <= 1) {
                canvasGroup.alpha = progress;
            } else {
                canvasGroup.alpha = 1;
                isFadingIn = false;
                lastCharacterAddedAt = Time.time;
            }
            return;
        }

        if (currentCharacterIdx >= selectedText.Length) {
            return;
        }

        char c = selectedText[currentCharacterIdx];
        var delay = delays[0];
        if (c == ' ') {
            delay = delays[1];
        } else if ((c == '.' || c == '?' || c == '!') && (selectedText[currentCharacterIdx + 1] == ' ')) {
            delay = delays[2];
        } else if (c == '$') {
            delay = delays[3];
        } else if (c == '[') {
            delay = delays[4];
        }

        if (!characterAdded) {
            if (c == '$') {
                text.text += '\n';
            } else if (c == ']') {
                Camera.main.GetComponent<SoundManager>().Play("knock");
            } else if (c == '#') {
                Camera.main.GetComponent<SoundManager>().Play("gunshot");
                isAnimatingBlood = true;
                bloodFadeStartedAt = Time.time;
            } else if (c != '[') {
                text.text += c;
            }
            characterAdded = true;
        }

        if (Time.time - lastCharacterAddedAt > delay) {
            characterAdded = false;
            currentCharacterIdx += 1;
            lastCharacterAddedAt = Time.time;
        }
    }

    void SetBloodAlpha(float a) {
        var c = gameObject.transform.Find("Canvas/BloodImage").GetComponent<Image>().color;
        c.a = a;
        gameObject.transform.Find("Canvas/BloodImage").GetComponent<Image>().color = c;
    }

    public void OnMenuClick()
    {
        Application.Quit();
    }

   /*
    EndingItem GetMatchingEnding(List<string> createdConnections) {
        foreach (var ending in endings) {
            bool isMatching = true;

            foreach (var (c1, c2) in ending.pairs) {
                if (createdConnections.Contains(c1) && createdConnections.Contains(c2)) {
                    return ending;
                }
            }

            foreach (string requiredConnection in ending.required) {
                if (!createdConnections.Contains(requiredConnection)) {
                    isMatching = false;
                    break;
                }
            }

            int createdOptional = 0;
            foreach (string optionalConnection in ending.optional) {
                if (createdConnections.Contains(optionalConnection)) {
                    ++createdOptional;
                }
            }

            if (createdOptional < ending.optionalMinAmount) {
                isMatching = false;
                break;
            }

            foreach (string bannedConnection in ending.banned) {
                if (createdConnections.Contains(bannedConnection)) {
                    isMatching = false;
                    break;
                }
            }


            if (isMatching) {
                return ending;
            }
        }

        return endings[endings.Count - 1];
    }
   */
}
