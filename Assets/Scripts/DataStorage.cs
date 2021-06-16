using System.Collections.Generic;
using UnityEngine;

public class DataStorage : MonoBehaviour
{
    [Header("Evidence pictures")]
    public Sprite p44magnumSprite;
    public Sprite unfinishedLetterSprite;
    public Sprite burntPillowSprite;
    public Sprite gunInABoxSprite;
    public Sprite halfBurntCigaretteSprite;
    public Sprite floorMopSprite;
    public Sprite cleanFloorSprite;
    public Sprite openWindowSprite;
    public Sprite dummySprite;
    public Sprite packOfCigarettesSprite;
    public Sprite bulletInTheBodySprite;
    public Sprite journalSprite;
    public Sprite knifeSprite;

    [Header("Internal")]
    public Dictionary<string, Sprite> spriteByName = new Dictionary<string, Sprite>();
    public Dictionary<string, string> evidenceDescriptions = new Dictionary<string, string>() {
        { ".44 Magnum", "A common caliber shell. Used. Recently." },
        { "Unfinished Letter", "\"Captain Johnson, forgive me, but I need a little more time. I swear if not for that damned neighbour that keeps showing up and defacing my door, I would have been done with the case days ago. Now I have to deal both with him and the rent collector.\"\n(scribbles show a dropped pen)" },
        { "Burnt Pillow", "The pillow seems to be burnt briefly on one side. Fairly recently, as the soot still stains my fingers." },
        { "Gun in a Box", "AutoMag pistol. Nice gun. Most police officers and PD affiliates have those. Uses magnum .44 ammo. Stored very carefully in the box. Looks like a prized possession." },
        { "Half-burnt Cigarette", "This cigarette is very recent. And only half-smoked. Someone stopped earlier than usual, given the rest of the tray contents. Could be the officers or Frank himself." },
        { "Floor Mop", "Given the tidiness of the floor here, I wonder why it would be still in the kitchen. Don’t people keep mops in the hallways or the storage room? Still slightly wet though." },
        { "Clean Floor", "The floor here is very clean, aside from the blood pool. Either the murderer entered without shoes, or they cleaned the floor afterwards." },
        { "Open Window", "The weather has not been the best for keeping the windows open. Either it’s Allen’s habit or it wasn’t him who opened it." },
        { "Dummy", "Nice practice target. Has a lot of bullet holes and… bullets stuck in them. " },
        { "Pack of Cigarettes", "That's a fairly popular brand." },
        { "Bullet in the Body", "Frank Allen was shot. And then stabbed. I am not sure if he was alive still after the shot." },
        { "Journal", "Ah, like every detective he kept his notebook updated. Although, the latest entry dates back to about a month ago. Something random, and the case is said to be closed." },
        { "Knife", "A bloodied kitchen knife sticking out of Frank’s chest. No fingerprints on it." },
    };
    public Dictionary<string, string> poiDescriptions = new Dictionary<string, string>() {
        { "LockedBedroomDrawer", "The drawer is locked. Maybe I can find a key?" },
        { "BedroomDrawerKey", "Why would he hide a key under an ash tray?" },
        { "LockedGunBox", "Locked using a combination lock..." },
        { "MapPiece", "Huh, looks like there's a hidden room behind this wall. I wonder if I can get there..." },
        { "Test 2", "Test description 2" },
    };

    public List<string> evidenceNames = new List<string>() { ".44 Magnum", "Unfinished Letter", "Burnt Pillow", "Gun in a Box", "Half-burnt Cigarette", "Floor Mop", "Clean Floor", "Open Window", "Dummy", "Pack of Cigarettes", "Bullet in the Body", "Journal", "Knife" };

    
    public List<(string, GameObject)> collectedEvidence = new List<(string, GameObject)>();

    // From, To, Thread, ThreadShortDesc, CD_Key, CD_Idx, CD_Opt
    public List<(string, string, GameObject, GameObject, string, int, int)> connectedThreads = new List<(string, string, GameObject, GameObject, string, int, int)>();

