using Microsoft.Win32;
using System.Windows.Threading;

namespace RestApplication
{
    public class Function
    {
        private static Function instance;

        public static Function Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new Function();
                }
                return instance;
            }

            private set
            {
                instance = value;
            }
        }

        private static DispatcherTimer timer;

        public static DispatcherTimer Timer
        {
            get
            {
                if (timer == null)
                {
                    timer = new DispatcherTimer();
                    timer.Tick += Timer_Tick;

                }
                return timer;
            }

            private set
            {
                timer = value;
            }
        }

        private static void Timer_Tick(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public void RunSleepCommand()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C rundll32.exe powrprof.dll,SetSuspendState 0,1,0";
            process.StartInfo = startInfo;
            process.Start();
        }

        #region Registry that open with window
        public void StartWithOS()
        {
            RegistryKey regkey = Registry.CurrentUser.CreateSubKey("Software\\RestApp");
            RegistryKey regstart = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            string keyvalue = "1";
            try
            {
                regkey.SetValue("Index", keyvalue);
                regstart.SetValue("RestApp", System.Reflection.Assembly.GetExecutingAssembly().Location);
                regkey.Close();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        #endregion


    }
}
