using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgressionVector;
using UnityEngine;

public class MainQuestManager : MonoBehaviour
{
    public PV_PlayerProgress<Ending> playerProgress;
    public PV_QuestData<Ending> questData;

    private string fileName;

    public MQEvidence evidence;
    public MQEndings endings;
    public MQConnections connections;

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

        questData = new PV_QuestData<Ending>(
            endings: endings.endingsById.Values,
            majorActions: GetMajorActions(connections),
            minor_actions: evidence.evidenceByID.Values.Concat(GetMinorActions(connections)),
            defaultEnding: GetDefaultEnding(endings),
            globalThreshold: 5,
            name: "MainQuest",
            DEBUG: false
            );

        playerProgress = new PV_PlayerProgress<Ending>(questData);
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
}

public static class FileDirectory
{
    public static readonly string GameFolder = Application.dataPath;

    public static readonly string EvidenceJsonFile = GameFolder + "/Scripts/MainQuestScripts/DataEvidence.json";

    public static readonly string EpiloguesFile = GameFolder + "/Scripts/MainQuestScripts/Endings.json";

    public static readonly string ConnectionsRestrictedJsonFile = GameFolder + "/Scripts/MainQuestScripts/DataConnectionsRestricted.json";

    public static readonly string ConnectionsUnrestrictedJsonFile = GameFolder + "/Scripts/MainQuestScripts/DataConnectionsUnrestricted.json";
}
