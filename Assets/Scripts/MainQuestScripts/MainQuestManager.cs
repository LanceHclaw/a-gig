using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgressionVector;
using UnityEngine;

public class MainQuestManager : MonoBehaviour
{
    //public bool initialized = false;

    public PV_PlayerProgress<Ending> playerProgress;
    public PV_QuestData<Ending> questData;

    private string fileName;

    public MQEvidence evidence;
    public MQEndings endings;
    public MQConnections connections;

    public List<(GameObject, Evidence)> collectedEvidence;
    public List<(int, int, GameObject, GameObject)> madeConnections;

    public MainQuestManager(PV_PlayerProgress<Ending> playerProgress, 
        PV_QuestData<Ending> questData,
        bool restricted)
    {
        this.playerProgress = playerProgress;
        this.questData = questData;

        fileName = restricted ? "DataRestricted.json" : "DataUnrestricted.json";
    }

    public void Instantiate(bool restricted)
    {
        fileName = restricted ? FileDirectory.ConnectionsRestrictedJsonFile : FileDirectory.ConnectionsUnrestrictedJsonFile;

        evidence = new MQEvidence();
        endings = new MQEndings();
        connections = new MQConnections(fileName, evidence, endings);

        collectedEvidence = new List<(GameObject, Evidence)>();
        madeConnections = new List<(int, int, GameObject, GameObject)>();

        questData = new PV_QuestData<Ending>(
            endings: endings.endingsById.Values,
            majorActions: GetMajorActions(connections),
            minor_actions: evidence.evidenceByID.Values.Concat(GetMinorActions(connections)),
            defaultEnding: GetDefaultEnding(endings),
            globalThreshold: 0,
            name: "MainQuest",
            DEBUG: false
            );

        SetupQuestData();

        playerProgress = new PV_PlayerProgress<Ending>(questData);

        //initialized = true;
    }

    public void RemoveThread((int, int, GameObject, GameObject) tuple)
    {
        madeConnections.Remove(tuple);
        var cOptions = GetConnectionByEvidence(GetEvidenceByID(tuple.Item1), GetEvidenceByID(tuple.Item2)).options;
        foreach (var opKey in cOptions.Keys.ToList())
        {
            if (cOptions[opKey])
            {
                if (HasZeroWeights(opKey)) playerProgress.RemoveMinorAction(opKey);
                else playerProgress.RemoveMajorAction(opKey);
            }
            cOptions[opKey] = false;
        }
    }

    public void CollectEvidence(int evidenceID, GameObject imageInstance)
    {
        var evidence = GetEvidenceByID(evidenceID);

        if (!playerProgress.minorActionFlags[evidence])
        {
            collectedEvidence.Add((imageInstance, evidence));
            playerProgress.MinorActionCompleted(evidence);
        }
    }

    public void ConfirmOption(Connection connection, Option option)
    {
        var opToRemove = connection.ChooseOption(option);
        if (HasZeroWeights(option)) playerProgress.MinorActionCompleted(option);
        else playerProgress.MajorActionCompleted(option);

        if (opToRemove == null) return;
        else
        {
            if (HasZeroWeights(opToRemove)) playerProgress.RemoveMinorAction(opToRemove);
            else playerProgress.RemoveMajorAction(opToRemove);
        }
    }

    public Option GetOptionByEvidenceId(int e1id, int e2id)
    {
        return GetConnectionByEvidence(evidence.evidenceByID[e1id], evidence.evidenceByID[e2id]).options.Where(y => y.Value == true).Select(x => x.Key).FirstOrDefault();
    }

    public Option GetOptionByEvidenceIdAndName(int e1id, int e2id, string name)
    {
        return GetConnectionByEvidence(evidence.evidenceByID[e1id], evidence.evidenceByID[e2id]).options.Where(x => x.Key.PV_name.Contains(name)).Select(x => x.Key).FirstOrDefault();
    }

    public Evidence GetEvidenceByID(int id)
    {
        return evidence.evidenceByID[id];
    }

    public Connection GetConnectionByEvidence(Evidence e1, Evidence e2)
    {
        return connections.ConnectionsByID[connections.connectionMatrix[e1.id, e2.id]];
    }

    public bool IsEvidenceCollected(int evidenceID)
    {
        return playerProgress.minorActionFlags[evidence.evidenceByID[evidenceID]];
    }



    void Awake()
    {
        Instantiate(GlobalVersionController.IsRestricted());
    }

    private IEnumerable<PV_Action<Ending>> GetMajorActions(MQConnections connections)
    {
        return connections.ConnectionsByID.SelectMany(x => x.Value.options.Keys).ToList().Where(y => !HasZeroWeights(y));
    }
    private IEnumerable<PV_Action<Ending>> GetMinorActions(MQConnections connections)
    {
        return connections.ConnectionsByID.SelectMany(x => x.Value.options.Keys).ToList().Where(y => HasZeroWeights(y));
    }

    private Ending GetDefaultEnding(MQEndings endings)
    {
        return endings.endingsById.Select(x => x.Value).Where(y => y.name == "Worst").First();
    }

    private bool HasZeroWeights(Option option) {
        return option.weights.Count == 0 || option.weights.All(x => x.Value == 0);
    }

