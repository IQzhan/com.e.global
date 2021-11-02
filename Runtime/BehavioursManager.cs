using System;
using System.Collections.Generic;

namespace E
{
    internal partial class Behaviours
    {
        private struct BehaviourIndex
        {
            public int type;

            public int index;
        }

        private SortedList<Type, List<GlobalBehaviour>> allBehaviours;

        private List<GlobalBehaviour> GetBehavioursByType(Type type)
        {
            if (allBehaviours == null)
            {
                allBehaviours = new SortedList<Type, List<GlobalBehaviour>>();
            }
            if (!allBehaviours.TryGetValue(type, out List<GlobalBehaviour> behaviours))
            {
                behaviours = new List<GlobalBehaviour>();
                allBehaviours.Add(type, behaviours);
            }
            return behaviours;
        }

        private void AddByType(in Type type, in GlobalBehaviour behaviour)
        {
            List<GlobalBehaviour> behaviours = GetBehavioursByType(type);
            if (!behaviours.Contains(behaviour))
            {
                behaviours.Add(behaviour);
            }
        }

        private List<GlobalBehaviour> GetByType(in Type type)
        {
            return GetBehavioursByType(type);
        }

        private List<GlobalBehaviour> GetByIndex(in int typeIndex)
        {
            return allBehaviours.Values[typeIndex];
        }

        private GlobalBehaviour GetByIndex(in int typeIndex, in int index)
        {
            return allBehaviours.Values[typeIndex][index];
        }

        private void Remove()
        {

        }
    }
}
