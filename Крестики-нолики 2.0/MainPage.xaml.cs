using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Devices;
using Microsoft.Phone.Shell;
using Крестики_нолики_2._0.Resources;

namespace Крестики_нолики_2._0 {
    public partial class MainPage : PhoneApplicationPage {
        public MainPage() {
            ThemeManager.ToLightTheme();            
            InitializeComponent();
        }

        private void singleDeviceBtn_Click(object sender, RoutedEventArgs e) {
            NavigationService.Navigate(new Uri(String.Format("/NewGame.xaml?bluetooth={0}", false), UriKind.Relative));
        }

        private void bluetoothBtn_Click(object sender, RoutedEventArgs e) {
            NavigationService.Navigate(new Uri(String.Format("/NewGame.xaml?bluetooth={0}", true), UriKind.Relative));
        }        
    }
}