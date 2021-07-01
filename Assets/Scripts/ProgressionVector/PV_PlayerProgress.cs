using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressionVector
{
    public class PV_PlayerProgress<E> 
        where E : IComparable
    {
        public Dictionary<PV_Action<E>, bool> actionFlags { get; private set; }

        public PV_PlayerProgress(PV_QuestData<E> questData)
        {
            actionFlags = new Dictionary<PV_Action<E>, bool>();
            foreach(var action in questData.all_majorActions) 
            {
                actionFlags.Add(action, false);
            }
        }

        public void ActionCompleted(PV_Action<E> action)
        {
            if (actionFlags.Keys.Contains(action) &&
                actionFlags[action] == false)
            {
                actionFlags[action] = true;
            }
            else throw new ArgumentException("This action has already been completed or does not belong to this quest.");
        }

        public void RemoveAction(PV_Action<E> action)
        {
            if (actionFlags.Keys.Contains(action) &&
                actionFlags[action] == true)
            {
                actionFlags[action] = false;
            }
            else throw new ArgumentException("This action has not been completed earlier or does not belong to this quest.");
        }

        public E FinishQuest(PV_QuestData<E> questData)
        {
            return questData.FinishQuest(this);
        }
    }
}
