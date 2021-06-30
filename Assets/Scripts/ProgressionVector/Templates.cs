using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressionVector
{
    public class PV_Action<E>
        where E : IComparable
    {
        public string PV_name; // optional

        public Dictionary<E, int> weights { get; protected set; }

        public PV_Action(IEnumerable<(E, int)> weights, string name = "")
        {
            this.weights = new Dictionary<E, int>();
            foreach (var tuple in weights)
            {
                this.weights.Add(tuple.Item1, tuple.Item2);
            }

            PV_name = name;
        }

        public PV_Action(Dictionary<E, int> weights, string name = "")
        {
            this.weights = weights;

            PV_name = name;
        }

        protected PV_Action(string name = "") { PV_name = name; }

        public override string ToString()
        {
            return PV_name;
        }
    }

    public class PV_OutputVector<E> : PV_Action<E>
        where E : IComparable
    {
        public PV_OutputVector(PV_QuestData<E> data)
        {
            weights = new Dictionary<E, int>();
            foreach (var item in data.all_endings) weights[item] = 0;
        }
    }

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
