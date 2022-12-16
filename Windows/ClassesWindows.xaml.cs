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
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;

namespace ECGTools.Windows
{
    /// <summary>
    /// Логика взаимодействия для ClassesWindows.xaml
    /// </summary>
    public partial class ClassesWindows : Window
    {
        private List<QRSClass> QrsClasses;
        public ClassesWindows(List<QRSClass> qrsClasses)
        {
            InitializeComponent();
            QrsClasses = qrsClasses;
            int ind = 0;
            lbCount.Content += qrsClasses.Count.ToString();
            foreach (QRSClass iten in QrsClasses)
            {
                listClasses.Children.Add(CreateButtonClasses(iten, ind));
                ind++;
            }
        }

        private Button CreateButtonClasses(QRSClass qrs, int ind)
        {
            WpfPlot plt = new WpfPlot();
            plt.Plot.Title("Тип QRS комплекса - " + ind);
            plt.Plot.AddSignal(qrs.average);
            plt.Plot.Grid(false);
            plt.Plot.XAxis.Ticks(false);
            plt.Plot.XAxis.Label(qrs.qrs.Count.ToString());
            plt.Plot.YAxis.Ticks(false);
            var image = plt.Plot.Render(200, 200);
            IntPtr handle = image.GetHbitmap();
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            Button bt = new Button();
            bt.Style = (System.Windows.Style)this.Resources["MyButtonStyle"];
            bt.Margin = new Thickness(5, 5, 5, 5);
            bt.Content = "";
            bt.Name = "bt" + ind;
            bt.Width = 200;
            bt.Height = 200;
            bt.Click += BtOnClick;
            bt.Background = new ImageBrush(bitmapSource);

            return bt;
        }

        private void BtOnClick(object sender, RoutedEventArgs e)
        {
            string name = ((Button) sender).Name.Remove(0, 2);
            ComplexexWindows complexexWindows = new ComplexexWindows(QrsClasses[Convert.ToInt32(name)]);
            complexexWindows.Owner = this;
            complexexWindows.Show();
        }
    }
}
