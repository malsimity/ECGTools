using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using EDFCSharp;
using Color = System.Drawing.Color;

namespace ECGTools
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Signal signal = null;
        private Signal filtSig = null;
        private List<QRS> qrs = null;

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PlotSig(Signal signal)
        {
            Plot.Plot.AddSignalXYConst(signal.x, signal.signal);
            Plot.Refresh();
        }

        private void PlotQRS(List<QRS> qrsList, Signal sig)
        {
            double[] xs = new double[qrsList.Count];
            double[] ys = new double[qrsList.Count];
            for (int i = 0; i < qrsList.Count; i++)
            {
                xs[i] = sig.x[qrsList[i].rPeak];
                ys[i] = sig.signal[qrsList[i].rPeak];
            }

            Plot.Plot.AddScatter(xs, ys, Color.Crimson, 0, 10f);
            Plot.Refresh();
        }

        private void Menu_Import_Open_OnClick(object sender, RoutedEventArgs e)
        {
            Signal signalBuf = FileEDF.ImportEdfSignal();
            if (signalBuf != null)
                signal = signalBuf;
            else
                return;
            Plot.Plot.Clear();
            filtSig = null;
            qrs = null;
            lbCountQRS.Content = "0";
            PlotSig(signal);
        }

        private void CbFilt_OnChecked(object sender, RoutedEventArgs e)
        {
            if (signal == null)
            {
                MessageBox.Show("Ипортируйте данные!");
                cbFilt.IsChecked = false;
                return;
            }

            int freq = signal.freq;
            int pow = 0;
            while (Math.Pow(2, pow) < signal.signal.Length)
            {
                pow++;
            }

            double[] sigBuf = new double[(int)Math.Pow(2, pow)];
            for (int i = 0; i < signal.signal.Length; i++)
                sigBuf[i] = signal.signal[i];
            for (int i = signal.signal.Length; i < (int)Math.Pow(2, pow); i++)
                sigBuf[i] = 0;
            
            sigBuf = FftSharp.Filter.BandPass(sigBuf, freq, Convert.ToDouble(tbLowfreq.Text), Convert.ToDouble(tbHightfreq.Text));
            double[] filtSigBuf = new double[signal.signal.Length];
            for (int i = 0; i < signal.signal.Length; i++)
                filtSigBuf[i] = sigBuf[i];

            filtSig = new Signal(filtSigBuf, freq);

            Plot.Plot.Clear();
            PlotSig(filtSig);
            if (qrs != null)
                PlotQRS(qrs, filtSig);
            tbLowfreq.IsEnabled = false;
            tbHightfreq.IsEnabled = false;
        }

        private void CbFilt_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (signal == null)
            {
                return;
            }

            Plot.Plot.Clear();
            PlotSig(signal);
            if (qrs != null)
                PlotQRS(qrs, signal);
            tbLowfreq.IsEnabled = true;
            tbHightfreq.IsEnabled = true;
        }

        private void BtDetect_OnClick(object sender, RoutedEventArgs e)
        {
            Signal bufSig = null;
            if (signal == null)
            {
                MessageBox.Show("Импортруйте сигнал!");
                return;
            }

            if (filtSig != null)
            {
                qrs = Detector.Detection(filtSig.signal, filtSig.freq);
                bufSig = filtSig;
            }
            else
            {
                qrs = Detector.Detection(signal.signal, signal.freq);
                bufSig = signal;
            }

            lbCountQRS.Content = qrs.Count.ToString();
            PlotQRS(qrs, bufSig);
        }

        private void Menu_Export_Open_OnClick(object sender, RoutedEventArgs e)
        {
            FileEDF.ExportEdfSignal(qrs);
        }

        private void Menu_Import_Anal_OnClick(object sender, RoutedEventArgs e)
        {
            qrs = FileEDF.ImportDataAnaliz();
            if (qrs == null)
                return;

            lbCountQRS.Content = qrs.Count;
            if (cbFilt.IsChecked == false)
            {
                PlotQRS(qrs, signal);
            }
            else
            {
                PlotQRS(qrs, filtSig);
            }
        }
    }
}
