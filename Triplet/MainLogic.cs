using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Triplet
{
    public interface IMainLogic
    {
        bool CheckToExistFile(string pathToFile);
        Dictionary<string, int> GetAllTriplet(string pathToFile, CancellationTokenSource cts);
    }

    public class MainLogic : IMainLogic
    {
        protected int changeCout;
        ConcurrentDictionary<string, int> allTriplet;
        CancellationTokenSource cts;

        public MainLogic()
        {
            allTriplet = new ConcurrentDictionary<string, int>();
        }

        public bool CheckToExistFile(string pathToFile)
        {
            bool result = System.IO.File.Exists(pathToFile);
            return result;
        }

        private string[] ReadFromFile(string pathToFile)
        {
            return File.ReadAllLines(pathToFile);
        }

        public Dictionary<string, int> GetAllTriplet(string pathToFile, CancellationTokenSource cts)
        {
            this.cts = cts;
            string[] allLine = ReadFromFile(pathToFile);
            DateTime startTime = DateTime.Now;
            ConcurrentDictionary<string, int> localAllTriplet = new ConcurrentDictionary<string, int>();
            foreach (string line in allLine)
            {
                TripletInLineAsync(line);
                if (cts.Token.IsCancellationRequested)
                    break;
            }
            return allTriplet.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private async void TripletInLineAsync(string line)
        {
            await Task.Run(() => TripletInLine(line));
        }

        private void TripletInLine(string line)
        {
            string[] wordInLine = line.Split();
            string triplet;
            foreach(string word in wordInLine)
            {
                for(int i=0; i<word.Length-2; i++)
                {
                    if (cts.Token.IsCancellationRequested)
                        break;
                    triplet = word.Substring(i, 3);
                    if (allTriplet.ContainsKey(triplet))
                        allTriplet.AddOrUpdate(triplet, 1, (key, oldValue) => oldValue + 1);
                    else
                        allTriplet.TryAdd(triplet, 1);
                }
                if (cts.Token.IsCancellationRequested)
                    break;
            }
        }
    }
}
