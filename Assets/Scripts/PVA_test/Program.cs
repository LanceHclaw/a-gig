using System;
using System.Collections.Generic;
using ProgressionVector;

namespace PVA_test
{
    public class Ending
    {
        private static int next_id = 0;
        public static int GetNumberOfEndings()
        {
            return next_id;
        }

        public readonly int ID;
        public Ending()
        {
            this.ID = next_id++;
        }
    }

    public class Action 
    {
        private static int next_id = 0;

        public readonly int ID;
        public Action(params Tuple<Ending, int>[] weights)
        {
            this.ID = next_id++;
        }
    }


    static class MyEndings
    {
        public static readonly Ending Melee = new Ending();
        public static readonly Ending Ranged = new Ending();
        public static readonly Ending Versatile = new Ending();
    }

    static class MyActions
    {
        public static readonly Action Pistol = new Action();
        public static readonly Ending Ranged = new Ending();
        public static readonly Ending Versatile = new Ending();
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Endings> endings = new List<Endings> { Endings.melee, Endings.ranged, Endings.versatile };
            Dictionary<IComparable<string>, IEnumerable<int>> actions = new Dictionary<IComparable<string>, IEnumerable<int>> {
                { MyActions.Pistol,       new int[] { 0, 4, 1 } },
                { "dagger",       new int[] { 3, 1, 4 } },
                { "axe",          new int[] { 4, 1, 1 } },
                { "bow",          new int[] { 1, 3, 2 } },
                { "small_stone",  new int[] { 0, 1, 0 } }
            };
            
            var classSelection = new ProgressionVector<string, Endings>(endings, actions);
            classSelection.ActionCompleted("pistol");
            classSelection.ActionCompleted("dagger");
            classSelection.ActionCompleted("axe");
            classSelection.ActionCompleted("bow");
            //classSelection.ActionCompleted("small_stone");
            //Console.WriteLine(classSelection.GetEnding().ToString());

            WriteAllEndings(classSelection.GetAllPaths());
            Console.WriteLine("-------------------------AllPathsTo(Endings.ranged)");
            foreach(var path in classSelection.AllPathsTo(Endings.ranged))
            {
                foreach (var action in path)
                {
                    Console.Write(action.ToString() + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("--------------------------GetOverlaps");
            WriteAllOverlaps(classSelection.GetOverlaps());

            Console.WriteLine("-------------------------GetEndingFrequencies");
            foreach(var end in classSelection.GetEndingFrequencies())
            {
                Console.WriteLine(end.Key.ToString() + " " + end.Value);
            }

            Console.WriteLine("-------------------------GetActionWeightForEnding");
            foreach (var act in classSelection.GetActionWeightForEnding(Endings.ranged))
            {
                Console.WriteLine(act.Key.ToString() + " " + act.Value);
            }

            Console.WriteLine("-------------------------GetActionDirection('dagger')");
            foreach (var end in classSelection.GetActionDirection("dagger"))
            {
                Console.WriteLine(end.Key.ToString() + " " + end.Value);
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
