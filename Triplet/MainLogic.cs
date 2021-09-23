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
        Dictionary<string, int> GetAllDoubling(string pathToFile, CancellationTokenSource cts, int countWord);
    }

    public class MainLogic : IMainLogic
    {
        protected int changeCout;
        ConcurrentDictionary<string, int> allRepetition, resultAllTriplet;

        CancellationTokenSource cts;

        public MainLogic()
        {
            allRepetition = new ConcurrentDictionary<string, int>();
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

        public Dictionary<string, int> GetAllDoubling(string pathToFile, CancellationTokenSource cts, int countWord)
        {
            this.cts = cts;
            string[] allLine = ReadFromFile(pathToFile);
            DateTime startTime = DateTime.Now;

            //Parallel.ForEach(allLine, TripletInLine);
            Parallel.ForEach(allLine, new ParallelOptions { MaxDegreeOfParallelism = countWord }, (q) => AmountCollocationInLine(q, countWord)) ;
            return allRepetition.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private void TripletInLine(string line)
        {
            string[] wordInLine = line.Split();
            string triplet;
            foreach(string word in wordInLine)
            {
                for(int i=0; i<word.Length-2; i++)
                {
                    triplet = word.Substring(i, 3);
                    if (allRepetition.ContainsKey(triplet))
                        allRepetition.AddOrUpdate(triplet, 1, (key, oldValue) => oldValue + 1);
                    else
                        allRepetition.TryAdd(triplet, 1);
                    if (cts.Token.IsCancellationRequested)
                        break;
                }
                if (cts.Token.IsCancellationRequested)
                    break;
            }
        }

        private void AmountCollocationInLine(string line, int countWordinPhrases)
        {
            string[] wordInLine = TextNormalization(line).Split();
            string phrases = "";
            if(wordInLine.Length >= countWordinPhrases)
            {
                for(int i = 0; i < wordInLine.Length - countWordinPhrases; i++)
                {
                    phrases = "";
                    for (int j = 0; j < countWordinPhrases; j++)
                    {
                        phrases += wordInLine[i+j];
                        if (j != countWordinPhrases - 1)
                            phrases += " ";

                        if (cts.Token.IsCancellationRequested)
                            break;
                    }
                    if (allRepetition.ContainsKey(phrases))
                        allRepetition.AddOrUpdate(phrases, 1, (key, oldValue) => oldValue + 1);
                    else
                        allRepetition.TryAdd(phrases, 1);
                    if (cts.Token.IsCancellationRequested)
                        break;
                }
            }
        }

        private string TextNormalization(string line)
        {
            line = line.Replace(",", ", ");
            while (line.Contains("  ")) 
            { 
                line = line.Replace("  ", " "); 
            }
            return line;
        }
    }
}
