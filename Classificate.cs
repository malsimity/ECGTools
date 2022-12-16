using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECGTools
{
    public class QRSClass
    {
        public double[] average { get; private set; }

        public List<double[]> qrs { get; private set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="n"> Длина массивов.</param>
        public QRSClass(int n)
        {
            qrs = new List<double[]>();
            average = new double[n];
        }

        public void AddQRS(double[] qrs)
        {
            if (qrs.Length != average.Length) { throw new ArgumentNullException("Неверная длина массива!");}
            this.qrs.Add(qrs);
            for (int i = 0; i < average.Length; i++)
            {
                average[i] += qrs[i];
            }
        }

        public void Remove(int index)
        {
            for (int i = 0; i < average.Length; i++)
            {
                average[i] += qrs[index][i] * (qrs.Count / (qrs.Count - 1));
            }
            qrs.RemoveAt(index);
        }

        public void CalcAverage()
        {
            for (int i = 0; i < average.Length; i++)
                average[i] /= qrs.Count;
        }
    }
    public static class Classificate
    {
        /// <summary>
        /// Метод нормализации массива к интервалу значений (a, b).
        /// </summary>
        /// <param name="signal"> Исходный массива.</param>
        /// <param name="a"> Нижняя граница интервала.</param>
        /// <param name="b"> Верхняя граница интервала</param>
        /// <returns> Нормализованный массив</returns>
        public static double[] NormalizationInteral(double[] signal, double a, double b)
        {
            int min = 0, max = 0;
            double argMin = 100000, argMax = -100000;
            for (int i = 0; i < signal.Length; i++)
            {
                if (argMax < signal[i])
                {
                    argMax = signal[i];
                    max = i;
                }

                if (argMin > signal[i])
                {
                    argMin = signal[i];
                    min = i;
                }
            }
            double[] sig = new double[signal.Length];
            for (int i = 0; i < sig.Length; i++)
            {
                sig[i] = (signal[i] - signal[min]) / (signal[max] - signal[min]);
                sig[i] = sig[i] * (b - a) + a;
            }
            return sig;
        }

        /// <summary>
        /// Вычисление меры Танимото (мера похожести сигналов). Сигналы должны содержать
        /// одинаковое число отсчётов.
        /// </summary>
        /// <param name="x"> Первый сигнал.</param>
        /// <param name="y"> Второй сигнал.</param>
        /// <returns> Значение меры Танимото.</returns>
        public static double Tanimoto(double[] x, double[] y)
        {
            int n = x.Length;
            double scalarProduct = 0;
            double normX = 0;
            double normY = 0;
            for (int i = 0; i < n; i++)
            {
                scalarProduct += x[i] * y[i];
                normX += x[i] * x[i];
                normY += y[i] * y[i];
            }
            return scalarProduct / (normX + normY - scalarProduct);
        }

        /// <summary>
        /// Расчёт похожести сигналов.
        /// </summary>
        /// <param name="x"> Первый сигнал.</param>
        /// <param name="y"> Второй сигнал.</param>
        /// <returns></returns>
        public static double CoefLike(double[] x, double[] y)
        {
            double normX = 0, normY = 0, scal = 0;
            for (int i = 0; i < x.Length; i++)
            {
                normX += x[i] * x[i];
                normY += y[i] * y[i];
                scal += x[i] * y[i];
            }

            normX = (double)Math.Sqrt(normX);
            normY = (double)Math.Sqrt(normY);


            return Tanimoto(x, y) * scal / (normX * normY);
        }

        /// <summary>
        /// Первоначальная кластеризация.
        /// </summary>
        /// <param name="qrsComplexs"> Лист массивов с QRS-комплексами.</param>
        /// <param name="coef"> Коэффициент похожести.</param>
        /// <returns></returns>
        public static List<QRSClass> FirstClasters(List<double[]> qrsComplexs, double coef = 0.97f)
        {
            int count = qrsComplexs.Count;
            int beg = 0;
            int end = 0;
            int len = qrsComplexs[0].Length;
            List<double[]> oporVect = new List<double[]> { qrsComplexs[0] };
            List<QRSClass> clust = new List<QRSClass>();
            clust.Add(new QRSClass(len));
            for (int i = 1; i < count; i++)
            {
                if (CoefLike(qrsComplexs[beg], qrsComplexs[i]) < coef || i == count - 1)
                {
                    end = i;
                    bool flag = true;
                    for (int j = 0; j < oporVect.Count; j++)
                        if (CoefLike(oporVect[j], qrsComplexs[beg]) > coef)
                        {
                            flag = false;
                            for (int k = beg; k < end; k++)
                                clust[j].AddQRS(qrsComplexs[k]);
                            break;
                        }
                    if (flag)
                    {
                        clust.Add(new QRSClass(len));
                        for (int k = beg; k < end; k++)
                            clust[clust.Count - 1].AddQRS(qrsComplexs[k]);
                        oporVect.Add(qrsComplexs[beg]);
                    }
                    beg = i;
                }
            }

            for (int i = 0; i < clust.Count; i++)
                clust[i].CalcAverage();
            return clust;
        }
    }
}
