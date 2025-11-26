using AmalgamLauncherUnoffical;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Markup;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;
        private const int WS_BORDER = 0x00800000;

        // flag to avoid re-entrant closing
        private bool _isClosing = false;

        public Form1()
        {
            InitializeComponent();

            // start invisible for fade-in
            this.Opacity = 0d;

            this.FormBorderStyle = FormBorderStyle.None;

            RemoveOutline();

            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;

            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            this.Resize += Form1_Resize;

            // handle form closing to perform fade-out
            this.FormClosing += Form1_FormClosing;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RemoveOutline();
        }

        private void RemoveOutline()
        {
            int style = GetWindowLong(this.Handle, GWL_STYLE);
            style &= ~WS_BORDER;
            SetWindowLong(this.Handle, GWL_STYLE, style);
        }

        private Point lastLocation;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastLocation = e.Location;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X,
                    (this.Location.Y - lastLocation.Y) + e.Y);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private download _downloader = new download();
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                button4.Enabled = false;
                button1.Enabled = false;
                progressBar1.Value = 0;
                progressBar1.Visible = true;

                download.DownloadVersion version;

                if (forceAVX2.Checked && forceFreetype.Checked)
                    version = download.DownloadVersion.AVX2AndFreetype;
                else if (forceAVX2.Checked)
                    version = download.DownloadVersion.AVX2Only;
                else if (forceFreetype.Checked)
                    version = download.DownloadVersion.FreetypeOnly;
                else
                    version = download.DownloadVersion.Normal;

                _downloader.ProgressChanged += Downloader_ProgressChanged;
                _downloader.DownloadCompleted += Downloader_DownloadCompleted;

                await _downloader.DownloadAndExtractAsync(version);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button1.Enabled = true;
                button4.Enabled = true;
                _downloader.ProgressChanged -= Downloader_ProgressChanged;
                _downloader.DownloadCompleted -= Downloader_DownloadCompleted;
                progressBar1.Visible = false; 
            }
        }

        private void Downloader_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action(() => progressBar1.Value = e.ProgressPercentage));
            }
            else
            {
                progressBar1.Value = e.ProgressPercentage;
            }
        }

        private void Downloader_DownloadCompleted(object sender, EventArgs e)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action(() => progressBar1.Visible = false));
            }
            else
            {
                progressBar1.Visible = false;
            }
        }

        private void forceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            forceAVX2.Checked = !forceAVX2.Checked;
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void freetypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            forceFreetype.Checked = !forceFreetype.Checked;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            button2.Enabled = false;
            button2.Text = "Checking DLL";

            injector injector = new injector();
            string dllPath = Path.Combine(Path.GetTempPath(), "AmalgamExtracted", "DirectXFix.dll");
            //spoof name nr 2 hehe

            if (!File.Exists(dllPath))
            {
                MessageBox.Show("DLL file not found. Please download first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button2.Enabled = true;
                button4.Enabled = true;
                button2.Text = "Inject";
                return;
            }
            //double check cuz why not
            if (!File.Exists(dllPath))
            {
                MessageBox.Show("DLL file not found. Please download first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button2.Enabled = true;
                button4.Enabled = true;
                button2.Text = "Inject";
                return;
            }
            await Task.Delay(100);
            button2.Text = "Waiting for TF2";
            button4.Enabled = true;

            bool success = await injector.InjectDLLAsync("tf_win64", dllPath);
            if (success)
            {
                button4.Enabled = false;
                button2.Text = "Success! Auto close in 5s";
                await Task.Delay(5000);
                this.Close();
            }
        }

        public void progressBar1_Click(object sender, EventArgs e)
        {

        }

        // Fade-in on load and keep the existing "holy animation" logic.
        private async void Form1_Load(object sender, EventArgs e)
        {
            // set initial visibility for controls (as before)
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            progressBar1.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            pictureBox1.Visible = false;
            pictureBox2.Visible = true;

            await FadeInAsync(100);
            // small delay as original
            await Task.Delay(2000);

            // switch splash images / controls as before
            pictureBox2.Visible = false;
            pictureBox1.Visible = true;
            button2.Visible = true;
            label1.Visible = true;
            label2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            button1.Visible = true;

            // perform smooth fade-in
            await FadeInAsync(50);
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // thread-safe button toggle delegate
                Action<bool> setButtons = enabled =>
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            button1.Enabled = enabled;
                            button2.Enabled = enabled;
                            button3.Enabled = enabled;
                            button4.Enabled = enabled;
                        }));
                    }
                    else
                    {
                        button1.Enabled = enabled;
                        button2.Enabled = enabled;
                        button3.Enabled = enabled;
                        button4.Enabled = enabled;
                    }
                };

                // Try to find an embedded resource that looks like the bypass exe
                string resourceName = null;
                string[] names = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
                foreach (var n in names)
                {
                    if (n.IndexOf("vac", StringComparison.OrdinalIgnoreCase) >= 0 &&
                        n.IndexOf("bypass", StringComparison.OrdinalIgnoreCase) >= 0 &&
                        n.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        resourceName = n;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(resourceName))
                {
                    await vac_bypass.RunFromEmbeddedResourceAsync(resourceName, setButtons);
                    return;
                }

                // Fallback: check the downloader's extract path for the exe
                string extractPath = _downloader?.GetExtractPath();
                if (!string.IsNullOrWhiteSpace(extractPath))
                {
                    string exePath = Path.Combine(extractPath, "vac-bypass-loader.exe");
                    if (File.Exists(exePath))
                    {
                        await vac_bypass.MainAsync(extractPath, setButtons);
                        return;
                    }
                }

                MessageBox.Show("VAC bypass executable not found as an embedded resource or in the extracted folder.", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting VAC bypass: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        // FormClosing handler to run fade-out before actual close
        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isClosing)
            {
                // cancel the close, run fade-out, then close for real
                e.Cancel = true;
                _isClosing = true;
                await FadeOutAsync(300);
                // now close without running fade again
                this.Close();
            }
            // else allow close to proceed
        }

        // Helper: smooth fade-in
        private async Task FadeInAsync(int durationMs = 300, int steps = 20)
        {
            if (durationMs <= 0) { this.Opacity = 1d; return; }
            double stepTime = durationMs / (double)steps;
            for (int i = 1; i <= steps; i++)
            {
                this.Opacity = i / (double)steps;
                await Task.Delay((int)stepTime);
            }
            this.Opacity = 1d;
        }

        // Helper: smooth fade-out
        private async Task FadeOutAsync(int durationMs = 300, int steps = 20)
        {
            if (durationMs <= 0) { this.Opacity = 0d; return; }
            double stepTime = durationMs / (double)steps;
            for (int i = steps - 1; i >= 0; i--)
            {
                this.Opacity = i / (double)steps;
                await Task.Delay((int)stepTime);
            }
            this.Opacity = 0d;
        }
    }
}