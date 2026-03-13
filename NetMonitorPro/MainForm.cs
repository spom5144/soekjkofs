using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Linq;
using IPPingStatus = System.Net.NetworkInformation.IPStatus;

namespace NetMonitorPro
{
    public class MainForm : Form
    {
        private TabControl mainTabs;
        private Panel topPanel;
        private Panel statusPanel;
        private Label statusLabel;
        private Label creditLabel;
        private ComboBox interfaceCombo;
        private Button btnRefreshInterfaces;
        private Label lblInterface;

        private ListView hostListView;
        private Button btnScanNetwork;
        private Button btnStopScan;
        private ProgressBar scanProgress;
        private Label lblScanStatus;
        private TextBox txtSubnet;
        private Label lblSubnet;

        private ListView packetListView;
        private TextBox packetDetailBox;
        private Button btnStartCapture;
        private Button btnStopCapture;
        private Button btnClearPackets;
        private Label lblPacketCount;
        private ComboBox cmbProtocolFilter;

        private TextBox txtPortTarget;
        private TextBox txtPortRange;
        private Button btnStartPortScan;
        private Button btnStopPortScan;
        private ListView portListView;
        private ProgressBar portScanProgress;
        private Label lblPortStatus;

        private Label lblBytesSent;
        private Label lblBytesRecv;
        private Label lblPacketsSent;
        private Label lblPacketsRecv;
        private Label lblActiveConnections;
        private ListView connectionListView;
        private Button btnRefreshTraffic;
        private System.Windows.Forms.Timer trafficTimer;
        private Panel statsPanel;

        private TextBox txtDnsHost;
        private Button btnDnsLookup;
        private Button btnReverseDns;
        private TextBox txtDnsResult;
        private TextBox txtPingHost;
        private Button btnPing;
        private TextBox txtPingResult;
        private TextBox txtTracertHost;
        private Button btnTracert;
        private TextBox txtTracertResult;
        private TextBox txtWhoisHost;
        private Button btnWhois;
        private TextBox txtWhoisResult;

        private RichTextBox logBox;
        private Button btnClearLog;
        private Button btnSaveLog;

        private CancellationTokenSource scanCts;
        private CancellationTokenSource captureCts;
        private CancellationTokenSource portScanCts;
        private int packetCount = 0;
        private long totalBytesSent = 0;
        private long totalBytesRecv = 0;

        private static readonly Color DarkBg = Color.FromArgb(18, 18, 24);
        private static readonly Color DarkPanel = Color.FromArgb(26, 26, 36);
        private static readonly Color DarkControl = Color.FromArgb(35, 35, 48);
        private static readonly Color AccentColor = Color.FromArgb(0, 180, 120);
        private static readonly Color AccentHover = Color.FromArgb(0, 210, 140);
        private static readonly Color AccentRed = Color.FromArgb(220, 50, 50);
        private static readonly Color AccentOrange = Color.FromArgb(255, 165, 0);
        private static readonly Color AccentBlue = Color.FromArgb(60, 140, 255);
        private static readonly Color TextPrimary = Color.FromArgb(230, 230, 240);
        private static readonly Color TextSecondary = Color.FromArgb(140, 140, 160);
        private static readonly Color BorderColor = Color.FromArgb(50, 50, 65);

        public MainForm()
        {
            InitializeForm();
            BuildUI();
            LoadNetworkInterfaces();
            AddLog("โปรแกรม Net Monitor Pro เริ่มทำงาน", AccentColor);
            AddLog("ผู้พัฒนา: ป้อม (Pom) | เวอร์ชัน 1.0.0", AccentBlue);
            AddLog("พร้อมใช้งาน - กรุณาเลือก Network Interface", TextSecondary);
        }

        private void InitializeForm()
        {
            this.Text = "Net Monitor Pro v1.0 - เครื่องมือตรวจสอบเครือข่าย | ผู้พัฒนา: ป้อม";
            this.Size = new Size(1280, 850);
            this.MinimumSize = new Size(1024, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = DarkBg;
            this.ForeColor = TextPrimary;
            this.Font = new Font("Segoe UI", 9F);
            this.DoubleBuffered = true;
        }

        private void BuildUI()
        {
            topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = DarkPanel,
                Padding = new Padding(15, 8, 15, 8)
            };
            topPanel.Paint += (s, e) =>
            {
                using var pen = new Pen(BorderColor);
                e.Graphics.DrawLine(pen, 0, topPanel.Height - 1, topPanel.Width, topPanel.Height - 1);
                using var brush = new LinearGradientBrush(new Point(0, 0), new Point(300, 0), AccentColor, AccentBlue);
                using var titleFont = new Font("Segoe UI", 18F, FontStyle.Bold);
                e.Graphics.DrawString("NET MONITOR PRO", titleFont, brush, 15, 8);
                using var subFont = new Font("Segoe UI", 8.5F);
                using var subBrush = new SolidBrush(TextSecondary);
                e.Graphics.DrawString("Network Analysis & Monitoring Tool v1.0  |  สร้างโดย: ป้อม (Pom)", subFont, subBrush, 17, 38);
            };

            lblInterface = CreateLabel("Network Interface:", 580, 10);
            lblInterface.AutoSize = true;
            topPanel.Controls.Add(lblInterface);

            interfaceCombo = new ComboBox
            {
                Location = new Point(580, 32),
                Size = new Size(480, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = DarkControl,
                ForeColor = TextPrimary,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            topPanel.Controls.Add(interfaceCombo);

            btnRefreshInterfaces = CreateButton("🔄", 1068, 30, 60, 28);
            btnRefreshInterfaces.Click += (s, e) => LoadNetworkInterfaces();
            topPanel.Controls.Add(btnRefreshInterfaces);

            this.Controls.Add(topPanel);

            mainTabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Padding = new Point(20, 6)
            };
            mainTabs.DrawMode = TabDrawMode.OwnerDrawFixed;
            mainTabs.DrawItem += MainTabs_DrawItem;
            mainTabs.ItemSize = new Size(150, 36);
            mainTabs.SizeMode = TabSizeMode.Fixed;

            mainTabs.TabPages.Add(CreateScannerTab());
            mainTabs.TabPages.Add(CreatePacketTab());
            mainTabs.TabPages.Add(CreatePortScanTab());
            mainTabs.TabPages.Add(CreateTrafficTab());
            mainTabs.TabPages.Add(CreateToolsTab());
            mainTabs.TabPages.Add(CreateLogTab());

            this.Controls.Add(mainTabs);

            statusPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = DarkPanel,
                Padding = new Padding(10, 0, 10, 0)
            };
            statusPanel.Paint += (s, e) =>
            {
                using var pen = new Pen(BorderColor);
                e.Graphics.DrawLine(pen, 0, 0, statusPanel.Width, 0);
            };

            statusLabel = new Label
            {
                Text = "พร้อมใช้งาน",
                ForeColor = AccentColor,
                Dock = DockStyle.Left,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 6, 0, 0)
            };
            statusPanel.Controls.Add(statusLabel);

