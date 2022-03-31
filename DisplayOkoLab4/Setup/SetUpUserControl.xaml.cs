using VirtualDisplay.Setup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VirtualDisplay
{
    /// <summary>
    /// Interaction logic for SetUpUserControl.xaml
    /// </summary>
    /// 

    public partial class SetUpUserControl : UserControl
    {
        public AddBtnPage Page1 { get; private set; }
        public DltBtnPage Page2 { get; private set; }
        public GeneralSettingPage Page3 { get; private set; }
        public AboutPage Page4 { get; private set; }

        ComPortConnector comPort;

        public SetUpUserControl(ComPortConnector cp)
        {
            comPort = cp;
            Page1 = new AddBtnPage(comPort);
            Page3 = new GeneralSettingPage();
            Page4 = new AboutPage();

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
                    Page1 = new AddBtnPage(comPort);
                    MainFrame.Content = Page1;
                    break;
                //case 1:
                  //  Page2 = new DltBtnPage(comPort);
                   // MainFrame.Content = Page2;
                   // break;
                case 1:
                    MainFrame.Content = Page3;
                    break;
                case 2:
                    MainFrame.Content = Page4;
                    break;
            }
        }
    }
}
