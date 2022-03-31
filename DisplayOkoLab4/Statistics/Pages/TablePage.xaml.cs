using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace VirtualDisplay.Statistics.Pages
{
    /// <summary>
    /// Interaction logic for TablePage.xaml
    /// </summary>
    public partial class TablePage : Page
    {
        private int numberOfRecPerPage;
        static Paging PagedTable = new Paging();
        static ButtonModel ButtonList = new ButtonModel();
        IList<StatisticStructure> statisticStructures;


        string Call_str = "Вызов";
        string Cancel_str = "Отмена";
        string Cancel_without_call_str = "Отмена без вызова";
        string Call_without_cancel_str = "Вызов без отмены";
        string Call_cancel_str = "Вызов-отмена";

        public TablePage(String bstr, String estr)
        {
            IList<ButtonModel> myList = ButtonList.GetData(bstr, estr);


            statisticStructures = StartCalc(myList);
            statisticStructures = statisticStructures.OrderByDescending(f => f.BtnDate).ToList();

            //IList<StatisticStructure> statisticStructures = new StatisticStructure(); 

            InitializeComponent();

            PagedTable.PageIndex = 1; //Sets the Initial Index to a default value

            int[] RecordsToShow = { 10, 20, 30, 50, 100 }; //This Array can be any number groups

            foreach (int RecordGroup in RecordsToShow)
            {
                NumberOfRecords.Items.Add(RecordGroup); //Fill the ComboBox with the Array
            }

            NumberOfRecords.SelectedItem = 10; //Initialize the ComboBox

            numberOfRecPerPage = Convert.ToInt32(NumberOfRecords.SelectedItem); //Convert the 
                                                                                //Combobox Output to type int

            DataTable firstTable = PagedTable.SetPaging(statisticStructures, numberOfRecPerPage); //Fill a 
                                                                                     //DataTable with the First set based on the numberOfRecPerPage

            //firstTable.Columns[0].ColumnName = "ID кнопка";
            dataGrid.ItemsSource = firstTable.DefaultView; //Fill the dataGrid with the 
                                                           //DataTable created previously
        }
           

        public string PageNumberDisplay()
        {

            int PagedNumber = numberOfRecPerPage * (PagedTable.PageIndex + 1);
            int StartPageNumber = PagedNumber - numberOfRecPerPage + 1;
            if (PagedNumber > statisticStructures.Count)
            {
                PagedNumber = statisticStructures.Count;
            }
            return StartPageNumber + "-" + PagedNumber + " из " + statisticStructures.Count; //This dramatically 
                                                                                //reduced the number of times I had to write this string statement
        }

        /*
        public List<StatisticStructure> GetStatistic(String bstr, String estr)
        {
            List<ButtonModel> myList = ButtonList.GetData(bstr, estr);
            return StartCalc(myList);
        }
        */

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = PagedTable.Next(statisticStructures, numberOfRecPerPage).DefaultView;
            PageInfo.Content = PageNumberDisplay();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = PagedTable.Previous(statisticStructures, numberOfRecPerPage).DefaultView;
            PageInfo.Content = PageNumberDisplay();
        }

        private void FirstButton_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = PagedTable.First(statisticStructures, numberOfRecPerPage).DefaultView;
            PageInfo.Content = PageNumberDisplay();
        }

        private void LastButton_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = PagedTable.Last(statisticStructures, numberOfRecPerPage).DefaultView;
            PageInfo.Content = PageNumberDisplay();
        }

        private void NumberOfRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            numberOfRecPerPage = Convert.ToInt32(NumberOfRecords.SelectedItem);
            dataGrid.ItemsSource = PagedTable.First(statisticStructures, numberOfRecPerPage).DefaultView;
            PageInfo.Content = PageNumberDisplay();
        }

        List<StatisticStructure> StartCalc(IList<ButtonModel> myList)
        {
            List<StatisticStructure> stat_struct_list = new List<StatisticStructure>();
            //StatisticStructure stat_struct_item = new StatisticStructure();

            // "алгоритм Олега"
            if (myList != null && myList.Count()>0)
            { 

                string CallEvent = null;
                string buttonFunction;
                string callDuration;

                bool beginFlag = false;
                //int i = myList.Count();
                //for (int j = i; j>= 0; j--)
                for (int j = myList.Count()-1; j >= 0; j--)
                //for (int j = 0; j< myList.Count(); j++)
                {
                    StatisticStructure stat_struct_item = new StatisticStructure();

                    if (beginFlag == false && myList[j].BtnFunction == Call_str)
                    {
                        /*сохраняем данные из текущей (i-той) записи о событии во временные переменные*/
                        //timeEvent = recTime; buttonFunction = recFun; beginFlag = true
                        CallEvent = myList[j].BtnDate;
                        buttonFunction = myList[j].BtnFunction;
                        beginFlag = true;
                    }                    
                    else if(beginFlag == false && myList[j].BtnFunction == Cancel_str)
                    {
                        
                        //записываем "событие" в статистику "отмена без вызова"
                        //            timeToStat = timeEvent, nameToStat =/*должно быть известно*/, funToStat = "отмена"*/
                        /*
                        stat_struct_item.BtnDate = myList[j].BtnDate;
                        stat_struct_item.BtnName = myList[j].BtnName;
                        stat_struct_item.BtnType = myList[j].BtnFunction;
                        stat_struct_item.CallDuration = Cancel_without_call_str;
                        //добавляем строку stat_struct_item в список stat_struct_list
                        stat_struct_list.Add(stat_struct_item);
                        */
                    }
                    else if(beginFlag == true && myList[j].BtnFunction == Call_str)
                    {
                        //записываем прошлое "событие" в статистику "вызов без отмены"
                        //timeToStat = timeEvent, nameToStat =/*должно быть известно*/, funToStat = "вызов", durationtoStat = "-" /* нужнопоставить прочерк*/
                        //сохраняем данные из текущей (i-той) записи о событии*/
                        //timeEvent = recTime; buttonFunction = recFun; beginFlag = true
                        /*
                        stat_struct_item.BtnDate = myList[j].BtnDate;
                        stat_struct_item.BtnName = myList[j].BtnName;
                        stat_struct_item.BtnType = myList[j].BtnFunction;
                        stat_struct_item.CallDuration = Call_without_cancel_str;
                        //добавляем строку stat_struct_item в список stat_struct_list
                        stat_struct_list.Add(stat_struct_item);
                        //beginFlag = true;
                        */
                    }
                    else if(beginFlag == true && myList[j].BtnFunction == Cancel_str)
                    {
                        //timeToStat = timeEvent, nameToStat =/*должно быть известно*/, durationtoStat = recTime - timeToStat /*в минутах*/
                        //beginFlag = false;
                        stat_struct_item.BtnDate = myList[j].BtnDate;
                        stat_struct_item.BtnName = myList[j].BtnName;
                        stat_struct_item.BtnType = Call_cancel_str;

                        DateTime CallEvent_dt = Convert.ToDateTime(CallEvent);
                        DateTime CancelEvent_dt = Convert.ToDateTime(myList[j].BtnDate);

                        TimeSpan diff = CancelEvent_dt - CallEvent_dt;

                        double diff_int = Math.Round(diff.TotalMinutes);
                        if (diff_int < 1)
                        {
                            stat_struct_item.CallDuration = "меньше 1 мин"; /*разница в минутах между нажатием на вызов (timeEvent) и отмену*/
                        }
                        else
                        {
                            stat_struct_item.CallDuration = diff_int.ToString() + " мин"; /*разница в минутах между нажатием на вызов (timeEvent) и отмену*/
                            //добавляем строку stat_struct_item в список stat_struct_list
                            
                        }
                        stat_struct_list.Add(stat_struct_item);
                        beginFlag = false;
                    }
                }
            }

            return stat_struct_list;
        }
    }
}
