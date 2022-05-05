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
using CefSharp;
using CefSharp.WinForms;
using System.Diagnostics;



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

       // private WMPLib.IWMPPlaylist playlist;
        public FormMain()
        {
           // CefSharp.CefSharpSettings.FocusedNodeChangedEnabled = true;
            CefSettings settings = new CefSettings();
            settings.CefCommandLineArgs["touch-events"] = "enabled";
            settings.CefCommandLineArgs.Add("disable-usb-keyboard-detect", "1");
            Cef.Initialize(settings);
            // ClassRegister.GetBrowserVersion();
            InitializeComponent();
            chromiumWebBrowser1.FrameLoadEnd += ChromiumWebBrowser1_FrameLoadEnd;
            /*
            if (Properties.Settings.Default.TimeForVideoStart > 0)
            {
                
                try
                {
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
                }catch(Exception ex) {MessageBox.Show(ex.Message); };
            }
            else { }

           */
           
            chromiumWebBrowser1.MenuHandler = new CustomContextHandler();
            chromiumWebBrowser1.LoadUrl(Properties.Settings.Default.URLforStart.ToString());
            chromiumWebBrowser1.Focus();
            //chromiumWebBrowser1.RenderProcessMessageHandler = new CMSRenderBrowser();
            
            /* try
             {
                 chromiumWebBrowser1.SetZoomLevel(2);
             }catch (Exception ex) { MessageBox.Show(ex.Message); }*/

        }

        private void ChromiumWebBrowser1_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            ChromiumWebBrowser browser = (ChromiumWebBrowser)sender;           
            browser.SetZoomLevel(Properties.Settings.Default.Zoom);
          //  MessageBox.Show("!! " + chromiumWebBrowser1.GetZoomLevelAsync().Result.ToString());
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {           
            if (Properties.Settings.Default.TimeForVideoStart > 0)
            {
                if (GetLastInputTime() == Properties.Settings.Default.TimeForVideoStart)
                {
                    StartVideo();
                }
                else if (GetLastInputTime() < 10)
                {
                    StopVideo();
                }
            }
            if (GetLastInputTime() == Properties.Settings.Default.TimeToMainPage)
            {               

                 if(chromiumWebBrowser1.Address != Properties.Settings.Default.URLforStart.ToString())
                    chromiumWebBrowser1.Load( Properties.Settings.Default.URLforStart.ToString());
            }

        }

        private void StartVideo()
        {
           /* if (playlist.count > 0)
            {
                axWindowsMediaPlayer1.Visible = true;
                axWindowsMediaPlayer1.Ctlcontrols.play();
                axWindowsMediaPlayer1.fullScreen = true;
            }*/
        }

        private void StopVideo()
        {
           /* axWindowsMediaPlayer1.fullScreen = false;
            axWindowsMediaPlayer1.Visible = false;
            axWindowsMediaPlayer1.Ctlcontrols.pause();*/
        }

      /*  private void axWindowsMediaPlayer1_KeyDownEvent(object sender, AxWMPLib._WMPOCXEvents_KeyDownEvent e)
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
        }*/

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopVideo();
            
           // axWindowsMediaPlayer1.playlistCollection.remove(playlist);
          
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
           

        }

    }

    /// <summary>
    /// меню по правому клику
    /// </summary>
    public class CustomContextHandler : IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser browserControl, CefSharp.IBrowser browser, IFrame frame, IContextMenuParams parameters,
            IMenuModel model)
        {
            model.Clear();
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, CefSharp.IBrowser browser, IFrame frame, IContextMenuParams parameters,
            CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, CefSharp.IBrowser browser, IFrame frame)
        {
        }

        public bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return true;
        }
    }

    /// <summary>
    /// перехват событий браузера
    /// </summary>
    class CMSRenderBrowser : IRenderProcessMessageHandler
    {
       
        public void OnFocusedNodeChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IDomNode node)
        {
            try
            {
                if (node != null)
                {
                    var a = node.ToString().Split(' ');
                    if (a[0] == "INPUT")
                    {
                        string pathToKay = Environment.GetFolderPath(Environment.SpecialFolder.System);
                        Process.Start(pathToKay + "\\osk.exe");
                    }
                }
                else 
                {
                    Process[] listprosecc = Process.GetProcesses();
                    foreach (Process oneproc in listprosecc)
                    {
                        Process[] List;
                        List = Process.GetProcessesByName("osk");                        
                        foreach (Process proc in List)
                        {
                            proc.Kill();
                        }
                        for (List = Process.GetProcessesByName("osk"); List.Length > 0; System.Threading.Thread.Sleep(50))
                            List = Process.GetProcessesByName("osk");
                       
                    }
                }
            }
            catch { }
           
        }

        public void OnContextCreated(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
        {
        }

        public void OnContextReleased(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
        {
        }


        public void OnUncaughtException(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, JavascriptException exception)
        {
        }
    }

}
