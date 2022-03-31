using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace VirtualDisplay
{
    /// <summary>
    /// Interaction logic for DisplayUserControl.xaml
    /// </summary>LoadedDispla
    public partial class DisplayUserControl : UserControl
    {

        private SqliteDataAccess db;
        private ComPortConnector cp;
        private MediaPlayer mp;

        //Collection of buttons save ListView 
        public ObservableCollection<ButtonModel> btn_list;

        public object InventaireItemGrid { get; private set; }

        public DisplayUserControl(ComPortConnector cp1)
        {
            //устанавливаем проигрываемый аудиофайл из настроек
            mp = new MediaPlayer
            {
                Volume = (Properties.Settings.Default.SoundValue) / 100
            };

            db = new SqliteDataAccess();

            DateTime now = DateTime.Today;


            InitializeComponent();


            //Устанавливаем значек звука в соответствии с настройками
            MuteButton(mp.Volume);

            //устанавливаем ширину списка по размеру окна, так без этого не будет работать горизонтальная прокрутка
            List_dg.Width = SystemParameters.PrimaryScreenHeight;

            string now_str = now.ToString("yyyy-MM-dd");
            //String now_str = now.ToString("yyyy.MM.dd");

            List<ButtonModel> btn_list_temp = new List<ButtonModel>();

            btn_list_temp = db.ShowButtons(now_str, now_str);

            btn_list = new ObservableCollection<ButtonModel>(btn_list_temp);

            List_dg.ItemsSource = btn_list;

            // Open Com port and add an event of data received
            cp = cp1;            
            cp.DataReceivedEvent += SerialPort_DataReceived;

            DataContext = this;
        }

        private void SerialPort_DataReceived(byte[] data)
        {
            if (data != null)
            {
                ButtonModel btn = new ButtonModel
                {
                    BtnId = cp.TakeBtnID(data)
                };
                
                btn.BtnName = db.FindButtonName(btn.BtnId);
                //btn.BtnFunction = db.FindButtonType(btn.BtnId);
                btn.BtnFunction = null;

                btn.BtnKeyCode = cp.TakePushButtonService(data[4]);
             
                if (btn.BtnName != null) //кнопка есть в БД Setup
                {
                    //btn.BtnDate = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                    btn.BtnDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //btn.BtnDate = DateTime.Now.ToString("{0:G}");

                  
                    if (btn.BtnKeyCode == 2) //определяем, что на кнопке нажата отмена
                    {
                        btn.BtnStatus = Constants.SERVED_STATUS;       
                    }
                    else //была нажата любая клавиша кроме отмены
                    {
                        btn.BtnStatus = Constants.ACTIVE_STATUS;
                    }

                    Dispatcher.BeginInvoke(new DisplayCallingBtnEvent(DisplayCallingBtn), btn);
                    db.SaveButtonsDisplay(btn);

                }                
            }
        }

        //отображает нажатую кнопку в списке и издает звуковой сигнал
        private delegate void DisplayCallingBtnEvent(ButtonModel bm);
        private void DisplayCallingBtn(ButtonModel bm)
        {
            //string path = Directory.GetCurrentDirectory()+"\\Melody\\" + Properties.Settings.Default.DefaultMelody;

            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Virtual Display\\Melody\\" + Properties.Settings.Default.DefaultMelody;
            DispatcherTimer timer = new DispatcherTimer();
            mp.Open(new Uri(path));
            mp.Play();


            if (bm.BtnStatus == Constants.SERVED_STATUS) // canceled call
            {
                if (btn_list != null)
                {
                    int indexOfCanceledBtn = btn_list.IndexOf(btn_list.Where(i => i.BtnId == bm.BtnId).FirstOrDefault()); // ищем индекс в списке последний вызов, который нужно удалить
                    if (indexOfCanceledBtn > -1) // если находим такой вызов
                    {
                        Last_call_tb.Text = bm.BtnName + " вызов отменен";
                        btn_list.RemoveAt(indexOfCanceledBtn);
                        btn_list.Insert(indexOfCanceledBtn, bm);
                    }
                }
            }
            else
            {
                Last_call_tb.Text = bm.BtnName;
                btn_list.Insert(0, bm);
            }

            timer.Interval = TimeSpan.FromSeconds(Properties.Settings.Default.Melody_time);
            timer.Start();
            timer.Tick += (o, args) =>
            {
                timer.Stop();
                mp.Stop();
            };            
        }
        
        
        private void DeleteButtonFromList(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            ButtonModel btn = b.DataContext as ButtonModel;


            db.ChangeEventStatusByBtnName(btn.BtnName, Constants.SERVED_STATUS);

            DateTime now = DateTime.Today;
            String now_str = now.ToString("yyyy-MM-dd");

            List<ButtonModel> btn_list_temp = new List<ButtonModel>();

            btn_list_temp = db.ShowButtons(now_str, now_str);

            btn_list = new ObservableCollection<ButtonModel>(btn_list_temp);

            List_dg.ItemsSource = btn_list;


            if (Last_call_tb.Text == btn.BtnName)
                Last_call_tb.Text = String.Empty;


            //Удаляем из textblock's, если удаляемый вызов последний
            if (btn_list.IndexOf(btn)==0) 
            {
                Last_call_tb.Text = String.Empty;
            }

            btn_list.Remove(btn);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SoundValue = Math.Round(Volume_slider.Value);
            Properties.Settings.Default.Save();
            cp.DataReceivedEvent -= SerialPort_DataReceived;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = (Slider)sender;

            if (Mute_btn != null)
            {
                double vlm = (s.Value)/100;               
                mp.Volume = vlm;
                Properties.Settings.Default.SoundValue = Math.Round(s.Value);
                Properties.Settings.Default.Save();
                MuteButton(vlm);
            }
        }

        private void ClickMuteButton(object sender, RoutedEventArgs e)
        {
            var mute_b = (Button)sender;

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

                mp.Volume = (Volume_slider.Value)/100;
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

        public void CloseWindow()
        {
            Properties.Settings.Default.SoundValue = Math.Round(Volume_slider.Value);
            Properties.Settings.Default.Save();
        }
    }
}
