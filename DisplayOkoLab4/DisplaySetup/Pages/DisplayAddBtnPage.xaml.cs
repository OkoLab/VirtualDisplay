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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VirtualDisplay.DisplaySetup.Pages
{
    /// <summary>
    /// Interaction logic for DisplayAddBtnPage.xaml
    /// </summary>
    public partial class DisplayAddBtnPage : Page
    {
        SqliteDataAccess setupDB;
        ComPortConnector comPort;

        //Collection of buttons save ListView 
        ObservableCollection<SaveButtonModel> btn_list;

        // Текст отображаемый в Text Box при открытии окна
        string initial_txt = "Введите название кнопки";

        public DisplayAddBtnPage(ComPortConnector cp)
        {
            InitializeComponent();

            //устанавливаем ширину списка по размеру окна, так без этого не будет работать горизонтальная прокрутка
            btn_list_dg.Width = SystemParameters.PrimaryScreenHeight;

            comPort = cp;
            comPort.DataReceivedEvent += SerialPort_DataReceived;

            Btn_name_tb.Text = initial_txt;

            setupDB = new SqliteDataAccess();

            //отображаем уже настроенные кнопки
            ShowBtn();
        }

        //Binding Setup BD and Delete_btn_lv 
        private void ShowBtn()
        {
            List<SaveButtonModel> l_sb = setupDB.ShowSetupButtons();
            if (l_sb != null)
                btn_list = new ObservableCollection<SaveButtonModel>(l_sb);
            else
                btn_list = new ObservableCollection<SaveButtonModel>();

            btn_list_dg.ItemsSource = btn_list;
        }

        public void SerialPort_DataReceived(byte[] data)
        {
            //SaveButtonModel btn = new SaveButtonModel();
            //имя кнопки заданное пользователем
            SaveButtonModel btn = new SaveButtonModel();

            // Get button name from textbox
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate { btn.BtnName = Btn_name_tb.Text; });

            //проверяем, что в TextBox введено имя кнопки                       
            if (!string.IsNullOrWhiteSpace(btn.BtnName) && (btn.BtnName != initial_txt))
            {
                if (data != null)
                {
                    //проверяем, есть ли уже кнопка с таким ID от производителя в БД
                    btn.BtnId = comPort.TakeBtnID(data);
                    if (setupDB.CheckID_btnInTable(btn.BtnId))
                    {
                        //если да, то просим удалить кнопку с таким ID из БД 
                        btn.BtnName = setupDB.GetName_btnInTable(btn.BtnId);
                        string txt = "Данная кнопка уже зарегистрирована под именем(номером): " + btn.BtnName + ". Для перезаписи удалите кнопку из базы данных настроенных кнопок.";
                        MessageBox.Show(txt,
                            "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        if (btn_list.Any(x => x.BtnName == btn.BtnName))
                        {
                            MessageBox.Show("Имя кнопки уже зарегистрировано. " + btn.BtnName + ". Для перезаписи удалите кнопку из базы данных настроенных кнопок.",
                                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                        else
                        {
                            btn.BtnDate = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
                            Dispatcher.BeginInvoke(new DisplayDataEvent(DisplayData), btn);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Введите имя кнопки", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //заменяем название на новый
        private delegate void ChangeDisplayDataEvent(int i, SaveButtonModel bm);
        private void ChangeDisplayData(int i, SaveButtonModel bm)
        {
            btn_list[i] = bm;
        }

        private delegate void DisplayDataEvent(SaveButtonModel bm);
        private void DisplayData(SaveButtonModel bm)
        {
            // Add to the end
            //btn_list.Add(bm);

            // Add to start
            btn_list.Insert(0, bm);

            setupDB.SaveButtonInSetup(bm);
            check_txt_tb.Text = "";
            check_ico_tb.Visibility = Visibility.Hidden;
        }

        // Delete button from Save_btn_lv
        private void DeleteButtonFromList(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            SaveButtonModel btn = b.DataContext as SaveButtonModel;
            btn_list.Remove(btn);
            setupDB.DeleteButton(btn.BtnId);
        }

        // Событие при нажатии мышкой на Text Box
        private void txtBox_NameOfButton_GotFocus(object sender, RoutedEventArgs e)
        {
            //Стираем установленный по умолчанию текст
            if (Btn_name_tb.Text == initial_txt)
                Btn_name_tb.Clear();
        }

        private void txtBox_NameOfButton_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Btn_name_tb.Text))
                Btn_name_tb.Text = initial_txt;
        }


        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            comPort.DataReceivedEvent -= SerialPort_DataReceived;
        }

        private void ClickKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            String str = Btn_name_tb.Text;
            if (!string.IsNullOrWhiteSpace(str) && (str != initial_txt))
            {
                if (btn_list.Any(x => x.BtnName == str))
                {
                    check_ico_tb.Visibility = Visibility.Visible;
                    check_ico_tb.Content = FindResource("No");
                    check_txt_tb.Text = "имя кнопки занято";
                    check_txt_tb.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    check_ico_tb.Visibility = Visibility.Visible;
                    check_ico_tb.Content = FindResource("Yes");
                    check_txt_tb.Text = "имя кнопки свободно";
                    check_txt_tb.Foreground = new SolidColorBrush(Colors.LimeGreen);
                }
            }
            else
            {
                check_ico_tb.Visibility = Visibility.Hidden;
                check_txt_tb.Text = "";
            }
        }
    }
}
