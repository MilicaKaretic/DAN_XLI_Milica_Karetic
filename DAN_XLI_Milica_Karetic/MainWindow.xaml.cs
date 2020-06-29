using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DAN_XLI_Milica_Karetic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static BackgroundWorker backgroundWorker;
        static string text;
        static string copyNum;

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.DoWork += worker_DoWork;
            backgroundWorker.RunWorkerCompleted += worker_RunWorkerCompleted;
            backgroundWorker.ProgressChanged += worker_ProgressChanged;
        }
        
        private void Print(string fileName, string text)
        {
            string file = fileName + ".txt";

            StreamWriter sw = new StreamWriter(file);
            using (sw)
            {
                sw.WriteLine(text);
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int c = 100 / int.Parse(copyNum);
            btnCancel.IsEnabled = true;
            for (int i = 1; i <= int.Parse(copyNum); i++)
            {
                Thread.Sleep(1000);
                string fileName = (i).ToString() + "." + DateTime.Now.ToString("dd_MM_yyyy_hh_mm");
                Print(fileName, text);

                if(backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    backgroundWorker.ReportProgress(0);
                    return;
                }
                backgroundWorker.ReportProgress(i * c);
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                lblMessage.Content = "Processing cancelled";
            }
            else if (e.Error != null)
            {
                lblMessage.Content = e.Error.Message;
            }
                    
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblMessage.Content = e.ProgressPercentage.ToString() + " %";
        }

        private void CancelPrint_Click(object sender, RoutedEventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
            }
            else
            {
                lblErr.Content = "No operation in progress to cancel";
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if(!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }
            else
            {
                lblErr.Content = "Printer is busy, please wait";
            }
           
        }

        private void TxtText_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox objTextBox = (TextBox)sender;
            text = objTextBox.Text;
            if (text != null && copyNum != null)
                btnPrint.IsEnabled = true;
        }

        private void TxtCopyNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox objTextBox = (TextBox)sender;
            copyNum = objTextBox.Text;
            if (text != null && copyNum != null)
                btnPrint.IsEnabled = true;
        }
    }
}
