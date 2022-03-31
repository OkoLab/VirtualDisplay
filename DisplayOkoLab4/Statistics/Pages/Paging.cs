using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualDisplay.Statistics.Pages
{
    class Paging
    {
        public int PageIndex { get; set; }

        DataTable PagedList = new DataTable();

        public DataTable SetPaging(IList<StatisticStructure> ListToPage, int RecordsPerPage)
        {
            int PageGroup = PageIndex * RecordsPerPage;

            //int PageGroup = 1 * RecordsPerPage;

            IList<StatisticStructure> PagedList = new List<StatisticStructure>();

            PagedList = ListToPage.Skip(PageGroup).Take(RecordsPerPage).ToList();

            DataTable FinalPaging = PagedTable(PagedList);

            ///FinalPaging.Columns.Count();


            //удаляем из таблицы колонку ID кнопки от производителя
            ///FinalPaging.Columns.RemoveAt(0);
            FinalPaging.Columns[0].ColumnName = "Время события";
            FinalPaging.Columns[1].ColumnName = "Имя парных кнопок";
            FinalPaging.Columns[2].ColumnName = "Сценарий";
            FinalPaging.Columns[3].ColumnName = "Длительность";
            //FinalPaging.Columns.Add("Время события");
            //FinalPaging.Columns[3].ColumnName = "Длительность";

            return FinalPaging;
        }

        public DataTable SetPaging(IList<StatisticStructureAllEvent> ListToPage, int RecordsPerPage)
        {
            int PageGroup = PageIndex * RecordsPerPage;

            //int PageGroup = 1 * RecordsPerPage;

            IList<StatisticStructureAllEvent> PagedList = new List<StatisticStructureAllEvent>();

            PagedList = ListToPage.Skip(PageGroup).Take(RecordsPerPage).ToList();

            DataTable FinalPaging = PagedTable(PagedList);

            ///FinalPaging.Columns.Count();


            FinalPaging.Columns[0].ColumnName = "Время события";
            FinalPaging.Columns[1].ColumnName = "Имя кнопки";
            //FinalPaging.Columns[2].ColumnName = "Событие";

            return FinalPaging;
        }

        private DataTable PagedTable<T>(IList<T> SourceList)
        {
            Type columnType = typeof(T);
            DataTable TableToReturn = new DataTable();
            

            foreach (var Column in columnType.GetProperties())
            {
                TableToReturn.Columns.Add(Column.Name, Column.PropertyType);
            }

            foreach (object item in SourceList)
            {
                DataRow ReturnTableRow = TableToReturn.NewRow();
                foreach (var Column in columnType.GetProperties())
                {
                    ReturnTableRow[Column.Name] = Column.GetValue(item);
                }
                TableToReturn.Rows.Add(ReturnTableRow);
            }
            return TableToReturn;
        }

        public DataTable Next(IList<StatisticStructure> ListToPage, int RecordsPerPage)
        {
            PageIndex++;
            if (PageIndex >= ListToPage.Count / RecordsPerPage)
            {
                PageIndex = ListToPage.Count / RecordsPerPage;
            }
            PagedList = SetPaging(ListToPage, RecordsPerPage);
            return PagedList;
        }

        public DataTable Previous(IList<StatisticStructure> ListToPage, int RecordsPerPage)
        {
            PageIndex--;
            if (PageIndex <= 0)
            {
                PageIndex = 0;
            }
            PagedList = SetPaging(ListToPage, RecordsPerPage);
            return PagedList;
        }

        public DataTable First(IList<StatisticStructure> ListToPage, int RecordsPerPage)
        {
            PageIndex = 0;
            PagedList = SetPaging(ListToPage, RecordsPerPage);
            return PagedList;
        }

        public DataTable Last(IList<StatisticStructure> ListToPage, int RecordsPerPage)
        {
            PageIndex = ListToPage.Count / RecordsPerPage;
            PagedList = SetPaging(ListToPage, RecordsPerPage);
            return PagedList;
        }

        public DataTable Next(IList<StatisticStructureAllEvent> ListToPage, int RecordsPerPage)
        {
            PageIndex++;
            if (PageIndex >= ListToPage.Count / RecordsPerPage)
            {
                PageIndex = ListToPage.Count / RecordsPerPage;
            }
            PagedList = SetPaging(ListToPage, RecordsPerPage);
            return PagedList;
        }

        public DataTable Previous(IList<StatisticStructureAllEvent> ListToPage, int RecordsPerPage)
        {
            PageIndex--;
            if (PageIndex <= 0)
            {
                PageIndex = 0;
            }
            PagedList = SetPaging(ListToPage, RecordsPerPage);
            return PagedList;
        }

        public DataTable First(IList<StatisticStructureAllEvent> ListToPage, int RecordsPerPage)
        {
            PageIndex = 0;
            PagedList = SetPaging(ListToPage, RecordsPerPage);
            return PagedList;
        }

        public DataTable Last(IList<StatisticStructureAllEvent> ListToPage, int RecordsPerPage)
        {
            PageIndex = ListToPage.Count / RecordsPerPage;
            PagedList = SetPaging(ListToPage, RecordsPerPage);
            return PagedList;
        }
    }
}
