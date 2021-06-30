using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressionVector
{
    public class PV_QuestData<E>
        where E : IComparable
    {
        public string PV_name; // optional

        public List<E> all_endings { get; private set; }
        public List<PV_Action<E>> all_actions { get; private set; }

        public PV_QuestData(IEnumerable<E> endings, IEnumerable<PV_Action<E>> actions, string name = "")
        {
            this.all_endings = new List<E>(endings);

            foreach(var action in actions)
            {
                foreach(var endKey in action.weights.Keys)
                {
                    if (!all_endings.Contains(endKey)) throw new ArgumentException("Action has weight for a non-existent ending.");
                }
            }

            this.all_actions = new List<PV_Action<E>>(actions);

            PV_name = name;
        }

        public override string ToString()
        {
            return PV_name;
        }
    }    
}
