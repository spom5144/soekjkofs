using System;
using System.Drawing;
using System.Windows.Forms;

namespace AndroidUnlockerPro
{
    public class GuideForm : Form
    {
        public GuideForm()
        {
            this.Text = "📖 คู่มือการใช้งาน Android Unlocker Pro v2.0";
            this.Size = new Size(850, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(18, 18, 24);
            this.ForeColor = Color.FromArgb(240, 240, 245);
            this.Font = new Font("Segoe UI", 10f);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10f)
            };

            tabControl.TabPages.Add(CreateTab("📋 เริ่มต้นใช้งาน", GetStartGuide()));
            tabControl.TabPages.Add(CreateTab("🔑 ลบ Pattern", GetPatternGuide()));
            tabControl.TabPages.Add(CreateTab("🔢 ลบ PIN", GetPinGuide()));
            tabControl.TabPages.Add(CreateTab("🛡️ ปลดล็อค FRP", GetFrpGuide()));
            tabControl.TabPages.Add(CreateTab("♻️ Factory Reset", GetResetGuide()));
            tabControl.TabPages.Add(CreateTab("📱 Recovery Mode", GetRecoveryGuide()));
            tabControl.TabPages.Add(CreateTab("❓ แก้ปัญหา", GetTroubleshootGuide()));

            this.Controls.Add(tabControl);
        }

        private TabPage CreateTab(string title, string content)
        {
            var tab = new TabPage(title);
            tab.BackColor = Color.FromArgb(25, 25, 35);
            tab.Padding = new Padding(15);

            var rtb = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(25, 25, 35),
                ForeColor = Color.FromArgb(220, 220, 230),
                Font = new Font("Segoe UI", 10.5f),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Text = content
            };

