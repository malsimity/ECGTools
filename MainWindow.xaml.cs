using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            // Считывание и преобразование данных
            string filleName = @"D:\Neurosoft\MyPROJECT\DataSet\Тестовые записи Accordix\ecg_record (24).edf";
            double constant = 0; // берём первый элемент массива из матлаба
            var edfFile = new EDFFile(filleName);
            int freq = (int)edfFile.Signals[0].FrequencyInHZ;
            double[] signal = new double[edfFile.Signals[0].SamplesCount];
            signal[0] = edfFile.Signals[0].ScaledSample(0) - constant;
            double[] x = new double[signal.Length];
            for (int i = 1; i < signal.Length; i++)
            {
                signal[i] = (float)edfFile.Signals[0].ScaledSample(i);
                signal[i] -= Math.Abs(signal[0]);
                x[i] = i / (double)freq;
            }
            signal[0] = constant;
            //x = x.Take(10000).ToArray();
            //signal = signal.Take(10000).ToArray();
            Plot.Plot.AddSignalXYConst(x, signal);
            Plot.Refresh();
        }
    }
}
