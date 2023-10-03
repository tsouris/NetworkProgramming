using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace NetworkProgramming
{
    //TASK 3
    //Add a restriction to the first task for the number of simultaneously connected clients.
    //If a client is inactive for 10 minutes, they should be disconnected.
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private IPEndPoint ep;
        //Dictionary represents a collection of keys and values in this case requestCount and lastRequestTime
        private Dictionary<string, (int requestCount, DateTime lastRequestTime)> clientRequestInfo = new Dictionary<string, (int, DateTime)>();
        private DispatcherTimer inactivityTimer;
        private const int MAX_CONCURRENT_CLIENTS = 5;

        public MainWindow()
        {
            InitializeComponent();

            udpClient = new UdpClient();
            ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);

            inactivityTimer = new DispatcherTimer();
            inactivityTimer.Interval = TimeSpan.FromMinutes(10);
            inactivityTimer.Tick += CheckForInactiveClients;
            inactivityTimer.Start();
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

                if (clientRequestInfo.ContainsKey(clientIdentifier))
                {
                    clientRequestInfo[clientIdentifier] = (info.requestCount, DateTime.Now);
                }
                else
                {
                    clientRequestInfo.Add(clientIdentifier, (1, DateTime.Now));
                }

                if(clientRequestInfo.Count >= MAX_CONCURRENT_CLIENTS)
                {
                    MessageBox.Show("Too many clients conected at the moment. Please try again later.");
                    return;
                }

                if (clientRequestInfo.ContainsKey(clientIdentifier))
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

        private void CheckForInactiveClients(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            foreach(var client in clientRequestInfo.Keys.ToList())
            {
                if ((now - clientRequestInfo[client].lastRequestTime).TotalMinutes >= 10)
                {
                    clientRequestInfo.Remove(client);
                }
            }
        }
    }
}
