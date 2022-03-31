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

namespace VirtualDisplay.Setup.Pages.AddBtnPages
{
    /// <summary>
    /// Interaction logic for AddBtnPage2.xaml
    /// </summary>
    public partial class AddBtnPage2 : Page
    {
        ComPortConnector comPort;
        public int CallbtnId = 0;

        public AddBtnPage2(ComPortConnector cp, string pair_btn_name)
        {
            InitializeComponent();

            namebtn_tb.Text += pair_btn_name;

            comPort = cp;
            comPort.DataReceivedEvent += SerialPort_DataReceived;
        }

        public void SerialPort_DataReceived(byte[] data)
        {
            int BtnId = comPort.TakeBtnID(data);
            SqliteDataAccess setupDB = new SqliteDataAccess(); 

            if (setupDB.CheckID_btnInTable(BtnId))
            {
                    //если да, то просим удалить кнопку с таким ID из БД 
                    string BtnName = setupDB.GetName_btnInTable(BtnId);
                    string BtnFunction = setupDB.GetFunction_btnInTable(BtnId);
                    string txt = "Данная кнопка уже зарегистрирована под именем(номером): " + BtnName + " Функция: " + BtnFunction + ". Для перезаписи удалите кнопку из базы данных настроенных кнопок.";
                    MessageBox.Show(txt, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                CallbtnId = BtnId;
                Dispatcher.BeginInvoke(new Action(() => {
                    page2_tb.Text = "Успешно";
                }));
                
            }
        }

        private void UnloadedPage(object sender, RoutedEventArgs e)
        {
            comPort.DataReceivedEvent -= SerialPort_DataReceived;
        }
    }
}
