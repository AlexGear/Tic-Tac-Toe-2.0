using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Info;

namespace Крестики_нолики_2._0 {
    public partial class NewGame : PhoneApplicationPage {
        private const uint ERR_BLUETOOTH_OFF = 0x8007048F;
        private const string defaultFirstMove = "Random";
        private string firstMove;
        private string rivalName;
        private PeerWatcher peerWatcher;
        private ObservableCollection<PeerAppInfo> peerApps;

        public NewGame() {
            InitializeComponent();
        }

        private bool IsBluetoothGame() {
            return Convert.ToBoolean(NavigationContext.QueryString["bluetooth"]);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e) {
            bool bluetooth = IsBluetoothGame();
            startBtn.IsEnabled = bluetooth ? false : true;
            startBtn.Content = bluetooth ? "Пригласить в игру" : "Начать игру";
            fieldSizeControl.Maximum = Consts.MaxFieldSize;
        }        
        
        private void ConnectionRequested(object sender, ConnectionRequestedEventArgs args) {
            try {
                Dispatcher.BeginInvoke(() => {
                    Bluetooth.ConnectToPeer(args.PeerInformation);
                    rivalName = args.PeerInformation.DisplayName;
                    var result = MessageBox.Show(String.Format("Пользователь {0} предлагает вам игру. Принять приглашение?",
                                                                rivalName), "Приглашение", MessageBoxButton.OKCancel);
                    if(result == MessageBoxResult.OK) {
                        Bluetooth.SendCommand(BluetoothCommands.AcceptInvite);
                    }
                    else {
                        Bluetooth.SendCommand(BluetoothCommands.DiscardInvite);
                        Bluetooth.Disconnect();
                    }
                });
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if(IsBluetoothGame()) {
                firstMoveGrid.Visibility = System.Windows.Visibility.Collapsed;
                peersGrid.Visibility = System.Windows.Visibility.Visible;

                peerApps = new ObservableCollection<PeerAppInfo>();
                peerList.ItemsSource = peerApps;
                PeerFinder.ConnectionRequested += ConnectionRequested;
                PeerFinder.Start();
                FindPeers();
            }
            else {                
                firstMoveGrid.Visibility = System.Windows.Visibility.Visible;
                peersGrid.Visibility = System.Windows.Visibility.Collapsed;
            }

            base.OnNavigatedTo(e);
        }
        
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
            StopWatching();
            PeerFinder.Stop();
            PeerFinder.ConnectionRequested -= ConnectionRequested;
            base.OnNavigatingFrom(e);
        }

        private void ShowBluetoothSettings() {
            ConnectionSettingsTask bluetoothSettingsTask = new ConnectionSettingsTask() {
                ConnectionSettingsType = ConnectionSettingsType.Bluetooth
            };
            bluetoothSettingsTask.Show();
        }

        private async void FindPeers() {
            try {
                var peers = await PeerFinder.FindAllPeersAsync();
                peerApps.Clear();
                StartWatching();
            }
            catch(Exception ex) {
                if((uint)ex.HResult == ERR_BLUETOOTH_OFF) {
                    PeerFinder.Stop();
                    var result = MessageBox.Show("Bluetooth отключен. Перейти в настройки Bluetooth?", "Bluetooth отключен", MessageBoxButton.OKCancel);
                    if(result == MessageBoxResult.OK) {
                        ShowBluetoothSettings();
                    }
                    NavigationService.GoBack();
                }
            }
        }

        private void StartWatching() {
            if(peerWatcher == null) {
                peerWatcher = PeerFinder.CreateWatcher();
            }
            peerWatcher.Added += (PeerWatcher sender, PeerInformation peer) => {
                Dispatcher.BeginInvoke(() => { peerApps.Add(new PeerAppInfo(peer)); });
            };
            peerWatcher.Removed += (PeerWatcher sender, PeerInformation peer) => {
                Dispatcher.BeginInvoke(() => {
                    foreach(var peerApp in peerApps) {
                        if(peerApp.PeerInfo.Equals(peer)) {
                            peerApps.Remove(peerApp);
                            break;
                        }
                    }
                });
            };
            peerWatcher.Start();
        }

        private void StopWatching() {
            if(peerWatcher != null) {
                peerWatcher.Stop();
                peerWatcher = null;
            }            
        }

        private int GetFieldSize() {
            return Convert.ToInt32(fieldSizeControl.Value);
        }

        private string GetFirstMove() {
            if(firstMove == null) {
                firstMove = defaultFirstMove;
            }
            return firstMove;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            if(IsBluetoothGame()) {
                var peer = peerList.SelectedItem as PeerAppInfo;
                Bluetooth.ConnectToPeer(peer.PeerInfo);
                Bluetooth.CommandRecieved += (byte command) => {
                    switch(command) {
                        case BluetoothCommands.AcceptInvite:
                            MessageBox.Show("accept");
                            break;
                        case BluetoothCommands.DiscardInvite:
                            MessageBox.Show("discard");
                            break;
                    }
                };
            }
            else {
                NavigationService.Navigate(new Uri(String.Format("/GamePage.xaml?FieldSize={0}&FirstMove={1}",
                                                               GetFieldSize(), GetFirstMove()), UriKind.Relative));
            }
        }

        private void fieldSizeControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if(fieldSizeControl != null) {
                fieldSizeShow.Text = String.Format("{0} x {0}", GetFieldSize());
            }
        }

        private void FirstControlChecked(object sender, RoutedEventArgs e) {
            firstMove = (sender as RadioButton).Tag as string;
        }

        private void peerList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            startBtn.IsEnabled = (peerList.SelectedItem != null);
        }       
    }
}