using System;
using System.Collections.Generic;
using ProgressionVector;

namespace PVA_test
{
    class Program
    {
        class TestClass : PV_Action<Endings>
        {
            string boop;

            public TestClass() : base(new List<(Endings, int)> { (Endings.melee, 1), (Endings.ranged, 2) } ) 
            {
                boop = "boop";
            }
        }

        static void Main(string[] args)
        {
            Dictionary<string, IEnumerable<int>> actions = new Dictionary<string, IEnumerable<int>> {
                { "pistol", new int[] { 0, 4, 1 } },
                { "dagger", new int[] { 3, 1, 4 } },
                { "axe", new int[] { 4, 1, 1 } },
                { "bow", new int[] { 1, 3, 2 } },
                { "small_stone", new int[] { 0, 3, 0 } }
            };
            List<Endings> endings = new List<Endings> { Endings.melee, Endings.ranged, Endings.versatile };

            var classSelection = new ProgressionVector<Endings>(endings, actions);
            classSelection.ActionCompleted("pistol");
            classSelection.ActionCompleted("dagger");
            classSelection.ActionCompleted("axe");
            classSelection.ActionCompleted("bow");
            classSelection.ActionCompleted("small_stone");
            //Console.WriteLine(classSelection.GetEnding().ToString());


            var test = new TestClass();
            var testQuest = new PV_QuestData<Endings>(endings, new List<PV_Action<Endings>> { test } );

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

            Console.WriteLine("-------------------------");
            foreach (var end in classSelection.GetActionDirection("dagger"))
            {
                Console.WriteLine(end.Key.ToString() + " " + end.Value);
            }
        }

        private static void WriteAllEndings(Dictionary<List<string>, Endings> paths)
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

        private static void WriteAllOverlaps(Dictionary<List<string>, List<Endings>> paths)
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
