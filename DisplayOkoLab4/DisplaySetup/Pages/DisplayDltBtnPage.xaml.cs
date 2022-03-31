using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace VirtualDisplay.DisplaySetup.Pages
{
    /// <summary>
    /// Interaction logic for DisplayDltBtnPage.xaml
    /// </summary>
    public partial class DisplayDltBtnPage : Page
    {
        //Collection of buttons delete ListView 
        ObservableCollection<SaveButtonModel> btn_list;
        ComPortConnector comPort;
        SqliteDataAccess setupDB;

        public DisplayDltBtnPage(ComPortConnector cp)
        {
            InitializeComponent();

            //устанавливаем ширину списка по размеру окна, так без этого не будет работать горизонтальная прокрутка
            btn_list_dg.Width = SystemParameters.PrimaryScreenHeight;

            btn_list = new ObservableCollection<SaveButtonModel>();
            btn_list_dg.ItemsSource = btn_list;

            setupDB = new SqliteDataAccess();
            comPort = cp;
            comPort.DataReceivedEvent += SerialPort_DataReceived;
            //ShowTabDeleteBtn();
        }

        public void SerialPort_DataReceived(byte[] data)
        {
            //имя кнопки заданное пользователем
            SaveButtonModel btn = new SaveButtonModel();
            if (data != null)
            {
                //проверяем, есть ли уже кнопка с таким ID от производителя в БД
                btn.BtnId = comPort.TakeBtnID(data);
                if (setupDB.CheckID_btnInTable(btn.BtnId))
                {
                    //если да, то просим удалить кнопку с таким ID из БД 
                    btn = setupDB.Get_FromTable(btn.BtnId);
                    Dispatcher.BeginInvoke(new DisplayDataEvent(DisplayData), btn);

                }
            }
        }

        private delegate void DisplayDataEvent(SaveButtonModel bm);
        private void DisplayData(SaveButtonModel bm)
        {
            btn_list.Add(bm);
        }

        // Delete a button from SetupDB and Delete_btn_lv
        private void DeleteButtonFromDB(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            SaveButtonModel btnModel = b.DataContext as SaveButtonModel;
            MessageBoxResult result = MessageBox.Show("Вы точно хотите удалить кнопку?", "Удаление кнопки", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                btn_list.Remove(btnModel);
                setupDB.DeleteButton(btnModel.BtnId);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            comPort.DataReceivedEvent -= SerialPort_DataReceived;
        }
    }
}
