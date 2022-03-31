using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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

namespace VirtualDisplay.Statistics.Pages
{
    /// <summary>
    /// Interaction logic for ChartPage.xaml
    /// </summary>
    public partial class ChartPage : Page
    {
        private readonly SqliteDataAccess db;

        public SeriesCollection Chart { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        public ChartValues<ObservableValue> Values { get; private set; }
        public object Mapper { get; set; }
        Brush brush;

        public ChartPage(String begin, String end)
        {
            db = new SqliteDataAccess();
            
            brush = new SolidColorBrush(Color.FromArgb(255, 92, 153, 214));

            DateTime d1 = DateTime.Parse(begin);
            DateTime d2 = DateTime.Parse(end);
            

            int days = (d2 - d1).Days;

            List<CountButtons> cb = new List<CountButtons>();

            InitializeComponent();

            List<string> date_str = new List<string>();

            //показывает график одного дня по часам
            if (days == 0)
            {
                cb = db.MethodCountButtons(begin, end, 0);
                date_str = (from a in cb select a.btn_date).ToList();

                X.Title = "Часы";

                for (int i = 0; i < date_str.Count(); i++)
                    date_str[i] = date_str[i] + "-" + (Int32.Parse(date_str[i]) + 1);
            }
            //проверить, что диапазон указан верный
            else if (0 < days && days < 31)
            {
                cb = db.MethodCountButtons(begin, end, 1);
                date_str = (from a in cb select a.btn_date).ToList();
                X.Title = "Дни";
            }
            //проверить, что диапазон указан верный
            else if (31 < days && days < 356)
            {
                cb = db.MethodCountButtons(begin, end, 2);
                date_str = (from a in cb select a.btn_date).ToList();
                X.Title = "Месяцы";
            }
            //проверить, что диапазон указан верный
            else if (days > 356)
            {
                cb = db.MethodCountButtons(begin, end, 3);
                date_str = (from a in cb select a.btn_date).ToList();
                X.Title = "Года";
            }

            List<int> count_i = (from a in cb select a.btn_count).ToList();
            List<ObservableValue> stp = new List<ObservableValue>();
            for (int i = 0; i < count_i.Count(); i++)
                stp.Add(new ObservableValue(count_i[i]));
            Values = new ChartValues<ObservableValue>(stp);

            ColumnSeries cs = new ColumnSeries
            {
                Title = "Количество вызовов " + begin + " по " + end + ": ",
                Values = Values,
                DataLabels = true,
                Fill = brush
            };

            Labels = date_str.ToArray();
            Formatter = value => value.ToString("N0");
            Chart = new SeriesCollection()
            {
                cs
            };

            DataContext = this;
        }
    }
}
