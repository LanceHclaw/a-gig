/*using System;
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



        

       



       

        

        #endregion
    }
}
*/