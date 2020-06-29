using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;


namespace DAN_XLI_Milica_Karetic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// backgroundworker instance
        /// </summary>
        static BackgroundWorker backgroundWorker;
        /// <summary>
        /// text to print
        /// </summary>
        static string text;
        /// <summary>
        /// number of copies
        /// </summary>
        static string copyNum;

        /// <summary>
        /// MainWindow constructor
        /// </summary>
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
        
        /// <summary>
        /// Method for printing text to file
        /// </summary>
        /// <param name="fileName">File location</param>
        /// <param name="text">Text to print</param>
        private void Print(string fileName, string text)
        {
            string file = fileName + ".txt";

            StreamWriter sw = new StreamWriter(file);
            using (sw)
            {
                sw.WriteLine(text);
            }
        }

        /// <summary>
        /// Do work method implementation
        /// </summary>
        /// <remarks>
        /// Print typed text into files
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //calculate percentage
            int c = 100 / int.Parse(copyNum);

            //for loop for printing text to specified number of files(number of copies)
            for (int i = 1; i <= int.Parse(copyNum); i++)
            {
                Thread.Sleep(1000);
                string fileName = (i).ToString() + "." + DateTime.Now.ToString("dd_MM_yyyy_hh_mm");
                
                //if cancel print button clicked
                if(backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    backgroundWorker.ReportProgress(0);
                    return;
                }
                //call print method for printing text to files
                Print(fileName, text);
                backgroundWorker.ReportProgress(i * c);
            }
            //completed message
            e.Result = "Completed";
        }

        /// <summary>
        /// Method for displaying result message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if cancel button is clicked
            if (e.Cancelled)
            {
                lblMessage.Content = "Processing cancelled";
            }
            //if errro occured
            else if (e.Error != null)
            {
                lblMessage.Content = e.Error.Message;
            }
            //if printing is done
            else
            {
                lblMessage.Content = e.Result.ToString();
                lblErr.Content = "";
            }
                    
        }

        /// <summary>
        /// Method for displaying progress bar work
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblMessage.Content = e.ProgressPercentage.ToString() + " %";
        }

        /// <summary>
        /// Button cancel print click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Button print click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if(!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
                btnCancel.IsEnabled = true;
            }
            else
            {
                lblErr.Content = "Printer is busy, please wait";
            }
           
        }

        /// <summary>
        /// Method for text change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtText_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox objTextBox = (TextBox)sender;
            text = objTextBox.Text;
            if (text != null && copyNum != null)
            {
                btnPrint.IsEnabled = true;
            }
                
        }

        /// <summary>
        /// Method for number of copies changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtCopyNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox objTextBox = (TextBox)sender;
            copyNum = objTextBox.Text;

            if (text != null && copyNum != null)
            {
                if(int.TryParse(copyNum, out int num))
                {
                    btnPrint.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Please enter number");
                }
            }
                
        }
    }
}
