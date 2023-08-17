using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using YuanliCore.Communication;

namespace SPIL_TCPSimulator
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string ipAddress = "127.0.0.1";
        private int port=1234;
        //  private TcpClient tcpClient;
        ClientCommunication clientCom;
    
        private string reMessage;
        private string sendMessage;
        public MainWindow()
        {
            InitializeComponent();


        }

        public string IpAddress { get => ipAddress; set => SetValue(ref ipAddress, value); }
        public int Port { get => port; set => SetValue(ref port, value); }
        public string ReMessage { get => reMessage; set => SetValue(ref reMessage, value); }
        public string SendMessage { get => sendMessage; set => SetValue(ref sendMessage, value); }
        public ICommand ConnectCommand => new RelayCommand(() =>
        {
            try
            {
      
                clientCom = new ClientCommunication(IpAddress, Port);
               
                clientCom.ReceiverMessage += ReceiverMessage;
                clientCom.ReceiverException += ReceiverException;
                MessageBox.Show("連線完成");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        });
        public ICommand SendMessageCommand => new RelayCommand(() =>
        {
            try
            {
        
                // 傳送指定的 Home 訊息給 B 機器             
                clientCom.Send(SendMessage);

            
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public ICommand OpeneCommand => new RelayCommand(() =>
        {
            try
            {


                clientCom.Open();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public ICommand CloseCommand => new RelayCommand( () =>
        {
            try
            {

                clientCom.Dispose();


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });

        private void ReceiverMessage(string message)
        {

            ReMessage = message;
        }
        private void ReceiverException(Exception exception)
        {
            MessageBox.Show($"{exception.Message}  ++{Thread.CurrentThread.ManagedThreadId}");
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            T oldValue = field;
            field = value;
            OnPropertyChanged(propertyName, oldValue, value);
        }
        protected virtual void OnPropertyChanged<T>(string name, T oldValue, T newValue)
        {
            // oldValue 和 newValue 目前沒有用到，代爾後需要再實作。
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }


}
