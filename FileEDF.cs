using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EDFCSharp;
using Microsoft.Win32;

namespace ECGTools
{
    public class Signal
    {
        public double[] signal { get; }

        public double[] x { get; }

        public int freq { get; }

        public Signal(double[] signal, int freq)
        {
            this.freq = freq;
            this.signal = signal;
            x = new double[signal.Length];
            for (int i = 0; i < signal.Length; i++)
            {
                x[i] = i / (double)freq;
            }
        }
    }
    static public class FileEDF
    {
        static public Signal ImportEdfSignal()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "EDF-файл (*.edf)|*.edf" };

            if (openFileDialog.ShowDialog() == true)
            {
                EDFSignal signal = new EDFFile(openFileDialog.FileName).Signals[0];
                double[] sig = new double[signal.SamplesCount];
                for (int i = 0; i < signal.SamplesCount; i++)
                {
                    sig[i] = signal.ScaledSample(i);
                }
                int freq = (int)signal.FrequencyInHZ;
                return new Signal(sig, freq);
            }
            else
            {
                return null;
            }
        }

        static public List<QRS> ImportDataAnaliz()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "analiz-файл (*.analiz)|*.analiz" };

            if (openFileDialog.ShowDialog() == false)
                return null;

            string[] text = System.IO.File.ReadAllLines(openFileDialog.FileName);

            List<QRS> qrs = new List<QRS>();
            for (int i = 0; i < text.Length; i++)
            {
                string[] line = text[i].Split(' ');
                int rPeak = Convert.ToInt32(line[0]);
                double ampl = Convert.ToDouble(line[1]);
                int qrsClass = Convert.ToInt32(line[2]);
                qrs.Add(new QRS(rPeak, ampl, qrsClass));
            }
            return qrs;
        }

        static public void ExportEdfSignal(List<QRS> qrs)
        {
            if (qrs == null) { MessageBox.Show("Записывать нечего!"); return; }

            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "analiz-файл (*.analiz)|*.analiz" };
            if (saveFileDialog.ShowDialog() == false)
                return;


            string filename = saveFileDialog.FileName;

            string[] text = new string[qrs.Count];
            for (int i = 0; i < text.Length; i++)
                text[i] = qrs[i].rPeak + " " + qrs[i].amplitude + " " + qrs[i].numberClass;
            System.IO.File.WriteAllLines(filename, text);
        }
    }
}
