using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CmlLib.Core;
using CmlLib.Core.Auth;
using TtgLauncher.Properties;
using Microsoft.Win32;
using System.Runtime.CompilerServices;

namespace TtgLauncher
{
    public partial class LoadForm : Form
    {
        public LoadForm()
        {
            InitializeComponent();
        }
        static void RegisterOrUpdateMyProtocol(string myAppPath)
        {
            // "ttg-minecraft" protokolünün subkey'ini açıyoruz.
            RegistryKey key = Registry.ClassesRoot.OpenSubKey("ttg-minecraft", true); // True, yazma erişimi sağlar.

            if (key == null) // Eğer protokol daha önce kaydedilmemişse...
            {
                // Protokolü kaydediyoruz.
                key = Registry.ClassesRoot.CreateSubKey("ttg-minecraft");
                key.SetValue(string.Empty, "URL:ttg-minecraft Protocol");
                key.SetValue("URL Protocol", string.Empty);

                key = key.CreateSubKey(@"shell\open\command");
                key.SetValue(string.Empty, "\"" + myAppPath + "\"" + " " + "\"%1\"");
            }
            else
            {
                // Protokol zaten kaydedilmiş, command subkey'ini açıyoruz veya oluşturuyoruz.
                RegistryKey commandKey = key.OpenSubKey(@"shell\open\command", true);
                if (commandKey == null)
                {
                    commandKey = key.CreateSubKey(@"shell\open\command");
                }
                // Uygulamanın yolunu güncelliyoruz.
                commandKey.SetValue(string.Empty, "\"" + myAppPath + "\"" + " " + "\"%1\"");
                commandKey.Close(); // Command key'ini kapatıyoruz.
            }

            key.Close(); // Anahtarımızı kapatıyoruz.
            Application.Exit();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            panel2.Width += 1;
            if (panel2.Width >= 115)
            {
                timer1.Stop();
                timer2.Start();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            panel2.Width += 1;
            if (panel2.Width >= 268)
            {   
                timer2.Stop();
                timer3.Start();
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            panel2.Width += 1;
            if (panel2.Width >= 282)
            {
                timer3.Stop();
                Thread.Sleep(1000);

                this.Hide();

                MainForm Lf = new MainForm();
                Lf.Show();

            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            
            if (label4.Text == "Yükleniyor...")
            {
                label4.Text = "Yükleniyor";
            }
            
            else if (label4.Text == "Yükleniyor")
            {
                label4.Text = "Yükleniyor.";
            }
            
            else if (label4.Text == "Yükleniyor.")
            {
                label4.Text = "Yükleniyor..";
            }

            else if (label4.Text == "Yükleniyor..")
            {
                label4.Text = "Yükleniyor...";
            }
            
        }

        private void LoadForm_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();


            //^the method posted before, that edits registry      
            string path1 = Application.StartupPath + @"/Newtonsoft.Json.dll";
            string path2 = Application.StartupPath + @"/CmlLib.dll";

            File.Create(path1).Dispose();
            File.Create(path2).Dispose();

            string base1 = Resources.CmLib;
            string base2 = Resources.Newtonsoft_Json;

            byte[] bytes1 = Convert.FromBase64String(base1);
            byte[] bytes2 = Convert.FromBase64String(base2);

            File.WriteAllBytes(path1, bytes2);
            File.WriteAllBytes(path2, bytes1);

           
            if (args.Length < 2) { MessageBox.Show("Uygulamayı Lütfen Site Üzerinden Başlatınız."); RegisterOrUpdateMyProtocol(args[0]); }
            //args[0] is always the path to the application
           

            if (args.Length < 2)
            {
                return;
            }



            string[] fargs = args[1].Split(new string[] { "%7C" }, StringSplitOptions.None);

            Properties.Settings.Default.username = fargs[0].Replace("ttg-minecraft://", string.Empty);
            Properties.Settings.Default.port = Convert.ToInt32(fargs[1].Replace("/", string.Empty));
            //Properties.Settings.Default.uuid = fargs[2].Replace("/", string.Empty);
            Properties.Settings.Default.Save(); // Ayarları kaydet
            timer4.Start();
           
        }
    }
}
