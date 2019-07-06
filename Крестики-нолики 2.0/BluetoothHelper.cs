using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
    public class PeerAppInfo {
        public PeerAppInfo(PeerInformation peerInformation) {
            PeerInfo = peerInformation;
            DisplayName = PeerInfo.DisplayName;
        }

        public string DisplayName { get; private set; }
        public PeerInformation PeerInfo { get; private set; }
    }

    public class Bluetooth {
        private static StreamSocket socket;
        private static DataWriter dataWriter;
        private static DataReader dataReader;

        public delegate void CommandRecievedHandler(byte command);
        public static event CommandRecievedHandler CommandRecieved;

        public static async void ConnectToPeer(PeerInformation peer) {
            try {
                socket = await PeerFinder.ConnectAsync(peer);
                PeerFinder.Stop();
                ListenForIncomingCommand();
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private static async void ListenForIncomingCommand() {
            byte command = await GetCommand();
            if(CommandRecieved != null)
                CommandRecieved(command);

            ListenForIncomingCommand();
        }

        private static async Task<byte> GetCommand() {
            if(dataReader == null)
                dataReader = new DataReader(socket.InputStream);
            await dataReader.LoadAsync(sizeof(byte));
            byte command = dataReader.ReadByte();            
            return command;
        }

        private static async Task<Int32> GetInt32() {
            if(dataReader == null)
                dataReader = new DataReader(socket.InputStream);

            await dataReader.LoadAsync(sizeof(Int32));
            return dataReader.ReadInt32();
        }

        public static void Disconnect() {
            if(dataReader != null) {
                dataReader.Dispose();
                dataReader = null;
            }

            if(dataWriter != null) {
                dataWriter.Dispose();
                dataWriter = null;
            }

            if(socket != null) {
                socket.Dispose();
                socket = null;
            }
        }

        public static async void SendCommand(byte command) {
            while(socket == null) ;
            if(dataWriter == null)
                dataWriter = new DataWriter(socket.OutputStream);

            dataWriter.WriteByte(command);
            await dataWriter.StoreAsync();
        }

        public static async void SendInt32(Int32 num) {
            if(dataWriter == null)
                dataWriter = new DataWriter(socket.OutputStream);

            dataWriter.WriteInt32(num);
            await dataWriter.StoreAsync();
        }
    }
}