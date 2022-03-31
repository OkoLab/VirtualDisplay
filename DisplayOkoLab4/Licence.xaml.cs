using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for Licence.xaml
    /// </summary>
    public partial class Licence : Window
    {
        SKGL.Validate vk1;

        public Licence(SKGL.Validate vk)
        {
            InitializeComponent();
            vk1 = vk;
            PCcode_tb.Text = vk1.MachineCode.ToString();
            //EncryptFile("file.txt", vk1.MachineCode.ToString);
        }

        private void ClosedLicenceWindow_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Button_licence_Click(object sender, RoutedEventArgs e)
        {
            vk1.Key = ValideKey_tb.Text;            

            if (vk1.IsValid) // если ключ верный, то зашифровываем его в файл
            {
                Crypt my_cr = new Crypt();
                // ключ может состоять из 8-ми символов
                String key = "youtubee";
                File.WriteAllText("file.txt", vk1.Key);
                my_cr.EncryptFile("file.txt", key);
                MyMessageBox mb = new MyMessageBox("Код лицензии верный.Программа разблокирована.");
                mb.Show();
                this.Close();
            }
            else
            {
                Result_tb.Text = "введен неверный код, повторите попытку";
            }
        }
        
        private void MinimizeLicenceWindow(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        //копируем код ком
        private void CopyPCcode_Click(object sender, RoutedEventArgs e)
        {
            string str = PCcode_tb.Text;
            Clipboard.SetData(DataFormats.Text, (Object)str);
            MyMessageBox mb = new MyMessageBox("Код компьютера скопирован в буфер обмена");
            mb.Show();
        }
    }
}
