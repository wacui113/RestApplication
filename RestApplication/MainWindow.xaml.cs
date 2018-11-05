using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;

namespace RestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int[] repeatTime = { 15, 20, 30 };

        private static System.Windows.Forms.NotifyIcon notification = new System.Windows.Forms.NotifyIcon();
        private static DispatcherTimer timer = new DispatcherTimer();
        double _End;
        int minutes, second;

        public MainWindow()
        {
            InitializeComponent();

        }

        #region Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBox();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            StartWithOS();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
                Minimize2Tray();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double _now = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second);
            if (_now >= _End)
            {
                StopTimer();
                System.Windows.MessageBox.Show("ĐÃ ĐẾN GIỜ NGHỈ NGƠI", "CẢNH BÁO", MessageBoxButton.OK, MessageBoxImage.Warning);
                RunSleepCommand();
            }
            txblTimer.Text = DisplayCountDown();
        }

        private void MinimizeItem_Click(object sender, RoutedEventArgs e)
        {
            Minimize2Tray();
        }

        private void LoadMenu_Notification(ref System.Windows.Forms.ContextMenu ctMenu)
        {
            ctMenu.MenuItems.Add("Mở giao diện chính", new EventHandler(Open));
            ctMenu.MenuItems.Add("Dừng", new EventHandler(MinimizeNotification));
            ctMenu.MenuItems.Add("Thoát", new EventHandler(Close));
        }

        private void Open(object sender, EventArgs e)
        {
            this.Show();
        }

        private void MinimizeNotification(object sender, EventArgs e)
        {
            notification.ContextMenu.MenuItems[1].Text = HandlerTimer(notification.ContextMenu.MenuItems[1].Text);
        }

        private void Close(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
            btn.Content = HandlerTimer(btn.Content.ToString());
        }
        #endregion

        #region Method

        #region Registry that open with window
        static void StartWithOS()
        {
            RegistryKey regkey = Registry.CurrentUser.CreateSubKey("Software\\RestApplication");
            RegistryKey regstart = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            string keyvalue = "1";
            try
            {
                regkey.SetValue("Index", keyvalue);
                regstart.SetValue("RestApplication", System.Reflection.Assembly.GetExecutingAssembly().Location);
                regkey.Close();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private string DisplayCountDown()
        {
            if (second <= 0)
            {
                minutes--;
                second = 60;
            }
            else
            {
                second--;
            }
            return (second < 10) ? string.Format("{0}:0{1}", minutes, second) : string.Format("{0}:{1}", minutes, second);
        }

        private void RunSleepCommand()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C rundll32.exe powrprof.dll,SetSuspendState 0,1,0";
            process.StartInfo = startInfo;
            process.Start();
        }

        private void LoadComboBox()
        {
            ComboBoxItem cbi = null;
            foreach (int item in repeatTime)
            {
                cbi = new ComboBoxItem();
                cbi.Content = item + " phút";
                cbRepeatTime.Items.Add(cbi);
            }

            cbRepeatTime.SelectedIndex = 0;
        }

        private void Minimize2Tray()
        {
            System.Windows.Forms.ContextMenu ctMenu = new System.Windows.Forms.ContextMenu();
            LoadMenu_Notification(ref ctMenu);

            notification.Icon = new System.Drawing.Icon("rest.ico");
            notification.ShowBalloonTip(500, "Thông báo", "Ứng dụng Nghỉ ngơi đã được thu nhỏ", ToolTipIcon.Info);

            notification.ContextMenu = ctMenu;
            notification.Visible = true;

            this.Hide();
        }

        private string HandlerTimer(string txt)
        {
            switch (txt)
            {
                case "Kích hoạt":
                    ActiveTimer();
                    break;
                case "Dừng":
                    StopTimer();
                    break;
            }

            return txt == "Dừng" ? "Kích hoạt" : "Dừng";
        }

        private void ActiveTimer()
        {
            DateTime _now = DateTime.Now;
            timer.Start();

            int plus = repeatTime[cbRepeatTime.SelectedIndex];
            _End = (_now.Hour * 3600 + _now.Minute * 60 + _now.Second) + plus * 60;
            minutes = plus;
            second = 0;

            Minimize2Tray();

            notification.ShowBalloonTip(700, "Thông báo", "Ứng dụng Nghỉ ngơi đã được kích hoạt", ToolTipIcon.Info);
        }

        private void StopTimer()
        {
            timer.Stop();
            txblTimer.Text = "";
            btnFunction.Content = "Kích hoạt";
            notification.ShowBalloonTip(700, "Thông báo", "Ứng dụng Nghỉ ngơi đã dừng hoạt động", ToolTipIcon.Info);
        }
        #endregion
    }
}