    // From: (To, (CommonDescription, List<Option, ShortDescription>)
    public Dictionary<string, List<(string, (string, List<(string, string)>))>> connectionDescriptions = new Dictionary<string, List<(string, (string, List<(string, string)>))>>() {
        { ".44 Magnum", new List<(string, (string, List<(string, string)>))>() {
            ("Burnt Pillow", ("", new List<(string, string)>() {
                ("Frank may have been testing muffling the gun shots.", "Frank muffled shots during practice"),
                ("The murdered must have muffled the gun shot with a pillow.", "Murderer muffled the gunshot")
            })),
            ("Gun in a Box", ("The shell fits the pistol.", new List<(string, string)>() {
                ("Frank must be using .44 Magnum when practicing.", "Frank used .44 for practice"),
                ("It's the most common caliber, anyone could be using it. Even the murderer.", "Murderer used .44")
            })),
            ("Open Window", ("Open windows ventilate the area quite well.", new List<(string, string)>() {
                ("Allen must have used it to ventilate the room after practice.", "Allan ventilated gunpowder smell"),
                ("The murderer may have wanted to hide the smell of gunpowder.", "Murderer got rid of gunpowder smell")
            })),
            ("Dummy", ("", new List<(string, string)>() {
                ("The caliber fits the size of the holes in the dummy. Allen must have been causing a lot of noise indeed", "Allen practiced shooting a lot"),
                ("Even though the holes may have been left by bullets of this caliber, it's hard to believe a sane man would shoot in his own house.", "Allen wouldn't shoot at home.")
            })),
            ("Bullet in the Body", ("The puncture wound is not very big. Just about the right size for .44 magnum.", new List<(string, string)>() {
                ("I can't believe he would shoot himself...", "Suicide?"),
                ("The murderer must have used the same type of weapon.", "Murderer shot Frank")
            })),
            ("Knife", ("", new List<(string, string)>() {
                ("Hold on... This bullet is out of place. What if I remove that knife... Hah. Either smart or desperately stupid. There's a puncture wound... Or rather - a bullet one.", "Knife covered the shot wound")
            }))
        } },
        { "Unfinished Letter", new List<(string, (string, List<(string, string)>))>() {
            ("Gun in a Box", ("", new List<(string, string)>() {
                ("I guess he was making noise indeed. That thing ain't quiet.", "Frank was a noisy weirdo"),
                ("I think this detective was quite serious with his business. He was ready to solve whatever he needed to solve.", "Frank took detective work seriously")
            })),
            ("Half-burnt Cigarette", ("", new List<(string, string)>() {
                ("He probably was just smoking and writing this when the murderer came. Must've gone to see what was going on.", "Smoked while writing")
            })),
            ("Journal", ("The letter said he was in the middle of a case or something.", new List<(string, string)>() {
                ("The latest entries must be missing.", "Someone tore the latest entries out"),
                ("He must've been about to start that case when the murder happened.", "Frank didn't start the case yet"),
                ("He must have been writing in a different journal or keeping it in his head. He seems to have not been himself lately", "Frank wasn't keeping notes of the case")
            }))
        } },
        { "Burnt Pillow", new List<(string, (string, List<(string, string)>))>() {
            ("Gun in a Box", ("", new List<(string, string)>() {
                ("If anything, I doubt one can muffle an AutoMag shot with a pillow. You can always try though.", "AutoMag is too loud to muffle")
            })),
            ("Half-burnt Cigarette", ("", new List<(string, string)>() {
                ("Allan used to smoke. Maybe at some point he did it in bed? Sould've been lucky to not burn the house down.", "Smoking accident?")
            })),
            ("Pack of Cigarettes", ("", new List<(string, string)>() {
                ("Having a cigarette pack next to the bed surely says he was doing it here. Must have narrowly avoided an accident.", "Frank had a habit of smoking in bed"),
            }))
        } },
        { "Gun in a Box", new List<(string, (string, List<(string, string)>))>() {
            ("Clean Floor", ("", new List<(string, string)>() {
                ("The gun is as neatly packed up as his kitchen is. The man was weird.", "Inconsistent standards of cleanliness"),
            })),
            ("Open Window", ("", new List<(string, string)>() {
                ("If he used to shoot frequently, he may well have been keeping windows open to ventilate the gunpowder stench.", "Gunpowder ventilation"),
                ("If people are already accusing you of noise, why keep your windows open? Surely someone else opened them.", "Someone else opened the windows"),
            })),
            ("Dummy", ("", new List<(string, string)>() {
                ("The holes definitely look like they were left by a similar weapon. And the noise complaints... I think the officer was riht about home shooting.", "Frank did shoot at home")
            })),
            ("Bullet in the Body", ("The bullet wound looks like it could've been caused by this weapon.",  new List<(string, string)>() {
                ("Suicide? But who put the weapon back in the case then? Must have been someone close.", "Suicide"),
                ("The murderer must have used a similar weapon. Or know of the existence of this one.", "Murderer used the same weapon")
            }))
        } },
        { "Half-burnt Cigarette", new List<(string, (string, List<(string, string)>))>() {
            ("Floor Mop", ("", new List<(string, string)>() {
                ("He must have been mopping a lot to maintain the floor cleanliness with all the ash from the cigarettes.", "Frank was a tidy man")
            })),
            ("Open Window", ("", new List<(string, string)>() {
                ("The window must be open to let the place ventilate a bit. Regardless of who smoked...", "Smoke ventilation"),
            })),
            ("Pack of Cigarettes", ("The cigarette is definitely from this pack.", new List<(string, string)> {
                ("Whoever smoked it uses the same ones.", "Someone has the same cigarettes"),
                ("It was Frank who smoked this one.", "Allen smoked this cigarette")
            })),
            ("Knife", ("What if it was the murderer who put the cigarette in the ashtray after all? Fire would cover all the tracks if the victim was smoking when murdered.", new List<(string, string)>() {
                ("The murderer wanted something in the house and didn't want fire to destroy it.", "Murderer was looking for something"),
                ("The murderer lives in the same building and didn't want to burn it down", "Murderer lives in the same building")
            }))
        } },
        { "Floor Mop", new List<(string, (string, List<(string, string)>))>() {
            ("Clean Floor", ("The mop was definitely used to clean the floor. Though it is slightly wet still.", new List<(string, string)>() {
                ("Frank must have been very particular about keeping the floor clean.", "Murderer wore no shoes"),
                ("Someone else used it recently. Murderer?", "Murderer used the mop recently?")
            })),
            ("Knife", ("", new List<(string, string)>() {
                ("The mop can be used to hold the knife without leaving fingerprints.", "Used to hide fingerprints")
            }))
        } },
        { "Clean Floor", new List<(string, (string, List<(string, string)>))>() {
            ("Open Window", ("If you hold your windows open, the floor ain't gonna be clean for long.", new List<(string, string)>() {
                ("Maybe that's why Frank is weirdly tidy about certain things?", "Reason for cleaning often"),
                ("Maybe Frank didn't open the windows much?", "Someone else opened it?")
            }))
        } },
        { "Open Window", new List<(string, (string, List<(string, string)>))>() {
            ("Pack of Cigarettes", ("", new List<(string, string)>() {
                ("I mean... If you smoke at home frequently, opening the windows makes sense if you don't want to suffocate.", "Ventilated cigarette stench")
            })),
            ("Knife", ("", new List<(string, string)>() {
                ("Frank has a gun. If the killer knew it, they could try to sneak up on him and into his house. What's a better way to do it than through a window? Then they could also be wearing gloves to leave no fingerprints.", "Silent kill?")
            }))
        } },
        { "Pack of Cigarettes", new List<(string, (string, List<(string, string)>))>() { } },
        { "Bullet in the Body", new List<(string, (string, List<(string, string)>))>() {
            ("Knife", ("I can't stop thinking why someone would thrust a knife into a bullet hole. Did they want to hide the fact that it was a gunshot?", new List<(string, string)>() {
                ("It's pretty smart.", "Hiding the evidence"),
                ("It's pretty stupid.", "Confirming the kill")
            }))
        } },
        { "Journal", new List<(string, (string, List<(string, string)>))>() { } },
        { "Knife", new List<(string, (string, List<(string, string)>))>() { } }
    };

