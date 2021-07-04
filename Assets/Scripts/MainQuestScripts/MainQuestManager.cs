using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgressionVector;
using UnityEngine;

public class MainQuestManager : MonoBehaviour
{
    PV_PlayerProgress<Ending> playerProgress;
    PV_QuestData<Ending> questData;

    public bool restricted;

    public string fileName;

    public MQEvidence evidence;
    //public AllEndings endings;

    public MainQuestManager(PV_PlayerProgress<Ending> playerProgress, 
        PV_QuestData<Ending> questData,
        bool restricted)
    {
        this.playerProgress = playerProgress;
        this.questData = questData;

        fileName = restricted ? "DataRestricted.json" : "DataUnrestricted.json";
    }

    void Awake()
    {
        evidence = new MQEvidence();
    }
}

public static class FileDirectory
{
    public static readonly string GameFolder = Application.dataPath;

    public static readonly string SpritesFolder = GameFolder + "/Images/UI/Items";

    public static readonly string EvidenceJsonFile = GameFolder + "/Scripts/MainQuestScripts/DataEvidence.json";

    public static readonly string EpiloguesFile = GameFolder + "/Scripts/MainQuestScripts/Endings.json";

    public static readonly string ConnectionsRestrictedJsonFile = GameFolder + "/Scripts/MainQuestScripts/DataConnectionsRestricted.json";

    public static readonly string ConnectionsUnrestrictedJsonFile = GameFolder + "/Scripts/MainQuestScripts/DataConnectionsUnrestricted.json";
}
