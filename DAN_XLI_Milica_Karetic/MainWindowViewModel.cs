using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DAN_XLI_Milica_Karetic
{
    public class MainWindowViewModel : BaseViewModel
    {
        private static bool _isRunning;
        private string _buttonLabel;
        private int currentProgress;
        private ICommand _command;
        private BackgroundWorker worker = new BackgroundWorker();

        public MainWindowViewModel()
        {
            worker.DoWork += DoWork;
            worker.ProgressChanged += ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            CurrentProgress = 0;
            _isRunning = true;
            ButtonLabel = "GO";
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (CurrentProgress >= 100)
            {
                CurrentProgress = 0;
            }

            while (CurrentProgress < 100 && !_isRunning)
            {
                worker.ReportProgress(CurrentProgress);
                Thread.Sleep(100);
                CurrentProgress++;
            }

            _isRunning = true;
        }

        public ICommand Command
        {
            get
            {
                return _command ?? (_command = new RelayCommand(x =>
                {
                    _isRunning = !_isRunning;

                    if (!_isRunning)
                    {
                        DoStuff();
                    }
                    else
                    {
                        ButtonLabel = "PAUSED";
                    }
                }));
            }
        }

        public int CurrentProgress
        {
            get { return currentProgress; }
            private set
            {
                if (currentProgress != value)
                {
                    currentProgress = value;
                    OnPropertyChanged("CurrentProgress");
                }
            }
        }

        public string ButtonLabel
        {
            get { return _buttonLabel; }
            private set
            {
                if (_buttonLabel != value)
                {
                    _buttonLabel = value;
                    OnPropertyChanged("ButtonLabel");
                }
            }
        }

        private void DoStuff()
        {
            ButtonLabel = "GO";
            worker.RunWorkerAsync();
        }
    }
}
