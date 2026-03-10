using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThaiSongGenerator
{
    public class MainForm : Form
    {
        private TextBox txtLyrics = null!;
        private ComboBox cmbStyle = null!;
        private ComboBox cmbReverb = null!;
        private ComboBox cmbEQ = null!;
        private Button btnGenerate = null!;
        private Button btnHelp = null!;
        private ProgressBar progressBar = null!;
        private Label lblStatus = null!;
        private Label lblTitle = null!;
        private Label lblLyrics = null!;
        private Label lblStyle = null!;
        private Label lblReverb = null!;
        private Label lblEQ = null!;
        private GroupBox grpEffects = null!;
        private GroupBox grpLyrics = null!;
        private Panel panelHeader = null!;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "Thai Song Generator - สร้างเพลงด้วยเนื้อเพลงภาษาไทย";
            this.Size = new Size(780, 750);
            this.MinimumSize = new Size(700, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(25, 25, 35);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10F);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(45, 20, 80)
            };

            lblTitle = new Label
            {
                Text = "🎵  สร้างเพลงด้วยเนื้อเพลงภาษาไทย  🎵",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 160, 255),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            panelHeader.Controls.Add(lblTitle);

            grpLyrics = new GroupBox
            {
                Text = "เนื้อเพลง (Lyrics)",
                Location = new Point(20, 85),
                Size = new Size(725, 240),
                ForeColor = Color.FromArgb(180, 140, 255),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            txtLyrics = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(15, 25),
                Size = new Size(695, 195),
                BackColor = Color.FromArgb(35, 35, 50),
                ForeColor = Color.FromArgb(220, 220, 240),
                Font = new Font("Tahoma", 12F),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "ฉันคิดถึงเธอทุกวัน\r\nเมื่อเธอจากไป\r\nหัวใจฉันเจ็บปวด\r\nน้ำตาไหลริน"
            };
            grpLyrics.Controls.Add(txtLyrics);

            grpEffects = new GroupBox
            {
                Text = "ตั้งค่าสไตล์และเอฟเฟกต์",
                Location = new Point(20, 340),
                Size = new Size(725, 200),
                ForeColor = Color.FromArgb(180, 140, 255),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            lblStyle = new Label
            {
                Text = "สไตล์เพลง:",
                Location = new Point(15, 35),
                Size = new Size(140, 28),
                ForeColor = Color.FromArgb(200, 200, 220),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight
            };

            cmbStyle = new ComboBox
            {
                Location = new Point(165, 35),
                Size = new Size(530, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(35, 35, 50),
                ForeColor = Color.FromArgb(220, 220, 240),
                Font = new Font("Segoe UI", 10F),
                FlatStyle = FlatStyle.Flat
            };
            cmbStyle.Items.AddRange(new object[]
            {
                "🎹 Thai Sounds & Clear Thai Letters - เสียงไทยชัดเจน",
                "💔 Slow Sad Heartbreak Song - เพลงรักเศร้าช้าๆ",
                "🎤 Thai Pop - ป๊อปไทย",
                "🎶 Thai Ballad Clear - เพลงบัลลาดไทยเสียงใส"
            });
            cmbStyle.SelectedIndex = 1;

            lblReverb = new Label
            {
                Text = "Reverb:",
                Location = new Point(15, 80),
                Size = new Size(140, 28),
                ForeColor = Color.FromArgb(200, 200, 220),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight
            };

            cmbReverb = new ComboBox
            {
                Location = new Point(165, 80),
                Size = new Size(530, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(35, 35, 50),
                ForeColor = Color.FromArgb(220, 220, 240),
                Font = new Font("Segoe UI", 10F),
                FlatStyle = FlatStyle.Flat
            };
            cmbReverb.Items.AddRange(new object[]
            {
                "Studio - สตูดิโอ",
                "Warm - อบอุ่น",
                "Talented - พรสวรรค์",
                "Dazzle - เจิดจรัส",
                "Distant - ห่างไกล",
                "KTV - คาราโอเกะ",
                "Corridor - ทางเดิน",
                "Natural - ธรรมชาติ",
                "CD - ซีดี",
                "Church - โบสถ์",
                "Concert - คอนเสิร์ต",
                "Stereo - สเตอริโอ",
                "Theater - โรงละคร",
                "Phonograph - แผ่นเสียง",
                "Fantasy - แฟนตาซี"
            });
            cmbReverb.SelectedIndex = 0;

            lblEQ = new Label
            {
                Text = "Equalizer:",
                Location = new Point(15, 125),
                Size = new Size(140, 28),
                ForeColor = Color.FromArgb(200, 200, 220),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight
            };

            cmbEQ = new ComboBox
            {
                Location = new Point(165, 125),
                Size = new Size(530, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(35, 35, 50),
                ForeColor = Color.FromArgb(220, 220, 240),
                Font = new Font("Segoe UI", 10F),
                FlatStyle = FlatStyle.Flat
            };
            cmbEQ.Items.AddRange(new object[]
            {
                "Standard Sound - เสียงมาตรฐาน",
                "Deep Lows - เสียงทุ้มลึก",
                "Crispy Sound - เสียงกรอบคม",
                "Clear Sound - เสียงใส"
            });
            cmbEQ.SelectedIndex = 0;

            grpEffects.Controls.AddRange(new Control[] { lblStyle, cmbStyle, lblReverb, cmbReverb, lblEQ, cmbEQ });

            progressBar = new ProgressBar
            {
                Location = new Point(20, 560),
                Size = new Size(725, 25),
                Style = ProgressBarStyle.Continuous,
                BackColor = Color.FromArgb(35, 35, 50)
            };

            lblStatus = new Label
            {
                Text = "สถานะ: พร้อมใช้งาน ✓",
                Location = new Point(20, 590),
                Size = new Size(725, 28),
                ForeColor = Color.FromArgb(100, 200, 100),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnGenerate = new Button
            {
                Text = "🎵  สร้างเพลง & ส่งออก MP3",
                Location = new Point(20, 625),
                Size = new Size(540, 55),
                BackColor = Color.FromArgb(100, 50, 180),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.FlatAppearance.MouseOverBackColor = Color.FromArgb(130, 70, 210);
            btnGenerate.Click += BtnGenerate_Click;

            btnHelp = new Button
            {
                Text = "📖 วิธีใช้งาน",
                Location = new Point(575, 625),
                Size = new Size(170, 55),
                BackColor = Color.FromArgb(50, 50, 80),
                ForeColor = Color.FromArgb(180, 180, 220),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnHelp.FlatAppearance.BorderSize = 1;
            btnHelp.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 150);
            btnHelp.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 110);
            btnHelp.Click += BtnHelp_Click;

            this.Controls.AddRange(new Control[]
            {
                panelHeader, grpLyrics, grpEffects,
                progressBar, lblStatus, btnGenerate, btnHelp
            });

            this.ResumeLayout(false);
        }

        private async void BtnGenerate_Click(object? sender, EventArgs e)
        {
            string lyrics = txtLyrics.Text.Trim();
            if (string.IsNullOrEmpty(lyrics))
            {
                MessageBox.Show(
                    "กรุณาพิมพ์เนื้อเพลงก่อนสร้างเพลง",
                    "แจ้งเตือน",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "บันทึกไฟล์เพลง MP3",
                Filter = "MP3 Files (*.mp3)|*.mp3|WAV Files (*.wav)|*.wav",
                DefaultExt = "mp3",
                FileName = "ThaiSong_" + DateTime.Now.ToString("yyyyMMdd_HHmmss")
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            string outputPath = sfd.FileName;
            MusicStyle style = (MusicStyle)cmbStyle.SelectedIndex;
            ReverbPreset reverb = (ReverbPreset)cmbReverb.SelectedIndex;
            EQPreset eq = (EQPreset)cmbEQ.SelectedIndex;

            btnGenerate.Enabled = false;
            lblStatus.Text = "สถานะ: กำลังสร้างเพลง...";
            lblStatus.ForeColor = Color.FromArgb(255, 200, 50);
            progressBar.Value = 0;

            try
            {
                float[]? audio = null;
                await Task.Run(() =>
                {
                    audio = AudioEngine.GenerateSong(lyrics, style, reverb, eq, progress =>
                    {
                        this.BeginInvoke(() => progressBar.Value = progress);
                    });
                });

                if (audio != null)
                {
                    await Task.Run(() =>
                    {
                        if (outputPath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                            AudioEngine.ExportToWav(audio, outputPath);
                        else
                            AudioEngine.ExportToMp3(audio, outputPath);
                    });

                    progressBar.Value = 100;
                    lblStatus.Text = $"สถานะ: สร้างเพลงสำเร็จ! ✓ บันทึกที่: {Path.GetFileName(outputPath)}";
                    lblStatus.ForeColor = Color.FromArgb(100, 255, 100);

                    var result = MessageBox.Show(
                        $"สร้างเพลงสำเร็จแล้ว!\n\nไฟล์: {outputPath}\n\nต้องการเปิดโฟลเดอร์ที่บันทึกหรือไม่?",
                        "สำเร็จ",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (result == DialogResult.Yes)
                    {
                        string? folder = Path.GetDirectoryName(outputPath);
                        if (folder != null)
                            System.Diagnostics.Process.Start("explorer.exe", folder);
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "สถานะ: เกิดข้อผิดพลาด ✗";
                lblStatus.ForeColor = Color.FromArgb(255, 80, 80);
                MessageBox.Show(
                    $"เกิดข้อผิดพลาดในการสร้างเพลง:\n{ex.Message}",
                    "ข้อผิดพลาด",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnGenerate.Enabled = true;
            }
        }

        private void BtnHelp_Click(object? sender, EventArgs e)
        {
            string helpText = @"╔══════════════════════════════════════════════════╗
║   วิธีใช้งาน Thai Song Generator                ║
║   สร้างเพลงด้วยเนื้อเพลงภาษาไทย               ║
╚══════════════════════════════════════════════════╝

📝 ขั้นตอนที่ 1: พิมพ์เนื้อเพลง
   - พิมพ์หรือวางเนื้อเพลงภาษาไทยในช่องเนื้อเพลง
   - สามารถพิมพ์หลายบรรทัดได้
   - ยิ่งเนื้อเพลงยาว เพลงก็จะยาวขึ้น

🎹 ขั้นตอนที่ 2: เลือกสไตล์เพลง
   • Thai Sounds & Clear Thai Letters
     - เสียงไทยดั้งเดิม ตัวอักษรไทยชัดเจน
   • Slow Sad Heartbreak Song
     - เพลงรักเศร้าช้าๆ เหมาะกับเนื้อเพลงอกหัก
   • Thai Pop
     - ป๊อปไทย จังหวะสนุกสนาน
   • Thai Ballad Clear
     - เพลงบัลลาดไทย เสียงใสชัดเจน

🔊 ขั้นตอนที่ 3: เลือก Reverb (เสียงก้อง)
   • Studio - เสียงสตูดิโอมาตรฐาน
   • Warm - เสียงอบอุ่นนุ่มนวล
   • Talented - เสียงมีมิติพรสวรรค์
   • Dazzle - เสียงเจิดจรัสมีประกาย
   • Distant - เสียงก้องไกล
   • KTV - เสียงคาราโอเกะ
   • Corridor - เสียงก้องทางเดิน
   • Natural - เสียงธรรมชาติ
   • CD - เสียงคุณภาพซีดี
   • Church - เสียงก้องโบสถ์
   • Concert - เสียงคอนเสิร์ตฮอลล์
   • Stereo - เสียงสเตอริโอกว้าง
   • Theater - เสียงโรงละคร
   • Phonograph - เสียงย้อนยุคแผ่นเสียง
   • Fantasy - เสียงแฟนตาซีมหัศจรรย์

🎚️ ขั้นตอนที่ 4: เลือก Equalizer (ปรับเสียง)
   • Standard Sound - เสียงมาตรฐานสมดุล
   • Deep Lows - เน้นเสียงทุ้มลึก
   • Crispy Sound - เสียงคมชัดกรอบ
   • Clear Sound - เสียงใสกระจ่าง

🎵 ขั้นตอนที่ 5: สร้างเพลง
   - คลิกปุ่ม ""สร้างเพลง & ส่งออก MP3""
   - เลือกที่บันทึกไฟล์ (รองรับ .mp3 และ .wav)
   - รอจนแถบความคืบหน้าเสร็จ 100%
   - เพลงจะถูกบันทึกเป็นไฟล์ที่เลือก

💡 เคล็ดลับ:
   • ใช้เนื้อเพลงที่มีสัมผัส จะได้ทำนองไพเราะ
   • เพลงเศร้าแนะนำ Reverb: Church หรือ Fantasy
   • เพลงป๊อปแนะนำ Reverb: Studio หรือ KTV
   • ลองเปลี่ยน EQ เพื่อหาเสียงที่ชอบ

⚙️ ข้อมูลทางเทคนิค:
   • รูปแบบเสียง: Stereo 44100Hz 16-bit
   • Bitrate MP3: 192 kbps
   • เอฟเฟกต์: Schroeder Reverb + 3-Band EQ
   • การสังเคราะห์: Multi-oscillator + ADSR Envelope

© Thai Song Generator v1.0
   พัฒนาด้วย C# .NET 8.0";

            MessageBox.Show(helpText, "📖 วิธีใช้งาน - Thai Song Generator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
