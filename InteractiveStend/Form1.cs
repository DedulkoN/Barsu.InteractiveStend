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
using System.IO;
using System.Configuration;



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

        private WMPLib.IWMPPlaylist playlist;
        public FormMain()
        {
            ClassRegister.GetBrowserVersion();
            InitializeComponent();
            

                string folder = global::InteractiveStend.Properties.Settings.Default.FolderVideos;            


               playlist = axWindowsMediaPlayer1.playlistCollection.newPlaylist("TempPlayList");
                playlist.clear();

                string[] filesin = Directory.GetFiles(folder);
                foreach (string file in filesin)
                {
                    WMPLib.IWMPMedia media = axWindowsMediaPlayer1.newMedia(file);
                    playlist.appendItem(media);
                }
                axWindowsMediaPlayer1.currentPlaylist = playlist;
           
           

           
        }

      

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GetLastInputTime() == Properties.Settings.Default.TimeForVideoStart)
            {
                StartVideo();
            }
            else if (GetLastInputTime()<10)
            {
                StopVideo();
            }
        }

        private void StartVideo()
        {
            if (playlist.count > 0)
            {
                axWindowsMediaPlayer1.Visible = true;
                axWindowsMediaPlayer1.Ctlcontrols.play();
                axWindowsMediaPlayer1.fullScreen = true;
            }
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

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopVideo();
            
            axWindowsMediaPlayer1.playlistCollection.remove(playlist);
            MessageBox.Show("!!");
        }
    }
}
