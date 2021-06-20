using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ProgressionVector
{
    public class ProgressionVector<A, E>
    {
        public IEnumerable<E> endings;
        public Dictionary<IComparable<A>, IEnumerable<int>> all_actions;

        private Dictionary<IComparable<A>, bool> playerActions;
        private int[] outputVector;

        /// <summary>
        /// Provide all the data to set up a progression vector algorithm for a given quest.
        /// </summary>
        /// <param name="endings">List of all endings (outcomes).</param>
        /// <param name="all_actions">A dictionary with action Ids and their respective weight vectors of length |endings|.</param>
        public ProgressionVector(IEnumerable<E> endings, Dictionary<IComparable<A>, IEnumerable<int>> all_actions)
        {
            this.playerActions = new Dictionary<IComparable<A>, bool>();

            foreach (var action in all_actions)
            {
                if (action.Value.Count() != endings.Count())
                    throw new ArgumentException("Each weight vector must be of the same length as the number of endings");

                playerActions.Add(action.Key, false);
            }

            this.outputVector = new int[endings.Count()];
            this.all_actions = all_actions;
            this.endings = endings;
        }

        /// <summary>
        /// Use to indicate the completion of some action by the player and modify the output vector.
        /// </summary>
        /// <param name="action">Name of the action to be found in all_actions dictionary.</param>
        public void ActionCompleted(IComparable<A> action)
        {
            if (!all_actions.ContainsKey(action)) throw new ArgumentException("This action is not present in this action space.");

            if (playerActions[action] == false)
            {
                playerActions[action] = true;
                AddToOutput(all_actions[action]);
            }
            else throw new ArgumentException("This action has already been completed.");
        }

        /// <summary>
        /// Use to indicate that the player has undone one of their actions.
        /// </summary>
        /// <param name="action">Name of the action to be found in all_actions dictionary.</param>
        public void RemovePlayerAction(IComparable<A> action)
        {
            if (!all_actions.ContainsKey(action)) throw new ArgumentException("This action is not present in this action space.");

            if (playerActions[action] == true)
            {
                playerActions[action] = false;
                SubtractFromOutput(all_actions[action]);
            }
            else throw new ArgumentException("This action has not been completed before.");
        }

        // Dictionary of all actions and their vector values can come from the class field itself
        /// <summary>
        /// Get outcome based on the given path.
        /// </summary>
        /// <param name="path">Vector of action-presence. Values of weights will be taken internally.</param>
        /// <returns></returns>
        public E GetPathOutput(Dictionary<IComparable<A>, bool> path)
        {
            var outcome = new int[endings.Count()];
            foreach (var flag in path)
            {
                if (flag.Value == true) AddVectors(all_actions[flag.Key], outcome);
            }

            return GetEnding(outcome);
        }

        /// <summary>
        /// Returns a dictionary [path, ending] that gives a list of all actions
        /// that lead to the given outcome.
        /// </summary>
        /// <returns></returns>
        public Dictionary<List<IComparable<A>>, E> GetAllPaths()
        {
            var pathOutputs = new Dictionary<List<IComparable<A>>, E>();
            var paths = AllPermutations();

            foreach (var item in paths)
            {
                var actions = from key in item.Keys.ToList() where item[key] == true select key;
                var result = GetPathOutput(item);
                pathOutputs.Add(actions.ToList(), result);
            }

            return pathOutputs;
        }

        public List<List<IComparable<A>>> AllPathsTo(E ending)
        {
            var output = new List<List<IComparable<A>>>();

            foreach(var item in GetAllPaths())
            {
                if (item.Value.Equals(ending)) output.Add(item.Key);
            }

            return output;
        }

        /// <summary>
        /// Get the ending based on all actions that were added (i.e. that the player completed).
        /// </summary>
        /// <returns></returns>
        public E GetEnding()
        {
            int max = -Int32.MaxValue;
            int maxIdx = 0;
            int i = 0;
            foreach (var val in outputVector)
            {
                if (val > max)
                {
                    maxIdx = i;
                    max = val;
                }
                i++;
            }
            return endings.ElementAt(maxIdx);
        }

        /// <summary>
        /// [Internal] Get Ending based on the given result vector.
        /// </summary>
        /// <param name="resultVector"></param>
        /// <returns></returns>
        private E GetEnding(int[] resultVector)
        {
            int max = -Int32.MaxValue;
            int maxIdx = 0;
            int i = 0;
            foreach (var val in resultVector)
            {
                if (val > max)
                {
                    maxIdx = i;
                    max = val;
                }
                i++;
            }
            return endings.ElementAt(maxIdx);
        }

        /// <summary>
        /// [Internal] Add vector to the final output vector.
        /// </summary>
        /// <param name="vector">Vector to be added.</param>
        private void AddToOutput(IEnumerable<int> vector)
        {
            if (vector.Count() != outputVector.Length) throw new ArgumentException("Vector to be added is not of the same length as the output vector.");

            int i = 0;
            foreach (var x in vector)
            {
                outputVector[i] += x;
                i++;
            }
        }

        /// <summary>
        /// [Internal] Subtract vector from the final output vector.
        /// </summary>
        /// <param name="vector">Vector to be subtracted.</param>
        private void SubtractFromOutput(IEnumerable<int> vector)
        {
            if (vector.Count() != outputVector.Length) throw new ArgumentException("Vector to be subtracted is not of the same length as the output vector.");

            int i = 0;
            foreach (var x in vector)
            {
                outputVector[i] -= x;
                i++;
            }
        }

        /// <summary>
        /// [Internal] Addition of two int arrays.
        /// </summary>
        /// <param name="inV"> Vector to be added.</param>
        /// <param name="baseV"> Base vector to have values added to.</param>
        /// <returns></returns>
        private IEnumerable<int> AddVectors(IEnumerable<int> inV, int[] baseV)
        {
            if (inV.Count() != baseV.Length) throw new ArgumentException("Vector to be added is not of the same length as the output vector.");

            var outV = baseV;
            int i = 0;
            foreach (var x in inV)
            {
                outV[i] += x;
                i++;
            }

            return outV;
        }

        /// <summary>
        /// [Internal] Generates a list of all permutations of flags of actions.
        /// </summary>
        /// <returns>List of Dictionaries [action, isPresent?]</returns>
        private List<Dictionary<IComparable<A>, bool>> AllPermutations()
        {
            var binary = new int[all_actions.Count];
            var binaryList = new List<int[]>();

            // works with ref of binList and adds new permutations into it
            GenerateAllBinaryVectors(all_actions.Count, binary, 0, ref binaryList);

            var output = new List<Dictionary<IComparable<A>, bool>>();

            foreach (var perm in binaryList)
            {
                var path = new Dictionary<IComparable<A>, bool>();
                int idx = 0;
                foreach(var action in all_actions)
                {
                    path.Add(action.Key, perm[idx] == 0 ? false : true);
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
    }
}
