using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace InteractiveStend
{
    public partial class FormMain : Form
    {

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        static int GetLastInputTime()
        {
            int idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (UInt32)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            int envTicks = Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                int lastInputTick = (Int32)lastInputInfo.dwTime;

                idleTime = envTicks - lastInputTick;
            }

            return ((idleTime > 0) ? (idleTime / 1000) : 0);
        }

        public FormMain()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.Ctlcontrols.pause();
            axWindowsMediaPlayer1.Visible = false;

        }

      

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GetLastInputTime() == 30)
            {
                StartVideo();
            }
            else if (GetLastInputTime()==0)
            {
                StopVideo();
            }
        }

        private void StartVideo()
        {
            axWindowsMediaPlayer1.Visible = true;
            axWindowsMediaPlayer1.Ctlcontrols.play();
            axWindowsMediaPlayer1.fullScreen = true;
        }

        private void StopVideo()
        {
            axWindowsMediaPlayer1.fullScreen = false;
            axWindowsMediaPlayer1.Visible = false;
            axWindowsMediaPlayer1.Ctlcontrols.pause();
        }

        private void axWindowsMediaPlayer1_KeyDownEvent(object sender, AxWMPLib._WMPOCXEvents_KeyDownEvent e)
        {
            StopVideo();
        }

        private void axWindowsMediaPlayer1_MouseDownEvent(object sender, AxWMPLib._WMPOCXEvents_MouseDownEvent e)
        {
            StopVideo();
        }

        private void axWindowsMediaPlayer1_MouseMoveEvent(object sender, AxWMPLib._WMPOCXEvents_MouseMoveEvent e)
        {
            StopVideo();
        }
    }
}
