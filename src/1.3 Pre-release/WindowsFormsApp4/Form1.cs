using amalgam_fireinjector;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
            RefreshListDLL();


        }
        public void RefreshListDLL()
        {
            string tempPath = Path.GetTempPath();
            string amalgamPath = Path.Combine(tempPath, "amalgamex");
            string[] targetDlls = {
                "Amalgamx64ReleaseAVX2.dll",
                "Amalgamx64Release.dll",
                "Amalgamx64ReleaseFreetype.dll",
                "Amalgamx64ReleaseFreetypeAVX2.dll",
                "Amalgamx64ReleaseAVX2_V2.dll",
                "Amalgamx64Release_V2.dll",
                "Amalgamx64ReleaseFreetype_V2.dll",
                "Amalgamx64ReleaseFreetypeAVX2_V2.dll"
            };
            checkedListBox1.Items.Clear();

            List<string> foundDlls = new List<string>();
            foreach (string dllName in targetDlls)
            {
                string fullPath = Path.Combine(amalgamPath, dllName);
                if (File.Exists(fullPath))
                {
                    checkedListBox1.Items.Add(dllName);
                    Console.WriteLine($"Found DLL: {dllName}");
                }
            }
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = $"Welcome, {Environment.UserName}";
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start("https://t.me/XeonLeon");
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            if (checkBox5.Checked)
            {
                if (checkBox1.Checked)
                {
                    if (checkBox2.Checked)
                    {

                        Console.WriteLine("Freetype and AVX2 V2");
                        button2.Text = "Downloading...";
                        Download download = new Download();
                        await download.DownloadF("https://nightly.link/TheGameEnhancer2004/Amalgam-v2/workflows/msbuild/master/Amalgamx64ReleaseFreetypeAVX2.zip", true);
                        button1.Enabled = true;
                        button2.Enabled = true;
                        button2.Text = "Download";
                        RefreshListDLL();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Freetype V2");
                        button2.Text = "Downloading...";
                        Download download = new Download();
                        await download.DownloadF("https://nightly.link/TheGameEnhancer2004/Amalgam-v2/workflows/msbuild/master/Amalgamx64ReleaseFreetype.zip", true);
                        button1.Enabled = true;
                        button2.Enabled = true;
                        button2.Text = "Download";
                        RefreshListDLL();
                        return;
                    }




                }
                if (checkBox2.Checked)
                {
                    Console.WriteLine("AVX2 V2");
                    button2.Text = "Downloading...";
                    Download download = new Download();
                    await download.DownloadF("https://nightly.link/TheGameEnhancer2004/Amalgam-v2/workflows/msbuild/master/Amalgamx64ReleaseAVX2.zip", true);
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button2.Text = "Download";
                    RefreshListDLL();
                    return;

                }
                else
                {
                    Console.WriteLine("Standart V2");
                    button2.Text = "Downloading...";
                    Download download = new Download();
                    await download.DownloadF("https://nightly.link/TheGameEnhancer2004/Amalgam-v2/workflows/msbuild/master/Amalgamx64Release.zip", true);
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button2.Text = "Download";
                    RefreshListDLL();
                    return;

                }


            }
            else { 
                if (checkBox1.Checked) 
                {
                    if (checkBox2.Checked)
                    {

                        Console.WriteLine("Freetype and AVX2");
                        button2.Text = "Downloading...";
                        Download download = new Download();
                        await download.DownloadF("https://nightly.link/rei-2/Amalgam/workflows/msbuild/master/Amalgamx64ReleaseFreetypeAVX2.zip");
                        button1.Enabled = true;
                        button2.Enabled = true;
                        button2.Text = "Download";
                        RefreshListDLL();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Freetype");
                        button2.Text = "Downloading...";
                        Download download = new Download();
                        await download.DownloadF("https://nightly.link/rei-2/Amalgam/workflows/msbuild/master/Amalgamx64ReleaseFreetype.zip");
                        button1.Enabled = true;
                        button2.Enabled = true;
                        button2.Text = "Download";
                        RefreshListDLL();
                        return;
                    }




                }
                if (checkBox2.Checked)
                {
                    Console.WriteLine("AVX2");
                    button2.Text = "Downloading...";
                    Download download = new Download();
                    await download.DownloadF("https://nightly.link/rei-2/Amalgam/workflows/msbuild/master/Amalgamx64ReleaseAVX2.zip");
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button2.Text = "Download";
                    RefreshListDLL();
                    return;

                }
                else
                {
                    Console.WriteLine("Standart");
                    button2.Text = "Downloading...";
                    Download download = new Download();
                    await download.DownloadF("https://nightly.link/rei-2/Amalgam/workflows/msbuild/master/Amalgamx64Release.zip");
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button2.Text = "Download";
                    RefreshListDLL();
                    return;

                }
            }
            
        }

        private void button1_Click(object sender, EventArgs e)

        {
            string tempPath = Path.GetTempPath();
            string amalgamPath = Path.Combine(tempPath, "amalgamex");
            button1.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = false;
            this.TopMost = false;
            if (checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please select a DLL to inject.");
                button1.Text = "Inject";
                button1.Enabled = true;
                button2.Enabled = true;
                button4.Enabled = true;
                this.TopMost = true;
                return;
            }
            if (checkBox6.Checked)
            {
                button1.Text = "Vac bypass...";
                try
                {
                    if (!Directory.Exists(amalgamPath))
                    {
                        Directory.CreateDirectory(amalgamPath);
                    }
                    string loaderPath = Path.Combine(amalgamPath, "VAC-Bypass-Loader.exe");

                    var assembly = Assembly.GetExecutingAssembly();
                    var resourceName = "amalgam_fireinjector.VAC-Bypass-Loader.exe"; // Change to your actual resource path

                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            using (FileStream fileStream = new FileStream(loaderPath, FileMode.Create, FileAccess.Write))
                            {
                                stream.CopyTo(fileStream);
                            }

                            Console.WriteLine($"Loader extracted to: {loaderPath}");
                            Process.Start(loaderPath);

                        }
                        else
                        {
                            Console.WriteLine("Embedded resource not found!");
                            MessageBox.Show("Embedded resource not found! Report to developer!");
                            this.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to extract and run loader: {ex.Message}");
                    button1.Text = "Injection failed!";
                }
            }

            ProgramMonitor.WaitForProgram("steam", 1000, 30);
            button1.Enabled = false;
            button1.Text = "Waiting for game...";
            button2.Enabled = false;
            button4.Enabled = false;
            if (checkBox3.Checked)
            { 
                while (!SteamMonitor.SteamLoginChecker.steam_logged_in)
                {
                    SteamMonitor.SteamLoginChecker.CheckSteamLogin();
                }
                Process.Start("steam://rungameid/440");
                ProgramMonitor.WaitForProgram("tf_win64", 1000, 60);

            }
            string selectedDllPath = checkedListBox1.CheckedItems.Count > 0
                ? checkedListBox1.CheckedItems[0].ToString()
                : null;

            if (string.IsNullOrEmpty(selectedDllPath))
            {
                MessageBox.Show("No DLL selected");
                button1.Text = "Injection Failed!";
                button1.Enabled = true;
                button2.Enabled = true;
                button4.Enabled = true;
                return;
            }
            selectedDllPath = Path.Combine(amalgamPath, selectedDllPath);
            Injector.selectedDllPath = selectedDllPath;
            bool success = Injector.Inject();
            if (success)
            {
                button1.Text = "Injection Successful!";
                if (checkBox4.Checked)
                {
                    this.Close();
                }
                button1.Enabled = true;
                button2.Enabled = true;
                button4.Enabled = true;

            }
            else
            {
                button1.Text = "Injection Failed!";
                button1.Enabled = true;
                button2.Enabled = true;
                button4.Enabled = true;
            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string tempPath = Path.GetTempPath();
                string amalgamPath = Path.Combine(tempPath, "amalgamex");

                if (Directory.Exists(amalgamPath))
                {
                    Directory.Delete(amalgamPath, true);
                    Console.WriteLine("Temp folder deleted successfully!");
                    MessageBox.Show("Temp folder deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Temp folder not found!");
                    MessageBox.Show("Temp folder not found!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete temp folder: {ex.Message}");
            }
            RefreshListDLL();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //    button1.Enabled = false;
            //    button2.Enabled = false;
            //    button3.Enabled = false;
            //    button4.Enabled = false;
            //    this.TopMost = false;
            //    try
            //    {
            //        // Get temp path and create amalgamex folder
            //        string tempPath = Path.GetTempPath();
            //        string amalgamPath = Path.Combine(tempPath, "amalgamex");

            //        // Create directory if it doesn't exist
            //        if (!Directory.Exists(amalgamPath))
            //        {
            //            Directory.CreateDirectory(amalgamPath);
            //        }

            //        // Path for the loader exe
            //        string loaderPath = Path.Combine(amalgamPath, "VAC-Bypass-Loader.exe");

            //        // Get the embedded resource
            //        var assembly = Assembly.GetExecutingAssembly();
            //        var resourceName = "amalgam_fireinjector.VAC-Bypass-Loader.exe"; // Change to your actual resource path

            //        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            //        {
            //            if (stream != null)
            //            {
            //                using (FileStream fileStream = new FileStream(loaderPath, FileMode.Create, FileAccess.Write))
            //                {
            //                    stream.CopyTo(fileStream);
            //                }

            //                Console.WriteLine($"Loader extracted to: {loaderPath}");
            //                Process.Start(loaderPath);
            //                ProgramMonitor.WaitForProgram("steam.exe", 100, 30);
            //                Console.WriteLine("Loader started successfully!");
            //                button1.Enabled = true;
            //                button2.Enabled = true;
            //                button3.Enabled = true;
            //                button4.Enabled = true;
            //                this.TopMost = true;
            //            }
            //            else
            //            {
            //                Console.WriteLine("Embedded resource not found!");
            //                this.TopMost = false;
            //                MessageBox.Show("Embedded resource not found! Auto close!");
            //                this.Close();
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Failed to extract and run loader: {ex.Message}");
            //        this.TopMost = false;
            //    }
        }



        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
