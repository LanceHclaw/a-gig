using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
            public static readonly Ending notReady = new Ending("notReady");

            public static IEnumerable<Ending> GetAllEndings()
            {
                return new List<Ending>{ melee, ranged, versatile };
            }
        }

        static void Main(string[] args)
        {
            PV_QuestData<Ending> myQuest = new PV_QuestData<Ending>(MyEndings.GetAllEndings(), MyActions.GetAllActions(), DEBUG: true);
            PV_PlayerProgress<Ending> myPlayerProgress = new PV_PlayerProgress<Ending>(myQuest);

            myQuest.DefineZeroSpace(MyEndings.notReady, 3);
            myQuest.AddExtraCondition(MyEndings.melee, (ending, progress) =>
            {
                return progress.actionFlags[MyActions.axe];
            });

            Console.WriteLine("\nmyQuest.GetAllPaths()-----------------------------------");
            PrintDictionaryList(myQuest.Debug_GetAllPaths());

            Console.WriteLine("\nmyQuest.GetAllPathsFrom(MyActions.axe, MyActions.bow)-----------------------------------");
            PrintDictionaryList(myQuest.Debug_GetAllPathsFrom(new List<PV_Action<Ending>> { MyActions.axe, MyActions.bow }));

            Console.WriteLine("\nmyQuest.GetAllPathsFrom(playerProgressVersion)(MyActions.axe, MyActions.bow)-----------------------------------");
            myPlayerProgress.ActionCompleted(MyActions.axe);
            myPlayerProgress.ActionCompleted(MyActions.bow);
            PrintDictionaryList(myQuest.Debug_GetAllPathsFrom(myPlayerProgress));
            myPlayerProgress.RemoveAction(MyActions.axe);
            myPlayerProgress.RemoveAction(MyActions.bow);

            Console.WriteLine("\nmyQuest.GetAllPathsFromTo(axe, bow, RANGED)-----------------------------------");
            PrintDictionaryList(myQuest.Debug_GetAllPathsFromTo(new List<PV_Action<Ending>> { MyActions.axe, MyActions.bow }, MyEndings.ranged));

            Console.WriteLine("\nmyQuest_PV.GetAllPathsFrom(playerProgressVersion)(axe, bow, RANGED)-----------------------------------");
            myPlayerProgress.ActionCompleted(MyActions.axe);
            myPlayerProgress.ActionCompleted(MyActions.bow);
            PrintDictionaryList(myQuest.Debug_GetAllPathsFromTo(myPlayerProgress, MyEndings.ranged));
            myPlayerProgress.RemoveAction(MyActions.axe);
            myPlayerProgress.RemoveAction(MyActions.bow);

            Console.WriteLine("\nGetActionFrequenciesForEnding(MyEndings.versatile)-----------------------------------");
            PrintDictionary(myQuest.Debug_GetActionFrequenciesForEnding(MyEndings.versatile));

            Console.WriteLine("\nGetActionDirection(MyActions.axe)-----------------------------------");
            PrintDictionary(myQuest.Debug_GetActionDirection(MyActions.axe));

            Console.WriteLine("\nAllPathsTo(MyEndings.versatile)-----------------------------------");
            PrintListOfList(myQuest.Debug_AllPathsTo(MyEndings.versatile));

            Console.WriteLine("\nGetOverlaps()-----------------------------------");
            PrintDictionaryListList(myQuest.Debug_GetOverlaps());

            Console.WriteLine("\nGetPathOutput(MyPath {bow, dagger})-----------------------------------");
            var MyPath = new Dictionary<PV_Action<Ending>, bool>
            {
                {MyActions.bow, true },
                {MyActions.dagger, true }
            };
            Console.WriteLine(myQuest.Debug_GetPathOutput(MyPath).ToString());

            myQuest.DefineZeroSpace(MyEndings.notReady, 3);

            Console.WriteLine("\nProduceFinalEnding(empty player progress) threshold 3 -----------------------------------");
            Console.WriteLine(myPlayerProgress.FinishQuest(myQuest).ToString());

            Console.WriteLine("\nProduceFinalEnding(bow, pistol) threshold 3 -----------------------------------");
            myPlayerProgress.ActionCompleted(MyActions.bow);
            myPlayerProgress.ActionCompleted(MyActions.pistol);
            Console.WriteLine(myPlayerProgress.FinishQuest(myQuest).ToString());

            Console.WriteLine("\nProduceFinalEnding(small_stone) threshold 3 -----------------------------------");
            myPlayerProgress.RemoveAction(MyActions.bow);
            myPlayerProgress.RemoveAction(MyActions.pistol);
            myPlayerProgress.ActionCompleted(MyActions.small_stone);
            Console.WriteLine(myPlayerProgress.FinishQuest(myQuest).ToString());

            Console.WriteLine("\nProduceFinalEnding(small_stone) WITH melee threshold = 0; globalThreshold = 3 -----------------------------------");
            myQuest.DefineIndividualThresholds(new Dictionary<Ending, int> { { MyEndings.melee, 0 } });
            Console.WriteLine(myPlayerProgress.FinishQuest(myQuest).ToString());

            
            Console.WriteLine("\nProduceFinalEnding(small_stone) WITH extra condition on axe and melee threshold = 0; globalThreshold = 3 -----------------------------------");
            myQuest.AddExtraCondition(MyEndings.melee, (ending, progress) =>
            {
                return progress.actionFlags[MyActions.axe];
            });
            Console.WriteLine(myPlayerProgress.FinishQuest(myQuest).ToString());

            Console.WriteLine("\nProduceFinalEnding(small_stone, axe) WITH extra condition on axe and melee threshold = 0; globalThreshold = 3 -----------------------------------");
            myPlayerProgress.ActionCompleted(MyActions.axe);
            Console.WriteLine(myPlayerProgress.FinishQuest(myQuest).ToString());
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
