using VirtualDisplay.DisplaySetup.Pages;
using System;
using System.Collections.Generic;
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

namespace VirtualDisplay.DisplaySetup
{
    /// <summary>
    /// Interaction logic for DisplaySetupUserControl.xaml
    /// </summary>
    public partial class DisplaySetupUserControl : UserControl
    {
        public DisplayAddBtnPage Page1 { get; private set; }
        public DisplayDltBtnPage Page2 { get; private set; }
        public DisplayGeneralSettingPage Page3 { get; private set; }
        public DisplayAboutBtnPage Page4 { get; private set; }

        ComPortConnector comPort;

        public DisplaySetupUserControl(ComPortConnector cp)
        {
            comPort = cp;
            Page1 = new DisplayAddBtnPage(comPort);
            Page3 = new DisplayGeneralSettingPage();
            Page4 = new DisplayAboutBtnPage();

            InitializeComponent();

            MainFrame.Content = Page1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);

            GridCursor.Margin = new Thickness(10 + (200 * index), 0, 0, 0);

            switch (index)
            {
                case 0:
                    Page1 = new DisplayAddBtnPage(comPort);
                    MainFrame.Content = Page1;
                    break;
                case 1:
                    Page2 = new DisplayDltBtnPage(comPort);
                    MainFrame.Content = Page2;
                    break;
                case 2:
                    MainFrame.Content = Page3;
                    break;
                case 3:
                    MainFrame.Content = Page4;
                    break;
            }
        }
    }
}
