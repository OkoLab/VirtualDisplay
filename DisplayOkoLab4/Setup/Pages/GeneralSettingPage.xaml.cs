using VirtualDisplay.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace VirtualDisplay
{
    /// <summary>
    /// Interaction logic for GeneralSettingPage.xaml
    /// </summary>
    public partial class GeneralSettingPage : Page
    {
        private MediaPlayer mp;
        DispatcherTimer timer = new DispatcherTimer();

        //ComPortConnector cp;

        //путь к мелодиям для вызовов
        string path_name = System.AppDomain.CurrentDomain.BaseDirectory + "Melody\\";

        public GeneralSettingPage()
        {
            //cp = new ComPortConnector();

            //устанавливаем проигрываемый аудиофайл из настроек
            mp = new MediaPlayer();
            mp.Volume = (Properties.Settings.Default.SoundValue) / 100;

            InitializeComponent();

            //Устанавливаем значек звука в соответствии с настройками
            MuteButton(mp.Volume);

            string[] files = null;
            try
            { 
                files  = Directory.GetFiles(path_name);
                foreach (string file in files)
                    DirectoriesComboBox.Items.Insert(0, Path.GetFileName(file));
            }
            catch (Exception ex) //не нашли папку с файлами
            {
                Logger.WriteLine(ex.Message + ". Код ошибки #4002");
                DirectoriesComboBox.Items.Insert(0, "Файлы с мелодией отсутствуют");
                Mute_btn.IsEnabled = false;
                Volume_slider.IsEnabled = false;
                Calling_MediaButton.IsEnabled = false;
            }     


            //выставляем значение мелодии в combobox из настроек
            int index_m = 0;
            if (DirectoriesComboBox.Items.Count > 2)
            {
                //проверям, существует ли в директории Melody файл из настроек, если true, то ищем его индек
                if (files.Contains(path_name + Properties.Settings.Default.DefaultMelody))
                {
                    foreach (string file in files)
                    {
                        String str = Path.GetFileName(file);
                        if (str == Properties.Settings.Default.DefaultMelody)
                            break;
                        index_m++;
                    }
                }
            }
            DirectoriesComboBox.SelectedIndex = index_m;

            //выставляем значение времени проигрывания мелодии в combobox
            int index_t = 0;
            if (Music_time_cb.Items.Count > 2)
            {
                switch (Properties.Settings.Default.Melody_time)
                {
                    case 3:
                        index_t = 0;
                        break;
                    case 5:
                        index_t = 1;
                        break;
                    case 10:
                        index_t = 2;
                        break;
                    case 20:
                        index_t = 3;
                        break;
                    case 25:
                        index_t = 4;
                        break;
                    default:
                        index_t = 0;
                        break;
                }
            }
            Music_time_cb.SelectedIndex = index_t;

            DataContext = this;
        }

        //устанавливаем путь к файлу с мелодией для вызова
        private void BillOpenFile(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fn = Path.GetFileName(openFileDialog.FileName);
                if (fn.Length > 35)
                {
                    fn = fn.Substring(0, 35) + "...";
                }

                try
                {
                    System.IO.File.Copy(openFileDialog.FileName, path_name + Path.GetFileName(openFileDialog.FileName), true);
                    DirectoriesComboBox.Items.Insert(0, Path.GetFileName(fn));
                }
                catch
                {
                    System.Windows.MessageBox.Show("Невозможно скопировать файл мелодии. Код ошибки #4001.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                DirectoriesComboBox.SelectedIndex = 0;
            }
        }

        private void CallingPlay(object sender, RoutedEventArgs e)
        {
            if (Calling_MediaButton.Content == FindResource("Play1"))
            {
                String str = path_name + Properties.Settings.Default.DefaultMelody;
                mp.Open(new Uri(str));
                mp.Play();
                mp.MediaEnded += media_MediaEnded1;
                Calling_MediaButton.Content = FindResource("Stop1");

                timer.Interval = TimeSpan.FromSeconds(Properties.Settings.Default.Melody_time);
                timer.Start();
                timer.Tick += (o, args) =>
                {
                    timer.Stop();
                    mp.Stop();
                    Calling_MediaButton.Content = FindResource("Play1");
                };
            }
            else
            {
                Calling_MediaButton.Content = FindResource("Play1");
                mp.Stop();
            }
        }

        //меняет значек на стрелку
        private void media_MediaEnded1(object sender, EventArgs e)
        {
            Calling_MediaButton.Content = FindResource("Play1");
        }

        private void CallingFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DefaultMelody = openFileDialog.FileName;
                Properties.Settings.Default.Save();

                string fn = Path.GetFileName(openFileDialog.FileName);
                if (fn.Length > 35)
                {
                    fn = fn.Substring(0, 35) + "...";
                }
                //calling_tb.Text = fn;
                //LoadProgressBar(calling_pb);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //Properties.Settings.Default.SoundValue = Math.Round(Volume_slider.Value);
            //Properties.Settings.Default.Save();
            //mediaPlayer.Dispose();
        }

        private void ClickMuteButton(object sender, RoutedEventArgs e)
        {
            var mute_b = (System.Windows.Controls.Button)sender;

            //если звук не выключен, то его нужно выключить
            if (mute_b.Content != FindResource("VolumeOff"))
            {
                Properties.Settings.Default.SoundValue = Math.Round(Volume_slider.Value);
                Properties.Settings.Default.Save();
                mute_b.Content = FindResource("VolumeOff");
                Volume_slider.Value = 0;
                mp.Volume = 0;
            }
            //если звук выключен, то его нужно включить, используя предыдущий уровень громкости
            else
            {
                Volume_slider.Value = Properties.Settings.Default.SoundValue;

                if (Volume_slider.Value == 0)
                {
                    Volume_slider.Value = 20;
                }
                if (0 < Volume_slider.Value && Volume_slider.Value <= 50)
                    mute_b.Content = FindResource("VolumeMedium");
                else if (Volume_slider.Value > 50)
                    mute_b.Content = FindResource("VolumeHigh");

                mp.Volume = (Volume_slider.Value) / 100;
            }
        }

        //устанавливаем значек в соответствии со слайдом
        private void MuteButton(double d)
        {
            if (d == 0)
                Mute_btn.Content = FindResource("VolumeOff");
            else if (0 < d && d <= 0.5)
                Mute_btn.Content = FindResource("VolumeMedium");
            else if (d > 0.5)
                Mute_btn.Content = FindResource("VolumeHigh");
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = (Slider)sender;

            if (Mute_btn != null)
            {
                
                double vlm = (s.Value) / 100;
                mp.Volume = vlm;
                Properties.Settings.Default.SoundValue = Math.Round(s.Value);
                Properties.Settings.Default.Save();
                MuteButton(vlm);
            }
        }

        private void Music_time_Selected(object sender, RoutedEventArgs e)
        {
            int i = 0;
            var cb = (System.Windows.Controls.ComboBox)sender;
            switch(cb.SelectedIndex)
            {
                case 0:
                    i = 3;
                    break;
                case 1:
                    i = 5;
                    break;
                case 2:
                    i = 10;
                    break;
                case 3:
                    i = 20;
                    break;
                case 4:
                    i = 25;
                    break;
            }
            Properties.Settings.Default.Melody_time = i;
            Properties.Settings.Default.Save();
        }

        private void DirectoriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = (System.Windows.Controls.ComboBox)sender;
            Properties.Settings.Default.DefaultMelody = cb.SelectedItem.ToString();
            Properties.Settings.Default.Save();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            //загружаем количество фактически поключенных COM портов
            string[] ports = cp.LoadComPort();
            ObservableCollection<string> ports_list = new ObservableCollection<string>(ports);
            LoadPorts_cb.ItemsSource = ports_list;

            int count_p = ports.Length;
            if (count_p > 0)
            {
                //берем из настроек название COM порта
                string comport_in_settings = Properties.Settings.Default.ComPortName;

                //ищем в массиве подключенных COM портов порт указанный в настройках программы
                int index = Array.IndexOf(ports, comport_in_settings);
                if (index >= 0)
                {
                    LoadPorts_cb.SelectedIndex = index;
                }
            }*/
        }

        private void OnSelectComPort(object sender, RoutedEventArgs e)
        {
            /*
            string cp_name = (string)LoadPorts_cb.SelectedItem;


            if (cp_name != null)
            {
                //закрываем COM порт, если открыт
                if (cp.IsOpen())
                    cp.ClosePort();
                
                MainWindow mw = System.Windows.Application.Current.Windows[0] as MainWindow;

                if (cp.OpenPort(cp_name))
                {
                    //сохраняем COM порт в настройках
                    Properties.Settings.Default.ComPortName = cp_name;
                    Properties.Settings.Default.Save();


                    Dispatcher.BeginInvoke(new Action(() => {
                        mw.Cp_status_tb.Text = "Состояние приемника: подключен к порту " + cp_name;
                        mw.Antenna_pi.Foreground = new SolidColorBrush(Colors.Green);
                    }));
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() => {
                        mw.Cp_status_tb.Text = "Состояние приемника: не обнаружен. Код ошибки #4003";
                        mw.Antenna_pi.Foreground = new SolidColorBrush(Colors.Red);
                    }));
                }
            }
            */
        }

        private void Delete_callingBD(object sender, RoutedEventArgs e)
        {
            SqliteDataAccess DB = new SqliteDataAccess();
            DB.DeleteAllButtonsFromCallingBD();
            //ShowDeleteDialog();
        }

        async private void ShowDeleteDialog()
        {
            DeleteButtonsFromCallingBD dialog = new DeleteButtonsFromCallingBD();
            this.IsEnabled = false;
            dialog.Show();
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
            });
            dialog.Close();
            this.IsEnabled = true;
        }

        private void Delete_setupBD(object sender, RoutedEventArgs e)
        {
            SqliteDataAccess DB = new SqliteDataAccess();
            DB.DeleteAllButtonsFromSetupBD();
            //ShowDeleteDialog();
        }

        /*
        private void Music_time_Selected(object sender, SelectionChangedEventArgs e)
        {
            var cb = (System.Windows.Controls.ComboBox)sender;
            //Properties.Settings.Default.Melody_time
            ComboBoxItem selectedItem = (ComboBoxItem)cb.SelectedItem;
            //string i = selectedItem.Content.ToString();
            Properties.Settings.Default.Save();
        }
        */
    }
}
