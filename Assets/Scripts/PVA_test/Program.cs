using System;
using System.Collections.Generic;
using ProgressionVector;

namespace PVA_test
{
    class Program
    {
        public static class MyActions
        {
            public static readonly PV_Action<Ending> pistol = new PV_Action<Ending>(
                new List<(Ending, int)> { (MyEndings.melee, 0), (MyEndings.ranged, 4), (MyEndings.versatile, 1) },
                "pistol"
            );
            public static readonly PV_Action<Ending> dagger = new PV_Action<Ending>(
                new List<(Ending, int)> { (MyEndings.melee, 3), (MyEndings.ranged, 1), (MyEndings.versatile, 4) },
                "dagger"
            );
            public static readonly PV_Action<Ending> axe = new PV_Action<Ending>(
                new List<(Ending, int)> { (MyEndings.melee, 4), (MyEndings.ranged, 1), (MyEndings.versatile, 1) },
                "axe"
            );
            public static readonly PV_Action<Ending> bow = new PV_Action<Ending>(
                new List<(Ending, int)> { (MyEndings.melee, 1), (MyEndings.ranged, 3), (MyEndings.versatile, 2) },
                "bow"
            );
            public static readonly PV_Action<Ending> small_stone = new PV_Action<Ending>(
                new List<(Ending, int)> { (MyEndings.melee, 0), (MyEndings.ranged, 1), (MyEndings.versatile, 0) },
                "small_stone"
            );

            public static IEnumerable<PV_Action<Ending>> GetAllActions()
            {
                return new List<PV_Action<Ending>> { pistol, bow, axe, dagger, small_stone};
            }
        }

        public class Ending : IComparable
        {
            public string name;
            public int CompareTo(object obj)
            {
                if (obj == null) return 1;
                return CompareTo(obj);
            }

            public override string ToString()
            {
                return name;
            }

            public Ending(string name = "") { this.name = name; }
        }

        public static class MyEndings
        {
            public static readonly Ending melee = new Ending("melee");
            public static readonly Ending ranged = new Ending("ranged");
            public static readonly Ending versatile = new Ending("versatile");

            public static IEnumerable<Ending> GetAllEndings()
            {
                return new List<Ending>{ melee, ranged, versatile };
            }
        }

        static void Main(string[] args)
        {
            PV_QuestData<Ending> myQuest = new PV_QuestData<Ending>(MyEndings.GetAllEndings(), MyActions.GetAllActions());
            PV_PlayerProgress<Ending> myPlayerProgress = new PV_PlayerProgress<Ending>(myQuest);
            ProgressionVector<Ending> myQuest_PV = new ProgressionVector<Ending>(myPlayerProgress, myQuest, DEBUG: true);

            Console.WriteLine("\nmyQuest_PV.GetAllPaths()-----------------------------------");
            PrintDictionaryList(myQuest_PV.GetAllPaths());

            Console.WriteLine("\nGetActionFrequenciesForEnding(MyEndings.versatile)-----------------------------------");
            PrintDictionary(myQuest_PV.GetActionFrequenciesForEnding(MyEndings.versatile));

            Console.WriteLine("\nGetActionDirection(MyActions.axe)-----------------------------------");
            PrintDictionary(myQuest_PV.GetActionDirection(MyActions.axe));

            Console.WriteLine("\nAllPathsTo(MyEndings.versatile)-----------------------------------");
            PrintListOfList(myQuest_PV.AllPathsTo(MyEndings.versatile));

            Console.WriteLine("\nGetOverlaps()-----------------------------------");
            PrintDictionaryListList(myQuest_PV.GetOverlaps());

            Console.WriteLine("\nGetPathOutput(MyPath {bow, dagger})-----------------------------------");
            var MyPath = new Dictionary<PV_Action<Ending>, bool>
            {
                {MyActions.bow, true },
                {MyActions.dagger, true }
            };
            Console.WriteLine(myQuest_PV.GetPathOutput(MyPath).ToString());

        }

        public static void PrintDictionary<T, N>(Dictionary<T, N> incDict)
        {
            foreach (var item in incDict)
            {
                Console.WriteLine(item.Key.ToString() + " " + item.Value);
            }
        }

        public static void PrintDictionaryList<T, N> (Dictionary<List<T>, N> incDict)
        {
            foreach (var item in incDict)
            {
                foreach (var subItem in item.Key)
                {
                    Console.Write(subItem.ToString() + " ");
                }
                Console.WriteLine(item.Value);
            }
        }

        public static void PrintDictionaryListList<T, N>(Dictionary<List<T>, List<N>> incDict)
        {
            foreach (var item in incDict)
            {
                foreach (var subItem in item.Key)
                {
                    Console.Write(subItem.ToString() + " ");
                }
                foreach (var subItem in item.Value)
                {
                    Console.Write(subItem.ToString() + " ");
                }
                Console.WriteLine();
            }
        }
        public static void PrintListOfList<T> (List<List<T>> incList)
        {
            foreach (var sublist in incList)
            {
                foreach (var item in sublist)
                {
                    Console.Write(item.ToString() + " ");
                }
                Console.WriteLine();
            }
        }
    }

}