            creditLabel = new Label
            {
                Text = "© 2025 ป้อม (Pom) - Net Monitor Pro | เครื่องมือสำหรับผู้ดูแลระบบเครือข่าย",
                ForeColor = TextSecondary,
                Dock = DockStyle.Right,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 6, 5, 0)
            };
            statusPanel.Controls.Add(creditLabel);

            this.Controls.Add(statusPanel);
        }

        private void MainTabs_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tab = mainTabs.TabPages[e.Index];
            var bounds = e.Bounds;
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            using var bgBrush = new SolidBrush(selected ? DarkControl : DarkPanel);
            e.Graphics.FillRectangle(bgBrush, bounds);

            if (selected)
            {
                using var accentPen = new Pen(AccentColor, 3);
                e.Graphics.DrawLine(accentPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }

            using var textBrush = new SolidBrush(selected ? AccentColor : TextSecondary);
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var font = new Font("Segoe UI", 9.5F, selected ? FontStyle.Bold : FontStyle.Regular);
            e.Graphics.DrawString(tab.Text, font, textBrush, bounds, sf);
        }

        private TabPage CreateScannerTab()
        {
            var tab = new TabPage("🔍 สแกนเครือข่าย");
            tab.BackColor = DarkBg;
            tab.ForeColor = TextPrimary;

            var controlPanel = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = DarkPanel, Padding = new Padding(10, 10, 10, 10) };

            lblSubnet = CreateLabel("ช่วง IP:", 10, 15);
            controlPanel.Controls.Add(lblSubnet);

            txtSubnet = new TextBox
            {
                Location = new Point(75, 12),
                Size = new Size(200, 28),
                BackColor = DarkControl,
                ForeColor = TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 10F),
                Text = "192.168.1"
            };
            controlPanel.Controls.Add(txtSubnet);

            btnScanNetwork = CreateButton("▶ เริ่มสแกน", 290, 10, 130, 32);
            btnScanNetwork.Click += BtnScanNetwork_Click;
            controlPanel.Controls.Add(btnScanNetwork);

            btnStopScan = CreateButton("⏹ หยุด", 430, 10, 100, 32);
            btnStopScan.BackColor = AccentRed;
            btnStopScan.Enabled = false;
            btnStopScan.Click += BtnStopScan_Click;
            controlPanel.Controls.Add(btnStopScan);

            lblScanStatus = CreateLabel("พร้อมสแกน", 550, 15);
            lblScanStatus.AutoSize = true;
            lblScanStatus.ForeColor = TextSecondary;
            controlPanel.Controls.Add(lblScanStatus);

            scanProgress = new ProgressBar
            {
                Location = new Point(750, 14),
                Size = new Size(250, 24),
                Style = ProgressBarStyle.Continuous
            };
            controlPanel.Controls.Add(scanProgress);

            tab.Controls.Add(controlPanel);

            hostListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = DarkBg,
                ForeColor = TextPrimary,
                Font = new Font("Consolas", 9.5F),
                BorderStyle = BorderStyle.None,
                HeaderStyle = ColumnHeaderStyle.Nonclickable
            };
            hostListView.Columns.Add("IP Address", 180);
            hostListView.Columns.Add("Hostname", 250);
            hostListView.Columns.Add("MAC Address", 180);
            hostListView.Columns.Add("สถานะ", 100);
            hostListView.Columns.Add("เวลาตอบ (ms)", 120);
            hostListView.Columns.Add("ระบบปฏิบัติการ (คาดเดา)", 200);
            hostListView.Columns.Add("พบเมื่อ", 180);
            tab.Controls.Add(hostListView);

            return tab;
        }

        private TabPage CreatePacketTab()
        {
            var tab = new TabPage("📦 ดักจับแพ็กเก็ต");
            tab.BackColor = DarkBg;

            var controlPanel = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = DarkPanel, Padding = new Padding(10) };

            btnStartCapture = CreateButton("▶ เริ่มจับ", 10, 10, 120, 32);
            btnStartCapture.Click += BtnStartCapture_Click;
            controlPanel.Controls.Add(btnStartCapture);

            btnStopCapture = CreateButton("⏹ หยุด", 140, 10, 100, 32);
            btnStopCapture.BackColor = AccentRed;
            btnStopCapture.Enabled = false;
            btnStopCapture.Click += BtnStopCapture_Click;
            controlPanel.Controls.Add(btnStopCapture);

            btnClearPackets = CreateButton("🗑 ล้าง", 250, 10, 100, 32);
            btnClearPackets.Click += (s, e) => { packetListView.Items.Clear(); packetCount = 0; lblPacketCount.Text = "แพ็กเก็ต: 0"; };
            controlPanel.Controls.Add(btnClearPackets);

            var lblFilter = CreateLabel("ตัวกรอง:", 370, 15);
            controlPanel.Controls.Add(lblFilter);

            cmbProtocolFilter = new ComboBox
            {
                Location = new Point(440, 12),
                Size = new Size(150, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = DarkControl,
                ForeColor = TextPrimary,
                FlatStyle = FlatStyle.Flat
            };
            cmbProtocolFilter.Items.AddRange(new object[] { "ทั้งหมด", "TCP", "UDP", "ICMP", "HTTP", "DNS" });
            cmbProtocolFilter.SelectedIndex = 0;
            controlPanel.Controls.Add(cmbProtocolFilter);

            lblPacketCount = CreateLabel("แพ็กเก็ต: 0", 620, 15);
            lblPacketCount.AutoSize = true;
            lblPacketCount.ForeColor = AccentColor;
            controlPanel.Controls.Add(lblPacketCount);

            tab.Controls.Add(controlPanel);

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 350,
                BackColor = BorderColor,
                SplitterWidth = 3
            };

            packetListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = DarkBg,
                ForeColor = TextPrimary,
                Font = new Font("Consolas", 9F),
                BorderStyle = BorderStyle.None
            };
            packetListView.Columns.Add("#", 60);
            packetListView.Columns.Add("เวลา", 100);
            packetListView.Columns.Add("ต้นทาง", 180);
            packetListView.Columns.Add("ปลายทาง", 180);
            packetListView.Columns.Add("โปรโตคอล", 90);
            packetListView.Columns.Add("ขนาด", 80);
            packetListView.Columns.Add("ข้อมูล", 350);
            packetListView.SelectedIndexChanged += PacketListView_SelectedIndexChanged;
            splitContainer.Panel1.Controls.Add(packetListView);

            packetDetailBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                BackColor = Color.FromArgb(20, 20, 30),
                ForeColor = AccentColor,
                Font = new Font("Consolas", 9.5F),
                BorderStyle = BorderStyle.None,
                WordWrap = false
            };
            splitContainer.Panel2.Controls.Add(packetDetailBox);
            tab.Controls.Add(splitContainer);

            return tab;
        }

        private TabPage CreatePortScanTab()
        {
            var tab = new TabPage("🔓 สแกนพอร์ต");
            tab.BackColor = DarkBg;

            var controlPanel = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = DarkPanel, Padding = new Padding(10) };

            var lblTarget = CreateLabel("เป้าหมาย:", 10, 15);
            controlPanel.Controls.Add(lblTarget);

            txtPortTarget = new TextBox
            {
                Location = new Point(85, 12),
                Size = new Size(180, 28),
                BackColor = DarkControl,
                ForeColor = TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 10F),
                Text = "127.0.0.1"
            };
            controlPanel.Controls.Add(txtPortTarget);

            var lblRange = CreateLabel("ช่วงพอร์ต:", 280, 15);
            controlPanel.Controls.Add(lblRange);

            txtPortRange = new TextBox
            {
                Location = new Point(365, 12),
                Size = new Size(130, 28),
                BackColor = DarkControl,
                ForeColor = TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 10F),
                Text = "1-1024"
            };
            controlPanel.Controls.Add(txtPortRange);

            btnStartPortScan = CreateButton("▶ เริ่มสแกน", 510, 10, 130, 32);
            btnStartPortScan.Click += BtnStartPortScan_Click;
            controlPanel.Controls.Add(btnStartPortScan);

            btnStopPortScan = CreateButton("⏹ หยุด", 650, 10, 100, 32);
            btnStopPortScan.BackColor = AccentRed;
            btnStopPortScan.Enabled = false;
            btnStopPortScan.Click += BtnStopPortScan_Click;
            controlPanel.Controls.Add(btnStopPortScan);

            lblPortStatus = CreateLabel("พร้อมสแกน", 770, 15);
            lblPortStatus.AutoSize = true;
            lblPortStatus.ForeColor = TextSecondary;
            controlPanel.Controls.Add(lblPortStatus);

            portScanProgress = new ProgressBar
            {
                Location = new Point(920, 14),
                Size = new Size(200, 24),
                Style = ProgressBarStyle.Continuous
            };
            controlPanel.Controls.Add(portScanProgress);

            tab.Controls.Add(controlPanel);

            portListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = DarkBg,
                ForeColor = TextPrimary,
                Font = new Font("Consolas", 9.5F),
                BorderStyle = BorderStyle.None
            };
            portListView.Columns.Add("พอร์ต", 100);
            portListView.Columns.Add("สถานะ", 120);
            portListView.Columns.Add("โปรโตคอล", 100);
            portListView.Columns.Add("บริการ", 200);
            portListView.Columns.Add("เวอร์ชัน/แบนเนอร์", 350);
            portListView.Columns.Add("เวลาตอบ (ms)", 120);
            tab.Controls.Add(portListView);

            return tab;
        }

        private TabPage CreateTrafficTab()
        {
            var tab = new TabPage("📊 ทราฟฟิก");
            tab.BackColor = DarkBg;

            statsPanel = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = DarkPanel, Padding = new Padding(15) };
            statsPanel.Paint += (s, e) =>
            {
                using var pen = new Pen(BorderColor);
                e.Graphics.DrawLine(pen, 0, statsPanel.Height - 1, statsPanel.Width, statsPanel.Height - 1);
            };

            int cardWidth = 200;
            int cardHeight = 80;
            int spacing = 15;

            lblBytesSent = CreateStatCard(statsPanel, "📤 ข้อมูลส่ง", "0 B", 10, 15, cardWidth, cardHeight, AccentBlue);
            lblBytesRecv = CreateStatCard(statsPanel, "📥 ข้อมูลรับ", "0 B", 10 + cardWidth + spacing, 15, cardWidth, cardHeight, AccentColor);
            lblPacketsSent = CreateStatCard(statsPanel, "📤 แพ็กเก็ตส่ง", "0", 10 + (cardWidth + spacing) * 2, 15, cardWidth, cardHeight, AccentOrange);
            lblPacketsRecv = CreateStatCard(statsPanel, "📥 แพ็กเก็ตรับ", "0", 10 + (cardWidth + spacing) * 3, 15, cardWidth, cardHeight, Color.FromArgb(180, 100, 255));
            lblActiveConnections = CreateStatCard(statsPanel, "🔗 การเชื่อมต่อ", "0", 10 + (cardWidth + spacing) * 4, 15, cardWidth, cardHeight, AccentRed);

            btnRefreshTraffic = CreateButton("🔄 รีเฟรช", 10 + (cardWidth + spacing) * 4 + cardWidth + spacing, 15, 100, 32);
            btnRefreshTraffic.Click += (s, e) => RefreshTrafficStats();
            statsPanel.Controls.Add(btnRefreshTraffic);

            tab.Controls.Add(statsPanel);

            connectionListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = DarkBg,
                ForeColor = TextPrimary,
                Font = new Font("Consolas", 9F),
                BorderStyle = BorderStyle.None
            };
            connectionListView.Columns.Add("โปรโตคอล", 90);
            connectionListView.Columns.Add("ที่อยู่ภายใน", 200);
            connectionListView.Columns.Add("ที่อยู่ภายนอก", 200);
            connectionListView.Columns.Add("สถานะ", 130);
            connectionListView.Columns.Add("PID", 80);
            connectionListView.Columns.Add("โปรเซส", 200);
            tab.Controls.Add(connectionListView);

            trafficTimer = new System.Windows.Forms.Timer { Interval = 2000 };
            trafficTimer.Tick += (s, e) => RefreshTrafficStats();
            trafficTimer.Start();

            return tab;
        }

        private TabPage CreateToolsTab()
        {
            var tab = new TabPage("🛠 เครื่องมือ");
            tab.BackColor = DarkBg;
            tab.AutoScroll = true;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(10),
                BackColor = DarkBg
            };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            var dnsPanel = CreateToolPanel("🌐 DNS Lookup");
            var dnsContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
            txtDnsHost = CreateTextInput("google.com", 0);
            txtDnsHost.Dock = DockStyle.Top;
            dnsContent.Controls.Add(txtDnsHost);
            var dnsButtons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, FlowDirection = FlowDirection.LeftToRight, BackColor = Color.Transparent, Padding = new Padding(0, 5, 0, 0) };
            btnDnsLookup = CreateButton("ค้นหา DNS", 0, 0, 110, 30);
            btnDnsLookup.Click += BtnDnsLookup_Click;
            btnReverseDns = CreateButton("Reverse DNS", 0, 0, 120, 30);
            btnReverseDns.Click += BtnReverseDns_Click;
            dnsButtons.Controls.AddRange(new Control[] { btnDnsLookup, btnReverseDns });
            dnsContent.Controls.Add(dnsButtons);
            txtDnsResult = CreateResultBox();
            txtDnsResult.Dock = DockStyle.Fill;
            dnsContent.Controls.Add(txtDnsResult);
            dnsContent.Controls.SetChildIndex(txtDnsResult, 0);
            dnsContent.Controls.SetChildIndex(dnsButtons, 1);
            dnsContent.Controls.SetChildIndex(txtDnsHost, 2);
            dnsPanel.Controls.Add(dnsContent);
            mainPanel.Controls.Add(dnsPanel, 0, 0);

            var pingPanel = CreateToolPanel("📡 Ping");
            var pingContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
            txtPingHost = CreateTextInput("8.8.8.8", 0);
            txtPingHost.Dock = DockStyle.Top;
            pingContent.Controls.Add(txtPingHost);
            var pingButtons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, BackColor = Color.Transparent, Padding = new Padding(0, 5, 0, 0) };
            btnPing = CreateButton("Ping (4 ครั้ง)", 0, 0, 130, 30);
            btnPing.Click += BtnPing_Click;
            pingButtons.Controls.Add(btnPing);
            pingContent.Controls.Add(pingButtons);
            txtPingResult = CreateResultBox();
            txtPingResult.Dock = DockStyle.Fill;
            pingContent.Controls.Add(txtPingResult);
            pingContent.Controls.SetChildIndex(txtPingResult, 0);
            pingContent.Controls.SetChildIndex(pingButtons, 1);
            pingContent.Controls.SetChildIndex(txtPingHost, 2);
            pingPanel.Controls.Add(pingContent);
            mainPanel.Controls.Add(pingPanel, 1, 0);

            var tracertPanel = CreateToolPanel("🗺 Traceroute");
            var tracertContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
            txtTracertHost = CreateTextInput("google.com", 0);
            txtTracertHost.Dock = DockStyle.Top;
            tracertContent.Controls.Add(txtTracertHost);
            var tracertButtons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, BackColor = Color.Transparent, Padding = new Padding(0, 5, 0, 0) };
            btnTracert = CreateButton("เริ่ม Traceroute", 0, 0, 140, 30);
            btnTracert.Click += BtnTracert_Click;
            tracertButtons.Controls.Add(btnTracert);
            tracertContent.Controls.Add(tracertButtons);
            txtTracertResult = CreateResultBox();
            txtTracertResult.Dock = DockStyle.Fill;
            tracertContent.Controls.Add(txtTracertResult);
            tracertContent.Controls.SetChildIndex(txtTracertResult, 0);
            tracertContent.Controls.SetChildIndex(tracertButtons, 1);
            tracertContent.Controls.SetChildIndex(txtTracertHost, 2);
            tracertPanel.Controls.Add(tracertContent);
            mainPanel.Controls.Add(tracertPanel, 0, 1);

            var whoisPanel = CreateToolPanel("📋 WHOIS Lookup");
            var whoisContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
            txtWhoisHost = CreateTextInput("google.com", 0);
            txtWhoisHost.Dock = DockStyle.Top;
            whoisContent.Controls.Add(txtWhoisHost);
            var whoisButtons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, BackColor = Color.Transparent, Padding = new Padding(0, 5, 0, 0) };
            btnWhois = CreateButton("ค้นหา WHOIS", 0, 0, 130, 30);
            btnWhois.Click += BtnWhois_Click;
            whoisButtons.Controls.Add(btnWhois);
            whoisContent.Controls.Add(whoisButtons);
            txtWhoisResult = CreateResultBox();
            txtWhoisResult.Dock = DockStyle.Fill;
            whoisContent.Controls.Add(txtWhoisResult);
            whoisContent.Controls.SetChildIndex(txtWhoisResult, 0);
            whoisContent.Controls.SetChildIndex(whoisButtons, 1);
            whoisContent.Controls.SetChildIndex(txtWhoisHost, 2);
            whoisPanel.Controls.Add(whoisContent);
            mainPanel.Controls.Add(whoisPanel, 1, 1);

            tab.Controls.Add(mainPanel);
            return tab;
        }

        private TabPage CreateLogTab()
        {
            var tab = new TabPage("📝 บันทึก");
            tab.BackColor = DarkBg;

            var controlPanel = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = DarkPanel, Padding = new Padding(10) };

            btnClearLog = CreateButton("🗑 ล้างบันทึก", 10, 10, 130, 30);
            btnClearLog.Click += (s, e) => logBox.Clear();
            controlPanel.Controls.Add(btnClearLog);

            btnSaveLog = CreateButton("💾 บันทึกไฟล์", 150, 10, 130, 30);
            btnSaveLog.Click += BtnSaveLog_Click;
            controlPanel.Controls.Add(btnSaveLog);

            tab.Controls.Add(controlPanel);

            logBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.FromArgb(15, 15, 22),
                ForeColor = TextPrimary,
                Font = new Font("Consolas", 9.5F),
                BorderStyle = BorderStyle.None,
                WordWrap = true
            };
            tab.Controls.Add(logBox);

            return tab;
        }

        private Label CreateStatCard(Panel parent, string title, string value, int x, int y, int w, int h, Color accentClr)
        {
            var card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = DarkControl
            };
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(accentClr, 2);
                e.Graphics.DrawLine(pen, 0, 0, 0, card.Height);
                using var brush = new SolidBrush(Color.FromArgb(30, accentClr));
                e.Graphics.FillRectangle(brush, 0, 0, card.Width, card.Height);
            };

            var titleLbl = new Label
            {
                Text = title,
                ForeColor = TextSecondary,
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(12, 8),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            card.Controls.Add(titleLbl);

            var valueLbl = new Label
            {
                Text = value,
                ForeColor = accentClr,
                Font = new Font("Consolas", 16F, FontStyle.Bold),
                Location = new Point(12, 30),
                AutoSize = true,
                BackColor = Color.Transparent,
                Tag = "value"
            };
            card.Controls.Add(valueLbl);

            parent.Controls.Add(card);
            return valueLbl;
        }

        private Panel CreateToolPanel(string title)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = DarkPanel,
                Margin = new Padding(5),
                Padding = new Padding(1)
            };

            var titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 35,
                ForeColor = AccentColor,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                BackColor = DarkControl
            };
            panel.Controls.Add(titleLabel);

            return panel;
        }

        private TextBox CreateTextInput(string defaultText, int y)
        {
            return new TextBox
            {
                Text = defaultText,
                BackColor = DarkControl,
                ForeColor = TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 10F),
                Height = 28
            };
        }

        private TextBox CreateResultBox()
        {
            return new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                BackColor = Color.FromArgb(20, 20, 30),
                ForeColor = AccentColor,
                Font = new Font("Consolas", 9F),
                BorderStyle = BorderStyle.None,
                WordWrap = false
            };
        }

        private Button CreateButton(string text, int x, int y, int w, int h)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = AccentHover;
            return btn;
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                ForeColor = TextSecondary,
                Font = new Font("Segoe UI", 9.5F),
                AutoSize = true
            };
        }

        private void LoadNetworkInterfaces()
        {
            interfaceCombo.Items.Clear();
            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up)
                    {
                        var ip = ni.GetIPProperties().UnicastAddresses
                            .FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork);
                        string ipStr = ip != null ? ip.Address.ToString() : "N/A";
                        interfaceCombo.Items.Add($"{ni.Name} - {ni.NetworkInterfaceType} [{ipStr}]");
                    }
                }
                if (interfaceCombo.Items.Count > 0)
                {
                    interfaceCombo.SelectedIndex = 0;
                    var selectedNi = NetworkInterface.GetAllNetworkInterfaces()
                        .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up);
                    if (selectedNi != null)
                    {
                        var ip = selectedNi.GetIPProperties().UnicastAddresses
                            .FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork);
                        if (ip != null)
                        {
                            var parts = ip.Address.ToString().Split('.');
                            txtSubnet.Text = $"{parts[0]}.{parts[1]}.{parts[2]}";
                        }
                    }
                }
                AddLog($"พบ {interfaceCombo.Items.Count} อินเทอร์เฟซเครือข่าย", AccentColor);
            }
            catch (Exception ex)
            {
                AddLog($"ข้อผิดพลาดในการโหลดอินเทอร์เฟซ: {ex.Message}", AccentRed);
            }
        }

        private async void BtnScanNetwork_Click(object sender, EventArgs e)
        {
            scanCts = new CancellationTokenSource();
            btnScanNetwork.Enabled = false;
            btnStopScan.Enabled = true;
            hostListView.Items.Clear();
            scanProgress.Value = 0;
            scanProgress.Maximum = 254;

            string subnet = txtSubnet.Text.Trim();
            AddLog($"เริ่มสแกนเครือข่าย {subnet}.0/24", AccentColor);
            SetStatus($"กำลังสแกน {subnet}.0/24...");

            int found = 0;
            var tasks = new List<Task>();

            for (int i = 1; i <= 254; i++)
            {
                if (scanCts.Token.IsCancellationRequested) break;
                string ip = $"{subnet}.{i}";
                int index = i;
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using var ping = new Ping();
                        var reply = await ping.SendPingAsync(ip, 500);
                        if (reply.Status == IPPingStatus.Success)
                        {
                            string hostname = "";
                            try
                            {
                                var entry = await Dns.GetHostEntryAsync(ip);
                                hostname = entry.HostName;
                            }
                            catch { hostname = "ไม่สามารถระบุได้"; }

                            string mac = GetMacFromArp(ip);
                            string os = GuessOS((int)reply.Options?.Ttl);
                            Interlocked.Increment(ref found);

                            this.Invoke(() =>
                            {
                                var item = new ListViewItem(ip);
                                item.SubItems.Add(hostname);
                                item.SubItems.Add(mac);
                                item.SubItems.Add("ออนไลน์");
                                item.SubItems.Add(reply.RoundtripTime.ToString());
                                item.SubItems.Add(os);
                                item.SubItems.Add(DateTime.Now.ToString("HH:mm:ss"));
                                item.ForeColor = AccentColor;
                                hostListView.Items.Add(item);
                            });
                        }
                    }
                    catch { }
                    finally
                    {
                        this.Invoke(() =>
                        {
                            if (scanProgress.Value < scanProgress.Maximum)
                                scanProgress.Value = Math.Min(index, scanProgress.Maximum);
                            lblScanStatus.Text = $"สแกน: {index}/254 | พบ: {found}";
                        });
                    }
                }, scanCts.Token));

                if (tasks.Count >= 50)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(t => t.IsCompleted);
                }
            }

            await Task.WhenAll(tasks);
            scanProgress.Value = scanProgress.Maximum;
            lblScanStatus.Text = $"เสร็จสิ้น | พบ {found} โฮสต์";
            btnScanNetwork.Enabled = true;
            btnStopScan.Enabled = false;
            AddLog($"สแกนเสร็จสิ้น: พบ {found} โฮสต์ในเครือข่าย {subnet}.0/24", AccentColor);
            SetStatus("สแกนเสร็จสิ้น");
        }

        private void BtnStopScan_Click(object sender, EventArgs e)
        {
            scanCts?.Cancel();
            btnScanNetwork.Enabled = true;
            btnStopScan.Enabled = false;
            AddLog("หยุดการสแกนเครือข่าย", AccentOrange);
            SetStatus("หยุดการสแกน");
        }

        private async void BtnStartCapture_Click(object sender, EventArgs e)
        {
            captureCts = new CancellationTokenSource();
            btnStartCapture.Enabled = false;
            btnStopCapture.Enabled = true;
            AddLog("เริ่มดักจับแพ็กเก็ต (จำลอง)", AccentColor);
            SetStatus("กำลังดักจับแพ็กเก็ต...");

            await Task.Run(async () =>
            {
                var rand = new Random();
                string[] protocols = { "TCP", "UDP", "HTTP", "HTTPS", "DNS", "ICMP", "ARP", "TLS" };
                string[] srcIPs = { "192.168.1.100", "192.168.1.1", "10.0.0.5", "172.16.0.1" };
                string[] dstIPs = { "8.8.8.8", "1.1.1.1", "142.250.190.78", "151.101.1.140", "104.244.42.1" };
                string[] infos = {
                    "SYN → [SYN,ACK] Seq=0 Win=65535",
                    "HTTP GET /api/v1/data HTTP/1.1",
                    "DNS Standard query A www.google.com",
                    "TLS Client Hello, Version: TLS 1.3",
                    "ACK Seq=1 Ack=1 Win=65535 Len=0",
                    "HTTPS → TLS 1.3 Application Data",
                    "DNS Standard query response A 142.250.190.78",
                    "TCP [RST, ACK] Seq=1 Win=0 Len=0",
                    "UDP 53 → 5353 Len=48",
                    "ICMP Echo (ping) request id=0x0001",
                    "ARP Who has 192.168.1.1? Tell 192.168.1.100",
                    "HTTP/1.1 200 OK (text/html)"
                };

                while (!captureCts.Token.IsCancellationRequested)
                {
                    await Task.Delay(rand.Next(100, 800), captureCts.Token).ContinueWith(t => { });
                    if (captureCts.Token.IsCancellationRequested) break;

                    string proto = protocols[rand.Next(protocols.Length)];
                    string filter = "";
                    this.Invoke(() => filter = cmbProtocolFilter.SelectedItem?.ToString() ?? "ทั้งหมด");
                    if (filter != "ทั้งหมด" && proto != filter) continue;

                    int pktNum = Interlocked.Increment(ref packetCount);
                    string src = srcIPs[rand.Next(srcIPs.Length)] + ":" + rand.Next(1024, 65535);
                    string dst = dstIPs[rand.Next(dstIPs.Length)] + ":" + (proto == "HTTP" ? 80 : proto == "HTTPS" ? 443 : proto == "DNS" ? 53 : rand.Next(1, 65535));
                    int size = rand.Next(40, 1500);
                    string info = infos[rand.Next(infos.Length)];

                    this.Invoke(() =>
                    {
                        var item = new ListViewItem(pktNum.ToString());
                        item.SubItems.Add(DateTime.Now.ToString("HH:mm:ss.fff"));
                        item.SubItems.Add(src);
                        item.SubItems.Add(dst);
                        item.SubItems.Add(proto);
                        item.SubItems.Add(size + " B");
                        item.SubItems.Add(info);

                        item.ForeColor = proto switch
                        {
                            "TCP" => AccentBlue,
                            "HTTP" => AccentColor,
                            "HTTPS" => Color.FromArgb(0, 200, 200),
                            "DNS" => AccentOrange,
                            "UDP" => Color.FromArgb(180, 180, 255),
                            "ICMP" => Color.FromArgb(255, 200, 100),
                            "ARP" => Color.FromArgb(255, 150, 150),
                            "TLS" => Color.FromArgb(200, 150, 255),
                            _ => TextPrimary
                        };

                        packetListView.Items.Add(item);
                        if (packetListView.Items.Count > 1)
                            packetListView.EnsureVisible(packetListView.Items.Count - 1);
                        lblPacketCount.Text = $"แพ็กเก็ต: {pktNum}";
                    });
                }
            });
        }

        private void BtnStopCapture_Click(object sender, EventArgs e)
        {
            captureCts?.Cancel();
            btnStartCapture.Enabled = true;
            btnStopCapture.Enabled = false;
            AddLog($"หยุดดักจับแพ็กเก็ต (รวม {packetCount} แพ็กเก็ต)", AccentOrange);
            SetStatus("หยุดดักจับแพ็กเก็ต");
        }

        private void PacketListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (packetListView.SelectedItems.Count == 0) return;
            var item = packetListView.SelectedItems[0];
            var sb = new StringBuilder();
            sb.AppendLine("╔══════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                    รายละเอียดแพ็กเก็ต                        ║");
            sb.AppendLine("╠══════════════════════════════════════════════════════════════╣");
            sb.AppendLine($"║  หมายเลข     : #{item.SubItems[0].Text,-45} ║");
            sb.AppendLine($"║  เวลา       : {item.SubItems[1].Text,-45} ║");
            sb.AppendLine($"║  ต้นทาง     : {item.SubItems[2].Text,-45} ║");
            sb.AppendLine($"║  ปลายทาง    : {item.SubItems[3].Text,-45} ║");
            sb.AppendLine($"║  โปรโตคอล   : {item.SubItems[4].Text,-45} ║");
            sb.AppendLine($"║  ขนาด       : {item.SubItems[5].Text,-45} ║");
            sb.AppendLine("╠══════════════════════════════════════════════════════════════╣");
            sb.AppendLine($"║  ข้อมูล     : {item.SubItems[6].Text,-45} ║");
            sb.AppendLine("╠══════════════════════════════════════════════════════════════╣");
            sb.AppendLine("║  Hex Dump (ตัวอย่าง):                                       ║");
            sb.AppendLine("║  0000: 45 00 00 3c 1c 46 40 00 40 06 b1 e6 ac 10 0a 63   ║");
            sb.AppendLine("║  0010: ac 10 0a 0c 00 50 01 bb 00 00 00 00 a0 02 72 10   ║");
            sb.AppendLine("║  0020: 00 00 00 00 02 04 05 b4 04 02 08 0a 00 9d 3b 14   ║");
            sb.AppendLine("╚══════════════════════════════════════════════════════════════╝");
            packetDetailBox.Text = sb.ToString();
        }

        private async void BtnStartPortScan_Click(object sender, EventArgs e)
        {
            portScanCts = new CancellationTokenSource();
            btnStartPortScan.Enabled = false;
            btnStopPortScan.Enabled = true;
            portListView.Items.Clear();

            string target = txtPortTarget.Text.Trim();
            string rangeStr = txtPortRange.Text.Trim();
            var parts = rangeStr.Split('-');
            int startPort = int.Parse(parts[0]);
            int endPort = parts.Length > 1 ? int.Parse(parts[1]) : startPort;
            int totalPorts = endPort - startPort + 1;

            portScanProgress.Value = 0;
            portScanProgress.Maximum = totalPorts;

            AddLog($"เริ่มสแกนพอร์ต {target} [{startPort}-{endPort}]", AccentColor);
            SetStatus($"กำลังสแกนพอร์ต {target}...");

            int openCount = 0;
            int scanned = 0;

            var tasks = new List<Task>();
            for (int port = startPort; port <= endPort; port++)
            {
                if (portScanCts.Token.IsCancellationRequested) break;
                int p = port;
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var sw = Stopwatch.StartNew();
                        using var client = new TcpClient();
                        var connectTask = client.ConnectAsync(target, p);
                        if (await Task.WhenAny(connectTask, Task.Delay(1000)) == connectTask && client.Connected)
                        {
                            sw.Stop();
                            Interlocked.Increment(ref openCount);
                            string service = GetServiceName(p);
                            string banner = await TryGrabBanner(client, p);
                            this.Invoke(() =>
                            {
                                var item = new ListViewItem(p.ToString());
                                item.SubItems.Add("เปิด");
                                item.SubItems.Add("TCP");
                                item.SubItems.Add(service);
                                item.SubItems.Add(banner);
                                item.SubItems.Add(sw.ElapsedMilliseconds.ToString());
                                item.ForeColor = AccentColor;
                                portListView.Items.Add(item);
                            });
                        }
                    }
                    catch { }
                    finally
                    {
                        int s = Interlocked.Increment(ref scanned);
                        this.Invoke(() =>
                        {
                            if (s <= portScanProgress.Maximum)
                                portScanProgress.Value = s;
                            lblPortStatus.Text = $"สแกน: {s}/{totalPorts} | เปิด: {openCount}";
                        });
                    }
                }, portScanCts.Token));

                if (tasks.Count >= 100)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(t => t.IsCompleted);
                }
            }

            await Task.WhenAll(tasks);
            portScanProgress.Value = portScanProgress.Maximum;
            lblPortStatus.Text = $"เสร็จสิ้น | พบ {openCount} พอร์ตเปิด";
            btnStartPortScan.Enabled = true;
            btnStopPortScan.Enabled = false;
            AddLog($"สแกนพอร์ตเสร็จ: พบ {openCount} พอร์ตเปิดจากทั้งหมด {totalPorts} พอร์ต", AccentColor);
            SetStatus("สแกนพอร์ตเสร็จสิ้น");
        }

        private void BtnStopPortScan_Click(object sender, EventArgs e)
        {
            portScanCts?.Cancel();
            btnStartPortScan.Enabled = true;
            btnStopPortScan.Enabled = false;
            AddLog("หยุดการสแกนพอร์ต", AccentOrange);
        }

        private void RefreshTrafficStats()
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(n => n.OperationalStatus == OperationalStatus.Up);

                long sent = 0, recv = 0, pSent = 0, pRecv = 0;
                foreach (var ni in interfaces)
                {
                    var stats = ni.GetIPStatistics();
                    sent += stats.BytesSent;
                    recv += stats.BytesReceived;
                    pSent += stats.UnicastPacketsSent;
                    pRecv += stats.UnicastPacketsReceived;
                }

                lblBytesSent.Text = FormatBytes(sent);
                lblBytesRecv.Text = FormatBytes(recv);
                lblPacketsSent.Text = pSent.ToString("N0");
                lblPacketsRecv.Text = pRecv.ToString("N0");

                var connections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
                lblActiveConnections.Text = connections.Length.ToString();

                connectionListView.BeginUpdate();
                connectionListView.Items.Clear();
                foreach (var conn in connections.Take(200))
                {
                    var item = new ListViewItem("TCP");
                    item.SubItems.Add(conn.LocalEndPoint.ToString());
                    item.SubItems.Add(conn.RemoteEndPoint.ToString());
                    item.SubItems.Add(conn.State.ToString());
                    item.SubItems.Add("-");
                    item.SubItems.Add("-");
                    item.ForeColor = conn.State == TcpState.Established ? AccentColor :
                                     conn.State == TcpState.TimeWait ? TextSecondary : AccentOrange;
                    connectionListView.Items.Add(item);
                }
                connectionListView.EndUpdate();
            }
            catch { }
        }

        private async void BtnDnsLookup_Click(object sender, EventArgs e)
        {
            try
            {
                string host = txtDnsHost.Text.Trim();
                txtDnsResult.Text = $"กำลังค้นหา DNS สำหรับ {host}...\r\n";
                AddLog($"DNS Lookup: {host}", AccentBlue);

                var entry = await Dns.GetHostEntryAsync(host);
                var sb = new StringBuilder();
                sb.AppendLine($"═══ DNS Lookup: {host} ═══\r\n");
                sb.AppendLine($"ชื่อโฮสต์: {entry.HostName}");
                sb.AppendLine($"\r\nที่อยู่ IP ({entry.AddressList.Length} รายการ):");
                foreach (var addr in entry.AddressList)
                    sb.AppendLine($"  → {addr} ({addr.AddressFamily})");
                if (entry.Aliases.Length > 0)
                {
                    sb.AppendLine($"\r\nชื่ออ้างอิง:");
                    foreach (var alias in entry.Aliases)
                        sb.AppendLine($"  → {alias}");
                }
                txtDnsResult.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                txtDnsResult.Text = $"ข้อผิดพลาด: {ex.Message}";
            }
        }

        private async void BtnReverseDns_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = txtDnsHost.Text.Trim();
                txtDnsResult.Text = $"กำลังค้นหา Reverse DNS สำหรับ {ip}...\r\n";
                var entry = await Dns.GetHostEntryAsync(ip);
                var sb = new StringBuilder();
                sb.AppendLine($"═══ Reverse DNS: {ip} ═══\r\n");
                sb.AppendLine($"ชื่อโฮสต์: {entry.HostName}");
                foreach (var addr in entry.AddressList)
                    sb.AppendLine($"  → {addr}");
                txtDnsResult.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                txtDnsResult.Text = $"ข้อผิดพลาด: {ex.Message}";
            }
        }

        private async void BtnPing_Click(object sender, EventArgs e)
        {
            try
            {
                string host = txtPingHost.Text.Trim();
                txtPingResult.Text = $"กำลัง Ping {host}...\r\n\r\n";
                AddLog($"Ping: {host}", AccentBlue);

                var sb = new StringBuilder();
                sb.AppendLine($"═══ Ping {host} ═══\r\n");

                using var ping = new Ping();
                long totalMs = 0;
                int success = 0;

                for (int i = 1; i <= 4; i++)
                {
                    try
                    {
                        var reply = await ping.SendPingAsync(host, 3000);
                        if (reply.Status == IPPingStatus.Success)
                        {
                            sb.AppendLine($"  ตอบกลับจาก {reply.Address}: bytes={reply.Buffer.Length} time={reply.RoundtripTime}ms TTL={reply.Options?.Ttl}");
                            totalMs += reply.RoundtripTime;
                            success++;
                        }
                        else
                        {
                            sb.AppendLine($"  คำขอหมดเวลา ({reply.Status})");
                        }
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"  ข้อผิดพลาด: {ex.Message}");
                    }
                    txtPingResult.Text = sb.ToString();
                    await Task.Delay(500);
                }

                sb.AppendLine($"\r\n═══ สถิติ ═══");
                sb.AppendLine($"  ส่ง: 4, สำเร็จ: {success}, สูญหาย: {4 - success} ({(4 - success) * 25}%)");
                if (success > 0)
                    sb.AppendLine($"  เวลาเฉลี่ย: {totalMs / success}ms");
                txtPingResult.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                txtPingResult.Text = $"ข้อผิดพลาด: {ex.Message}";
            }
        }

        private async void BtnTracert_Click(object sender, EventArgs e)
        {
            try
            {
                string host = txtTracertHost.Text.Trim();
                txtTracertResult.Text = $"กำลัง Traceroute ไปยัง {host}...\r\n\r\n";
                AddLog($"Traceroute: {host}", AccentBlue);

                var sb = new StringBuilder();
                sb.AppendLine($"═══ Traceroute ไปยัง {host} ═══\r\n");

                using var ping = new Ping();
                for (int ttl = 1; ttl <= 30; ttl++)
                {
                    var options = new PingOptions(ttl, true);
                    var reply = await ping.SendPingAsync(host, 3000, new byte[32], options);

                    string addr = reply.Address?.ToString() ?? "*";
                    string hostname = "";
                    if (reply.Address != null)
                    {
                        try
                        {
                            var entry = await Dns.GetHostEntryAsync(reply.Address);
                            hostname = $" [{entry.HostName}]";
                        }
                        catch { }
                    }

                    if (reply.Status == IPPingStatus.Success || reply.Status == IPPingStatus.TtlExpired)
                        sb.AppendLine($"  {ttl,2}  {reply.RoundtripTime,5}ms  {addr}{hostname}");
                    else
                        sb.AppendLine($"  {ttl,2}  *  คำขอหมดเวลา");

                    txtTracertResult.Text = sb.ToString();

                    if (reply.Status == IPPingStatus.Success) break;
                }

                sb.AppendLine($"\r\nTraceroute เสร็จสิ้น");
                txtTracertResult.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                txtTracertResult.Text = $"ข้อผิดพลาด: {ex.Message}";
            }
        }

        private async void BtnWhois_Click(object sender, EventArgs e)
        {
            try
            {
                string host = txtWhoisHost.Text.Trim();
                txtWhoisResult.Text = $"กำลังค้นหา WHOIS สำหรับ {host}...\r\n";
                AddLog($"WHOIS Lookup: {host}", AccentBlue);

                using var client = new TcpClient();
                await client.ConnectAsync("whois.iana.org", 43);
                using var stream = client.GetStream();
                var query = Encoding.ASCII.GetBytes(host + "\r\n");
                await stream.WriteAsync(query);

                var sb = new StringBuilder();
                sb.AppendLine($"═══ WHOIS: {host} ═══\r\n");
                var buffer = new byte[4096];
                int read;
                while ((read = await stream.ReadAsync(buffer)) > 0)
                {
                    sb.Append(Encoding.ASCII.GetString(buffer, 0, read));
                }
                txtWhoisResult.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                txtWhoisResult.Text = $"ข้อผิดพลาด: {ex.Message}\r\n\r\nหมายเหตุ: WHOIS อาจถูกบล็อกโดยไฟร์วอลล์";
            }
        }

        private void BtnSaveLog_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = "Text Files|*.txt|All Files|*.*",
                FileName = $"NetMonitorPro_Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, logBox.Text);
                AddLog($"บันทึกไฟล์บันทึกไปที่: {sfd.FileName}", AccentColor);
            }
        }

        private void AddLog(string message, Color color)
        {
            if (logBox == null) return;
            if (logBox.InvokeRequired)
            {
                logBox.Invoke(() => AddLog(message, color));
                return;
            }
            logBox.SelectionStart = logBox.TextLength;
            logBox.SelectionLength = 0;
            logBox.SelectionColor = TextSecondary;
            logBox.AppendText($"[{DateTime.Now:HH:mm:ss}] ");
            logBox.SelectionColor = color;
            logBox.AppendText(message + "\n");
            logBox.ScrollToCaret();
        }

        private void SetStatus(string text)
        {
            if (statusLabel.InvokeRequired)
                statusLabel.Invoke(() => statusLabel.Text = text);
            else
                statusLabel.Text = text;
        }

        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private static string GetMacFromArp(string ip)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "arp",
                        Arguments = $"-a {ip}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                var lines = output.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains(ip))
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2) return parts[1].ToUpper();
                    }
                }
            }
            catch { }
            return "N/A";
        }

        private static string GuessOS(int ttl)
        {
            if (ttl <= 0) return "ไม่ทราบ";
            if (ttl <= 64) return "Linux/Unix/macOS";
            if (ttl <= 128) return "Windows";
            if (ttl <= 255) return "Cisco/Network Device";
            return "ไม่ทราบ";
        }

        private static string GetServiceName(int port)
        {
            return port switch
            {
                20 => "FTP Data",
                21 => "FTP",
                22 => "SSH",
                23 => "Telnet",
                25 => "SMTP",
                53 => "DNS",
                67 => "DHCP Server",
                68 => "DHCP Client",
                80 => "HTTP",
                110 => "POP3",
                119 => "NNTP",
                123 => "NTP",
                135 => "MSRPC",
                137 => "NetBIOS Name",
                138 => "NetBIOS Datagram",
                139 => "NetBIOS Session",
                143 => "IMAP",
                161 => "SNMP",
                162 => "SNMP Trap",
                389 => "LDAP",
                443 => "HTTPS",
                445 => "SMB",
                465 => "SMTPS",
                514 => "Syslog",
                587 => "SMTP (Submission)",
                636 => "LDAPS",
                993 => "IMAPS",
                995 => "POP3S",
                1080 => "SOCKS Proxy",
                1433 => "MSSQL",
                1434 => "MSSQL Browser",
                1521 => "Oracle DB",
                1723 => "PPTP",
                3306 => "MySQL",
                3389 => "RDP",
                5432 => "PostgreSQL",
                5900 => "VNC",
                5985 => "WinRM HTTP",
                5986 => "WinRM HTTPS",
                6379 => "Redis",
                8080 => "HTTP Proxy",
                8443 => "HTTPS Alt",
                9090 => "Web Admin",
                27017 => "MongoDB",
                _ => "ไม่ทราบ"
            };
        }

        private static async Task<string> TryGrabBanner(TcpClient client, int port)
        {
            try
            {
                client.ReceiveTimeout = 1000;
                client.SendTimeout = 1000;
                var stream = client.GetStream();

                if (port == 80 || port == 8080)
                {
                    var request = Encoding.ASCII.GetBytes("HEAD / HTTP/1.0\r\nHost: target\r\n\r\n");
                    await stream.WriteAsync(request);
                }

                if (stream.DataAvailable || port == 21 || port == 22 || port == 25 || port == 110)
                {
                    await Task.Delay(500);
                    if (stream.DataAvailable)
                    {
                        var buffer = new byte[512];
                        int read = await stream.ReadAsync(buffer);
                        string banner = Encoding.ASCII.GetString(buffer, 0, read).Trim();
                        return banner.Length > 80 ? banner.Substring(0, 80) + "..." : banner;
                    }
                }
            }
            catch { }
            return "-";
        }

    }
}
