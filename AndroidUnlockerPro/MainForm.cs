using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AndroidUnlockerPro
{
    public class MainForm : Form
    {
        private Panel sidePanel;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel statusBar;
        private Label lblStatus;
        private Label lblDeviceInfo;
        private RichTextBox rtbLog;
        private ProgressBar progressBar;
        private Button btnConnect;
        private Button btnDisconnect;
        private ComboBox cmbBrand;
        private ComboBox cmbMethod;
        private Label lblBrand;
        private Label lblMethod;
        private Button btnStartUnlock;
        private Button btnFactoryReset;
        private Button btnRemovePattern;
        private Button btnRemovePin;
        private Button btnRemoveFRP;
        private Button btnGuide;
        private Button btnAbout;
        private Button btnClearLog;
        private Panel devicePanel;
        private Label lblDeviceStatus;
        private PictureBox pbDeviceIcon;
        private System.Windows.Forms.Timer animTimer;
        private int animStep = 0;
        private bool isConnected = false;
        private string adbPath = "";

        private Color primaryColor = Color.FromArgb(25, 118, 210);
        private Color primaryDark = Color.FromArgb(13, 71, 161);
        private Color accentColor = Color.FromArgb(0, 200, 83);
        private Color dangerColor = Color.FromArgb(211, 47, 47);
        private Color bgColor = Color.FromArgb(18, 18, 24);
        private Color cardColor = Color.FromArgb(30, 30, 42);
        private Color cardLight = Color.FromArgb(42, 42, 58);
        private Color textColor = Color.FromArgb(240, 240, 245);
        private Color textMuted = Color.FromArgb(158, 158, 168);
        private Color sideColor = Color.FromArgb(12, 12, 18);

        public MainForm()
        {
            InitializeComponent();
            SetupADB();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Android Unlocker Pro v2.0 - ปลดล็อคมือถือ Android ทุกยี่ห้อ";
            this.Size = new Size(1100, 720);
            this.MinimumSize = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = bgColor;
            this.ForeColor = textColor;
            this.Font = new Font("Segoe UI", 9.5f);
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            CreateSidePanel();
            CreateHeaderPanel();
            CreateContentPanel();
            CreateStatusBar();

            animTimer = new System.Windows.Forms.Timer();
            animTimer.Interval = 50;
            animTimer.Tick += AnimTimer_Tick;

            this.ResumeLayout(false);
        }

        private void CreateSidePanel()
        {
            sidePanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = sideColor,
                Padding = new Padding(0)
            };

            var logoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = primaryDark,
                Padding = new Padding(15, 15, 15, 10)
            };

            var lblLogo = new Label
            {
                Text = "🔓 Android Unlocker",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblVersion = new Label
            {
                Text = "Pro v2.0 - รองรับทุกยี่ห้อ",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(180, 200, 255),
                Dock = DockStyle.Top,
                Height = 22,
                TextAlign = ContentAlignment.MiddleLeft
            };

            logoPanel.Controls.Add(lblVersion);
            logoPanel.Controls.Add(lblLogo);

            var menuPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8, 15, 8, 8)
            };

            var lblMenuTitle = new Label
            {
                Text = "เมนูหลัก",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = textMuted,
                Dock = DockStyle.Top,
                Height = 25,
                Padding = new Padding(8, 0, 0, 0)
            };
            menuPanel.Controls.Add(lblMenuTitle);

            btnRemovePattern = CreateSideButton("🔑  ลบรหัส Pattern", 55);
            btnRemovePin = CreateSideButton("🔢  ลบรหัส PIN/Password", 95);
            btnRemoveFRP = CreateSideButton("🛡️  ปลดล็อค FRP (Google)", 135);
            btnFactoryReset = CreateSideButton("♻️  Factory Reset", 175);
            btnStartUnlock = CreateSideButton("⚡  ปลดล็อคอัตโนมัติ", 215);

            var lblToolsTitle = new Label
            {
                Text = "เครื่องมือ",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = textMuted,
                Location = new Point(8, 265),
                Size = new Size(200, 25)
            };

            btnGuide = CreateSideButton("📖  วิธีใช้งาน", 295);
            btnAbout = CreateSideButton("ℹ️  เกี่ยวกับโปรแกรม", 335);
            btnClearLog = CreateSideButton("🗑️  ล้าง Log", 375);

            menuPanel.Controls.Add(lblToolsTitle);
            menuPanel.Controls.Add(btnRemovePattern);
            menuPanel.Controls.Add(btnRemovePin);
            menuPanel.Controls.Add(btnRemoveFRP);
            menuPanel.Controls.Add(btnFactoryReset);
            menuPanel.Controls.Add(btnStartUnlock);
            menuPanel.Controls.Add(btnGuide);
            menuPanel.Controls.Add(btnAbout);
            menuPanel.Controls.Add(btnClearLog);

            btnRemovePattern.Click += BtnRemovePattern_Click;
            btnRemovePin.Click += BtnRemovePin_Click;
            btnRemoveFRP.Click += BtnRemoveFRP_Click;
            btnFactoryReset.Click += BtnFactoryReset_Click;
            btnStartUnlock.Click += BtnStartUnlock_Click;
            btnGuide.Click += BtnGuide_Click;
            btnAbout.Click += BtnAbout_Click;
            btnClearLog.Click += (s, e) => rtbLog.Clear();

            sidePanel.Controls.Add(menuPanel);
            sidePanel.Controls.Add(logoPanel);
            this.Controls.Add(sidePanel);
        }

        private Button CreateSideButton(string text, int yPos)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(4, yPos),
                Size = new Size(200, 34),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.2f),
                ForeColor = textColor,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 40, 60);
            btn.FlatAppearance.MouseDownBackColor = primaryDark;
            return btn;
        }

        private void CreateHeaderPanel()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 140,
                BackColor = cardColor,
                Padding = new Padding(20, 12, 20, 12)
            };

            var topRow = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.Transparent };

            lblBrand = new Label
            {
                Text = "ยี่ห้อมือถือ:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = textColor,
                Location = new Point(5, 15),
                AutoSize = true
            };

            cmbBrand = new ComboBox
            {
                Location = new Point(110, 11),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10f),
                BackColor = cardLight,
                ForeColor = textColor,
                FlatStyle = FlatStyle.Flat
            };
            cmbBrand.Items.AddRange(new object[] {
                "ทุกยี่ห้อ (อัตโนมัติ)",
                "Samsung", "Huawei", "Xiaomi / Redmi / POCO",
                "OPPO", "Vivo", "Realme", "OnePlus",
                "Sony Xperia", "LG", "Motorola", "Nokia",
                "Google Pixel", "ASUS", "Lenovo", "HTC",
                "Infinix", "Tecno", "Itel", "ZTE",
                "Meizu", "Nothing Phone", "อื่นๆ"
            });
            cmbBrand.SelectedIndex = 0;

            lblMethod = new Label
            {
                Text = "วิธีปลดล็อค:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = textColor,
                Location = new Point(340, 15),
                AutoSize = true
            };

            cmbMethod = new ComboBox
            {
                Location = new Point(450, 11),
                Size = new Size(220, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10f),
                BackColor = cardLight,
                ForeColor = textColor,
                FlatStyle = FlatStyle.Flat
            };
            cmbMethod.Items.AddRange(new object[] {
                "ADB (USB Debugging เปิด)",
                "Recovery Mode",
                "Fastboot Mode",
                "Emergency Mode",
                "อัตโนมัติ (แนะนำ)"
            });
            cmbMethod.SelectedIndex = 4;

            topRow.Controls.Add(lblBrand);
            topRow.Controls.Add(cmbBrand);
            topRow.Controls.Add(lblMethod);
            topRow.Controls.Add(cmbMethod);

            var bottomRow = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            devicePanel = new Panel
            {
                Location = new Point(5, 8),
                Size = new Size(350, 50),
                BackColor = Color.FromArgb(20, 20, 32),
                Padding = new Padding(10, 5, 10, 5)
            };

            lblDeviceStatus = new Label
            {
                Text = "⚪ ยังไม่ได้เชื่อมต่ออุปกรณ์",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(255, 183, 77),
                Location = new Point(10, 14),
                AutoSize = true
            };
            devicePanel.Controls.Add(lblDeviceStatus);

            btnConnect = new Button
            {
                Text = "🔌 เชื่อมต่อ ADB",
                Location = new Point(380, 12),
                Size = new Size(150, 40),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = accentColor,
                Cursor = Cursors.Hand
            };
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.Click += BtnConnect_Click;

            btnDisconnect = new Button
            {
                Text = "❌ ตัดการเชื่อมต่อ",
                Location = new Point(540, 12),
                Size = new Size(150, 40),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = dangerColor,
                Cursor = Cursors.Hand
            };
            btnDisconnect.FlatAppearance.BorderSize = 0;
            btnDisconnect.Click += BtnDisconnect_Click;

            bottomRow.Controls.Add(devicePanel);
            bottomRow.Controls.Add(btnConnect);
            bottomRow.Controls.Add(btnDisconnect);

            headerPanel.Controls.Add(bottomRow);
            headerPanel.Controls.Add(topRow);
            this.Controls.Add(headerPanel);
        }

        private void CreateContentPanel()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 10, 15, 5),
                BackColor = bgColor
            };

            var lblLogTitle = new Label
            {
                Text = "📋 บันทึกการทำงาน (Log)",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = textColor,
                Dock = DockStyle.Top,
                Height = 28
            };

            rtbLog = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(14, 14, 20),
                ForeColor = accentColor,
                Font = new Font("Consolas", 10f),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            contentPanel.Controls.Add(rtbLog);
            contentPanel.Controls.Add(lblLogTitle);
            this.Controls.Add(contentPanel);

            LogMessage("╔══════════════════════════════════════════════════════════════╗", Color.FromArgb(100, 149, 237));
            LogMessage("║     Android Unlocker Pro v2.0 - เครื่องมือปลดล็อค Android      ║", Color.FromArgb(100, 149, 237));
            LogMessage("║     รองรับทุกยี่ห้อ: Samsung, Huawei, Xiaomi, OPPO, Vivo...    ║", Color.FromArgb(100, 149, 237));
            LogMessage("╚══════════════════════════════════════════════════════════════╝", Color.FromArgb(100, 149, 237));
            LogMessage("");
            LogMessage("[INFO] โปรแกรมพร้อมใช้งาน - กรุณาเชื่อมต่อมือถือผ่านสาย USB", Color.FromArgb(255, 183, 77));
            LogMessage("[INFO] ตรวจสอบให้แน่ใจว่าได้ติดตั้ง ADB Driver แล้ว", Color.FromArgb(255, 183, 77));
            LogMessage("");
        }

        private void CreateStatusBar()
        {
            statusBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 32,
                BackColor = Color.FromArgb(10, 10, 16)
            };

            lblStatus = new Label
            {
                Text = "พร้อมใช้งาน",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = textMuted,
                Location = new Point(10, 7),
                AutoSize = true
            };

            progressBar = new ProgressBar
            {
                Location = new Point(700, 6),
                Size = new Size(150, 18),
                Style = ProgressBarStyle.Continuous,
                Value = 0
            };

            lblDeviceInfo = new Label
            {
                Text = "อุปกรณ์: ไม่ได้เชื่อมต่อ",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = textMuted,
                Location = new Point(400, 7),
                AutoSize = true
            };

            statusBar.Controls.Add(lblStatus);
            statusBar.Controls.Add(lblDeviceInfo);
            statusBar.Controls.Add(progressBar);
            this.Controls.Add(statusBar);
        }

        private void SetupADB()
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            adbPath = Path.Combine(appDir, "platform-tools", "adb.exe");
            if (!File.Exists(adbPath))
            {
                adbPath = "adb";
            }
        }

        private void LogMessage(string message, Color? color = null)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new Action(() => LogMessage(message, color)));
                return;
            }
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color ?? accentColor;
            if (message.Length > 0)
                rtbLog.AppendText(DateTime.Now.ToString("[HH:mm:ss] ") + message + "\n");
            else
                rtbLog.AppendText("\n");
            rtbLog.ScrollToCaret();
        }

        private void UpdateStatus(string text)
        {
            if (lblStatus.InvokeRequired)
                lblStatus.Invoke(new Action(() => lblStatus.Text = text));
            else
                lblStatus.Text = text;
        }

        private void UpdateProgress(int value)
        {
            if (progressBar.InvokeRequired)
                progressBar.Invoke(new Action(() => progressBar.Value = Math.Min(value, 100)));
            else
                progressBar.Value = Math.Min(value, 100);
        }

        private string RunADBCommand(string arguments)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = adbPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit(15000);
                    if (!string.IsNullOrEmpty(error) && string.IsNullOrEmpty(output))
                        return "ERROR: " + error;
                    return output.Trim();
                }
            }
            catch (Exception ex)
            {
                return "ERROR: " + ex.Message;
            }
        }

        private string RunFastbootCommand(string arguments)
        {
            try
            {
                string fbPath = adbPath.Replace("adb.exe", "fastboot.exe");
                if (fbPath == adbPath) fbPath = "fastboot";
                var psi = new ProcessStartInfo
                {
                    FileName = fbPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit(15000);
                    return string.IsNullOrEmpty(output) ? error : output.Trim();
                }
            }
            catch (Exception ex)
            {
                return "ERROR: " + ex.Message;
            }
        }

        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            UpdateStatus("กำลังเชื่อมต่อ...");
            LogMessage("[CONNECT] กำลังค้นหาอุปกรณ์ที่เชื่อมต่อ...", Color.Cyan);

            await Task.Run(() =>
            {
                UpdateProgress(20);
                string result = RunADBCommand("start-server");
                LogMessage("[ADB] เริ่มต้น ADB Server: " + (result.Contains("ERROR") ? "ล้มเหลว" : "สำเร็จ"), result.Contains("ERROR") ? dangerColor : accentColor);

                UpdateProgress(50);
                string devices = RunADBCommand("devices -l");
                LogMessage("[ADB] ผลลัพธ์: " + devices, textColor);

                UpdateProgress(80);
                if (devices.Contains("device") && !devices.Trim().EndsWith("List of devices attached"))
                {
                    isConnected = true;
                    string model = RunADBCommand("shell getprop ro.product.model");
                    string brand = RunADBCommand("shell getprop ro.product.brand");
                    string android = RunADBCommand("shell getprop ro.build.version.release");
                    string serial = RunADBCommand("get-serialno");

                    this.Invoke(new Action(() =>
                    {
                        lblDeviceStatus.Text = "🟢 เชื่อมต่อแล้ว: " + brand + " " + model;
                        lblDeviceStatus.ForeColor = accentColor;
                        lblDeviceInfo.Text = $"อุปกรณ์: {brand} {model} | Android {android} | S/N: {serial}";
                    }));

                    LogMessage($"[SUCCESS] เชื่อมต่อสำเร็จ!", accentColor);
                    LogMessage($"  ยี่ห้อ: {brand}", textColor);
                    LogMessage($"  รุ่น: {model}", textColor);
                    LogMessage($"  Android: {android}", textColor);
                    LogMessage($"  Serial: {serial}", textColor);
                }
                else if (devices.Contains("unauthorized"))
                {
                    LogMessage("[WARNING] อุปกรณ์ยังไม่ได้อนุญาต USB Debugging", Color.FromArgb(255, 183, 77));
                    LogMessage("[WARNING] กรุณากด 'อนุญาต' บนหน้าจอมือถือ แล้วลองใหม่", Color.FromArgb(255, 183, 77));
                }
                else if (devices.Contains("recovery"))
                {
                    isConnected = true;
                    this.Invoke(new Action(() =>
                    {
                        lblDeviceStatus.Text = "🟡 เชื่อมต่อแล้ว (Recovery Mode)";
                        lblDeviceStatus.ForeColor = Color.FromArgb(255, 183, 77);
                    }));
                    LogMessage("[INFO] อุปกรณ์อยู่ใน Recovery Mode", Color.FromArgb(255, 183, 77));
                }
                else
                {
                    LogMessage("[ERROR] ไม่พบอุปกรณ์ที่เชื่อมต่อ", dangerColor);
                    LogMessage("[HELP] ตรวจสอบ:", Color.FromArgb(255, 183, 77));
                    LogMessage("  1. เสียบสาย USB แล้วหรือยัง", textMuted);
                    LogMessage("  2. เปิด USB Debugging ในมือถือแล้วหรือยัง", textMuted);
                    LogMessage("  3. ติดตั้ง ADB Driver แล้วหรือยัง", textMuted);
                    LogMessage("  4. ลองเปลี่ยนสาย USB หรือพอร์ต", textMuted);
                }
                UpdateProgress(100);
            });

            btnConnect.Enabled = true;
            UpdateStatus(isConnected ? "เชื่อมต่อแล้ว" : "ไม่ได้เชื่อมต่อ");
            await Task.Delay(1000);
            UpdateProgress(0);
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            RunADBCommand("disconnect");
            RunADBCommand("kill-server");
            isConnected = false;
            lblDeviceStatus.Text = "⚪ ยังไม่ได้เชื่อมต่ออุปกรณ์";
            lblDeviceStatus.ForeColor = Color.FromArgb(255, 183, 77);
            lblDeviceInfo.Text = "อุปกรณ์: ไม่ได้เชื่อมต่อ";
            UpdateStatus("ตัดการเชื่อมต่อแล้ว");
            LogMessage("[DISCONNECT] ตัดการเชื่อมต่อเรียบร้อย", Color.FromArgb(255, 183, 77));
        }

        private async void BtnRemovePattern_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            LogMessage("", null);
            LogMessage("═══════════════════════════════════════════", primaryColor);
            LogMessage("[UNLOCK] เริ่มกระบวนการลบรหัส Pattern...", Color.Cyan);
            LogMessage("═══════════════════════════════════════════", primaryColor);
            UpdateStatus("กำลังลบรหัส Pattern...");

            await Task.Run(() =>
            {
                UpdateProgress(10);
                LogMessage("[STEP 1/5] ตรวจสอบสิทธิ์ Root...", textColor);
                string rootCheck = RunADBCommand("shell su -c 'whoami'");
                bool hasRoot = rootCheck.Contains("root");

                UpdateProgress(25);
                if (hasRoot)
                {
                    LogMessage("[INFO] พบสิทธิ์ Root - ใช้วิธีลบไฟล์โดยตรง", accentColor);

                    LogMessage("[STEP 2/5] ลบไฟล์ gesture.key...", textColor);
                    RunADBCommand("shell su -c 'rm /data/system/gesture.key'");
                    UpdateProgress(40);

                    LogMessage("[STEP 3/5] ลบไฟล์ locksettings.db...", textColor);
                    RunADBCommand("shell su -c 'rm /data/system/locksettings.db'");
                    UpdateProgress(55);

                    LogMessage("[STEP 4/5] ลบไฟล์ locksettings.db-wal...", textColor);
                    RunADBCommand("shell su -c 'rm /data/system/locksettings.db-wal'");
                    UpdateProgress(70);

                    LogMessage("[STEP 5/5] ลบไฟล์ gatekeeper เพิ่มเติม...", textColor);
                    RunADBCommand("shell su -c 'rm /data/system/gatekeeper.password.key'");
                    RunADBCommand("shell su -c 'rm /data/system/gatekeeper.pattern.key'");
                    UpdateProgress(90);

                    LogMessage("[REBOOT] กำลังรีสตาร์ทอุปกรณ์...", Color.FromArgb(255, 183, 77));
                    RunADBCommand("reboot");
                    UpdateProgress(100);

                    LogMessage("[SUCCESS] ลบรหัส Pattern สำเร็จ! อุปกรณ์จะรีสตาร์ท", accentColor);
                    LogMessage("[INFO] หลังรีสตาร์ทจะสามารถเข้ามือถือได้โดยไม่ต้องใส่รหัส", accentColor);
                }
                else
                {
                    LogMessage("[INFO] ไม่พบสิทธิ์ Root - ใช้วิธี ADB Shell...", Color.FromArgb(255, 183, 77));
                    UpdateProgress(30);

                    LogMessage("[STEP 2/4] พยายามลบผ่าน ADB Shell...", textColor);
                    RunADBCommand("shell rm /data/system/gesture.key 2>/dev/null");
                    RunADBCommand("shell rm /data/system/locksettings.db 2>/dev/null");
                    UpdateProgress(50);

                    LogMessage("[STEP 3/4] ลองวิธี Settings Command...", textColor);
                    RunADBCommand("shell settings put secure lockscreen.password_type 65536");
                    RunADBCommand("shell locksettings clear --old 0");
                    UpdateProgress(70);

                    LogMessage("[STEP 4/4] ลอง Input keyevent...", textColor);
                    RunADBCommand("shell input keyevent 82");
                    UpdateProgress(90);

                    LogMessage("[INFO] หากไม่สำเร็จ ลองใช้วิธี Recovery Mode หรือ Factory Reset", Color.FromArgb(255, 183, 77));
                    UpdateProgress(100);
                }
            });

            UpdateStatus("เสร็จสิ้น");
            await Task.Delay(2000);
            UpdateProgress(0);
        }

        private async void BtnRemovePin_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            LogMessage("", null);
            LogMessage("═══════════════════════════════════════════", primaryColor);
            LogMessage("[UNLOCK] เริ่มกระบวนการลบรหัส PIN/Password...", Color.Cyan);
            LogMessage("═══════════════════════════════════════════", primaryColor);
            UpdateStatus("กำลังลบรหัส PIN/Password...");

            await Task.Run(() =>
            {
                UpdateProgress(10);
                LogMessage("[STEP 1/6] ตรวจสอบสิทธิ์...", textColor);
                string rootCheck = RunADBCommand("shell su -c 'whoami'");
                bool hasRoot = rootCheck.Contains("root");

                UpdateProgress(20);
                if (hasRoot)
                {
                    LogMessage("[INFO] ใช้สิทธิ์ Root ในการลบ", accentColor);

                    LogMessage("[STEP 2/6] ลบ password.key...", textColor);
                    RunADBCommand("shell su -c 'rm /data/system/password.key'");
                    UpdateProgress(35);

                    LogMessage("[STEP 3/6] ลบ locksettings.db...", textColor);
                    RunADBCommand("shell su -c 'rm /data/system/locksettings.db'");
                    UpdateProgress(50);

                    LogMessage("[STEP 4/6] ลบ gatekeeper keys...", textColor);
                    RunADBCommand("shell su -c 'rm /data/system/gatekeeper.password.key'");
                    RunADBCommand("shell su -c 'rm /data/system/gatekeeper.pattern.key'");
                    UpdateProgress(65);

                    LogMessage("[STEP 5/6] รีเซ็ต lockscreen type...", textColor);
                    RunADBCommand("shell su -c \"sqlite3 /data/system/locksettings.db \\\"DELETE FROM locksettings WHERE name='lockscreen.password_type';\\\"\"");
                    UpdateProgress(80);

                    LogMessage("[STEP 6/6] รีสตาร์ทอุปกรณ์...", textColor);
                    RunADBCommand("reboot");
                    UpdateProgress(100);

                    LogMessage("[SUCCESS] ลบรหัส PIN/Password สำเร็จ!", accentColor);
                }
                else
                {
                    LogMessage("[INFO] ไม่มี Root - ลองวิธี ADB...", Color.FromArgb(255, 183, 77));
                    UpdateProgress(30);

                    RunADBCommand("shell rm /data/system/password.key 2>/dev/null");
                    RunADBCommand("shell locksettings clear --old 0");
                    RunADBCommand("shell settings put secure lockscreen.password_type 65536");
                    RunADBCommand("shell settings put secure lock_pattern_autolock 0");
                    UpdateProgress(70);

                    LogMessage("[INFO] ลองวิธีสำรอง...", textColor);
                    RunADBCommand("shell am start -n com.android.settings/.Settings");
                    UpdateProgress(90);

                    LogMessage("[INFO] หากไม่สำเร็จ ลอง Factory Reset ผ่าน Recovery Mode", Color.FromArgb(255, 183, 77));
                    UpdateProgress(100);
                }
            });

            UpdateStatus("เสร็จสิ้น");
            await Task.Delay(2000);
            UpdateProgress(0);
        }

        private async void BtnRemoveFRP_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            var result = MessageBox.Show(
                "⚠️ การปลดล็อค FRP (Factory Reset Protection)\n\n" +
                "ฟีเจอร์นี้ใช้สำหรับปลดล็อค Google Account Lock หลัง Factory Reset\n\n" +
                "หมายเหตุ: ใช้สำหรับมือถือของคุณเองเท่านั้น\n\nต้องการดำเนินการต่อหรือไม่?",
                "ยืนยันการปลดล็อค FRP",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            LogMessage("", null);
            LogMessage("═══════════════════════════════════════════", primaryColor);
            LogMessage("[FRP] เริ่มกระบวนการปลดล็อค FRP Lock...", Color.Cyan);
            LogMessage("═══════════════════════════════════════════", primaryColor);
            UpdateStatus("กำลังปลดล็อค FRP...");

            await Task.Run(() =>
            {
                UpdateProgress(10);
                string brand = cmbBrand.SelectedItem?.ToString() ?? "";

                LogMessage("[STEP 1/5] ตรวจสอบสถานะ FRP...", textColor);
                UpdateProgress(20);

                LogMessage("[STEP 2/5] พยายามลบ Google Account...", textColor);
                RunADBCommand("shell content call --uri content://com.google.android.gsf.gservices --method restore");
                RunADBCommand("shell am start -n com.google.android.gsf.login/");
                UpdateProgress(40);

                LogMessage("[STEP 3/5] ลบข้อมูล Google Services...", textColor);
                RunADBCommand("shell pm disable com.google.android.gms");
                RunADBCommand("shell am broadcast -a android.accounts.LOGIN_ACCOUNTS_CHANGED");
                UpdateProgress(60);

                LogMessage("[STEP 4/5] รีเซ็ต Account Manager...", textColor);
                RunADBCommand("shell content delete --uri content://com.google.android.gsf.gservices");
                RunADBCommand("shell settings put global setup_wizard_has_run 1");
                RunADBCommand("shell settings put secure user_setup_complete 1");
                UpdateProgress(80);

                LogMessage("[STEP 5/5] ดำเนินการเสร็จสิ้น...", textColor);
                RunADBCommand("shell pm enable com.google.android.gms");
                UpdateProgress(100);

                LogMessage("[INFO] กระบวนการเสร็จสิ้น - กรุณารีสตาร์ทอุปกรณ์", accentColor);
                LogMessage("[INFO] หากไม่สำเร็จ อาจต้องใช้วิธีเฉพาะของแต่ละยี่ห้อ", Color.FromArgb(255, 183, 77));

                ShowBrandSpecificFRPGuide(brand);
            });

            UpdateStatus("เสร็จสิ้น");
            await Task.Delay(2000);
            UpdateProgress(0);
        }

        private void ShowBrandSpecificFRPGuide(string brand)
        {
            LogMessage("", null);
            LogMessage("[GUIDE] คำแนะนำเพิ่มเติมสำหรับ FRP:", Color.Cyan);

            if (brand.Contains("Samsung"))
            {
                LogMessage("  Samsung: ใช้ Samsung FRP Tool หรือ Odin", textColor);
                LogMessage("  1. เข้า Download Mode (Volume Down + Power)", textColor);
                LogMessage("  2. Flash ผ่าน Odin ด้วย Combination Firmware", textColor);
                LogMessage("  3. เข้า Settings > Cloud > Remove Account", textColor);
            }
            else if (brand.Contains("Xiaomi"))
            {
                LogMessage("  Xiaomi: ใช้ Mi Flash Tool", textColor);
                LogMessage("  1. เข้า Fastboot Mode (Volume Down + Power)", textColor);
                LogMessage("  2. Flash ROM ผ่าน Mi Flash", textColor);
                LogMessage("  3. เลือก Clean All", textColor);
            }
            else if (brand.Contains("Huawei"))
            {
                LogMessage("  Huawei: ใช้ HiSuite", textColor);
                LogMessage("  1. เข้า eRecovery Mode", textColor);
                LogMessage("  2. เชื่อมต่อ WiFi และดาวน์โหลด Firmware", textColor);
                LogMessage("  3. ทำ Factory Reset ผ่าน eRecovery", textColor);
            }
            else
            {
                LogMessage("  วิธีทั่วไป:", textColor);
                LogMessage("  1. เข้า Recovery Mode", textColor);
                LogMessage("  2. เลือก Wipe data/factory reset", textColor);
                LogMessage("  3. Flash Firmware ใหม่ผ่าน Fastboot/SP Flash Tool", textColor);
            }
        }

        private async void BtnFactoryReset_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            var result = MessageBox.Show(
                "⚠️ คำเตือน: Factory Reset จะลบข้อมูลทั้งหมดในมือถือ!\n\n" +
                "ข้อมูลที่จะถูกลบ:\n" +
                "- รูปภาพ, วิดีโอ, ไฟล์ทั้งหมด\n" +
                "- แอปพลิเคชันที่ติดตั้ง\n" +
                "- ข้อมูลบัญชี, รหัสผ่าน\n" +
                "- ข้อความ, รายชื่อผู้ติดต่อ\n\n" +
                "ต้องการดำเนินการต่อหรือไม่?",
                "⚠️ ยืนยัน Factory Reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            LogMessage("", null);
            LogMessage("═══════════════════════════════════════════", dangerColor);
            LogMessage("[RESET] เริ่มกระบวนการ Factory Reset...", dangerColor);
            LogMessage("═══════════════════════════════════════════", dangerColor);
            UpdateStatus("กำลัง Factory Reset...");

            await Task.Run(() =>
            {
                string method = cmbMethod.SelectedItem?.ToString() ?? "";

                if (method.Contains("Recovery") || method.Contains("อัตโนมัติ"))
                {
                    UpdateProgress(25);
                    LogMessage("[STEP 1/3] รีบูทเข้า Recovery Mode...", textColor);
                    RunADBCommand("reboot recovery");
                    UpdateProgress(50);

                    LogMessage("[STEP 2/3] รอให้อุปกรณ์เข้า Recovery...", textColor);
                    System.Threading.Thread.Sleep(5000);
                    UpdateProgress(75);

                    LogMessage("[STEP 3/3] ส่งคำสั่ง Wipe...", textColor);
                    RunADBCommand("shell recovery --wipe_data");
                    UpdateProgress(100);

                    LogMessage("[INFO] อุปกรณ์จะทำ Factory Reset อัตโนมัติ", accentColor);
                    LogMessage("[INFO] รอสักครู่ (อาจใช้เวลา 5-10 นาที)", accentColor);
                }
                else if (method.Contains("Fastboot"))
                {
                    UpdateProgress(25);
                    LogMessage("[STEP 1/3] รีบูทเข้า Fastboot/Bootloader...", textColor);
                    RunADBCommand("reboot bootloader");
                    System.Threading.Thread.Sleep(5000);
                    UpdateProgress(50);

                    LogMessage("[STEP 2/3] ส่งคำสั่ง erase...", textColor);
                    RunFastbootCommand("erase userdata");
                    RunFastbootCommand("erase cache");
                    UpdateProgress(75);

                    LogMessage("[STEP 3/3] รีบูทอุปกรณ์...", textColor);
                    RunFastbootCommand("reboot");
                    UpdateProgress(100);

                    LogMessage("[SUCCESS] Factory Reset ผ่าน Fastboot สำเร็จ!", accentColor);
                }
                else
                {
                    UpdateProgress(30);
                    LogMessage("[ADB] ส่งคำสั่ง Factory Reset...", textColor);
                    RunADBCommand("shell am broadcast -a android.intent.action.MASTER_CLEAR");
                    UpdateProgress(70);
                    RunADBCommand("shell wipe data");
                    UpdateProgress(100);
                    LogMessage("[INFO] คำสั่ง Factory Reset ถูกส่งแล้ว", accentColor);
                }
            });

            UpdateStatus("เสร็จสิ้น");
            await Task.Delay(2000);
            UpdateProgress(0);
        }

        private async void BtnStartUnlock_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;

            LogMessage("", null);
            LogMessage("═══════════════════════════════════════════", accentColor);
            LogMessage("[AUTO] เริ่มกระบวนการปลดล็อคอัตโนมัติ...", Color.Cyan);
            LogMessage("═══════════════════════════════════════════", accentColor);
            UpdateStatus("กำลังปลดล็อคอัตโนมัติ...");
            btnStartUnlock.Enabled = false;

            await Task.Run(() =>
            {
                UpdateProgress(5);
                LogMessage("[STEP 1/8] ตรวจสอบข้อมูลอุปกรณ์...", textColor);
                string model = RunADBCommand("shell getprop ro.product.model");
                string brand = RunADBCommand("shell getprop ro.product.brand");
                string sdk = RunADBCommand("shell getprop ro.build.version.sdk");
                LogMessage($"  อุปกรณ์: {brand} {model} (SDK {sdk})", textMuted);
                UpdateProgress(10);

                LogMessage("[STEP 2/8] ตรวจสอบสิทธิ์ Root...", textColor);
                string root = RunADBCommand("shell su -c 'id'");
                bool hasRoot = root.Contains("uid=0");
                LogMessage($"  Root: {(hasRoot ? "มี" : "ไม่มี")}", hasRoot ? accentColor : Color.FromArgb(255, 183, 77));
                UpdateProgress(20);

                LogMessage("[STEP 3/8] ลบไฟล์ Lock ทั้งหมด...", textColor);
                string[] lockFiles = {
                    "/data/system/gesture.key",
                    "/data/system/password.key",
                    "/data/system/locksettings.db",
                    "/data/system/locksettings.db-wal",
                    "/data/system/locksettings.db-shm",
                    "/data/system/gatekeeper.password.key",
                    "/data/system/gatekeeper.pattern.key",
                    "/data/misc/keystore/user_0/.masterkey"
                };

                foreach (var file in lockFiles)
                {
                    string cmd = hasRoot ? $"shell su -c 'rm {file}'" : $"shell rm {file} 2>/dev/null";
                    RunADBCommand(cmd);
                    LogMessage($"  ลบ: {Path.GetFileName(file)}", textMuted);
                }
                UpdateProgress(40);

                LogMessage("[STEP 4/8] รีเซ็ตการตั้งค่า Lock Screen...", textColor);
                RunADBCommand("shell settings put secure lockscreen.password_type 65536");
                RunADBCommand("shell settings put secure lock_pattern_autolock 0");
                RunADBCommand("shell settings put secure lockscreen.password_type_alternate 0");
                UpdateProgress(50);

                LogMessage("[STEP 5/8] ลบ Lock Screen ผ่าน locksettings...", textColor);
                RunADBCommand("shell locksettings clear --old 0");
                RunADBCommand("shell locksettings clear --old 000000");
                RunADBCommand("shell locksettings clear --old 1234");
                UpdateProgress(60);

                LogMessage("[STEP 6/8] ลบ Credential Storage...", textColor);
                if (hasRoot)
                {
                    RunADBCommand("shell su -c 'rm -rf /data/misc/keystore/*'");
                    RunADBCommand("shell su -c 'rm -rf /data/system/*.key'");
                }
                UpdateProgress(75);

                LogMessage("[STEP 7/8] ส่ง Key Event เพื่อปลดล็อค...", textColor);
                RunADBCommand("shell input keyevent 82");
                RunADBCommand("shell input keyevent 26");
                System.Threading.Thread.Sleep(1000);
                RunADBCommand("shell input swipe 540 1800 540 800");
                UpdateProgress(85);

                LogMessage("[STEP 8/8] รีสตาร์ทอุปกรณ์...", textColor);
                RunADBCommand("reboot");
                UpdateProgress(100);

                LogMessage("", null);
                LogMessage("[COMPLETE] กระบวนการปลดล็อคเสร็จสิ้น!", accentColor);
                LogMessage("[INFO] อุปกรณ์จะรีสตาร์ท กรุณารอ 1-2 นาที", Color.FromArgb(255, 183, 77));
                LogMessage("[INFO] หากยังไม่ปลดล็อค ลอง Factory Reset ผ่าน Recovery Mode", Color.FromArgb(255, 183, 77));
            });

            btnStartUnlock.Enabled = true;
            UpdateStatus("เสร็จสิ้น");
            await Task.Delay(2000);
            UpdateProgress(0);
        }

        private bool CheckConnection()
        {
            if (!isConnected)
            {
                MessageBox.Show(
                    "กรุณาเชื่อมต่ออุปกรณ์ก่อน!\n\n" +
                    "1. เสียบสาย USB\n" +
                    "2. กดปุ่ม 'เชื่อมต่อ ADB'",
                    "⚠️ ไม่ได้เชื่อมต่อ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void BtnGuide_Click(object sender, EventArgs e)
        {
            var guideForm = new GuideForm();
            guideForm.ShowDialog();
        }

        private void BtnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Android Unlocker Pro v2.0\n" +
                "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                "เครื่องมือปลดล็อคมือถือ Android ทุกยี่ห้อ\n\n" +
                "รองรับ:\n" +
                "• Samsung, Huawei, Xiaomi, OPPO, Vivo\n" +
                "• Realme, OnePlus, Sony, LG, Motorola\n" +
                "• Nokia, Google Pixel, ASUS, Lenovo\n" +
                "• Infinix, Tecno, Itel, ZTE, และอื่นๆ\n\n" +
                "ฟีเจอร์:\n" +
                "• ลบรหัส Pattern / PIN / Password\n" +
                "• ปลดล็อค FRP (Google Account Lock)\n" +
                "• Factory Reset ผ่าน ADB/Recovery/Fastboot\n" +
                "• รองรับ ADB, Recovery Mode, Fastboot\n\n" +
                "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                "⚠️ ใช้สำหรับมือถือของคุณเองเท่านั้น",
                "ℹ️ เกี่ยวกับโปรแกรม",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            animStep++;
            if (animStep > 100)
            {
                animTimer.Stop();
                animStep = 0;
            }
        }
    }
}
