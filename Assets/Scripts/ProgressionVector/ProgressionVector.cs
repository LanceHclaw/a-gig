﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressionVector
{
    public class ProgressionVector<E>
        where E : IComparable
    {
        public PV_PlayerProgress<E> playerProgress;
        public PV_QuestData<E> questData;

        // optional field, does not exist if not in debug mode
        private List<Dictionary<PV_Action<E>, bool>> all_permutations;

        public ProgressionVector(PV_PlayerProgress<E> playerProgress, PV_QuestData<E> questData, bool DEBUG=false)
        {
            this.playerProgress = playerProgress;
            this.questData = questData;

            if (DEBUG)
            {
                all_permutations = AllPermutations();
            }
        }

        #region Public API

        /// <summary>
        /// Produces a list of all paths that lead to the specified ending.
        /// </summary>
        /// <param name="ending"></param>
        /// <returns>List of List of actions.</returns>
        public List<List<PV_Action<E>>> AllPathsTo(E ending)
        {
            var output = new List<List<PV_Action<E>>>();

            foreach (var item in GetAllPaths())
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
        /// <returns></returns>
        public Dictionary<E, float> GetActionDirection(PV_Action<E> action)
        {
            var pathsWithA = from x in GetAllPaths() where x.Key.Contains(action) select x;
            var total = pathsWithA.Count();
            if (total == 0) return new Dictionary<E, float>();

            var outDict = new Dictionary<E, float>();

            foreach (var ending in questData.all_endings) outDict[ending] = 0;
            foreach (var path in pathsWithA) outDict[path.Value] += 1f;
            foreach (var key in outDict.Keys) outDict[key] = outDict[key] / total;

            return outDict;
        }

        /// <summary>
        /// Calculates the ratio of each action to the number of all actions that lead to the given ending.
        /// </summary>
        /// <param name="outcome"></param>
        /// <returns></returns>
        public Dictionary<PV_Action<E>, float> GetActionFrequenciesForEnding(E ending)
        {
            var allPaths = AllPathsTo(ending);
            var total = allPaths.Count;
            var outDict = new Dictionary<PV_Action<E>, float>();


            foreach (var action in questData.all_actions) outDict.Add(action, 0f);

            foreach (var path in allPaths)
            {
                foreach (var action in path) outDict[action] += 1f;
            }

            foreach (var action in questData.all_actions) outDict[action] = outDict[action] / total;

            return outDict;
        }

        /// <summary>
        /// Returns a dictionary [path, ending] that gives a list of all actions
        /// that lead to the given outcome.
        /// </summary>
        /// <returns></returns>
        public Dictionary<List<PV_Action<E>>, E> GetAllPaths()
        {
            var pathOutputs = new Dictionary<List<PV_Action<E>>, E>();
            var paths = all_permutations ?? AllPermutations();

            foreach (var path in paths)
            {
#warning double check if I need ToList() here
                var actions = from key in path.Keys.ToList() where path[key] == true select key;
                var result = GetPathOutput(path);
                pathOutputs.Add(actions.ToList(), result);
            }

            return pathOutputs;
        }

        /// <summary>
        /// Returns all paths achievable from the already completed set of actions.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Dictionary<List<PV_Action<E>>, E> GetAllPathsFrom(IEnumerable<PV_Action<E>> path)
        {
            var allPaths = GetAllPaths();
            var outPaths = from pAll in allPaths where path.All(a => pAll.Key.Contains(a)) select pAll;

            return outPaths.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Returns all paths achievable from the given player progress.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Dictionary<List<PV_Action<E>>, E> GetAllPathsFrom(PV_PlayerProgress<E> playerProgress)
        {
            var allPaths = GetAllPaths();
            var playerActions = playerProgress.actionFlags.Where(a => a.Value == true).Select(a => a.Key);
            var outPaths = from pAll in allPaths where playerActions.All(a => pAll.Key.Contains(a)) select pAll;

            return outPaths.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Returns all paths that lead from the already completed set of actions to the given ending.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Dictionary<List<PV_Action<E>>, E> GetAllPathsFromTo(IEnumerable<PV_Action<E>> path, E ending)
        {
            var allPaths = GetAllPaths();
            var outPaths = from pAll in allPaths where pAll.Value.Equals(ending) && path.All(a => pAll.Key.Contains(a)) select pAll;

            return outPaths.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Returns all paths that lead from the given player progress to the given ending.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Dictionary<List<PV_Action<E>>, E> GetAllPathsFromTo(PV_PlayerProgress<E> playerProgress, E ending)
        {
            var allPaths = GetAllPaths();
            var playerActions = playerProgress.actionFlags.Where(a => a.Value == true).Select(a => a.Key);
            var outPaths = from pAll in allPaths where pAll.Value.Equals(ending) && playerActions.All(a => pAll.Key.Contains(a)) select pAll;

            return outPaths.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Returns the number of paths that lead to each ending.
        /// </summary>
        /// <returns>Dictionary [ending, #times]</returns>
        public Dictionary<E, int> GetEndingFrequencies()
        {
            var allPaths = GetAllPaths();
            var outDict = new Dictionary<E, int>();
            foreach (var end in questData.all_endings)
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
        public Dictionary<List<PV_Action<E>>, List<E>> GetOverlaps()
        {
            var pathOutputs = GetAllOutputVectors();
            var outDict = new Dictionary<List<PV_Action<E>>, List<E>>();

            foreach (var item in pathOutputs)
            {
                var eList = FindEqualMaxValues(item.Value);
                if (eList.Count > 1)
                    outDict.Add(item.Key, eList);
            }
            return outDict;
        }

        /// <summary>
        /// Get outcome based on the given path.
        /// </summary>
        /// <param name="path">Vector of action-presence. Values of weights will be taken internally.</param>
        /// <returns></returns>
        public E GetPathOutput(Dictionary<PV_Action<E>, bool> path)
        {
            return GetEnding(GetOutputVector(path));
        }

        /// <summary>
        /// Get final evaluation of player progress
        /// </summary>
        /// <param name="playerProgress"></param>
        /// <param name="questData"></param>
        /// <returns></returns>
        public E ProduceFinalEnding()
        {
            return GetPathOutput(playerProgress.actionFlags);
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
#warning double check if I need .ToList()
                var actions = from key in path.Keys.ToList() where path[key] == true select key;
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
            var outcome = new PV_OutputVector<E>(questData);
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
        private E GetEnding(PV_OutputVector<E> outputVector)
        {
            int max = -Int32.MaxValue;
            E output = default;

            foreach (var weight in outputVector.weights)
            {
                if (weight.Value > max)
                {
                    max = weight.Value;
                    output = weight.Key;
                }
            }
            return output;
        }

        /// <summary>
        /// [Internal] Generates a list of all permutations of flags of actions.
        /// </summary>
        /// <returns>List of Dictionaries [action, isPresent?]</returns>
        private List<Dictionary<PV_Action<E>, bool>> AllPermutations()
        {
            var binaryList = new List<int[]>();
            GenerateAllBinaryVectors(questData.all_actions.Count, new int[questData.all_actions.Count], 0, ref binaryList);

            var output = new List<Dictionary<PV_Action<E>, bool>>();

            foreach (var perm in binaryList)
            {
                var path = new Dictionary<PV_Action<E>, bool>();
                int idx = 0;
                foreach (var action in questData.all_actions)
                {
                    path.Add(action, perm[idx] == 0 ? false : true);
                    idx++;
                }
                output.Add(path);
            }

            return output;
        }

        /// <summary>
        /// [Internal] [Helper] Generates all permutations of a binary vector of length = |actions|.
        /// Used to generate all permutations of action vector.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="arr"></param>
        /// <param name="i"></param>
        /// <param name="output"></param>
        private static void GenerateAllBinaryVectors(int n, int[] arr, int i, ref List<int[]> output)
        {
            if (i == n)
            {
                // need to cast to create a new copy, otw all members of output get overwritten with each iteration
                output.Add(arr.ToArray());
                return;
            }

            arr[i] = 0;
            GenerateAllBinaryVectors(n, arr, i + 1, ref output);
            arr[i] = 1;
            GenerateAllBinaryVectors(n, arr, i + 1, ref output);
        }

        /// <summary>
        /// [Internal] Returns a list of indices of all maximum values of the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>List of indices of max values of the vector.</returns>
        private List<E> FindEqualMaxValues(PV_OutputVector<E> vector)
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
