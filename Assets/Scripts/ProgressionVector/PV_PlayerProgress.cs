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

        public Dictionary<PV_Action<E>, bool> minorActionFlags { get; private set; }

        public PV_PlayerProgress(PV_QuestData<E> questData)
        {
            actionFlags = new Dictionary<PV_Action<E>, bool>();
            minorActionFlags = new Dictionary<PV_Action<E>, bool>();

            foreach(var action in questData.all_majorActions) 
            {
                actionFlags.Add(action, false);
            }

            foreach (var minAction in questData.all_minorActions)
            {
                minorActionFlags.Add(minAction, false);
            }
        }

        public PV_PlayerProgress(Dictionary<PV_Action<E>, bool> presetMajorActions, PV_QuestData<E> questData, Dictionary<PV_Action<E>, bool> presetMinorActions = null)
        {
            actionFlags = new Dictionary<PV_Action<E>, bool>();
            minorActionFlags = new Dictionary<PV_Action<E>, bool>();

            foreach (var action in questData.all_majorActions)
            {
                actionFlags.Add(action, false);
            }

            foreach (var minAction in questData.all_minorActions)
            {
                minorActionFlags.Add(minAction, false);
            }

            foreach (var comp in presetMajorActions)
            {
                if (comp.Value) actionFlags[comp.Key] = comp.Value;
            }

            if (presetMinorActions != null) 
            {
                foreach (var minAction in presetMinorActions)
                {
                    minorActionFlags[minAction.Key] = minAction.Value;
                }
            }
        }

        public PV_PlayerProgress(List<PV_Action<E>> presetCompletedActions, PV_QuestData<E> questData, List<PV_Action<E>> presetCompletedMinor = null)
        {
            actionFlags = new Dictionary<PV_Action<E>, bool>();
            minorActionFlags = new Dictionary<PV_Action<E>, bool>();

            foreach (var action in questData.all_majorActions)
            {
                actionFlags.Add(action, false);
            }

            foreach (var minAction in questData.all_minorActions)
            {
                minorActionFlags.Add(minAction, false);
            }

            foreach (var comp in presetCompletedActions)
            {
                actionFlags[comp] = true;
            }

            if (presetCompletedMinor != null)
            {
                foreach (var comp in presetCompletedMinor)
                {
                    minorActionFlags[comp] = true;
                }
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
