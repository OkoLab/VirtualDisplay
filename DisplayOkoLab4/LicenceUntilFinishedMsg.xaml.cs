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

namespace VirtualDisplay
{
    /// <summary>
    /// Interaction logic for LicenceUntilFinishedMsg.xaml
    /// </summary>
    public partial class LicenceUntilFinishedMsg : Window
    {
        public LicenceUntilFinishedMsg(SKGL.Validate vk, double interval)
        {
            InitializeComponent();
            string buff = vk.Key;

            for (int i = 5; i <= 17; i = i + 6)
            {
                buff = buff.Insert(i, "-");
            }
            

            Licence_tb.Text = "Лицензия " + buff + " заканчивается через " + interval + " дней";
            PCcode_tb.Text = "Код компьютера: " + vk.MachineCode;
        }

        private void ClosedWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_licence_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void CopyPCcode_Click(object sender, RoutedEventArgs e)
        {
            string str = PCcode_tb.Text;
            Clipboard.SetData(DataFormats.Text, (Object)str);
            MyMessageBox mb = new MyMessageBox("Код компьютера скопирован в буфер обмена");
            mb.Show();
        }
    }
}
