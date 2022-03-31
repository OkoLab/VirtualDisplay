using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System;
using System.Windows.Input;
using System.IO;
using MahApps.Metro.Controls;
using VirtualDisplay.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using VirtualDisplay.DisplaySetup;

namespace VirtualDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    //коды ошибок 2XXX

    public partial class MainWindow : Window
    {
        DisplayUserControl displayUser;
        StatisticsUserControl statisticsUser;

        DisplaySetupUserControl setupUser;

        //настройка для ПЭК
        //SetUpUserControl setUpUser;

        ComPortConnector cp;
        string cp_name = null;

        SqliteDataAccess sq = new SqliteDataAccess();

        public MainWindow()
        {
            //SKGL.SerialKeyConfiguration skc = new SKGL.SerialKeyConfiguration();
            //SKGL.Validate validateKey = new SKGL.Validate(skc);

            cp = new ComPortConnector();

            //добавляем события подключения-отключения от USB портов компьютера
            ComWatcher cw = new ComWatcher();
            cw.UsbAddedEventHandler += USBAdded;
            cw.UsbRemoveEventHandler += USBRemove;

            InitializeComponent();

            //SqliteDataAccess sq = new SqliteDataAccess();

            //открываем соединение с COM портом
            ConnectionToCom();
                 
            //string[] ports = cp.LoadComPort();



            //Licence lw = new Licence(validateKey);
            //lw.Owner = MainWindow;
            //lw.ShowDialog();
        }

        //функция зашружает и открывает COM порт  
        private void ConnectionToCom()
        {
            //берем из настроек название COM порта
            //string comport_in_settings = Properties.Settings.Default.ComPortName;

            //загружаем количество фактически поключенных COM портов
            string[] ports = cp.LoadComPort();

            //строка для тестирования
            //string[] ports = { "com1", "com2", "com3", "com4" };

            int count_p = ports.Length;

            ObservableCollection<string> ports_list;

            if (count_p != 0)
                ports_list = new ObservableCollection<string>(ports);
            else
            {
                ports_list = new ObservableCollection<string>();
                cp_name = null;
            }
            //LoadPorts_cb.ItemsSource = ports_list;

            Dispatcher.BeginInvoke(new Action(() => {LoadPorts_cb.ItemsSource = ports_list;
                SelectPortDialog.IsOpen = true;
            }));
            

            //SelectPortDialog.IsOpen = true;

            /*
            if (count_p > 0)
            {
                /*
                //ищем в массиве подключенных COM портов порт указанный в настройках программы
                int index = Array.IndexOf(ports, comport_in_settings);
                if (index >= 0)
                {
                    cp_name = ports[index];
                }
                else //COM порт в настройках не найден в массиве подключенных COM портов
                {
                    ObservableCollection<string> ports_list = new ObservableCollection<string>(ports);
                    LoadPorts_cb.ItemsSource = ports_list;
                    SelectPortDialog.IsOpen = true;
                }
                */

            /*
                //к компьютеру подключен один порт
                if (count_p == 1)
                {
                    cp_name = ports[0];
                }
            */
            /*
                //к компьютеру подключено несколько портов
                else
                {
                    ObservableCollection<string> ports_list = new ObservableCollection<string>(ports);
                    LoadPorts_cb.ItemsSource = ports_list;
                    SelectPortDialog.IsOpen = true;
                }

              */


            if (cp_name != null)
            {
                    if (cp.OpenPort(cp_name))
                    {
                        Dispatcher.BeginInvoke(new Action(() => {
                        Cp_status_tb.Text = "Состояние приемника: подключен к порту " + cp_name;
                        Antenna_pi.Foreground = new SolidColorBrush(Colors.Green);
                        }));
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() => {
                            Cp_status_tb.Text = "Состояние приемника: не обнаружен. Код ошибки #2003";
                        Antenna_pi.Foreground = new SolidColorBrush(Colors.Red);
                        }));
                    }
            }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() => {
                        //Cp_status_tb.Text = "Состояние усилителя: не обнаружен. Код ошибки #2002";
                        Cp_status_tb.Text = "";
                        //Antenna_pi.Foreground = new SolidColorBrush(Colors.Red);
                        Antenna_pi.Foreground = new SolidColorBrush(Colors.Gray);
                    }));
                }
            
            /*
            else

            {
                Dispatcher.BeginInvoke(new Action(() => {
                    Cp_status_tb.Text = "Состояние усилителя: не обнаружен. Код ошибки #2001";
                Antenna_pi.Foreground = new SolidColorBrush(Colors.Red);
                }));
            }
            */
        }

        //Событие, когда во время программы подключились к USB порту
        private void USBAdded(object sender, EventArgs e)
        {
            //проверяем открыт ли наш COM порт с усилителем, если нет,
            //то вызываем функцию для подключения к усилителю
            if (!cp.IsOpen())
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    System.Windows.Forms.Application.Restart();

                    Application.Current.Shutdown();
                }));

                /*
                Dispatcher.BeginInvoke(new Action(() => {
                    SelectPortDialog.IsOpen = true;
                }));
                */
                //ConnectionToCom(); // ком порт закрыт

            }
        }

        async private void ShowTakeOutCableDialog()
        {
            TakeOutTheCableDialog dialog = new TakeOutTheCableDialog();
            this.IsEnabled = false;
            dialog.Show();
            await Task.Run(() =>
            {
                Thread.Sleep(3000);
            });
            dialog.Close();
            this.IsEnabled = true;
        }

        //Событие, когда во время программы отключились от USB порта
        private void USBRemove(object sender, EventArgs e)
        {
            //проверяем открыт ли наш COM порт с училителем, если нет,
            //то вызываем функцию для подключения к усилителю
            if (!cp.IsOpen())
            {                
                Dispatcher.BeginInvoke(new Action(() => {
                    Cp_status_tb.Text = "Соединение COM порт потеряно. Перезапустите приложение. Код ошибки #2004";
                    Antenna_pi.Foreground = new SolidColorBrush(Colors.Red);
                    ShowTakeOutCableDialog();
                }));
            }
            //  ConnectionToCom(); // ком порт закрыт
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListViewMenu.SelectedIndex;
            GridMain.Children.Clear();

            switch (index)
            {
                case 0:
                    displayUser = new DisplayUserControl(cp);
                    GridMain.Children.Add(displayUser);
                    break;
                case 1:
                    statisticsUser = new StatisticsUserControl(cp);
                    GridMain.Children.Add(statisticsUser);
                    break;
                case 2:
                    setupUser = new DisplaySetupUserControl(cp);
                    GridMain.Children.Add(setupUser);
                    break;

                default:
                    break;
            }
        }        

        private void OnSelectComPort(object sender, RoutedEventArgs e)
        {
            cp_name = (string)LoadPorts_cb.SelectedItem;
            SelectPortDialog.IsOpen = false;

            if (cp_name != null)
            {
                if (cp.OpenPort(cp_name))
                {
                    //сохраняем COM порт в настройках
                    //Properties.Settings.Default.ComPortName = cp_name;
                    //Properties.Settings.Default.Save();

                    Dispatcher.BeginInvoke(new Action(() => {
                        Cp_status_tb.Text = "Состояние приемника: подключен к порту " + cp_name;
                        Antenna_pi.Foreground = new SolidColorBrush(Colors.Green);
                    }));
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() => {
                        Cp_status_tb.Text = "Состояние приемника: не обнаружен. Код ошибки #2003";
                        Antenna_pi.Foreground = new SolidColorBrush(Colors.Red);
                    }));
                }
            }
        }

        private void OnClosingWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Закрыть приложение?", "Закрыть приложение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {

                if (cp.IsOpen())
                    cp.ClosePort();

                if (displayUser != null)
                    displayUser.CloseWindow();
            }
            else
                e.Cancel = true;
        }

        private void OnLoadedWindow(object sender, RoutedEventArgs e)
        {
            //SKGL.SerialKeyConfiguration skc = new SKGL.SerialKeyConfiguration();
            //SKGL.Validate validateKey = new SKGL.Validate(skc);
            //string fl = "file.txt";

            // проверить, если файл с лицензионным ключем существует
            // если существует файл, то берем ключ
            // если файл отсутствует, то приложение запускается впервые
            /*
            if (File.Exists(fl))
            {
                Crypt my_cr = new Crypt();
                // ключ может состоять из 8-ми символов
                String key = "youtubee";
                //my_cr.EncryptFile("file.txt", key);
                //берем ключ из файла и проверяем его на правильность
                validateKey.Key = my_cr.DecryptFile(fl, key);
                if(!validateKey.IsValid || validateKey.IsExpired) //если ключ из файла не совподает, то выводим лицензионное окно
                {
                    Licence lw = new Licence(validateKey);
                    lw.Owner = this;

                    File.Delete(fl); //удаляем файл с ключем, так как ключ ошибочный или закончился
                                     //затемняем основное окно
                    this.Opacity = 0.4;
                    lw.ShowDialog();
                    this.Opacity = 1;
                }
                else
                {
                    DateTime start_d = validateKey.ExpireDate.AddDays(-60); //начинаем с этой даты сообщать пользователю сколько дней осталось до конца работы ключа               
                    DateTime now_dt = DateTime.Now.Date;
                    TimeSpan ts = validateKey.ExpireDate.Subtract(now_dt); //сравниваем дату окончания 
                    double interval_days = ts.TotalDays;
                    if(now_dt > start_d ) //за два месяца сообщаем клиенту сколько дне осталось до конца работы лицензии
                    {
                        LicenceUntilFinishedMsg lf = new LicenceUntilFinishedMsg(validateKey, interval_days);
                        lf.Owner = this;
                        //затемняем основное окно
                        this.Opacity = 0.4;
                        lf.ShowDialog();
                        this.Opacity = 1;
                    }
                }
            }
            else
            {
                Licence lw = new Licence(validateKey);
                lw.Owner = this;
                //затемняем основное окно
                this.Opacity = 0.4;
                lw.ShowDialog();
                this.Opacity = 1;
            }
            */
        } 
    }
}