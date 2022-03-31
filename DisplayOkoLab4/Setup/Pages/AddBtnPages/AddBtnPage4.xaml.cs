using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace VirtualDisplay.Setup.Pages.AddBtnPages
{
    /// <summary>
    /// Interaction logic for AddBtnPage3.xaml
    /// </summary>
    public partial class AddBtnPage4 : Page
    {        
        public AddBtnPage4(List<SaveButtonModel> l_sb)
        {
            InitializeComponent();

            //устанавливаем ширину списка по размеру окна, так без этого не будет работать горизонтальная прокрутка
            btn_list_dg.Width = SystemParameters.PrimaryScreenHeight;

            //отображаем уже настроенные кнопки
            ShowBtn(l_sb);
        }

        //Binding Setup BD and Delete_btn_lv 
        private void ShowBtn(List<SaveButtonModel> l_sb)
        {
            //Collection of buttons save ListView 
            ObservableCollection<SaveButtonModel> btn_list;

            if (l_sb != null)
                btn_list = new ObservableCollection<SaveButtonModel>(l_sb);
            else
                btn_list = new ObservableCollection<SaveButtonModel>();

            btn_list_dg.ItemsSource = btn_list;
        }
    }
}