            tab.Controls.Add(rtb);
            return tab;
        }

        private string GetStartGuide()
        {
            return @"╔══════════════════════════════════════════════════════════╗
║            คู่มือเริ่มต้นใช้งาน Android Unlocker Pro v2.0              ║
╚══════════════════════════════════════════════════════════╝

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📋 สิ่งที่ต้องเตรียม:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  1. คอมพิวเตอร์ Windows 10/11 (64-bit)
  2. สาย USB สำหรับเชื่อมต่อมือถือ (แนะนำสายแท้)
  3. ADB Platform Tools (ดาวน์โหลดจาก Google)
  4. ไดรเวอร์ USB ของยี่ห้อมือถือ

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📥 วิธีติดตั้ง ADB:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  1. เปิดเว็บไซต์: developer.android.com/tools/releases/platform-tools
  2. กด ""Download SDK Platform-Tools for Windows""
  3. แตกไฟล์ ZIP ไปที่ C:\platform-tools\
  4. หรือวางไว้ในโฟลเดอร์เดียวกับโปรแกรม

  ★ หรือใช้ Chocolatey: choco install adb

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 วิธีเปิด USB Debugging:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  ★ ต้องทำก่อนที่มือถือจะถูกล็อค! ★

  1. ไปที่ ตั้งค่า (Settings)
  2. เลื่อนลงไปที่ เกี่ยวกับโทรศัพท์ (About Phone)
  3. กด หมายเลข Build (Build Number) ติดต่อกัน 7 ครั้ง
  4. จะปรากฏข้อความ ""คุณเป็นนักพัฒนาแล้ว""
  5. กลับไปที่ ตั้งค่า > ตัวเลือกนักพัฒนา (Developer Options)
  6. เปิด USB Debugging (การดีบัก USB)
  7. เสียบสาย USB > กด อนุญาต (Allow) บนมือถือ

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🔌 วิธีเชื่อมต่อกับโปรแกรม:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  1. เสียบสาย USB เชื่อมต่อมือถือกับคอมพิวเตอร์
  2. เปิดโปรแกรม Android Unlocker Pro
  3. กดปุ่ม ""🔌 เชื่อมต่อ ADB""
  4. รอให้โปรแกรมตรวจพบอุปกรณ์
  5. เมื่อสถานะเป็น 🟢 สีเขียว แสดงว่าเชื่อมต่อสำเร็จ

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
⚡ วิธีเลือกการปลดล็อค:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  ★ หากเปิด USB Debugging ไว้:
    → ใช้ ลบรหัส Pattern / PIN / Password (ไม่สูญเสียข้อมูล)

  ★ หากไม่ได้เปิด USB Debugging:
    → ใช้ Factory Reset ผ่าน Recovery Mode (สูญเสียข้อมูล)

  ★ หากมีปัญหา Google Account Lock (FRP):
    → ใช้ปลดล็อค FRP

  ★ ไม่แน่ใจ:
    → กด ""ปลดล็อคอัตโนมัติ"" โปรแกรมจะลองทุกวิธี";
        }

        private string GetPatternGuide()
        {
            return @"╔══════════════════════════════════════════════════════════╗
║                    วิธีลบรหัส Pattern (รูปแบบ)                          ║
╚══════════════════════════════════════════════════════════╝

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
เงื่อนไข:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  • ต้องเปิด USB Debugging ไว้ก่อนที่จะถูกล็อค
  • หรือมือถือต้อง Root แล้ว
  • หรือใช้วิธี Recovery Mode (ไม่ต้องเปิด USB Debugging)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
วิธีที่ 1: ลบไฟล์ gesture.key (แนะนำ)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  ★ ไม่สูญเสียข้อมูล ★

  1. เชื่อมต่อมือถือผ่าน USB
  2. กดปุ่ม ""เชื่อมต่อ ADB"" ให้สถานะเป็นสีเขียว
  3. กดเมนู ""🔑 ลบรหัส Pattern"" ที่เมนูด้านซ้าย
  4. โปรแกรมจะดำเนินการ:
     - ลบไฟล์ gesture.key
     - ลบไฟล์ locksettings.db
     - ลบไฟล์ gatekeeper keys
  5. มือถือจะรีสตาร์ทอัตโนมัติ
  6. หลังเปิดเครื่อง จะไม่มี Pattern Lock อีกต่อไป

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
วิธีที่ 2: ผ่าน Google Find My Device (ออนไลน์)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  ★ ไม่ต้องเสียบ USB ★

  1. เปิดเว็บ google.com/android/find บนคอมพิวเตอร์
  2. ล็อกอินด้วย Google Account ที่ใช้ในมือถือ
  3. เลือกอุปกรณ์
  4. กด ""ล็อคอุปกรณ์"" (Secure Device)
  5. ตั้ง PIN/Password ใหม่
  6. ใช้ PIN/Password ใหม่นี้เปิดเครื่อง

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
วิธีที่ 3: รอให้แบตหมด + บัญชี Google (Android 4.4 หรือต่ำกว่า)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  1. ใส่ Pattern ผิด 5 ครั้ง
  2. จะปรากฏ ""ลืมรูปแบบ?"" (Forgot Pattern?)
  3. กดเข้าไป > ใส่ Google Account
  4. รีเซ็ต Pattern ใหม่ได้เลย

  ★ วิธีนี้ใช้ได้เฉพาะ Android 4.4 หรือต่ำกว่า";
        }

        private string GetPinGuide()
        {
            return @"╔══════════════════════════════════════════════════════════╗
║               วิธีลบรหัส PIN / Password                              ║
╚══════════════════════════════════════════════════════════╝

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
วิธีที่ 1: ผ่าน ADB (แนะนำ - ไม่สูญเสียข้อมูล)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  เงื่อนไข: เปิด USB Debugging ไว้ก่อน

  1. เชื่อมต่อมือถือผ่าน USB
  2. กด ""เชื่อมต่อ ADB""
  3. กดเมนู ""🔢 ลบรหัส PIN/Password""
  4. โปรแกรมจะลบไฟล์:
     - password.key
     - locksettings.db
     - gatekeeper.password.key
  5. มือถือรีสตาร์ท > ไม่ต้องใส่ PIN/Password

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
วิธีที่ 2: Samsung Find My Mobile (เฉพาะ Samsung)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  เงื่อนไข: ล็อกอิน Samsung Account ไว้ในมือถือ

  1. เปิดเว็บ findmymobile.samsung.com
  2. ล็อกอินด้วย Samsung Account
  3. เลือกอุปกรณ์
  4. กด ""Unlock"" (ปลดล็อค)
  5. รหัสจะถูกลบ > เข้าเครื่องได้เลย

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
วิธีที่ 3: Google Find My Device
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  1. เปิด google.com/android/find
  2. ล็อกอิน Google Account
  3. กด ""ล็อคอุปกรณ์"" > ตั้งรหัสใหม่
  4. ใช้รหัสใหม่ที่ตั้งเปิดเครื่อง

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
วิธีที่ 4: Factory Reset (สูญเสียข้อมูล)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  ★ ใช้เมื่อไม่สามารถใช้วิธีอื่นได้ ★

  ดูรายละเอียดในแท็บ ""Factory Reset""";
        }

        private string GetFrpGuide()
        {
            return @"╔══════════════════════════════════════════════════════════╗
║          วิธีปลดล็อค FRP (Factory Reset Protection)                  ║
╚══════════════════════════════════════════════════════════╝

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
FRP คืออะไร?
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  FRP (Factory Reset Protection) คือระบบป้องกันการขโมย
  ของ Google ที่ทำงานหลังจาก Factory Reset
  จะต้องใส่ Google Account เดิมที่เคยล็อกอินไว้
  จึงจะใช้งานมือถือได้

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
★ Samsung:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  1. เชื่อมต่อ WiFi ในหน้า Setup
  2. ที่หน้า Google Account > กดช่อง Email
  3. กด Keyboard icon > เปิด Samsung Keyboard Settings
  4. ไปที่ About Samsung Keyboard > Licenses
  5. กดลิงก์ > เปิด Browser > ดาวน์โหลด Bypass APK
  6. หรือใช้วิธี ADB (เชื่อมต่อ USB > กด ""ปลดล็อค FRP"")

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
★ Xiaomi / Redmi / POCO:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  1. เชื่อมต่อ WiFi > กลับหน้าแรก
  2. กด Emergency Call > พิมพ์ *#*#4636#*#*
  3. เปิด Browser > ดาวน์โหลด QuickShortcutMaker
  4. เปิด App > ค้นหา Google Account Manager
  5. เพิ่ม Account ใหม่ > รีบูต

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
★ Huawei:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  1. ที่หน้า Google Account > กดช่อง Email > เลือกข้อความ
  2. Share > Messages > ส่ง SMS พร้อมลิงก์
  3. กดลิงก์ > เปิด Browser
  4. ดาวน์โหลด QuickShortcutMaker
  5. ค้นหา Google Account Manager > เพิ่ม Account ใหม่

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
★ OPPO / Vivo / Realme:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  1. เชื่อมต่อ WiFi
  2. กลับไปหน้า Setup > แตะ Emergency Call
  3. พิมพ์ *#812# หรือ *#813#
  4. หรือใช้ TalkBack: กด Volume Up+Down ค้าง 3 วินาที
  5. ไปที่ TalkBack Settings > Help > กดลิงก์ > Browser
  6. ดาวน์โหลด FRP Bypass APK

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
★ วิธี ADB (ทุกยี่ห้อ):
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  เงื่อนไข: ต้องเข้าถึง ADB ได้ (เปิด USB Debugging ในหน้า Setup)

  1. เชื่อมต่อ USB > กด ""เชื่อมต่อ ADB""
  2. กดเมนู ""🛡️ ปลดล็อค FRP""
  3. โปรแกรมจะ:
     - ลบ Google Account จากเครื่อง
     - ข้าม Setup Wizard
     - รีเซ็ต Account Manager
  4. รีบูท > เข้าตั้งค่า > ลบ Account เดิม > เพิ่มใหม่";
        }

        private string GetResetGuide()
        {
            return @"╔══════════════════════════════════════════════════════════╗
║            วิธี Factory Reset (รีเซ็ตค่าจากโรงงาน)                   ║
╚══════════════════════════════════════════════════════════╝

  ⚠️ คำเตือน: ข้อมูลทั้งหมดจะถูกลบ!
  (รูปภาพ, วิดีโอ, แอป, ข้อความ, รายชื่อ)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
วิธีที่ 1: ผ่าน ADB (เชื่อมต่อ USB)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  1. เชื่อมต่อมือถือ > กด ""เชื่อมต่อ ADB""
  2. กดเมนู ""♻️ Factory Reset""
  3. ยืนยันการ Reset
  4. รอ 5-10 นาที

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
วิธีที่ 2: ผ่าน Recovery Mode (ไม่ต้องเสียบ USB)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  1. ปิดเครื่องมือถือ
  2. เข้า Recovery Mode (ดูปุ่มกดในแท็บ Recovery Mode)
  3. ใช้ปุ่ม Volume Up/Down เลื่อนเมนู
  4. เลือก ""Wipe data/factory reset""
  5. กดปุ่ม Power เพื่อยืนยัน
  6. เลือก ""Yes -- delete all user data""
  7. รอจนเสร็จ
  8. เลือก ""Reboot system now""
  9. เครื่องจะเปิดขึ้นมาใหม่เหมือนเพิ่งซื้อ

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
วิธีที่ 3: ผ่าน Fastboot (ขั้นสูง)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  1. ปิดเครื่อง > เข้า Fastboot Mode
  2. เชื่อมต่อ USB
  3. เลือกวิธี ""Fastboot Mode"" ในโปรแกรม
  4. กดเมนู ""♻️ Factory Reset""";
        }

        private string GetRecoveryGuide()
        {
            return @"╔══════════════════════════════════════════════════════════╗
║         วิธีเข้า Recovery Mode ของแต่ละยี่ห้อ                        ║
╚══════════════════════════════════════════════════════════╝

  ★ ปิดเครื่องก่อน แล้วกดปุ่มค้างตามนี้:

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 Samsung:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • มีปุ่ม Bixby: Power + Volume Up + Bixby
  • ไม่มีปุ่ม Bixby: Power + Volume Up
  • Samsung เก่า: Power + Volume Up + Home

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 Huawei / Honor:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • กด Power + Volume Up ค้าง 10-15 วินาที
  • ปล่อย Power เมื่อเห็นโลโก้ ยังคงกด Volume Up

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 Xiaomi / Redmi / POCO:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • กด Power + Volume Up ค้าง
  • เมื่อสั่นให้ปล่อย Power ยังคงกด Volume Up
  • จะเข้า Mi Recovery > เลือก English

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 OPPO / Realme:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • กด Power + Volume Down ค้าง
  • ColorOS Recovery > เลือก English
  • บางรุ่น: Power + Volume Up

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 Vivo:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • กด Power + Volume Up ค้างพร้อมกัน
  • ปล่อยเมื่อเห็น Vivo logo > เข้า Recovery

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 OnePlus:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • กด Power + Volume Down ค้าง
  • เลือกภาษา > Recovery Mode

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 Sony Xperia:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • กด Power ค้าง > เลือก Restart
  • พอสั่น กด Volume Down ค้าง

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 LG:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • กด Power + Volume Down ค้าง
  • เมื่อเห็น LG logo ปล่อย Power แล้วกดใหม่
  • ยังคงกด Volume Down ตลอด

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 Google Pixel:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • กด Power + Volume Down ค้าง
  • เลื่อนไปที่ Recovery mode > กด Power
  • เห็น Android ตัวอยู่ > กด Power + Volume Up สั้นๆ

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 Nokia:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • กด Power + Volume Up ค้าง
  • หรือ Power + Volume Down (แล้วแต่รุ่น)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 Motorola:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • กด Power + Volume Down ค้าง
  • จะเข้า Bootloader > เลื่อนไป Recovery > กด Power

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 ASUS:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • กด Power + Volume Down ค้าง
  • เลือก Recovery Mode > กด Volume Up

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📱 Infinix / Tecno / Itel:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • กด Power + Volume Up ค้าง
  • บางรุ่น: Power + Volume Up + Volume Down";
        }

        private string GetTroubleshootGuide()
        {
            return @"╔══════════════════════════════════════════════════════════╗
║                    แก้ปัญหาที่พบบ่อย                                  ║
╚══════════════════════════════════════════════════════════╝

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
❌ ปัญหา: ไม่พบอุปกรณ์ (No device found)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  วิธีแก้:
  1. ตรวจสอบสาย USB - ใช้สายที่รองรับ Data Transfer (ไม่ใช่สายชาร์จอย่างเดียว)
  2. ลองเปลี่ยนพอร์ต USB (ใช้ USB 2.0 แทน USB 3.0)
  3. ติดตั้งไดรเวอร์ USB:
     - Samsung: samsung.com/us/support/downloads
     - Google USB Driver: developer.android.com
     - อื่นๆ: ค้นหา ""[ยี่ห้อ] USB driver download""
  4. กดปุ่ม ""ตัดการเชื่อมต่อ"" แล้วกด ""เชื่อมต่อ ADB"" ใหม่
  5. ลองรีสตาร์ทคอมพิวเตอร์

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
❌ ปัญหา: ADB Unauthorized
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  วิธีแก้:
  1. ดูหน้าจอมือถือ > กด ""อนุญาต"" (Allow USB Debugging)
  2. หากหน้าจอล็อค:
     - ลองถอด-เสียบสาย USB ใหม่
     - หากยังไม่ได้ ต้องใช้ Recovery Mode แทน

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
❌ ปัญหา: ไม่พบ ADB (ADB not found)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  วิธีแก้:
  1. ดาวน์โหลด Platform Tools จาก Google
  2. แตกไฟล์ไปที่ C:\platform-tools\
  3. หรือวางในโฟลเดอร์เดียวกับโปรแกรม
  4. เพิ่ม Path ใน Environment Variables:
     - System Properties > Environment Variables
     - เพิ่ม C:\platform-tools ใน Path

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
❌ ปัญหา: ปลดล็อคแล้วยังมีรหัสอยู่
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  วิธีแก้:
  1. ลองรีบูตเครื่องอีกครั้ง
  2. ลองกด ""ปลดล็อคอัตโนมัติ"" (ลองทุกวิธีพร้อมกัน)
  3. หากยังไม่ได้ ใช้ Factory Reset ผ่าน Recovery Mode

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
❌ ปัญหา: เข้า Recovery Mode ไม่ได้
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  วิธีแก้:
  1. ตรวจสอบว่ากดปุ่มถูกต้อง (ดูแท็บ Recovery Mode)
  2. ต้องปิดเครื่องก่อน (ไม่ใช่แค่ล็อคหน้าจอ)
  3. กดปุ่มค้างจนกว่าจะเข้า (10-15 วินาที)
  4. หากกดปุ่ม Volume + Power ไม่ได้ ลอง:
     - ชาร์จแบตให้เต็มก่อน
     - กดปุ่มแรงๆ และค้างไว้นาน

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
❌ ปัญหา: FRP Lock หลัง Factory Reset
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  วิธีแก้:
  1. จำ Google Account เดิมได้ > ล็อกอินปกติ
  2. จำไม่ได้ > ใช้ฟีเจอร์ ""ปลดล็อค FRP"" ในโปรแกรม
  3. หรือ Account Recovery ผ่าน accounts.google.com/signin/recovery

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
💡 เคล็ดลับ:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

  • เปิด USB Debugging ไว้เสมอ (ป้องกันปัญหาในอนาคต)
  • ใช้ Smart Lock (ปลดล็อคอัตโนมัติที่บ้าน/ที่ทำงาน)
  • จด PIN/Password ไว้ในที่ปลอดภัย
  • Backup ข้อมูลเข้า Google Drive เป็นประจำ
  • ล็อกอิน Google Account และ Samsung Account ไว้เสมอ";
        }
    }
}