    private void SetupQuestData()
    {
        var davisEnding = endings.endingsById[0];
        var nKnifeEnding = endings.endingsById[1];
        var nGunEnding = endings.endingsById[2];
        var SuicideEnding = endings.endingsById[3];
        var ContradictionsEnding = endings.endingsById[4];
        var HeartAttackEnding = endings.endingsById[5];
        var SlipUpEnding = endings.endingsById[6];
        var WTFEnding = endings.endingsById[7];
        var worstEnding = endings.endingsById[8];

        questData.DefineIndividualThresholds(new Dictionary<Ending, int> {
            {davisEnding, 9 },
            {nKnifeEnding, 8 },
            {nGunEnding, 11 },
            {SuicideEnding, 9 },
            {ContradictionsEnding, 4 },
            {HeartAttackEnding, 5 },
            {SlipUpEnding, 5 },
            {WTFEnding, 5 },
            {worstEnding, 0 }
        });

        // Davis Required
        questData.AddExtraCondition(davisEnding, (ending, progress) =>
        {
            return progress.actionFlags[GetOptionByEvidenceIdAndName(0, 10, "B12/2")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(0, 12, "B14")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(1, 11, "C13/1")];
        });

        // Neigbour Knife Required
        questData.AddExtraCondition(nKnifeEnding, (ending, progress) =>
        {
            return progress.actionFlags[GetOptionByEvidenceIdAndName(0, 8, "B10/1")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(3, 7, "E9/1")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(3, 8, "E10")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(4, 12, "F14/2")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(5, 6, "G8/1")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(7, 12, "I14")];
        });

        // Neigbour Gun Required
        questData.AddExtraCondition(nGunEnding, (ending, progress) =>
        {
            return progress.actionFlags[GetOptionByEvidenceIdAndName(0, 3, "B5/2")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(0, 8, "B10/1")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(0, 10, "B12/2")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(3, 10, "E12/2")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(4, 12, "F14/2")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(5, 6, "G8/1")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(10, 12, "L14/2")];
        });

        // Suicide Required
        questData.AddExtraCondition(SuicideEnding, (ending, progress) =>
        {
            return progress.actionFlags[GetOptionByEvidenceIdAndName(0, 8, "B10/1")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(0, 10, "B12/1")]
            && (
            progress.actionFlags[GetOptionByEvidenceIdAndName(1, 11, "C13/2")]
            || progress.actionFlags[GetOptionByEvidenceIdAndName(1, 11, "C13/3")]
            )
            && progress.actionFlags[GetOptionByEvidenceIdAndName(3, 8, "E10")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(3, 10, "E12/1")];
        });

        // Contradictions
        questData.AddExtraCondition(ContradictionsEnding, (ending, progress) =>
        {
            return
            (progress.actionFlags[GetOptionByEvidenceIdAndName(3, 10, "E12/1")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(0, 10, "B12/1")])
            ||
            (progress.actionFlags[GetOptionByEvidenceIdAndName(0, 8, "B10/2")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(0, 10, "B12/1")])
            ||
            (progress.actionFlags[GetOptionByEvidenceIdAndName(0, 8, "B10/2")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(1, 3, "C5/1")])
            ||
            (progress.actionFlags[GetOptionByEvidenceIdAndName(0, 8, "B10/2")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(3, 8, "E10")])
            ||
            (progress.actionFlags[GetOptionByEvidenceIdAndName(1, 3, "C5/2")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(1, 11, "C13/2")]);  
        });

        // HeartAttack Required
        questData.AddExtraCondition(HeartAttackEnding, (ending, progress) =>
        {
            return progress.actionFlags[GetOptionByEvidenceIdAndName(1, 4, "C6")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(2, 4, "D6")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(2, 9, "D11")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(4, 9, "F11/2")]
            && (progress.actionFlags[GetOptionByEvidenceIdAndName(6, 7, "H9/1")]
            || progress.actionFlags[GetOptionByEvidenceIdAndName(6, 7, "H9/2")]);
        });

        // SlipUp Required
        questData.AddExtraCondition(SlipUpEnding, (ending, progress) =>
        {
            return progress.actionFlags[GetOptionByEvidenceIdAndName(3, 5, "E8")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(5, 12, "G14")]
            && progress.actionFlags[GetOptionByEvidenceIdAndName(7, 9, "I11")]
            && (progress.actionFlags[GetOptionByEvidenceIdAndName(5, 6, "G8/1")]
            || progress.actionFlags[GetOptionByEvidenceIdAndName(5, 6, "G8/2")])
            && (progress.actionFlags[GetOptionByEvidenceIdAndName(6, 7, "H9/1")]
            || progress.actionFlags[GetOptionByEvidenceIdAndName(6, 7, "H9/2")]);
        });
    }
}

public static class FileDirectory
{
    public static readonly string JsonFolder = "JsonFiles";

    public static readonly string EvidenceJsonFile = JsonFolder + "/DataEvidence";

    public static readonly string EpiloguesFile = JsonFolder + "/Endings";

    public static readonly string ConnectionsRestrictedJsonFile = JsonFolder + "/DataConnectionsRestricted";

    public static readonly string ConnectionsUnrestrictedJsonFile = JsonFolder + "/DataConnectionsUnrestricted";
}
