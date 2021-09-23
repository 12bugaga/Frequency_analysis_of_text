using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Triplet
{
    public interface IStartApp
    {
        void StartProgram(CancellationTokenSource cts);
    }
    class StartApp : IStartApp
    {
        private readonly IWorkWithConsole _workWithConsole;
        private readonly IMainLogic _mainLogic;

        public StartApp(IWorkWithConsole workWithConsole, IMainLogic mainLogic)
        {
            _workWithConsole = workWithConsole;
            _mainLogic = mainLogic;
        }

        public void StartProgram(CancellationTokenSource _cts)
        {
            CancellationTokenSource cts = _cts;
            string pathToFile = _workWithConsole.InputPath();
            int amountWordsInPhrases = _workWithConsole.InputAmountWords();
            Dictionary<string, int> allTriplet = new Dictionary<string, int>();
            if (!_mainLogic.CheckToExistFile(pathToFile))
                _workWithConsole.MessageFileNotExist();
            else
            {
                DateTime startTime = DateTime.Now;
                new Thread(delegate () {
                    allTriplet = _mainLogic.GetAllDoubling(pathToFile, cts, amountWordsInPhrases);
                    _workWithConsole.OutputResult(allTriplet, 50, startTime);
                }).Start();

                
                _workWithConsole.CancelApp(cts);
            }


        }
    }
}
