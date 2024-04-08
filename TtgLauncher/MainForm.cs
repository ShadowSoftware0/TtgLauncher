using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using CmlLib.Core;
using CmlLib.Core.Auth;
using System.Threading;
using static System.Collections.Specialized.BitVector32;

namespace TtgLauncher
{
    public partial class MainForm : Form
    {
        int ports = Properties.Settings.Default.port;
        string username = Properties.Settings.Default.username;
        public MainForm()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            label2.Text = username;
        }

        MSession mSession;

        private void path()
        {
            var path = new MinecraftPath();

            var launcher = new CMLauncher(path);

            launcher.FileChanged += (e) =>
            {
                listBox1.Items.Add(string.Format("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount));
               
                
            
            };

            launcher.ProgressChanged += (s, e) =>
            {
                //Console.WriteLine("{0}%", e.ProgressPercentage);
            };

            foreach (var item in launcher.GetAllVersions())
            {
                comboBox1.Items.Add((item.Name));
                comboBox1.Visible = false;
            }
            Thread thread = new Thread(() => launch());
            thread.IsBackground = true;
            thread.Start();
        }

        public static string version;
        


        private void launch()
        {

            listBox1.Show();

            var path = new MinecraftPath();

            var launcher = new CMLauncher(path);

            launcher.FileChanged += (e) =>
            {
                listBox1.Items.Add(string.Format("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount));
            };


            var launchOption = new MLaunchOption
            {
                MaximumRamMb = 4096,
                ServerIp = "213.238.177.195",
                ServerPort = ports,
                Session = MSession.CreateOfflineSession(username),
                VersionType = "TiktakGames",
                GameLauncherName = "TiktakGames",
                GameLauncherVersion = "2"
                
                
            };
            try
            {
                var process = launcher.CreateProcess("OptiFine 1.20.4", launchOption);
           

                process.Start();
                
            }
            catch (Exception ex)
            {
               
                downloadoptifine();


            }
            this.Close();
            Application.Exit();
        }
        private void downloadoptifine()
        {
            MessageBox.Show("Mod Bulunamadı Lütfen Bekleyiniz İndiriliyor");
            string downloadUrl = "https://github.com/ShadowSoftware0/1.20.4-OptiFine/archive/refs/heads/main.zip";
            string minecraftVersionsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft\\versions");

            // Dosya adını URL'den çıkar ve zip dosyasının kaydedileceği yolu belirle
            string fileName = Path.GetFileName(new Uri(downloadUrl).LocalPath);
            string zipFilePath = Path.Combine(minecraftVersionsPath, fileName);

            // Versions klasörünün varlığını kontrol et ve yoksa oluştur
            if (!Directory.Exists(minecraftVersionsPath))
            {
                Directory.CreateDirectory(minecraftVersionsPath);
            }

            using (var client = new WebClient())
            {
                try
                {
                    // Zip dosyasını indir
                    client.DownloadFile(downloadUrl, zipFilePath);

                    // Zip dosyasını çıkar
                    string extractPath = Path.Combine(minecraftVersionsPath, Path.GetFileNameWithoutExtension(fileName));
                    if (!Directory.Exists(extractPath))
                    {
                        Directory.CreateDirectory(extractPath);
                    }
                    ZipFile.ExtractToDirectory(zipFilePath, extractPath);

                    
                    string optifineFolderPath = Path.Combine(extractPath+ "/1.20.4-OptiFine-main", "OptiFine 1.20.4");
                    string targetPath = Path.Combine(minecraftVersionsPath, "OptiFine 1.20.4");

                    // Hedefte aynı isimde klasör varsa silinir (Dikkatli olun, bu veri kaybına neden olabilir)
                    if (Directory.Exists(targetPath))
                    {
                        Directory.Delete(targetPath, true);
                    }

                    Directory.Move(optifineFolderPath, targetPath);
                    
                    File.Delete(minecraftVersionsPath+"/main.zip");
                    Directory.Delete(minecraftVersionsPath+"/main",true);
                    MessageBox.Show("Sürüm İndirildi Kurulum Başlatılıyor");
                    // İndirme ve çıkarma işleminden sonra gerekiyorsa başka işlemleri tetikle
                    launch();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message);
                }
            }

        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Visible = false;
            listBox1.Hide();

            label1.Text = "Hoşgeldiniz.";

            var Request = WebRequest.Create("https://minotar.net/cube/"+username+"/100.png");

            var response = Request.GetResponse();
            var stream = response.GetResponseStream();

            pictureBox1.Image = Bitmap.FromStream(stream);

            path();

        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        Point lastPoint;
        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Visible = false;

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
    }
}
