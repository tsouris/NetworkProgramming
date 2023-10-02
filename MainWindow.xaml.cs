using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;

namespace NetworkProgramming
{
    //TASK 2
    //Add a restriction to the first task regarding the number of requests for a specific client within a certain time interval.
    //For example, a client cannot send more than 10 requests per hour.
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private IPEndPoint ep;
        //Dictionary represents a collection of keys and values in this case requestCount and lastRequestTime
        private Dictionary<string, (int requestCount, DateTime lastRequestTime)> clientRequestInfo = new Dictionary<string, (int, DateTime)>();

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
                //Use IP Adress as the client identifier
                string clientIdentifier = ep.Address.ToString();

                if (clientRequestInfo.TryGetValue(clientIdentifier, out var info))
                {
                    if (info.requestCount >= 10 && (DateTime.Now - info.lastRequestTime).TotalHours < 1)
                    {
                        MessageBox.Show("You have exceeded the limit. Please try again later");
                        return;
                    }
                }

                if(clientRequestInfo.ContainsKey(clientIdentifier))
                {
                    clientRequestInfo[clientIdentifier] = (info.requestCount, DateTime.Now);
                }
                else
                {
                    clientRequestInfo.Add(clientIdentifier, (1, DateTime.Now));
                }

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
