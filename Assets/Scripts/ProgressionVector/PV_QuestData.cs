using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgressionVector
{
    public class PV_QuestData<E>
        where E : IComparable
    {
        public string PV_name; // optional
        public List<E> all_endings { get; private set; }
        public List<PV_Action<E>> all_majorActions { get; private set; }
        public List<PV_Action<E>> all_minorActions { get; private set; }
        public Dictionary<E, List<Func<E, PV_PlayerProgress<E>, bool>>> all_extraConditions { get; private set; }
        public E defaultEnding { get; private set; }
        public Dictionary<E, int> thresholds { get; private set; }

        // optional field, does not exist if not in debug mode
        private List<Dictionary<PV_Action<E>, bool>> all_permutations;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endings">All meaningful endings of the game</param>
        /// <param name="majorActions">All actions directly impacting the outcome of the quest.</param>
        /// <param name="minor_actions">All actions that do NOT directly impact the outcome of the quest.</param>
        /// <param name="extra_conditions">Extra conditions that will help determine the outcome of the quest.</param>
        /// <param name="defaultEnding">Ending to be output in case the player doesn't get enough progress for any other ending. 
        /// Returns default(E) if not specified. Can be set externally using DefineZeroSpace method.</param>
        /// <param name="globalThreshold">Threshold of progress to be bypassed with action weights in any path. Defaults to 0.</param>
        /// <param name="name">Optional name of the quest.</param>
        /// <param name="DEBUG">Set to TRUE if you plan to run multiple debug methods on the instance. Consumes memory but improves performance for debugging.</param>
        public PV_QuestData(
            IEnumerable<E> endings, 
            IEnumerable<PV_Action<E>> majorActions,
            IEnumerable<PV_Action<E>> minor_actions = null,
            Dictionary<E, List<Func<E, PV_PlayerProgress<E>, bool>>> extra_conditions = null,
            E defaultEnding = default, 
            int globalThreshold = -Int32.MaxValue,
            string name = "", 
            bool DEBUG = false)
        {
            if ((defaultEnding == null || defaultEnding.Equals(default(E))) && globalThreshold != -Int32.MaxValue)
                throw new ArgumentNullException("You cannot specify the threshold without defining the zero space. Check that your default ending is not equal to default(E).");

            this.all_endings = new List<E>(endings);
            this.thresholds = new Dictionary<E, int>();
            foreach (E ending in endings)
                thresholds.Add(ending, globalThreshold);
            this.defaultEnding = defaultEnding;

            foreach (var action in majorActions)
            {
                foreach (var endKey in action.weights.Keys)
                {
                    if (!all_endings.Contains(endKey)) throw new ArgumentException("Action has weight for a non-existent ending.");
                }
            }

            this.all_majorActions = new List<PV_Action<E>>(majorActions);

            all_minorActions = minor_actions == null ? new List<PV_Action<E>>() : new List<PV_Action<E>>(minor_actions);

            if (extra_conditions == null)
            {
                this.all_extraConditions = new Dictionary<E, List<Func<E, PV_PlayerProgress<E>, bool>>>();
                foreach (var ending in endings)
                    all_extraConditions[ending] = new List<Func<E, PV_PlayerProgress<E>, bool>>();
            }
            else this.all_extraConditions = extra_conditions;      

            PV_name = name;
            if (DEBUG)
            {
                all_permutations = AllPermutations();
            }
        }

        public override string ToString()
        {
            return PV_name;
        }

        #region Control Functions

        public void AddExtraCondition(E ending, Func<E, PV_PlayerProgress<E>, bool> condition)
        {
            this.all_extraConditions[ending].Add(condition);
        }

        /// <summary>
        /// Defines which ending should be returned if the output vector doesn't pass extra conditions.
        /// </summary>
        /// <param name="ending">Ending to be considered "default".</param>
        /// /// <param name="threshold">Global threshold on all endings to be bypassed. Defaults to 0.</param>
        public void DefineZeroSpace(E ending, int threshold = 0)
        {
            this.defaultEnding = ending;
            foreach (var item in thresholds) thresholds[item.Key] = threshold;
        }

        /// <summary>
        /// Set distinct thresholds for each specified ending. You may only specify endings whose thresholds you want to differ from the default value.
        /// </summary>
        /// <param name="thresholds"></param>
        public void DefineIndividualThresholds (Dictionary<E, int> thresholds)
        {
            foreach (var item in thresholds)
                this.thresholds[item.Key] = item.Value;
        }

        /// <summary>
        /// Get output based on weights and extra conditions, including thresholds.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public E FinishQuest(PV_PlayerProgress<E> playerProgress)
        {
            var weights = GetOutputVector(playerProgress.actionFlags).weights;
            foreach (var weight in weights)
            {
                if (!CheckThreshold(weight)) weights.Remove(weight.Key);
            }

            RunExtraConditions(ref weights, playerProgress);

            var sortedVector =  weights.OrderByDescending(x => x.Value);
            if (sortedVector.Count() == 0)
                return defaultEnding;

            return sortedVector.First().Key;
        }

        private bool CheckThreshold(KeyValuePair<E, int> endingWeight)
        {
            return thresholds[endingWeight.Key] <= endingWeight.Value;
        }

        private void RunExtraConditions(ref Dictionary<E, int> endWeights, PV_PlayerProgress<E> playerProgress)
        {
            foreach (var weight in endWeights)
            {
                foreach (var condition in all_extraConditions[weight.Key])
                {
                    if (!condition(weight.Key, playerProgress))
                    {
                        endWeights.Remove(weight.Key);
                        break;
                    }
                }
            }
        }

        private bool RunExtraConditionsOnEnding(PV_PlayerProgress<E> playerProgress, E ending)
        {
            foreach (var condition in all_extraConditions[ending])
            {
                if (!condition(ending, playerProgress))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Debug Functions

        /// <summary>
        /// Produces a list of all paths that lead to the specified ending.
        /// </summary>
        /// <param name="ending"></param>
        /// <param name="raw">Set to true if you want to ignore extra conditions and thresholds.</param>
        /// <returns>List of List of actions.</returns>
        public List<List<PV_Action<E>>> Debug_AllPathsTo(E ending, bool raw = false)
        {
            if (!all_endings.Contains(ending)) throw new ArgumentException($"Ending {ending} is not part of this quest");

            var output = new List<List<PV_Action<E>>>();

            foreach (var item in Debug_GetAllPaths(raw))
            {
                if (item.Value.Equals(ending)) output.Add(item.Key);
            }

            return output;
        }

        /// <summary>
        /// Shows the ratio of the given action present in paths to each ending over all 
        /// paths containing this action.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="raw">Set to true if you want to ignore extra conditions and thresholds.</param>
        /// <returns></returns>
        public Dictionary<E, float> Debug_GetActionDirection(PV_Action<E> action, bool raw = false)
        {
            if (!all_majorActions.Contains(action)) throw new ArgumentException($"Action {action} is not part of this quest.");

            var pathsWithA = from x in Debug_GetAllPaths(raw) where x.Key.Contains(action) select x;
            var total = pathsWithA.Count();
            if (total == 0) return new Dictionary<E, float>();

            var outDict = new Dictionary<E, float>();

            foreach (var ending in all_endings) outDict[ending] = 0;
            foreach (var path in pathsWithA) outDict[path.Value] += 1f;
            foreach (var key in outDict.Keys) outDict[key] = outDict[key] / total;

            return outDict;
        }

        /// <summary>
        /// Calculates the ratio of each action to the number of all actions that lead to the given ending.
        /// </summary>
        /// <param name="outcome"></param>
        /// <param name="raw">Set to true if you want to ignore extra conditions and thresholds.</param>
        /// <returns></returns>
        public Dictionary<PV_Action<E>, float> Debug_GetActionFrequenciesForEnding(E ending, bool raw = false)
        {
            if (!all_endings.Contains(ending)) throw new ArgumentException($"Ending {ending} is not part of this quest");

            var allPaths = Debug_AllPathsTo(ending, raw);
            var total = allPaths.Count;
            var outDict = new Dictionary<PV_Action<E>, float>();


            foreach (var action in all_majorActions) outDict.Add(action, 0f);

            foreach (var path in allPaths)
            {
                foreach (var action in path) outDict[action] += 1f;
            }

            foreach (var action in all_majorActions) outDict[action] = outDict[action] / total;

            return outDict;
        }

        /// <summary>
        /// Returns a dictionary [path, ending] that gives a list of all actions
        /// that lead to the given outcome.
        /// </summary>
        /// <param name="raw">Set to true if you want to ignore extra conditions and thresholds.</param>
        /// <returns></returns>
        public Dictionary<List<PV_Action<E>>, E> Debug_GetAllPaths(bool raw = false)
        {
            var pathOutputs = new Dictionary<List<PV_Action<E>>, E>();
            var paths = all_permutations ?? AllPermutations();

            foreach (var path in paths)
            {
                var actions = from key in path.Keys.ToList() where path[key] == true select key;
                var result = Debug_GetPathOutput(path, raw);
                pathOutputs.Add(actions.ToList(), result);
            }

            return pathOutputs;
        }

        /// <summary>
        /// Returns all paths achievable from the already completed set of actions.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="raw">Set to true if you want to ignore extra conditions and thresholds.</param>
        /// <returns></returns>
        public Dictionary<List<PV_Action<E>>, E> Debug_GetAllPathsFrom(IEnumerable<PV_Action<E>> path, bool raw = false)
        {
            foreach (var action in path)
                if (!all_majorActions.Contains(action)) 
                    throw new ArgumentException($"Action {action} is not part of this quest.");

            var allPaths = Debug_GetAllPaths(raw);
            var outPaths = from pAll in allPaths where path.All(a => pAll.Key.Contains(a)) select pAll;

            return outPaths.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Returns all paths achievable from the given player progress.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="raw">Set to true if you want to ignore extra conditions and thresholds.</param>
        /// <returns></returns>
        public Dictionary<List<PV_Action<E>>, E> Debug_GetAllPathsFrom(PV_PlayerProgress<E> playerProgress, bool raw = false)
        {
            foreach (var action in playerProgress.actionFlags)
                if (!all_majorActions.Contains(action.Key))
                    throw new ArgumentException($"Action {action} is not part of this quest.");

            var allPaths = Debug_GetAllPaths(raw);
            var playerActions = playerProgress.actionFlags.Where(a => a.Value == true).Select(a => a.Key);
            var outPaths = from pAll in allPaths where playerActions.All(a => pAll.Key.Contains(a)) select pAll;

            return outPaths.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Returns all paths that lead from the already completed set of actions to the given ending.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="raw">Set to true if you want to ignore extra conditions and thresholds.</param>
        /// <returns></returns>
        public Dictionary<List<PV_Action<E>>, E> Debug_GetAllPathsFromTo(IEnumerable<PV_Action<E>> path, E ending, bool raw = false)
        {
            foreach (var action in path)
                if (!all_majorActions.Contains(action))
                    throw new ArgumentException($"Action {action} is not part of this quest.");
            if (!all_endings.Contains(ending)) 
                throw new ArgumentException($"Ending {ending} is not part of this quest");

            var allPaths = Debug_GetAllPaths(raw);
            var outPaths = from pAll in allPaths where pAll.Value.Equals(ending) && path.All(a => pAll.Key.Contains(a)) select pAll;

            return outPaths.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Returns all paths that lead from the given player progress to the given ending.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="raw">Set to true if you want to ignore extra conditions and thresholds.</param>
        /// <returns></returns>
        public Dictionary<List<PV_Action<E>>, E> Debug_GetAllPathsFromTo(PV_PlayerProgress<E> playerProgress, E ending, bool raw = false)
        {
            foreach (var action in playerProgress.actionFlags)
                if (!all_majorActions.Contains(action.Key))
                    throw new ArgumentException($"Action {action} is not part of this quest.");
            if (!all_endings.Contains(ending))
                throw new ArgumentException($"Ending {ending} is not part of this quest");

            var allPaths = Debug_GetAllPaths(raw);
            var playerActions = playerProgress.actionFlags.Where(a => a.Value == true).Select(a => a.Key);
            var outPaths = from pAll in allPaths where pAll.Value.Equals(ending) && playerActions.All(a => pAll.Key.Contains(a)) select pAll;

            return outPaths.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Returns the number of paths that lead to each ending.
        /// </summary>
        /// <param name="raw">Set to true if you want to ignore extra conditions and thresholds.</param>
        /// <returns>Dictionary [ending, #times]</returns>
        public Dictionary<E, int> Debug_GetEndingFrequencies(bool raw = false)
        {
            var allPaths = Debug_GetAllPaths(raw);
            var outDict = new Dictionary<E, int>();
            foreach (var end in all_endings)
            {
                outDict.Add(end, 0);
            }

            foreach (var path in allPaths)
            {
                outDict[path.Value] += 1;
            }

            return outDict;
        }

        /// <summary>
        /// Use to retrieve all paths that can result in multiple endings.
        /// </summary>
        /// <returns>Dictionary [path, endings].</returns>
        public Dictionary<List<PV_Action<E>>, List<E>> Debug_GetOverlaps(bool raw = false)
        {
            var pathOutputs = GetAllOutputVectors();
            var outDict = new Dictionary<List<PV_Action<E>>, List<E>>();

            foreach (var item in pathOutputs)
            {
                var eList = FindEqualMaxValues(item.Value);
                if (!raw)
                {
                    var pp = new PV_PlayerProgress<E>(item.Key, this);
                    eList = (from e in eList where CheckThresholdsAndExtra(pp, e) select e).ToList();
                }
                if (eList.Count > 1)
                    outDict.Add(item.Key, eList);
            }
            return outDict;
        }

        /// <summary>
        /// Get outcome based on the given path.
        /// </summary>
        /// <param name="path">Vector of action-presence. Values of weights will be taken internally.</param>
        /// <param name="raw">Set to true if you want to ignore extra conditions and thresholds.</param>
        /// <returns></returns>
        public E Debug_GetPathOutput(Dictionary<PV_Action<E>, bool> path, bool raw = false)
        {
            foreach (var action in path)
                if (!all_majorActions.Contains(action.Key))
                    throw new ArgumentException($"Action {action} is not part of this quest.");

            if (raw) return GetRawEnding(GetOutputVector(path));
            else return FinishQuest(new PV_PlayerProgress<E>(path, this));
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// [Internal] Addition of two int arrays.
        /// </summary>
        /// <param name="actionWeights"> Vector to be added.</param>
        /// <param name="outputVector"> Vector that will be changed.</param>
        /// <returns></returns>
        private void AddWeightsToVector(PV_Action<E> actionWeights, ref PV_OutputVector<E> outputVector)
        {
            if (actionWeights.weights.Count() != actionWeights.weights.Count) throw new ArgumentException("Vector to be added is not of the same length as the output vector.");

            foreach (var weight in actionWeights.weights)
            {
                outputVector.weights[weight.Key] += weight.Value;
            }
        }

        /// <summary>
        /// [Internal] Searches all paths and returns output vectors corresponding to each of them.
        /// </summary>
        /// <returns></returns>
        private Dictionary<List<PV_Action<E>>, PV_OutputVector<E>> GetAllOutputVectors()
        {
            var pathOutputs = new Dictionary<List<PV_Action<E>>, PV_OutputVector<E>>();
            var paths = all_permutations ?? AllPermutations();

            foreach (var path in paths)
            {
                var actions = from key in path.Keys where path[key] == true select key;
                var result = GetOutputVector(path);
                pathOutputs.Add(actions.ToList(), result);
            }

            return pathOutputs;
        }

        /// <summary>
        /// [Internal] Returns an output vector based on the dictionary of flags.
        /// </summary>
        /// <param name="path">Dictionary of [action, isCompleted?].</param>
        /// <returns></returns>
        private PV_OutputVector<E> GetOutputVector(Dictionary<PV_Action<E>, bool> path)
        {
            var outcome = new PV_OutputVector<E>(this);
            foreach (var flag in path)
            {
                if (flag.Value == true) AddWeightsToVector(flag.Key, ref outcome);
            }

            return outcome;
        }

        /// <summary>
        /// [Internal] Get Ending based on the given result vector.
        /// </summary>
        /// <param name="resultVector"></param>
        /// <returns></returns>
        private E GetRawEnding(PV_OutputVector<E> outputVector)
        {
            return outputVector.weights.OrderByDescending(x => x.Value).First().Key;
        }

        /// <summary>
        /// [Internal] Generates a list of all permutations of flags of actions.
        /// </summary>
        /// <returns>List of Dictionaries [action, isPresent?]</returns>
        private List<Dictionary<PV_Action<E>, bool>> AllPermutations()
        {
            var binaryList = GenerateAllBinaryVectors();

            var output = new List<Dictionary<PV_Action<E>, bool>>();

            foreach (var perm in binaryList)
            {
                var path = new Dictionary<PV_Action<E>, bool>();
                int idx = 0;
                foreach (var action in all_majorActions)
                {
                    path.Add(action, perm[idx] == 0 ? false : true);
                    idx++;
                }
                output.Add(path);
            }

            return output;
        }

        private bool CheckThresholdsAndExtra (PV_PlayerProgress<E> playerProgress, E ending)
        {
            var outVect = GetOutputVector(playerProgress.actionFlags);
            return (CheckThreshold(new KeyValuePair<E, int>(ending, outVect.weights[ending])) && (RunExtraConditionsOnEnding(playerProgress, ending)));
        }

        /// <summary>
        /// [Internal] [Wrapper] Decides whether to use multithreaded approach or single-threaded and returns all binary strings of length all_majorActions.
        /// </summary>
        /// <returns></returns>
        private List<int[]> GenerateAllBinaryVectors()
        {
            if (all_majorActions.Count >= 25) return GenerateAllBinaryVectorsThreaded();
            else
            {
                var list = new List<int[]>();
                GenerateAllBinaryVectorsLinear(all_majorActions.Count, new int[all_majorActions.Count], 0, ref list);
                return list;
            }
        }

        /// <summary>
        /// [Internal] [Helper] Uses threading to generate all permutations of a binary vector of length = |actions|.
        /// </summary>
        /// <returns></returns>
        private List<int[]> GenerateAllBinaryVectorsThreaded()
        {
            var threadCount = Environment.ProcessorCount;
            var threads = new Thread[threadCount];
            var outList = new List<int[]>();

            List<int[]> startingSets = new List<int[]>();
            GenerateAllBinaryVectorsLinear((int)Math.Log2(threadCount), new int[(int)Math.Log2(threadCount)], 0, ref startingSets);

            for (var i = 0; i < threadCount; i++)
            {
                var set = startingSets[i];
                var arr = new int[all_majorActions.Count];
                set.CopyTo(arr, 0);

                threads[i] = new Thread(() => GenerateAllBinaryVectorsLinear(all_majorActions.Count, arr, set.Length, ref outList));
                threads[i].Start();
            }
            
            foreach (var t in threads) { t.Join(); }
            return outList;
        }

        /// <summary>
        /// [Internal] [Helper] Generates all permutations of a binary vector of length = |actions|.
        /// Used to generate all permutations of action vector.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="arr"></param>
        /// <param name="i"></param>
        /// <param name="output"></param>
        private static void GenerateAllBinaryVectorsLinear(int n, int[] arr, int i, ref List<int[]> output)
        {
            if (i == n)
            {
                lock (output)
                {
                    // need to cast to create a new copy, otw all members of output get overwritten with each iteration
                    output.Add(arr.ToArray());
                }
                return;
            }

            arr[i] = 0;
            GenerateAllBinaryVectorsLinear(n, arr, i + 1, ref output);
            arr[i] = 1;
            GenerateAllBinaryVectorsLinear(n, arr, i + 1, ref output);
        }

        /// <summary>
        /// [Internal] Returns a list of indices of all maximum values of the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>List of indices of max values of the vector.</returns>
        private static List<E> FindEqualMaxValues(PV_OutputVector<E> vector)
        {
            var keys = new List<E>();
            var max = -Int32.MaxValue;

            foreach (var weight in vector.weights)
            {
                if (weight.Value > max)
                {
                    max = weight.Value;
                    keys.Clear();
                    keys.Add(weight.Key);
                }
                else if (weight.Value == max) keys.Add(weight.Key);
            }

            return keys;
        }

        #endregion
    }
}
