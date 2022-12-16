using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ScottPlot;
using Image = System.Drawing.Image;

namespace ECGTools.Windows
{
    /// <summary>
    /// Логика взаимодействия для ComplexexWindows.xaml
    /// </summary>
    public partial class ComplexexWindows : Window
    {
        private QRSClass QrsClass;
        public ComplexexWindows(QRSClass qrsClass)
        {
            InitializeComponent();
            QrsClass = qrsClass;
            lbCount.Content += qrsClass.qrs.Count.ToString();
            for (int i = 0; i < qrsClass.qrs.Count; i++)
                listClasses.Children.Add(CreateButtonClasses(qrsClass.qrs[i]));

        }

        private System.Windows.Controls.Image CreateButtonClasses(double[] qrs)
        {
            WpfPlot plt = new WpfPlot();
            plt.Margin = new Thickness(5, 5, 5, 5);
            plt.Plot.AddSignal(qrs);
            plt.Plot.Grid(false);
            plt.Plot.XAxis.Ticks(false);
            plt.Plot.YAxis.Ticks(false);

            //Image image = Image.FromHbitmap(plt.Plot.Render(200, 200).GetHbitmap());
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Margin = new Thickness(5, 5, 5, 5);
            IntPtr intPtr = plt.Plot.Render(200, 200).GetHbitmap();
            image.Source = Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            image.Width = 200;
            image.Height = 200;
            return image;
        }
    }
}
