using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoraWatermarkRemover
{
    public class MainForm : Form
    {
        private Panel headerPanel;
        private Panel sidePanel;
        private Panel mainPanel;
        private Panel statusPanel;
        private PictureBox previewBox;
        private PictureBox resultBox;
        private Label lblStatus;
        private Label lblFileInfo;
        private ProgressBar progressBar;
        private Button btnSelectFile;
        private Button btnSelectFolder;
        private Button btnProcess;
        private Button btnSave;
        private Button btnHowTo;
        private Button btnAbout;
        private ComboBox cmbMode;
        private ComboBox cmbPosition;
        private NumericUpDown nudWidth;
        private NumericUpDown nudHeight;
        private NumericUpDown nudMargin;
        private NumericUpDown nudBlend;
        private CheckBox chkAutoDetect;
        private ListBox lstFiles;
        private string currentFilePath = "";
        private string outputFilePath = "";
        private Bitmap? originalImage;
        private Bitmap? processedImage;
        private readonly Color accentColor = Color.FromArgb(99, 102, 241);
        private readonly Color darkBg = Color.FromArgb(15, 15, 25);
        private readonly Color cardBg = Color.FromArgb(24, 24, 40);
        private readonly Color surfaceBg = Color.FromArgb(32, 32, 52);
        private readonly Color textPrimary = Color.FromArgb(240, 240, 255);
        private readonly Color textSecondary = Color.FromArgb(160, 160, 185);
        private readonly Color successColor = Color.FromArgb(34, 197, 94);
        private readonly Color warningColor = Color.FromArgb(234, 179, 8);

        public MainForm()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Sora AI Watermark Remover v1.0 — พัฒนาโดย ป้อม";
            this.Size = new Size(1280, 820);
            this.MinimumSize = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = darkBg;
            this.ForeColor = textPrimary;
            this.DoubleBuffered = true;
            this.Font = new Font("Segoe UI", 9.5f);
        }

        private void SetupUI()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = cardBg,
                Padding = new Padding(20, 0, 20, 0)
            };
            headerPanel.Paint += (s, e) =>
            {
                using var brush = new LinearGradientBrush(headerPanel.ClientRectangle,
                    Color.FromArgb(30, 99, 102, 241), Color.FromArgb(30, 168, 85, 247),
                    LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(brush, headerPanel.ClientRectangle);

                using var pen = new Pen(Color.FromArgb(40, 99, 102, 241), 1);
                e.Graphics.DrawLine(pen, 0, headerPanel.Height - 1, headerPanel.Width, headerPanel.Height - 1);

                using var titleFont = new Font("Segoe UI", 16f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(textPrimary);
                e.Graphics.DrawString("🎬  Sora AI Watermark Remover", titleFont, titleBrush, 20, 16);

                using var subFont = new Font("Segoe UI", 9f);
                using var subBrush = new SolidBrush(textSecondary);
                e.Graphics.DrawString("v1.0  |  พัฒนาโดย ป้อม", subFont, subBrush, headerPanel.Width - 200, 22);
            };
            this.Controls.Add(headerPanel);

            sidePanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 320,
                BackColor = cardBg,
                Padding = new Padding(15, 10, 15, 10),
                AutoScroll = true
            };
            sidePanel.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(40, 99, 102, 241), 1);
                e.Graphics.DrawLine(pen, sidePanel.Width - 1, 0, sidePanel.Width - 1, sidePanel.Height);
            };
            this.Controls.Add(sidePanel);

            statusPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = cardBg,
                Padding = new Padding(15, 0, 15, 0)
            };
            this.Controls.Add(statusPanel);

            lblStatus = CreateLabel("พร้อมใช้งาน", 8.5f);
            lblStatus.ForeColor = successColor;
            lblStatus.Dock = DockStyle.Left;
            lblStatus.AutoSize = false;
            lblStatus.Width = 500;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            statusPanel.Controls.Add(lblStatus);

            progressBar = new ProgressBar
            {
                Dock = DockStyle.Right,
                Width = 250,
                Style = ProgressBarStyle.Continuous,
                Height = 18
            };
            statusPanel.Controls.Add(progressBar);

            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = darkBg,
                Padding = new Padding(15)
            };
            this.Controls.Add(mainPanel);

            SetupSidePanel();
            SetupMainPanel();

            this.Controls.SetChildIndex(headerPanel, 0);
            this.Controls.SetChildIndex(statusPanel, 1);
            this.Controls.SetChildIndex(sidePanel, 2);
            this.Controls.SetChildIndex(mainPanel, 3);
        }

        private void SetupSidePanel()
        {
            int y = 10;

            var lblSection1 = CreateSectionLabel("📂  เลือกไฟล์", ref y);
            sidePanel.Controls.Add(lblSection1);

            btnSelectFile = CreateStyledButton("เลือกไฟล์ภาพ / วิดีโอ", accentColor);
            btnSelectFile.Location = new Point(15, y);
            btnSelectFile.Size = new Size(275, 40);
            btnSelectFile.Click += BtnSelectFile_Click;
            sidePanel.Controls.Add(btnSelectFile);
            y += 50;

            btnSelectFolder = CreateStyledButton("เลือกโฟลเดอร์ (ทำหลายไฟล์)", surfaceBg);
            btnSelectFolder.Location = new Point(15, y);
            btnSelectFolder.Size = new Size(275, 40);
            btnSelectFolder.Click += BtnSelectFolder_Click;
            sidePanel.Controls.Add(btnSelectFolder);
            y += 50;

            lstFiles = new ListBox
            {
                Location = new Point(15, y),
                Size = new Size(275, 80),
                BackColor = surfaceBg,
                ForeColor = textPrimary,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 8.5f)
            };
            sidePanel.Controls.Add(lstFiles);
            y += 90;

            lblFileInfo = CreateLabel("ยังไม่ได้เลือกไฟล์", 8.5f);
            lblFileInfo.Location = new Point(15, y);
            lblFileInfo.Size = new Size(275, 20);
            lblFileInfo.ForeColor = textSecondary;
            sidePanel.Controls.Add(lblFileInfo);
            y += 30;

            var sep1 = CreateSeparator(ref y);
            sidePanel.Controls.Add(sep1);

            var lblSection2 = CreateSectionLabel("⚙️  ตั้งค่าการลบลายน้ำ", ref y);
            sidePanel.Controls.Add(lblSection2);

            var lblMode = CreateLabel("โหมดการลบ:", 9f);
            lblMode.Location = new Point(15, y);
            sidePanel.Controls.Add(lblMode);
            y += 22;

            cmbMode = new ComboBox
            {
                Location = new Point(15, y),
                Size = new Size(275, 30),
                BackColor = surfaceBg,
                ForeColor = textPrimary,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f)
            };
            cmbMode.Items.AddRange(new object[]
            {
                "Inpaint (เติมสีอัตโนมัติ)",
                "Crop (ตัดขอบภาพ)",
                "Blur (เบลอลายน้ำ)",
                "Pixel Fill (เติมพิกเซลใกล้เคียง)"
            });
            cmbMode.SelectedIndex = 0;
            sidePanel.Controls.Add(cmbMode);
            y += 38;

            var lblPos = CreateLabel("ตำแหน่งลายน้ำ:", 9f);
            lblPos.Location = new Point(15, y);
            sidePanel.Controls.Add(lblPos);
            y += 22;

            cmbPosition = new ComboBox
            {
                Location = new Point(15, y),
                Size = new Size(275, 30),
                BackColor = surfaceBg,
                ForeColor = textPrimary,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f)
            };
            cmbPosition.Items.AddRange(new object[]
            {
                "ขวาล่าง (ค่าเริ่มต้น Sora)",
                "ซ้ายล่าง",
                "ขวาบน",
                "ซ้ายบน",
                "กึ่งกลางล่าง"
            });
            cmbPosition.SelectedIndex = 0;
            sidePanel.Controls.Add(cmbPosition);
            y += 38;

            chkAutoDetect = new CheckBox
            {
                Text = "ตรวจจับลายน้ำอัตโนมัติ",
                Location = new Point(15, y),
                Size = new Size(275, 25),
                ForeColor = textPrimary,
                Checked = true,
                Font = new Font("Segoe UI", 9f)
            };
            sidePanel.Controls.Add(chkAutoDetect);
            y += 32;

            var lblWidth = CreateLabel("ความกว้างลายน้ำ (px):", 8.5f);
            lblWidth.Location = new Point(15, y);
            sidePanel.Controls.Add(lblWidth);

            nudWidth = CreateNumericUpDown(180, 15, y + 20, 120);
            sidePanel.Controls.Add(nudWidth);

            var lblHeight = CreateLabel("ความสูง (px):", 8.5f);
            lblHeight.Location = new Point(155, y);
            sidePanel.Controls.Add(lblHeight);

            nudHeight = CreateNumericUpDown(50, 155, y + 20, 120);
            sidePanel.Controls.Add(nudHeight);
            y += 52;

            var lblMargin = CreateLabel("ระยะขอบ (px):", 8.5f);
            lblMargin.Location = new Point(15, y);
            sidePanel.Controls.Add(lblMargin);

            nudMargin = CreateNumericUpDown(10, 15, y + 20, 120);
            nudMargin.Maximum = 100;
            sidePanel.Controls.Add(nudMargin);

            var lblBlend = CreateLabel("ความเนียน (%):", 8.5f);
            lblBlend.Location = new Point(155, y);
            sidePanel.Controls.Add(lblBlend);

            nudBlend = CreateNumericUpDown(85, 155, y + 20, 120);
            nudBlend.Maximum = 100;
            sidePanel.Controls.Add(nudBlend);
            y += 55;

            var sep2 = CreateSeparator(ref y);
            sidePanel.Controls.Add(sep2);

            btnProcess = CreateStyledButton("▶  เริ่มลบลายน้ำ", Color.FromArgb(34, 197, 94));
            btnProcess.Location = new Point(15, y);
            btnProcess.Size = new Size(275, 45);
            btnProcess.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            btnProcess.Click += BtnProcess_Click;
            sidePanel.Controls.Add(btnProcess);
            y += 55;

            btnSave = CreateStyledButton("💾  บันทึกไฟล์", surfaceBg);
            btnSave.Location = new Point(15, y);
            btnSave.Size = new Size(133, 38);
            btnSave.Enabled = false;
            btnSave.Click += BtnSave_Click;
            sidePanel.Controls.Add(btnSave);

            btnHowTo = CreateStyledButton("📖  วิธีใช้", surfaceBg);
            btnHowTo.Location = new Point(157, y);
            btnHowTo.Size = new Size(133, 38);
            btnHowTo.Click += BtnHowTo_Click;
            sidePanel.Controls.Add(btnHowTo);
            y += 48;

            btnAbout = CreateStyledButton("ℹ️  เกี่ยวกับ", surfaceBg);
            btnAbout.Location = new Point(15, y);
            btnAbout.Size = new Size(275, 35);
            btnAbout.Click += BtnAbout_Click;
            sidePanel.Controls.Add(btnAbout);
        }

        private void SetupMainPanel()
        {
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                BackColor = darkBg,
                SplitterDistance = 450,
                SplitterWidth = 6
            };
            mainPanel.Controls.Add(splitContainer);

            var lblBefore = CreateLabel("📷  ภาพต้นฉบับ", 10f);
            lblBefore.Dock = DockStyle.Top;
            lblBefore.Height = 30;
            lblBefore.ForeColor = textSecondary;
            splitContainer.Panel1.Controls.Add(lblBefore);

            previewBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = surfaceBg,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None
            };
            previewBox.Paint += PreviewBox_Paint;
            splitContainer.Panel1.Controls.Add(previewBox);
            splitContainer.Panel1.Controls.SetChildIndex(previewBox, 1);

            var lblAfter = CreateLabel("✨  ผลลัพธ์", 10f);
            lblAfter.Dock = DockStyle.Top;
            lblAfter.Height = 30;
            lblAfter.ForeColor = textSecondary;
            splitContainer.Panel2.Controls.Add(lblAfter);

            resultBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = surfaceBg,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None
            };
            resultBox.Paint += ResultBox_Paint;
            splitContainer.Panel2.Controls.Add(resultBox);
            splitContainer.Panel2.Controls.SetChildIndex(resultBox, 1);
        }

        private void PreviewBox_Paint(object? sender, PaintEventArgs e)
        {
            if (previewBox.Image == null)
            {
                DrawPlaceholder(e.Graphics, previewBox, "ลากไฟล์มาวางที่นี่\nหรือกดปุ่ม \"เลือกไฟล์\"");
            }
        }

        private void ResultBox_Paint(object? sender, PaintEventArgs e)
        {
            if (resultBox.Image == null)
            {
                DrawPlaceholder(e.Graphics, resultBox, "ผลลัพธ์จะแสดงที่นี่\nหลังจากลบลายน้ำเสร็จ");
            }
        }

        private void DrawPlaceholder(Graphics g, PictureBox box, string text)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var font = new Font("Segoe UI", 12f);
            using var brush = new SolidBrush(Color.FromArgb(80, 160, 160, 185));
            var size = g.MeasureString(text, font);
            g.DrawString(text, font, brush,
                (box.Width - size.Width) / 2,
                (box.Height - size.Height) / 2);
        }

        private void BtnSelectFile_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "เลือกไฟล์ภาพหรือวิดีโอ",
                Filter = "ไฟล์ที่รองรับ|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.webp;*.mp4;*.avi;*.mkv;*.mov;*.wmv|" +
                         "ไฟล์ภาพ|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.webp|" +
                         "ไฟล์วิดีโอ|*.mp4;*.avi;*.mkv;*.mov;*.wmv|" +
                         "ไฟล์ทั้งหมด|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadFile(ofd.FileName);
            }
        }

        private void BtnSelectFolder_Click(object? sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog
            {
                Description = "เลือกโฟลเดอร์ที่มีไฟล์ภาพ/วิดีโอ"
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                lstFiles.Items.Clear();
                var extensions = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.mp4", "*.avi", "*.mkv", "*.mov" };
                foreach (var ext in extensions)
                {
                    foreach (var file in Directory.GetFiles(fbd.SelectedPath, ext))
                    {
                        lstFiles.Items.Add(file);
                    }
                }
                lblFileInfo.Text = $"พบ {lstFiles.Items.Count} ไฟล์";
                UpdateStatus($"โหลดโฟลเดอร์: {lstFiles.Items.Count} ไฟล์", successColor);

                if (lstFiles.Items.Count > 0)
                {
                    lstFiles.SelectedIndex = 0;
                    LoadFile(lstFiles.Items[0].ToString()!);
                }
            }
        }

        private void LoadFile(string path)
        {
            currentFilePath = path;
            lstFiles.Items.Clear();
            lstFiles.Items.Add(Path.GetFileName(path));

            var ext = Path.GetExtension(path).ToLower();
            bool isVideo = ext is ".mp4" or ".avi" or ".mkv" or ".mov" or ".wmv";

            if (isVideo)
            {
                previewBox.Image = null;
                originalImage?.Dispose();
                originalImage = null;
                var fi = new FileInfo(path);
                lblFileInfo.Text = $"วิดีโอ: {fi.Name} ({fi.Length / 1024 / 1024}MB)";
                UpdateStatus("โหลดวิดีโอเรียบร้อย — กดเริ่มลบลายน้ำ (ต้องมี FFmpeg)", warningColor);
            }
            else
            {
                try
                {
                    originalImage?.Dispose();
                    originalImage = new Bitmap(path);
                    previewBox.Image = originalImage;
                    lblFileInfo.Text = $"ภาพ: {originalImage.Width}x{originalImage.Height} px";
                    UpdateStatus("โหลดภาพเรียบร้อย — กดเริ่มลบลายน้ำ", successColor);
                }
                catch (Exception ex)
                {
                    UpdateStatus($"ผิดพลาด: {ex.Message}", Color.Red);
                }
            }

            processedImage?.Dispose();
            processedImage = null;
            resultBox.Image = null;
            btnSave.Enabled = false;
        }

        private async void BtnProcess_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                MessageBox.Show("กรุณาเลือกไฟล์ก่อน!", "แจ้งเตือน",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnProcess.Enabled = false;
            progressBar.Value = 0;
            UpdateStatus("กำลังประมวลผล...", warningColor);

            var ext = Path.GetExtension(currentFilePath).ToLower();
            bool isVideo = ext is ".mp4" or ".avi" or ".mkv" or ".mov" or ".wmv";

            try
            {
                if (isVideo)
                {
                    await ProcessVideoAsync(currentFilePath);
                }
                else
                {
                    await Task.Run(() => ProcessImage());
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"ผิดพลาด: {ex.Message}", Color.Red);
            }
            finally
            {
                btnProcess.Enabled = true;
            }
        }

        private void ProcessImage()
        {
            if (originalImage == null) return;

            var src = (Bitmap)originalImage.Clone();
            int mode = 0;
            int posIndex = 0;
            int wmWidth = 180, wmHeight = 50, margin = 10, blend = 85;

            this.Invoke(() =>
            {
                mode = cmbMode.SelectedIndex;
                posIndex = cmbPosition.SelectedIndex;
                wmWidth = (int)nudWidth.Value;
                wmHeight = (int)nudHeight.Value;
                margin = (int)nudMargin.Value;
                blend = (int)nudBlend.Value;
            });

            this.Invoke(() => { progressBar.Value = 20; });

            var wmRect = GetWatermarkRect(src.Width, src.Height, wmWidth, wmHeight, margin, posIndex);

            this.Invoke(() => { progressBar.Value = 40; });

            Bitmap result;
            switch (mode)
            {
                case 0:
                    result = InpaintRemove(src, wmRect, blend);
                    break;
                case 1:
                    result = CropRemove(src, wmRect, posIndex);
                    break;
                case 2:
                    result = BlurRemove(src, wmRect, blend);
                    break;
                case 3:
                    result = PixelFillRemove(src, wmRect, blend);
                    break;
                default:
                    result = InpaintRemove(src, wmRect, blend);
                    break;
            }

            this.Invoke(() => { progressBar.Value = 90; });

            processedImage?.Dispose();
            processedImage = result;

            this.Invoke(() =>
            {
                resultBox.Image = processedImage;
                progressBar.Value = 100;
                btnSave.Enabled = true;
                UpdateStatus("ลบลายน้ำเสร็จสิ้น! กดบันทึกเพื่อเซฟไฟล์", successColor);
            });
        }

        private Rectangle GetWatermarkRect(int imgW, int imgH, int wmW, int wmH, int margin, int posIndex)
        {
            return posIndex switch
            {
                0 => new Rectangle(imgW - wmW - margin, imgH - wmH - margin, wmW, wmH),
                1 => new Rectangle(margin, imgH - wmH - margin, wmW, wmH),
                2 => new Rectangle(imgW - wmW - margin, margin, wmW, wmH),
                3 => new Rectangle(margin, margin, wmW, wmH),
                4 => new Rectangle((imgW - wmW) / 2, imgH - wmH - margin, wmW, wmH),
                _ => new Rectangle(imgW - wmW - margin, imgH - wmH - margin, wmW, wmH)
            };
        }

        private Bitmap InpaintRemove(Bitmap src, Rectangle rect, int blendPercent)
        {
            var result = (Bitmap)src.Clone();
            float blend = blendPercent / 100f;

            int sampleY = rect.Top - 5;
            if (sampleY < 0) sampleY = rect.Bottom + 5;
            sampleY = Math.Clamp(sampleY, 0, src.Height - 1);

            for (int y = rect.Top; y < rect.Bottom && y < result.Height; y++)
            {
                for (int x = rect.Left; x < rect.Right && x < result.Width; x++)
                {
                    if (x < 0 || y < 0) continue;

                    int srcX = x;
                    int srcY = sampleY;

                    int leftDist = x - rect.Left;
                    int rightDist = rect.Right - x;
                    int topDist = y - rect.Top;
                    int bottomDist = rect.Bottom - y;
                    int minDist = Math.Min(Math.Min(leftDist, rightDist), Math.Min(topDist, bottomDist));

                    float edgeFactor = 1.0f;
                    int edgeZone = 8;
                    if (minDist < edgeZone)
                    {
                        edgeFactor = (float)minDist / edgeZone;
                    }

                    Color sampleColor;
                    if (rect.Left > 10 && rect.Right < result.Width - 10)
                    {
                        Color leftColor = result.GetPixel(rect.Left - 3, Math.Clamp(y, 0, result.Height - 1));
                        Color rightColor = result.GetPixel(Math.Min(rect.Right + 3, result.Width - 1), Math.Clamp(y, 0, result.Height - 1));
                        float t = (float)(x - rect.Left) / rect.Width;
                        sampleColor = Color.FromArgb(
                            (int)(leftColor.R * (1 - t) + rightColor.R * t),
                            (int)(leftColor.G * (1 - t) + rightColor.G * t),
                            (int)(leftColor.B * (1 - t) + rightColor.B * t)
                        );
                    }
                    else
                    {
                        sampleColor = result.GetPixel(Math.Clamp(srcX, 0, result.Width - 1), Math.Clamp(srcY, 0, result.Height - 1));
                    }

                    Color origColor = result.GetPixel(x, y);
                    float f = blend * edgeFactor;
                    Color finalColor = Color.FromArgb(
                        (int)(origColor.R * (1 - f) + sampleColor.R * f),
                        (int)(origColor.G * (1 - f) + sampleColor.G * f),
                        (int)(origColor.B * (1 - f) + sampleColor.B * f)
                    );

                    result.SetPixel(x, y, finalColor);
                }
            }

            return result;
        }

        private Bitmap CropRemove(Bitmap src, Rectangle wmRect, int posIndex)
        {
            int cropSize = Math.Max(wmRect.Width, wmRect.Height) + 10;
            int newW = src.Width;
            int newH = src.Height;
            int srcX = 0, srcY = 0;

            switch (posIndex)
            {
                case 0:
                case 1:
                case 4:
                    newH = src.Height - wmRect.Height - 10;
                    break;
                case 2:
                case 3:
                    newH = src.Height - wmRect.Height - 10;
                    srcY = wmRect.Height + 10;
                    break;
            }

            newW = Math.Max(newW, 1);
            newH = Math.Max(newH, 1);

            var result = new Bitmap(newW, newH);
            using (var g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(src, new Rectangle(0, 0, newW, newH),
                    new Rectangle(srcX, srcY, newW, newH), GraphicsUnit.Pixel);
            }
            return result;
        }

        private Bitmap BlurRemove(Bitmap src, Rectangle rect, int blendPercent)
        {
            var result = (Bitmap)src.Clone();
            int radius = Math.Max(5, blendPercent / 5);

            for (int y = rect.Top; y < rect.Bottom && y < result.Height; y++)
            {
                for (int x = rect.Left; x < rect.Right && x < result.Width; x++)
                {
                    if (x < 0 || y < 0) continue;

                    int r = 0, g = 0, b = 0, count = 0;
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        for (int dx = -radius; dx <= radius; dx++)
                        {
                            int nx = Math.Clamp(x + dx, 0, result.Width - 1);
                            int ny = Math.Clamp(y + dy, 0, result.Height - 1);
                            if (nx >= rect.Left && nx < rect.Right && ny >= rect.Top && ny < rect.Bottom)
                                continue;
                            Color c = result.GetPixel(nx, ny);
                            r += c.R; g += c.G; b += c.B; count++;
                        }
                    }

                    if (count > 0)
                    {
                        float blend = blendPercent / 100f;
                        Color orig = result.GetPixel(x, y);
                        Color avg = Color.FromArgb(r / count, g / count, b / count);
                        result.SetPixel(x, y, Color.FromArgb(
                            (int)(orig.R * (1 - blend) + avg.R * blend),
                            (int)(orig.G * (1 - blend) + avg.G * blend),
                            (int)(orig.B * (1 - blend) + avg.B * blend)
                        ));
                    }
                }
            }
            return result;
        }

        private Bitmap PixelFillRemove(Bitmap src, Rectangle rect, int blendPercent)
        {
            var result = (Bitmap)src.Clone();
            float blend = blendPercent / 100f;

            for (int y = rect.Top; y < rect.Bottom && y < result.Height; y++)
            {
                for (int x = rect.Left; x < rect.Right && x < result.Width; x++)
                {
                    if (x < 0 || y < 0) continue;

                    int nearX = (x < rect.Left + rect.Width / 2) ?
                        Math.Max(rect.Left - 3, 0) : Math.Min(rect.Right + 3, result.Width - 1);
                    int nearY = Math.Clamp(y, 0, result.Height - 1);

                    Color nearColor = result.GetPixel(nearX, nearY);
                    Color origColor = result.GetPixel(x, y);

                    result.SetPixel(x, y, Color.FromArgb(
                        (int)(origColor.R * (1 - blend) + nearColor.R * blend),
                        (int)(origColor.G * (1 - blend) + nearColor.G * blend),
                        (int)(origColor.B * (1 - blend) + nearColor.B * blend)
                    ));
                }
            }
            return result;
        }

        private async Task ProcessVideoAsync(string inputPath)
        {
            string ffmpegPath = FindFFmpeg();
            if (string.IsNullOrEmpty(ffmpegPath))
            {
                MessageBox.Show(
                    "ไม่พบ FFmpeg!\n\nกรุณาดาวน์โหลด FFmpeg แล้ววางไว้ในโฟลเดอร์เดียวกับโปรแกรม\nหรือติดตั้ง FFmpeg ลงในระบบ\n\nดาวน์โหลดได้ที่: https://ffmpeg.org/download.html",
                    "ต้องการ FFmpeg",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int posIndex = cmbPosition.SelectedIndex;
            int wmWidth = (int)nudWidth.Value;
            int wmHeight = (int)nudHeight.Value;
            int margin = (int)nudMargin.Value;
            int mode = cmbMode.SelectedIndex;

            string outputDir = Path.GetDirectoryName(inputPath)!;
            string outputName = Path.GetFileNameWithoutExtension(inputPath) + "_no_watermark" + Path.GetExtension(inputPath);
            outputFilePath = Path.Combine(outputDir, outputName);

            string filterArgs = "";
            if (mode == 1)
            {
                string cropFilter = posIndex switch
                {
                    0 or 1 or 4 => $"crop=iw:ih-{wmHeight + margin}:0:0",
                    2 or 3 => $"crop=iw:ih-{wmHeight + margin}:0:{wmHeight + margin}",
                    _ => $"crop=iw:ih-{wmHeight + margin}:0:0"
                };
                filterArgs = $"-vf \"{cropFilter}\"";
            }
            else
            {
                string delogoX, delogoY;
                switch (posIndex)
                {
                    case 0:
                        delogoX = $"iw-{wmWidth + margin}";
                        delogoY = $"ih-{wmHeight + margin}";
                        break;
                    case 1:
                        delogoX = $"{margin}";
                        delogoY = $"ih-{wmHeight + margin}";
                        break;
                    case 2:
                        delogoX = $"iw-{wmWidth + margin}";
                        delogoY = $"{margin}";
                        break;
                    case 3:
                        delogoX = $"{margin}";
                        delogoY = $"{margin}";
                        break;
                    case 4:
                        delogoX = $"(iw-{wmWidth})/2";
                        delogoY = $"ih-{wmHeight + margin}";
                        break;
                    default:
                        delogoX = $"iw-{wmWidth + margin}";
                        delogoY = $"ih-{wmHeight + margin}";
                        break;
                }
                filterArgs = $"-vf \"delogo=x={delogoX}:y={delogoY}:w={wmWidth}:h={wmHeight}:show=0\"";
            }

            string args = $"-i \"{inputPath}\" {filterArgs} -c:a copy -y \"{outputFilePath}\"";

            UpdateStatus("กำลังประมวลผลวิดีโอ (อาจใช้เวลาสักครู่)...", warningColor);
            progressBar.Style = ProgressBarStyle.Marquee;

            await Task.Run(() =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                process?.WaitForExit();
            });

            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 100;

            if (File.Exists(outputFilePath))
            {
                btnSave.Enabled = true;
                UpdateStatus($"ลบลายน้ำวิดีโอเสร็จสิ้น! บันทึกที่: {outputFilePath}", successColor);
                MessageBox.Show($"ลบลายน้ำวิดีโอเสร็จแล้ว!\n\nบันทึกไฟล์ที่:\n{outputFilePath}",
                    "สำเร็จ!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                UpdateStatus("ผิดพลาดในการประมวลผลวิดีโอ", Color.Red);
            }
        }

        private string FindFFmpeg()
        {
            string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
            if (File.Exists(localPath)) return localPath;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "where",
                    Arguments = "ffmpeg",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi);
                string output = p!.StandardOutput.ReadToEnd().Trim();
                p.WaitForExit();
                if (!string.IsNullOrEmpty(output)) return output.Split('\n')[0].Trim();
            }
            catch { }

            string[] commonPaths = {
                @"C:\ffmpeg\bin\ffmpeg.exe",
                @"C:\Program Files\ffmpeg\bin\ffmpeg.exe",
                @"C:\Tools\ffmpeg.exe",
                Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\ffmpeg\bin\ffmpeg.exe")
            };

            foreach (var p in commonPaths)
            {
                if (File.Exists(p)) return p;
            }

            return "";
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (processedImage != null)
            {
                using var sfd = new SaveFileDialog
                {
                    Title = "บันทึกภาพที่ลบลายน้ำแล้ว",
                    Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp",
                    FileName = Path.GetFileNameWithoutExtension(currentFilePath) + "_no_watermark"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var format = Path.GetExtension(sfd.FileName).ToLower() switch
                    {
                        ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                        ".bmp" => ImageFormat.Bmp,
                        _ => ImageFormat.Png
                    };
                    processedImage.Save(sfd.FileName, format);
                    UpdateStatus($"บันทึกไฟล์เรียบร้อย: {sfd.FileName}", successColor);
                }
            }
            else if (!string.IsNullOrEmpty(outputFilePath) && File.Exists(outputFilePath))
            {
                MessageBox.Show($"วิดีโอถูกบันทึกอัตโนมัติที่:\n{outputFilePath}",
                    "ข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnHowTo_Click(object? sender, EventArgs e)
        {
            string howto = @"╔══════════════════════════════════════════════════╗
║        📖 วิธีใช้งาน Sora AI Watermark Remover        ║
╚══════════════════════════════════════════════════╝

📌 ขั้นตอนที่ 1: เลือกไฟล์
   • กดปุ่ม ""เลือกไฟล์ภาพ / วิดีโอ"" เพื่อเลือกไฟล์ที่ต้องการ
   • รองรับไฟล์ภาพ: PNG, JPG, BMP, GIF, WebP
   • รองรับไฟล์วิดีโอ: MP4, AVI, MKV, MOV, WMV
   • หรือกด ""เลือกโฟลเดอร์"" เพื่อทำหลายไฟล์พร้อมกัน

📌 ขั้นตอนที่ 2: ตั้งค่า
   • เลือกโหมดการลบ:
     - Inpaint: เติมสีอัตโนมัติ (แนะนำ) ✨
     - Crop: ตัดขอบภาพออก
     - Blur: เบลอบริเวณลายน้ำ
     - Pixel Fill: เติมพิกเซลใกล้เคียง
   • เลือกตำแหน่งลายน้ำ (ค่าเริ่มต้น: ขวาล่าง)
   • ปรับขนาดบริเวณลายน้ำ (กว้าง/สูง)
   • ปรับความเนียนของการลบ

📌 ขั้นตอนที่ 3: ลบลายน้ำ
   • กดปุ่ม ""▶ เริ่มลบลายน้ำ""
   • รอให้โปรแกรมประมวลผล

📌 ขั้นตอนที่ 4: บันทึกไฟล์
   • กดปุ่ม ""💾 บันทึกไฟล์""
   • เลือกตำแหน่งและรูปแบบไฟล์ที่ต้องการ

⚠️ หมายเหตุสำหรับวิดีโอ:
   • ต้องมี FFmpeg ติดตั้งในเครื่อง
   • ดาวน์โหลดได้ที่: https://ffmpeg.org/download.html
   • วาง ffmpeg.exe ไว้ในโฟลเดอร์เดียวกับโปรแกรม

💡 เคล็ดลับ:
   • ลายน้ำ Sora AI มักอยู่ที่มุมขวาล่าง
   • ลองปรับขนาดลายน้ำให้พอดีเพื่อผลลัพธ์ที่ดี
   • โหมด Inpaint จะให้ผลลัพธ์ที่เนียนที่สุด";

            MessageBox.Show(howto, "วิธีใช้งาน — Sora AI Watermark Remover",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnAbout_Click(object? sender, EventArgs e)
        {
            string about = @"╔══════════════════════════════════════════╗
║   🎬 Sora AI Watermark Remover v1.0     ║
╚══════════════════════════════════════════╝

👨‍💻 พัฒนาโดย: ป้อม
📧 ผู้สร้าง: ป้อม (Pom Developer)

🛠️ เทคโนโลยี: C# .NET 8 | Windows Forms
📦 เวอร์ชัน: 1.0.0

📝 รายละเอียด:
โปรแกรมลบลายน้ำ Sora AI สำหรับไฟล์ภาพและวิดีโอ
รองรับหลายโหมดการลบลายน้ำ พร้อม GUI ภาษาไทย

⚖️ ข้อจำกัดความรับผิดชอบ:
โปรแกรมนี้จัดทำเพื่อการศึกษาเท่านั้น
กรุณาเคารพลิขสิทธิ์และข้อตกลงการใช้งาน
ของผู้ให้บริการ AI ที่เกี่ยวข้อง

© 2025 ป้อม — สงวนลิขสิทธิ์";

            MessageBox.Show(about, "เกี่ยวกับโปรแกรม",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateStatus(string text, Color color)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(() => UpdateStatus(text, color));
                return;
            }
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }

        private Label CreateLabel(string text, float fontSize)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = textPrimary,
                Font = new Font("Segoe UI", fontSize)
            };
        }

        private Label CreateSectionLabel(string text, ref int y)
        {
            var lbl = new Label
            {
                Text = text,
                Location = new Point(15, y),
                AutoSize = true,
                ForeColor = accentColor,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold)
            };
            y += 28;
            return lbl;
        }

        private Panel CreateSeparator(ref int y)
        {
            var sep = new Panel
            {
                Location = new Point(15, y),
                Size = new Size(275, 1),
                BackColor = Color.FromArgb(40, 99, 102, 241)
            };
            y += 12;
            return sep;
        }

        private Button CreateStyledButton(string text, Color bgColor)
        {
            var btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = bgColor,
                ForeColor = textPrimary,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                Math.Min(bgColor.R + 20, 255),
                Math.Min(bgColor.G + 20, 255),
                Math.Min(bgColor.B + 20, 255));
            return btn;
        }

        private NumericUpDown CreateNumericUpDown(int value, int x, int y, int width)
        {
            return new NumericUpDown
            {
                Location = new Point(x, y),
                Size = new Size(width, 28),
                BackColor = surfaceBg,
                ForeColor = textPrimary,
                Minimum = 10,
                Maximum = 2000,
                Value = value,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9.5f)
            };
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            originalImage?.Dispose();
            processedImage?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