    private List<string> formattedConnectionsFirst = new List<string>() { "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N" };
    private List<string> formattedConnectionsSecond = new List<string>() { "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14" };

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        spriteByName = new Dictionary<string, Sprite>() {
            { ".44 Magnum", p44magnumSprite },
            { "Unfinished Letter", unfinishedLetterSprite },
            { "Burnt Pillow", burntPillowSprite },
            { "Gun in a Box", gunInABoxSprite },
            { "Half-burnt Cigarette", halfBurntCigaretteSprite },
            { "Floor Mop", floorMopSprite },
            { "Clean Floor", cleanFloorSprite },
            { "Open Window", openWindowSprite },
            { "Dummy", dummySprite },
            { "Pack of Cigarettes", packOfCigarettesSprite },
            { "Bullet in the Body", bulletInTheBodySprite },
            { "Journal", journalSprite },
            { "Knife", knifeSprite },
        };
    }

    public List<string> GetFormattedCreatedConnections() {
        var result = new List<string>();
        foreach (var (_, _, _, _, CD_Key, CD_Idx, CD_Opt) in connectedThreads) {
            if (CD_Key != null) {
                result.Add(FormatConnection(CD_Key, CD_Idx, CD_Opt));
                Debug.Log(FormatConnection(CD_Key, CD_Idx, CD_Opt));
            }
        }

        return result;
    }

    private string FormatConnection(string key, int idx, int opt) {
        string first = formattedConnectionsFirst[evidenceNames.IndexOf(key)];
        string second = formattedConnectionsSecond[evidenceNames.IndexOf(connectionDescriptions[key][idx].Item1)];

        if (connectionDescriptions[key][idx].Item2.Item2.Count > 1) {
            return first + second + "/" + (opt + 1).ToString();
        }

        return first + second;
    }

    public bool IsEvidenceCollected(string name) {
        foreach (var (n, _) in collectedEvidence) {
            if (n == name) {
                return true;
            }
        }
        return false;
    }
}
