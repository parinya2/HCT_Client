using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using SimpleWifi;
using System.IO;
using System.Timers;

namespace HCT_Client
{
    class WifiManager
    {
        static bool AUTO_WIFI_CONNECT_AVAILABLE = false;
        static WifiManager instance;
        //static Wifi wifi;
        static string targetWifiName = null;
        static string targetWifiPassword = null;
        static bool isConnectingWifi = false;

        Timer timer;
        const int TIMER_INTERVAL_MS = 3000;        

        private WifiManager()
        {
            if (AUTO_WIFI_CONNECT_AVAILABLE)
            {
                timer = new Timer();
                timer.Elapsed += new ElapsedEventHandler(TimerEvent);
                timer.Interval = TIMER_INTERVAL_MS;
                timer.Enabled = true;
            }
        }

        public static WifiManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WifiManager();
                }
                return instance;
            }
        }

        public static void InitInstance()
        {
            LoadWifiNameAndPassword();

            if (instance == null)
            {
                instance = new WifiManager();
            }

            if (!AUTO_WIFI_CONNECT_AVAILABLE)
                return;

            //wifi = new Wifi();
        }

        private static void TimerEvent(object source, ElapsedEventArgs e)
        {
            if (isConnectingWifi)
                return;

            try
            {
                /*List<AccessPoint> aps = wifi.GetAccessPoints();
                foreach (AccessPoint ap in aps)
                {
                    if (ap.Name.Equals(targetWifiName))
                    {
                        if (ap.IsConnected)
                            return;

                        isConnectingWifi = true;

                        AuthRequest request = new AuthRequest(ap);
                        request.Password = targetWifiPassword;
                        ap.ConnectAsync(request, false, x => { isConnectingWifi = false; });
                    }
                }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wifi error = " + ex.ToString());
            }
        }

        private static void LoadWifiNameAndPassword()
        {
            string wifiFilePath = Util.GetExecutingPath() + "/Config/Wifi.txt";
            if (!File.Exists(wifiFilePath))
            {
                AUTO_WIFI_CONNECT_AVAILABLE = false;
                return;
            }                

            string[] contents = File.ReadAllLines(wifiFilePath);
            if (contents.Length >= 2)
            {
                string wifiName = contents[0].Trim();
                string wifiPassword = contents[1].Trim();

                targetWifiName = wifiName.Length > 0 ? wifiName : null;
                targetWifiPassword = wifiPassword.Length > 0 ? wifiPassword : null;
            }

            if (targetWifiName == null || targetWifiPassword == null)
            {
                AUTO_WIFI_CONNECT_AVAILABLE = false;
            }
        }
    }
}
