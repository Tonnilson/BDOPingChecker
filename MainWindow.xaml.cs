using System;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.Net;

namespace BDOPingChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static BackgroundWorker pingWorker = new BackgroundWorker();
        private static IPAddress worldIP = IPAddress.None;
        private System.Windows.Forms.NotifyIcon notifyIcon;

        enum ServiceType
        {
            Auth = 8888,
            Lobby = 8889,
            Chat = 8883,
            World = 8884
        }

        public MainWindow()
        {
            InitializeComponent();
            this.MouseDown += delegate { try { if(!Properties.Settings.Default.Locked) DragMove(); } catch (Exception) { } };

            notifyIcon = new System.Windows.Forms.NotifyIcon(); //Create a TaskTray item
            notifyIcon.Icon = Properties.Resources.bdo_blackspirit; //Set Tasktray icon
            notifyIcon.Visible = true;
            notifyIcon.Text = "Black Desert Ping Checker";

            notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Settings").Click += (s, e) => this.ShowSettingsMenu();
            notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => CloseApp();
            this.ShowInTaskbar = false;
        }

        //Application being closed, close all active windows (in case settings was some how left open..?)
        private void CloseApp()
        {
            foreach (Window window in Application.Current.Windows)
                window.Close();
        }

        private void ShowSettingsMenu() => 
            new SettingsWindow().Show();

        private void Window_Deactivated(object sender, EventArgs e) => ((Window)sender).Topmost = true;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = Properties.Settings.Default.LastPosition.X; //Set last known position of X
            this.Top = Properties.Settings.Default.LastPosition.Y; //Set the last known position of Y
            BackgroundOpacity.Opacity = Properties.Settings.Default.WindowOpacity; //Set the background opacity based off saved opacity.

            pingWorker.DoWork += new DoWorkEventHandler(checkServicePings);
            pingWorker.RunWorkerAsync(); //Run our ping check in the background on a different thread
        }

        private void checkServicePings(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                worldIP = FetchServiceIP(ServiceType.World); //Find a remote IP connection based on the World Port

                try
                {
                    //Check if a connection was found
                    if (worldIP == IPAddress.None)
                        throw new Exception();

                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        socket.Blocking = true;

                        //Create a stopwatch and start
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        //Start the TCP Connection and wait for it to connect so we can get the response time
                        socket.ReceiveTimeout = 9999; //Set timeout for 9.9s
                        socket.Connect(worldIP, (int)ServiceType.World);
                        stopwatch.Stop(); //Stop watching

                        labelContent(WorldLabel, String.Format("{0}ms", Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds))); //Calculate response time and display
                        socket.Close();
                    }
                }
                catch (Exception)
                {
                    //Most likely couldn't find the connection or maybe even timed out.
                    labelContent(WorldLabel, "NCON");
                }
                finally
                {
                    Thread.Sleep(900); //Create some slack
                }
            }
        }

        //Dispatch helper to commit changes on the UI Thread
        public static void labelContent(Label label, string Content) => label.Dispatcher.BeginInvoke(new Action(() => { label.Content = Content; }));

        private static IPAddress FetchServiceIP(ServiceType port)
        {
            TcpConnectionInformation[] connections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections(); //Fetch all active TCP Connections from the system
            
            //Loop through all the connections and find the endpoint that has the targeted port
            foreach (TcpConnectionInformation c in connections)
                if (c.RemoteEndPoint.Port == (int)port && c.State == TcpState.Established)
                    return c.RemoteEndPoint.Address; //Match found, return the remote endpoint IP

            return IPAddress.None; //No match
        }

        //Executed on application close, save current position on screen and opacity of background.
        private void SaveSettings(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.LastPosition = RestoreBounds.Location;
            Properties.Settings.Default.WindowOpacity = BackgroundOpacity.Opacity;
            Properties.Settings.Default.Save();
            notifyIcon.Dispose();
        }
    }
}
