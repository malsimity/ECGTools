using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDFCSharp;

namespace ECGTools
{
    public class QRS
    {
        public int rPeak { get; }

        public double amplitude { get; }

        public int numberClass { get; set; }

        public QRS(int rPeak, double amplitude, int numberClass)
        {
            this.rPeak = rPeak;
            this.amplitude = amplitude;
            this.numberClass = numberClass;
        }

    }
    static class Detector
    {
        public static double QRSAmplitude(double[] signal)
        {
            int windows = signal.Length / 100;

            double[] amplSignal = new double[signal.Length / windows + 1];

            int j = 0;

            for (int i = 0; i < signal.Length - 2 * windows; i += windows)
            {
                int indMax = 0, indMin = 0;
                double max = 0, min = 1000;
                for (int k = i; k < i + windows; k++)
                {
                    if (signal[k] > max)
                    {
                        max = signal[k];
                        indMax = k;
                    }
                    if (signal[k] < min)
                    {
                        min = signal[k];
                        indMin = k;
                    }
                }
                amplSignal[j] = signal[indMax] - signal[indMin];
                j++;
            }

            Array.Sort(amplSignal);

            return amplSignal[amplSignal.Length / 2];
        }

        static public List<QRS> Detection(double[] signal, int freq)
        {
            double[] derivative = new double[signal.Length - 1];
            for (int i = 0; i < derivative.Length; i++)
            {
                derivative[i] = (signal[i] - signal[i + 1]) * (signal[i] - signal[i + 1]);
            }

            double max = 0.5 * QRSAmplitude(derivative);
            List<QRS> qrs = new List<QRS>();
            for (int i = 0; i < derivative.Length; i++)
            {
                if (derivative[i] > max)
                {
                    int j = i;
                    int rPeak = 0;
                    double amplitude = 0;
                    while (i < derivative.Length && derivative[i] > max)
                    {
                        i++;
                    }
                    for (int k = j; k < i; k++)
                        if (derivative[k] > amplitude)
                        {
                            amplitude = derivative[k];
                            rPeak = k;
                        }
                    qrs.Add(new QRS(rPeak, signal[rPeak], 0));
                    i += (int)(0.2 * freq);
                }
            }
            return qrs;
        }
    }
}
