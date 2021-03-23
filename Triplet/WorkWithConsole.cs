using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Triplet
{
    public interface IWorkWithConsole
    {
        string InputPath();
        void MessageFileNotExist();
        void OutputResult(Dictionary<string, int> triplet, DateTime time);
        void CancelApp(CancellationTokenSource cts);
    }

    public class WorkWithConsole : IWorkWithConsole
    {
        public string InputPath()
        {
            string path;
            Console.WriteLine("Input path to file: ");
            path = Console.ReadLine();
            return path;
        }

        public void MessageFileNotExist()
        {
            Console.WriteLine("File not exist!");
        }

        public void OutputResult(Dictionary<string, int> triplet, DateTime time)
        {
            string result="";
            int count = 10, i=0;
            foreach(string key in triplet.Keys)
            {
                if (i == count)
                    break;
                if (result != "")
                    result += ", ";
                result += key + "=" + triplet[key];
                i++;
            }
            result += "\n" + (DateTime.Now - time).TotalMilliseconds;
            Console.WriteLine(result);
        }

        public void CancelApp(CancellationTokenSource cts)
        {
            Console.WriteLine("***\nFor cancel press any key\n***");
            Console.ReadKey();
            cts.Cancel();
        }
    }
}
