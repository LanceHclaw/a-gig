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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="name">ToString() will return this parameter which is empty by default.</param>
        public PV_Action(IEnumerable<(E, int)> weights, string name = "")
        {
            this.weights = new Dictionary<E, int>();
            foreach (var tuple in weights)
            {
                this.weights.Add(tuple.Item1, tuple.Item2);
            }

            PV_name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="name">ToString() will return this parameter which is empty by default.</param>
        public PV_Action(Dictionary<E, int> weights, string name = "")
        {
            this.weights = weights;

            PV_name = name;
        }

        /// <summary>
        /// Only use this for minor actions to avoid null-reference errors
        /// </summary>
        /// <param name="name">ToString() will return this parameter which is empty by default.</param>
        public PV_Action(string name = "") { PV_name = name; }

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
}
