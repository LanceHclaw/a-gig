using System;
using System.Collections;
using System.Collections.Generic;

namespace ProgressionVector
{
    public class ProgressionVector<A, E>
    {
        public IEnumerable endings;
        public IEnumerable all_actions;
        public IEnumerable<int> action_weights;

        public ProgressionVector(IEnumerable<E> endings, IEnumerable<A> all_actions, IEnumerable<int> weights)
        {
            this.all_actions = all_actions;
            this.action_weights = weights;
            this.endings = endings;
        }
    }
}
