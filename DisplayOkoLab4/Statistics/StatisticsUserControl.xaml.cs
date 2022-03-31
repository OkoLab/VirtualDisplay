using VirtualDisplay.Statistics.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace VirtualDisplay
{
    /// <summary>
    /// Interaction logic for StatisticsUserControl.xaml
    /// </summary>
    public partial class StatisticsUserControl : UserControl
    {
        private ComPortConnector cp;
        private SqliteDataAccess db;

        private const string ACTIVE_STATUS = "active";
        private const string SERVED_STATUS = "served";

        private TablePage Table_p { get; set; }
        private TablePageAllEvent TableAllEvent_p { get; set; }
        private ChartPage Chart_p { get; set; }

        public StatisticsUserControl(ComPortConnector cp1)
        {
            InitializeComponent();

            db = new SqliteDataAccess();

            // Open Com port and add an event of data received
            this.cp = cp1;
            this.cp.DataReceivedEvent += SerialPort_DataReceived;
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

                btn.BtnStatus = ACTIVE_STATUS;


                if (btn.BtnName != null)
                {
                    btn.BtnDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    //btn.BtnDate = DateTime.Now.ToString("F");

                    db.SaveButtonsDisplay(btn);
                }
            }
        }

        private void DateSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime now = DateTime.Today;

            switch ((sender as ComboBox).SelectedIndex)
            {
                case 0: //сегодня
                    Begin_dp.SelectedDate = now;
                    End_dp.SelectedDate = now;
                    break;
                case 1: //вчера
                    Begin_dp.SelectedDate = now.AddDays(-1);
                    End_dp.SelectedDate = now.AddDays(-1);
                    break;
                case 2: //этот месяц
                    Begin_dp.SelectedDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    End_dp.SelectedDate = now;
                    break;
                case 3: //прошлый месяц
                    if (now.Month == 1) //проверяем, есди дата равна 1 января, то кроме месяца меняем год
                    {
                        if (now.Day == 1)
                        {
                            Begin_dp.SelectedDate = new DateTime(now.AddYears(-1).Year, now.AddMonths(-1).Month, 1, 0, 0, 0, DateTimeKind.Utc);
                            End_dp.SelectedDate = new DateTime(now.AddYears(-1).Year, now.AddMonths(-1).Month, DateTime.DaysInMonth(now.AddYears(-1).Year, now.AddMonths(-1).Month), 0, 0, 0, DateTimeKind.Utc);
                        }
                    }
                    else
                    {
                        Begin_dp.SelectedDate = new DateTime(now.Year, now.AddMonths(-1).Month, 1, 0, 0, 0, DateTimeKind.Utc);
                        End_dp.SelectedDate = new DateTime(now.Year, now.AddMonths(-1).Month, DateTime.DaysInMonth(now.Year, now.AddMonths(-1).Month), 0, 0, 0, DateTimeKind.Utc);
                    }
                    break;
                case 4:  //год                   
                    Begin_dp.SelectedDate = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    End_dp.SelectedDate = now;
                    break;
            }
        }                

        private void OnClickFindButton(object sender, RoutedEventArgs e)
        {
            if (Begin_dp.SelectedDate == null)
            {
                MessageBox.Show("Заполните, пожалуйста, начало периода", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                Begin_dp.Focus();
            }
            else if(End_dp.SelectedDate == null)
            {
                MessageBox.Show("Заполните, пожалуйста, конец периода", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                End_dp.Focus();
            }
            else
            {
                DateTime bdt = Begin_dp.SelectedDate.Value.Date;
                DateTime edt = End_dp.SelectedDate.Value.Date;
                if (DateTime.Compare(bdt, edt) > 0)
                {
                    MessageBox.Show("Дача начало периода должна быть раньше, чем дата конца периода", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    Begin_dp.Focus();
                }
                else
                {
                    String bstr = bdt.ToString("yyyy-MM-dd");
                    String estr = edt.ToString("yyyy-MM-dd");

                    /* выбор отображения ПЭК 
                    if (summary_statistics_rb.IsChecked == true)
                    {
                        //отображаем сводную таблицу
                        Table_p = new TablePage(bstr, estr);
                        MainFrame.Content = Table_p;
                    }
                    else
                    {
                        TableAllEvent_p = new TablePageAllEvent(bstr, estr);
                        MainFrame.Content = TableAllEvent_p;

                    }
                    */

                    //выбор отображения статистики: таблица или график
                    if (table_rb.IsChecked == true)
                    {
                        TableAllEvent_p = new TablePageAllEvent(bstr, estr);
                        MainFrame.Content = TableAllEvent_p;
                    }
                    else
                    {
                        Chart_p = new ChartPage(bstr, estr);
                        MainFrame.Content = Chart_p;
                    }                    
                } 
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            cp.DataReceivedEvent -= SerialPort_DataReceived;
        }
    }
}
    