using System;
using System.Collections.Generic;
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

namespace VirtualDisplay.Setup.Pages.AddBtnPages
{
    /// <summary>
    /// Interaction logic for AddBtnPage1.xaml
    /// </summary>
    public partial class AddBtnPage1 : Page
    {
        public AddBtnPage1()
        {
            InitializeComponent();

        }

        public string GetBtnName()
        {
            return BtnName_tb.Text;
        }
    }
}
