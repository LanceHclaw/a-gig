﻿using System;
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
    }
    enum Endings
    {
        melee,
        ranged,
        versatile
    }
}
