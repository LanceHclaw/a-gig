using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        // dictionary of all actions and their vector values can come from the class field itself
        public E GetPathOutput(Dictionary<IComparable<A>, bool> path)
        {
            var outcome = new int[endings.Count()];
            foreach (var flag in path)
            {
                if (flag.Value == true) AddVectors(all_actions[flag.Key], outcome);
            }

            return GetEnding(outcome);
        }

        public Dictionary<List<IComparable<A>>, E> GetAllPaths()
        {
            var pathOutputs = new Dictionary<List<IComparable<A>>, E>();
            var paths = AllPermutations();

            foreach(var item in paths)
            {
                var actions = from key in item.Keys.ToList() where item[key] == true select key;
                var result = GetPathOutput(item);
                pathOutputs.Add(actions.ToList(), result);
            }

            return pathOutputs;
        }

        private List<Dictionary<IComparable<A>, bool>> AllPermutations()
        {
            var binary = new int[all_actions.Count];
            var binaryList = new List<int[]>();

            // works with ref of binList and adds new permutations into it
            GenerateAllBinaryStrings(all_actions.Count, binary, 0, ref binaryList);

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

        private static void GenerateAllBinaryStrings(int n, int[] arr, int i, ref List<int[]> output)
        {
            if (i == n)
            {
                output.Add(arr.ToArray());
                return;
            }

            arr[i] = 0;
            GenerateAllBinaryStrings(n, arr, i + 1, ref output);
            arr[i] = 1;
            GenerateAllBinaryStrings(n, arr, i + 1, ref output);
        }
    }
}
