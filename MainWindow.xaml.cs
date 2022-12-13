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

        private EDFSignal signal = null;
        private double[] filtSig = null;

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void PlotSig(EDFSignal signal)
        {
            float[] sig = new float[signal.SamplesCount];
            float[] x = new float[sig.Length];
            int freq = (int)signal.FrequencyInHZ;
            for (int i = 0; i < sig.Length; i++)
            {
                sig[i] = (float)signal.ScaledSample(i);
                sig[i] -= Math.Abs(sig[0]);
                x[i] = i / (float)freq;
            }

            Plot.Plot.AddSignalXYConst(x, sig);
            Plot.Refresh();
        }

        public void PlotSig(double[] signal, int freq)
        {
            float[] x = new float[signal.Length];
            for (int i = 0; i < signal.Length; i++)
                x[i] = i / (float) freq;

            Plot.Plot.AddSignalXYConst(x, signal);
            Plot.Refresh();
        }

        private void Menu_Import_Open_OnClick(object sender, RoutedEventArgs e)
        {
            signal = FileEDF.ImportEdfSignal();
            PlotSig(signal);
        }

        private void CbFilt_OnChecked(object sender, RoutedEventArgs e)
        {
            if (signal == null)
            {
                MessageBox.Show("Ипортируйте данные!");
                return;
            }

            int freq = (int)signal.FrequencyInHZ;
            int pow = 0;
            while (Math.Pow(2, pow) < signal.SamplesCount)
            {
                pow++;
            }

            double[] sigBuf = new double[(int)Math.Pow(2, pow)];
            for (int i = 0; i < signal.SamplesCount; i++)
                sigBuf[i] = signal.ScaledSample(i);
            for (int i = (int)signal.SamplesCount; i < (int)Math.Pow(2, pow); i++)
                sigBuf[i] = 0;
            
            sigBuf = FftSharp.Filter.BandPass(sigBuf, freq, Convert.ToDouble(tbLowfreq.Text), Convert.ToDouble(tbHightfreq.Text));
            filtSig = new double[signal.SamplesCount];
            for (int i = 0; i < signal.SamplesCount; i++)
                filtSig[i] = sigBuf[i];

            Plot.Plot.Clear();
            PlotSig(filtSig, freq);
            tbLowfreq.IsEnabled = false;
            tbHightfreq.IsEnabled = false;
        }

        private void CbFilt_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (signal == null)
            {
                MessageBox.Show("Ипортируйте данные!");
                return;
            }

            Plot.Plot.Clear();
            PlotSig(signal);
            tbLowfreq.IsEnabled = true;
            tbHightfreq.IsEnabled = true;
        }
    }
}
