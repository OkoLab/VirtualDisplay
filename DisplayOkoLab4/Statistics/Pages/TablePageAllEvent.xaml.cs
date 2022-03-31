using System;
using System.Collections.Generic;
using System.Data;
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

namespace VirtualDisplay.Statistics.Pages
{
    /// <summary>
    /// Interaction logic for TablePageAllEvent.xaml
    /// </summary>
    public partial class TablePageAllEvent : Page
    {
        private int numberOfRecPerPage;
        static Paging PagedTable = new Paging();
        static ButtonModel ButtonList = new ButtonModel();
        IList<StatisticStructureAllEvent> statisticStructuresAllEvent;

        public TablePageAllEvent(String bstr, String estr)
        {
            IList<ButtonModel> myList = ButtonList.GetData(bstr, estr);

            statisticStructuresAllEvent = CopyStructure(myList);
            statisticStructuresAllEvent = statisticStructuresAllEvent.OrderByDescending(f => f.BtnDate).ToList();

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

                DataTable firstTable = PagedTable.SetPaging(statisticStructuresAllEvent, numberOfRecPerPage); //Fill a 
                                                                                                      //DataTable with the First set based on the numberOfRecPerPage

                //firstTable.Columns[0].ColumnName = "ID кнопка";
                dataGrid.ItemsSource = firstTable.DefaultView; //Fill the dataGrid with the 
                                                               //DataTable created previously
        }

        List<StatisticStructureAllEvent> CopyStructure(IList<ButtonModel> myList)
        {
            List<StatisticStructureAllEvent> stat_struct_list = new List<StatisticStructureAllEvent>(myList.Count);

            if (myList != null && myList.Count > 0)
            {
                for (int j = 0; j < myList.Count; j++)
                {
                    StatisticStructureAllEvent stat_struct_item = new StatisticStructureAllEvent();

                    stat_struct_item.BtnDate = myList[j].BtnDate;
                    stat_struct_item.BtnName = myList[j].BtnName;
                    //stat_struct_item.BtnType = myList[j].BtnFunction;
                    stat_struct_list.Add(stat_struct_item);
                }
            }

            return stat_struct_list;
        }
        public string PageNumberDisplay()
        {

                int PagedNumber = numberOfRecPerPage * (PagedTable.PageIndex + 1);
                int StartPageNumber = PagedNumber - numberOfRecPerPage + 1;
                if (PagedNumber > statisticStructuresAllEvent.Count)
                {
                    PagedNumber = statisticStructuresAllEvent.Count;
                }
                return StartPageNumber + "-" + PagedNumber + " из " + statisticStructuresAllEvent.Count; //This dramatically 
                                                                                                 //reduced the number of times I had to write this string statement
        }


            private void NextButton_Click(object sender, RoutedEventArgs e)
            {
                dataGrid.ItemsSource = PagedTable.Next(statisticStructuresAllEvent, numberOfRecPerPage).DefaultView;
                PageInfo.Content = PageNumberDisplay();
            }

            private void PreviousButton_Click(object sender, RoutedEventArgs e)
            {
                dataGrid.ItemsSource = PagedTable.Previous(statisticStructuresAllEvent, numberOfRecPerPage).DefaultView;
                PageInfo.Content = PageNumberDisplay();
            }

            private void FirstButton_Click(object sender, RoutedEventArgs e)
            {
                dataGrid.ItemsSource = PagedTable.First(statisticStructuresAllEvent, numberOfRecPerPage).DefaultView;
                PageInfo.Content = PageNumberDisplay();
            }

            private void LastButton_Click(object sender, RoutedEventArgs e)
            {
                dataGrid.ItemsSource = PagedTable.Last(statisticStructuresAllEvent, numberOfRecPerPage).DefaultView;
                PageInfo.Content = PageNumberDisplay();
            }

            private void NumberOfRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                numberOfRecPerPage = Convert.ToInt32(NumberOfRecords.SelectedItem);
                dataGrid.ItemsSource = PagedTable.First(statisticStructuresAllEvent, numberOfRecPerPage).DefaultView;
                PageInfo.Content = PageNumberDisplay();
            }
        
    }
}
