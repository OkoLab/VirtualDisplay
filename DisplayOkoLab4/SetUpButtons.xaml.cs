using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace DisplayOkoLab4
{
    /// <summary>
    /// Interaction logic for SetUpButtons.xaml
    /// </summary>
    public partial class SetUpButtons : Window
    {
        ComPortConnector comPort = new ComPortConnector();
        //ButtonModel btn = new ButtonModel();
        //List<ButtonModel> list = new List<ButtonModel>();


        ObservableCollection<ButtonModel> btnlist;



        //Текст отображаемы в Text Box при открытии окна
        string initial_txt = "Введите название кнопки";

        public SetUpButtons()
        {
            InitializeComponent();
            btnlist = new ObservableCollection<ButtonModel>();
            SetUpBtnList.ItemsSource = btnlist;


            if (comPort.OpenPort())
            {
                MessageBox.Show("com порт открыт", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                comPort.DataReceivedEvent += SerialPort_DataReceived;
            }
            else
                MessageBox.Show("Ошибка: com порт не был открыт", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
           

            txtBox_NameOfButton.Text = initial_txt;
        }

        private void SerialPort_DataReceived(byte[] data)
        {
            ButtonModel btn = new ButtonModel();
            btn.BtnId = comPort.TakeBtnNumber(data);
            String btnname = "0";
            Dispatcher.Invoke(DispatcherPriority.Normal,(ThreadStart)delegate {btnname = txtBox_NameOfButton.Text; });
            btn.BtnName = btnname;
            btn.BtnService = comPort.TakePushButtonService(data[4]);
            SqliteDataAccess btnDataBase = new SqliteDataAccess();
            bool checkID = btnDataBase.CheckID_btnInTable(btn.BtnId);
            bool checkName = btnDataBase.CheckName_btnInTable(btn.BtnId);
            
            //проверяем, что в TextBox введено имя кнопки                       
            if (!string.IsNullOrWhiteSpace(btnname) || btnname != initial_txt)
            {

                //проверяем, есть ли уже кнопка с таким ID от производителя
                if (checkID)
                {
                    MessageBox.Show("Данная кнопка уже зарегистрирована. Перезаписать кнопку?", "Сохранить кнопку", MessageBoxButton.YesNo, MessageBoxImage.Question);
                }
                else
                {
                    //проверяем, если ли уже кнопки с таким названием кнопки
                    if (checkName)
                    {
                        MessageBox.Show("Имя кнопки уже зарегистрирована. Сохранить кнопку под этим именем?", "Сохранить кнопку", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    }
                    else
                    {
                        //btnDataBase.SaveButton(btn);
                        this.Dispatcher.BeginInvoke(new DisplayDataEvent(DisplayData), btn);                       

                        //Dispatcher.Invoke(new EventHandler(DisplayData));
                        //MessageBox.Show("Кнопка сохранена", "Кнопка сохранена", MessageBoxButton.OK, MessageBoxImage.Question);
                    }
                }
            }
            else
            {
                MessageBox.Show("Введите название или номер кнопки", "Название кнопки", MessageBoxButton.OK, MessageBoxImage.Information);
            }
                                 
        }

        private delegate void DisplayDataEvent(ButtonModel bm);
        private void DisplayData(ButtonModel bm)
        {
            //SetUpBtnList.Items.Add(new { name_btn = txtBox_NameOfButton.Text});
            btnlist.Add(bm);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            comPort.ClosePort();
        }

        public string ButtonClicked()
        {
            string txt = txtBox_NameOfButton.Text;
            return txt;

        }

        //Удаляем кнопку из ListView
        private void DeleteButton(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            ButtonModel btnModel = b.CommandParameter as ButtonModel;
            //MessageBoxResult result = MessageBox.Show("Вы точно хотите удалить кнопку" + btnModel.BtnName, "Удаление кнопки", MessageBoxButton.YesNo, MessageBoxImage.Question);
            MessageBoxResult result = new MessageBoxResult();
            result = MessageBox.Show("Вы точно хотите удалить кнопку?", "Удаление кнопки", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                btnlist.Remove(btnModel);
                /*
                SetUpBtnList.SelectedItem = ((Button)sender).DataContext;
                SetUpBtnList.Items.Remove(SetUpBtnList.SelectedItem);
                */
            }
            
        }

        //Событие при нажатии мышкой на Text Box
        private void txtBox_NameOfButton_GotFocus(object sender, RoutedEventArgs e)
        {
            //Стираем установленный по умолчанию текст
            if(txtBox_NameOfButton.Text == initial_txt)
            {
                txtBox_NameOfButton.Clear();
                //txtBox_NameOfButton.
            }
                

        }

        //Возвращает текст введенный в Text Box
        private string TextBoxNameOfButton()
        {
            string buff = txtBox_NameOfButton.Text;
            return buff;
        }

        // Сохраняет список кнопок
        private void SaveListButtonInDB(object sender, RoutedEventArgs e)
        {
            SqliteDataAccess btnDataBase = new SqliteDataAccess();

            /* Для тестирования
            ObservableCollection<ButtonModel> test = new ObservableCollection<ButtonModel>()
            {
                new ButtonModel(){BtnId=1, BtnName="111", BtnService=1 },
                new ButtonModel(){BtnId=2, BtnName="222", BtnService=1 },
                new ButtonModel(){BtnId=3, BtnName="333", BtnService=1 },
                new ButtonModel(){BtnId=4, BtnName="444", BtnService=1 },
                new ButtonModel(){BtnId=5, BtnName="555", BtnService=1 }
            };
            btnDataBase.SaveButton(test);
            */

            if (btnlist.Count > 0)
                btnDataBase.SaveButton(btnlist);
            else
                MessageBox.Show("Список сохраняемых кнопок пуст. Добавьте кнопку", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                
        }
    }
}
