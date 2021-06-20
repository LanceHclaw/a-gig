using System;
using System.Collections.Generic;
using ProgressionVector;

namespace PVA_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<IComparable<string>, IEnumerable<int>> actions = new Dictionary<IComparable<string>, IEnumerable<int>> {
                { "pistol", new int[] { 0, 4, 1 } },
                { "dagger", new int[] { 3, 1, 4 } },
                { "axe", new int[] { 4, 1, 1 } },
                { "bow", new int[] { 1, 3, 2 } }
            };
            List<Endings> endings = new List<Endings> { Endings.melee, Endings.ranged, Endings.versatile };

            var classSelection = new ProgressionVector<string, Endings>(endings, actions);
            classSelection.ActionCompleted("pistol");
            classSelection.ActionCompleted("dagger");
            classSelection.ActionCompleted("axe");
            classSelection.ActionCompleted("bow");
            //Console.WriteLine(classSelection.GetEnding().ToString());

            WriteAllEndings(classSelection.GetAllPaths());
            Console.WriteLine("-------------------------");
            foreach(var path in classSelection.AllPathsTo(Endings.ranged))
            {
                foreach (var action in path)
                {
                    Console.Write(action.ToString() + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("--------------------------");
            WriteAllOverlaps(classSelection.GetOverlaps());

            Console.WriteLine("-------------------------");
            foreach(var end in classSelection.GetEndingFrequencies())
            {
                Console.WriteLine(end.Key.ToString() + " " + end.Value);
            }

            Console.WriteLine("-------------------------");
            foreach (var act in classSelection.GetActionWeightForEnding(Endings.ranged))
            {
                Console.WriteLine(act.Key.ToString() + " " + act.Value);
            }
        }

        private static void WriteAllEndings(Dictionary<List<IComparable<string>>, Endings> paths)
        {
            foreach(var path in paths)
            {
                foreach(var act in path.Key)
                {
                    Console.Write(act.ToString() + " ");
                }
                Console.WriteLine(path.Value);
            }
        }

        private static void WriteAllOverlaps(Dictionary<List<IComparable<string>>, List<Endings>> paths)
        {
            foreach (var path in paths)
            {
                foreach (var act in path.Key)
                {
                    Console.Write(act.ToString() + " ");
                }
                Console.Write(" || ");
                foreach (var end in path.Value)
                {
                    Console.Write(end + " ");
                }
                Console.WriteLine();
            }
        }
    }
    enum Endings
    {
        melee,
        ranged,
        versatile
    }
}
