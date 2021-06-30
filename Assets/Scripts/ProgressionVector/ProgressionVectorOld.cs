using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ProgressionVector
{
    public class ProgressionVectorOld<A, E> 
        where A : IComparable 
        where E : IComparable
    {
        public IEnumerable<E> endings;
        public Dictionary<A, IEnumerable<int>> all_actions;

        private Dictionary<A, bool> playerActions;
        private int[] outputVector;

        #region Public API
        /// <summary>
        /// Provide all the data to set up a progression vector algorithm for a given quest.
        /// </summary>
        /// <param name="endings">List of all endings (outcomes).</param>
        /// <param name="all_actions">A dictionary with action Ids and their respective weight vectors of length |endings|.</param>
        public ProgressionVectorOld(IEnumerable<E> endings, Dictionary<A, IEnumerable<int>> all_actions)
        {
            this.playerActions = new Dictionary<A, bool>();

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
        /// Calculates the ratio of each action to the number of all actions that lead to the given ending.
        /// </summary>
        /// <param name="outcome"></param>
        /// <returns></returns>
        public Dictionary<A, float> GetActionWeightForEnding(E outcome)
        {
            var allPaths = AllPathsTo(outcome);
            var total = allPaths.Count;

            var outDict = new Dictionary<A, float>();
            foreach (var act in all_actions) outDict.Add(act.Key, 0f);

            foreach(var path in allPaths)
            {
                foreach (var action in path) outDict[action] += 1f;
            }

            foreach (var act in all_actions) outDict[act.Key] = outDict[act.Key] / total;

            return outDict;
        }

        /// <summary>
        /// Shows the ratio of the given action present in paths to each ending over all 
        /// paths containing this action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Dictionary<E, float> GetActionDirection(A action)
        {
            var pathsWithA = from x in GetAllPaths() where x.Key.Contains(action) select x;

            var total = 0f;

            var outDict = new Dictionary<E, float>();
            foreach (var end in endings) outDict[end] = 0;

            foreach(var path in pathsWithA)
            {
                total++;
                outDict[path.Value] += 1f;
            }

            if (total != 0) foreach (var end in endings) outDict[end] = outDict[end] / total;

            return outDict;
        }

        /// <summary>
        /// Use to indicate the completion of some action by the player and modify the output vector.
        /// </summary>
        /// <param name="action">Name of the action to be found in all_actions dictionary.</param>
        public void ActionCompleted(A action)
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
        public void RemovePlayerAction(A action)
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
        /// Returns the number of paths that lead to each ending.
        /// </summary>
        /// <returns>Dictionary [ending, #times]</returns>
        public Dictionary<E, int> GetEndingFrequencies()
        {
            var allPaths = GetAllPaths();
            var outDict = new Dictionary<E, int>();
            foreach (var end in endings)
            {
                outDict.Add(end, 0);
            }

            foreach(var path in allPaths)
            {
                outDict[path.Value] += 1;
            }

            return outDict;
        }

        /// <summary>
        /// Use to retrieve all paths that can result in multiple endings.
        /// </summary>
        /// <returns>Dictionary [path, endings].</returns>
        public Dictionary<List<A>, List<E>> GetOverlaps()
        {
            var pathOutputs = GetAllOutputVectors();
            var outDict = new Dictionary<List<A>, List<E>>();

            foreach(var item in pathOutputs)
            {
                var indList = FindEqualMaxValues(item.Value);
                if (indList.Count > 1)
                {
                    var elist = new List<E>();
                    foreach (var idx in indList) elist.Add(endings.ElementAt(idx));

                    outDict.Add(item.Key, elist);
                }
            }

            return outDict;
        }

        /// <summary>
        /// Produces a list of all paths that lead to the specified ending.
        /// </summary>
        /// <param name="ending"></param>
        /// <returns>List of List of actions.</returns>
        public List<List<A>> AllPathsTo(E ending)
        {
            var output = new List<List<A>>();

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

        #endregion

        #region Private Functions

        

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
        /// [Internal] Returns a list of indices of all maximum values of the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>List of indices of max values of the vector.</returns>
        private List<int> FindEqualMaxValues(int[] vector)
        {
            var indices = new List<int>();
            var max = vector[0];
            indices.Add(0);

            for(var i = 1; i < vector.Length; i++)
            {
                if (vector[i] > max)
                {
                    max = vector[i];
                    indices.Clear();
                    indices.Add(i);
                }
                else if (vector[i] == max)
                {
                    indices.Add(i);
                }
            }

            return indices;
        }

        #endregion
    }
}
