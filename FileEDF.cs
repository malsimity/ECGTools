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
    static public class FileEDF
    {
        static public EDFSignal ImportEdfSignal()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "EDF-файл (*.edf)|*.edf" };

            if (openFileDialog.ShowDialog() == true)
            {
                return new EDFFile(openFileDialog.FileName).Signals[0];
            }
            else
            {
                MessageBox.Show("Ошибка считывания файла!");
                return null;
            }
        }

        static public void ImportDataAnaliz()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "analiz-файл (*.analiz)|*.analiz" };

            // Логика считывания файлов анализа
        }

        static public void ExportEdfSignal(int[] peakR, int[] classes)
        {
            if (peakR == null || classes == null) { throw new ArgumentNullException("Записывать нечего"); }

            // Далее логика записи номеров отчётов, где зафиксированы R-пики и номера классов, к которым принадлежат эти R-пики

        }
    }
}
