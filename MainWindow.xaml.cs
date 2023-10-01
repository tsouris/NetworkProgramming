using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace NetworkProgramming
{
    //TASK 1
    //Create a server application that allows you to find prices for computer components.A typical example of operation:
    //The client application connects to the server;
    //The client application sends a request for the price of a specific component(for example, the price of a processor);
    //The server returns a response;
    //The client can send a new request or disconnect.
    //A large number of clients can be connected to one server simultaneously.Use UDP sockets to solve this task.
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private IPEndPoint ep;

        public MainWindow()
        {
            InitializeComponent();

            udpClient = new UdpClient();
            ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
        }

        private void GetPrice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string componentName = tbComponent.Text;

                byte[] requestData = Encoding.UTF8.GetBytes(componentName);
                udpClient.Send(requestData, requestData.Length, ep);

                byte[] responseData = udpClient.Receive(ref ep);
                string price = Encoding.UTF8.GetString(responseData);

                PriceTextBlock.Text = price;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                udpClient.Close();
                MessageBox.Show("Disconnected from server.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
