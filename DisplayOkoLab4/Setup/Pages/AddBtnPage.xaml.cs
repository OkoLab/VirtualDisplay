using VirtualDisplay.Dialogs;
using VirtualDisplay.Setup.Pages.AddBtnPages;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace VirtualDisplay
{
    /// <summary>
    /// Interaction logic for AddBtnPage.xaml
    /// </summary>
    public partial class AddBtnPage : Page
    {
        SqliteDataAccess setupDB;
        ComPortConnector comPort;

        //Collection of buttons save ListView 
        ObservableCollection<SaveButtonModel> btn_list;

        // Текст отображаемый в Text Box при открытии окна
        string initial_txt = "Выберете название парных кнопок вызов-отмена";

        int page_number;

        string save_txt_btn = "Сохранить пару";
        string delete_txt_btn = "Удалить пару";

        AddBtnPage1 addBtnP1 = null;
        AddBtnPage2 addBtnP2 = null;
        AddBtnPage3 addBtnP3 = null;


        SaveButtonModel button_call;
        SaveButtonModel button_cancel;

        
        public AddBtnPage(ComPortConnector cp)
        {

            InitializeComponent();

           

            //устанавливаем ширину списка по размеру окна, так без этого не будет работать горизонтальная прокрутка
            //btn_list_dg.Width = SystemParameters.PrimaryScreenHeight;

            comPort = cp;
            //comPort.DataReceivedEvent += SerialPort_DataReceived;

            //Btn_name_tb.Text = initial_txt;
                    
            setupDB = new SqliteDataAccess();

            //проверяем БД настроек пуста или нет
            if(setupDB.IsEmpty())
            {
                page_number = 0;
                addBtnP1 = new AddBtnPage1();    
                MainFrame.Content = addBtnP1;
            }
            else //если БД пустая, то отображаем меню настройки
            {
                //если БД не пустая, то отображаем настроенные кнопки
                Back_btn.Visibility = Visibility.Hidden;
                Next_btn.Content = delete_txt_btn;
                MainFrame.Content = new AddBtnPage4(setupDB.ShowSetupButtons());
                
            }
            
            //отображаем уже настроенные кнопки
          //  ShowBtn();
        }

        async private void ShowSaveDialog()
        {
            SaveButtonsPairDialog dialog = new SaveButtonsPairDialog();
            this.IsEnabled = false;
            dialog.Show();
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
            });
            dialog.Close();
            this.IsEnabled = true;
        }

        async private void ShowDeleteDialog()
        {
            DeleteButtonsPairsDialog dialog = new DeleteButtonsPairsDialog();
            this.IsEnabled = false;
            dialog.Show();
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
            });
            dialog.Close();
            this.IsEnabled = true;
        }

        private void OnClick_Next(object sender, RoutedEventArgs e)
        {
            if(Next_btn.Content.ToString() == delete_txt_btn)
            {
                Back_btn.Visibility = Visibility.Hidden;
                DialogDeleteBtnPairs.IsOpen = true;
            }
            else if(Next_btn.Content.ToString() == save_txt_btn)
            {
                if (addBtnP3.CancelbtnId == 0) //нажали далее, не нажав кнопку вызова
                    MessageBox.Show("Чтобы продолжить нужно нажать на кнопку для отмены вызова", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                else if(button_call.BtnId == addBtnP3.CancelbtnId)
                    MessageBox.Show("Кнопка отмены совпадает с кнопкой вызов", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                {
                    button_cancel.BtnId = addBtnP3.CancelbtnId;

                    //сохраняем кнопки вызова и отмены в БД Setup
                    string time_now = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");

                    button_call.BtnDate = time_now;
                    button_cancel.BtnDate = time_now;

                    setupDB.SaveButtonInSetup(button_call);
                    setupDB.SaveButtonInSetup(button_cancel);
                    Back_btn.Visibility = Visibility.Hidden;
                    Next_btn.Content = delete_txt_btn;
                    ShowSaveDialog();
                    MainFrame.Content = new AddBtnPage4(setupDB.ShowSetupButtons());
                    page_number = 0;
                }
            }
            else
            {
                //используя переменную page_number определяем какая страница открыта
                switch(page_number)
                {
                    case 0:
                        if(addBtnP1 != null)
                        {
                            string btnname = addBtnP1.GetBtnName();
                            if (!string.IsNullOrWhiteSpace(btnname) && (btnname != initial_txt))
                            {
                                button_call = new SaveButtonModel();
                                button_call.BtnName = btnname;
                                button_call.BtnFunction = "Вызов";
                                button_cancel = new SaveButtonModel();
                                button_cancel.BtnName = btnname;
                                button_cancel.BtnFunction = "Отмена";

                                addBtnP2 = new AddBtnPage2(comPort, btnname);
                                MainFrame.Content = addBtnP2;
                                Back_btn.IsEnabled = true;
                                page_number += 1;
                            }
                            else
                            {
                                MessageBox.Show("Введите имя кнопки", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);                            
                            }
                        }
                        break;
                    case 1:
                        if(addBtnP2.CallbtnId == 0) //нажали далее, не нажав кнопку вызова
                            MessageBox.Show("Введите имя кнопки", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        else
                        {
                            button_call.BtnId = addBtnP2.CallbtnId;
                            addBtnP3 = new AddBtnPage3(comPort, button_call.BtnName);
                            MainFrame.Content = addBtnP3;
                            Next_btn.Content = save_txt_btn;
                            page_number += 1;
                        }
                        break;
                  /*      
                    case 2:
                        if (addBtnP3.CancelbtnId == 0) //нажали далее, не нажав кнопку вызова
                            MessageBox.Show("Введите имя кнопки", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        else
                        {
                            button_cancel.BtnId = addBtnP3.CancelbtnId;

                            //сохраняем кнопки вызова и отмены в БД Setup
                            string time_now = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
                            
                            button_call.BtnDate = time_now;
                            button_cancel.BtnDate = time_now;

                            Next_btn.Content = save_txt_btn;
                            Back_btn.Visibility = Visibility.Hidden;

                            page_number = 0;
                        }
                        //Back_btn.IsEnabled = true;
                        //comPort.DataReceivedEvent += SerialPort_DataReceived;
                        //page_number += 1;
                        break;
                  */
                }
            }           
        }

        private void OnClick_Back(object sender, RoutedEventArgs e)
        {
            String btnname;

            switch (page_number)
            {
                case 1:
                    addBtnP1 = new AddBtnPage1(); 
                    MainFrame.Content = addBtnP1;
                    Back_btn.IsEnabled = false;
                    page_number -= 1;
                    break;
                case 2:
                    btnname = addBtnP1.GetBtnName();
                    addBtnP2 = new AddBtnPage2(comPort, btnname);
                    MainFrame.Content = addBtnP2;
                    Next_btn.Content = "Далее";
                    page_number -= 1;
                    break;
                case 3:
                    btnname = addBtnP1.GetBtnName();
                    addBtnP3 = new AddBtnPage3(comPort, btnname);
                    MainFrame.Content = addBtnP3;
                    Next_btn.Content = save_txt_btn;
                    page_number -= 1;
                    break;
            }
        }

        private void OnSelectYes(object sender, RoutedEventArgs e)
        {
            setupDB.DeleteAllButtons();
            DialogDeleteBtnPairs.IsOpen = false;
            
            ShowDeleteDialog();

            addBtnP1 = new AddBtnPage1();
            MainFrame.Content = addBtnP1;
            Next_btn.Content = "Далее";
            Back_btn.IsEnabled = false;
        }


        /*
        //Binding Setup BD and Delete_btn_lv 
        private void ShowBtn()
        {
            List<SaveButtonModel> l_sb = setupDB.ShowSetupButtons();
            if (l_sb != null)
                btn_list = new ObservableCollection<SaveButtonModel>(l_sb);
            else
                btn_list = new ObservableCollection<SaveButtonModel>();

            //btn_list_dg.ItemsSource = btn_list;
        }

        public void SerialPort_DataReceived(byte[] data)
        {
            //SaveButtonModel btn = new SaveButtonModel();
            //имя кнопки заданное пользователем
            SaveButtonModel btn = new SaveButtonModel();           

            // Get button name from textbox
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate { btn.BtnName = Btn_name_tb.Text; });
            // Get button function from comboboxbox
            //Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate { btn.BtnFunction = CallCancel_cb.SelectedItem.ToString(); });
            int function_btn = -1;
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate { function_btn = CallCancel_cb.SelectedIndex; });
            //проверяем, что в TextBox введено имя кнопки                       
            if (!string.IsNullOrWhiteSpace(btn.BtnName) && (btn.BtnName != initial_txt))
            {
                //проверяем выбрана ли функция у кнопки: вызов или отмена
                if (function_btn != -1)
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
                            MessageBox.Show(txt, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                //присваимваем выбранную функцию кнопки
                                //btn.BtnFunction = CallCancel_cb.SelectedItem.ToString();
                                if (function_btn == 0)
                                    btn.BtnFunction = "Вызов";
                                else
                                    btn.BtnFunction = "Отмена";

                                Dispatcher.BeginInvoke(new DisplayDataEvent(DisplayData), btn);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Введите функцию кнопки: вызов или отмена", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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
            btn_list.Add(bm);
            setupDB.SaveButtonInSetup(bm);
            //check_txt_tb.Text = "";
            //check_ico_tb.Visibility = Visibility.Hidden;
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
            //if (Btn_name_tb.Text == initial_txt)
              //  Btn_name_tb.Clear();
        }

        private void txtBox_NameOfButton_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Btn_name_tb.Text))
                //Btn_name_tb.Text = initial_txt;
        }


        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            comPort.DataReceivedEvent -= SerialPort_DataReceived;
        }

        /*
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
        */
    }
}
