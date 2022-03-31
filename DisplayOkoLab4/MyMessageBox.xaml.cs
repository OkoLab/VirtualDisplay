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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VirtualDisplay
{
    /// <summary>
    /// Interaction logic for MyMessageBox.xaml
    /// </summary>
    public partial class MyMessageBox : Window
    {
        DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        public MyMessageBox(string txt)
        {
            InitializeComponent();
            window_text_tb.Text = txt;
            timer.Interval = new TimeSpan(0,0,2);
            timer.Tick += new EventHandler(timer_Tick);
        }

        public new void Show()
        {
            timer.Start();
            base.Show();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //set default result if necessary

            timer.Stop();
            this.Close();
        }
    }
}
