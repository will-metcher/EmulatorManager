using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace EmulatorManager {
    public partial class MainMenu : Form {

        private Color listBg = Color.FromArgb(18, 35, 53);
        private Color infoBg = Color.FromArgb(23,45,68);
        private Font title = new Font("Verdana, Geneva, sans-serif",20);
        private Font h1 = new Font("Verdana, Geneva, sans-serif", 30);

        private Panel infoPanel = new Panel();
        private Label gameTitle = new Label();
        private PictureBox logo;
        private Button launchBtn = new Button();
        private Label playTime = new Label();
        private Panel achievementPanel = new Panel();

        string[] ignoreDirs = ConfigurationManager.AppSettings["ignoreddirs"].Split(',');
        string[] ignoreFileExt = ConfigurationManager.AppSettings["ignoredexts"].Split(',');

        PlayTimeManager ptm = new PlayTimeManager();

        public MainMenu() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {

            launchBtn.Click += new EventHandler(LaunchGame);
            AchievementManager am = new AchievementManager();
            LoadEmulators();
        }

        private void LoadEmulators() {
            Panel gamePanel = new Panel();
            gamePanel.Size = new Size(450, ClientRectangle.Height - 20);
            gamePanel.Location = new Point(0,20);
            gamePanel.AutoScroll = true;
            gamePanel.BackColor = listBg;
            this.Controls.Add(gamePanel);

            infoPanel.Size = new Size(ClientRectangle.Width - 450, ClientRectangle.Height - 20);
            infoPanel.Location = new Point(450, 20);
            infoPanel.BackColor = infoBg;
           
            this.Controls.Add(infoPanel);
            infoPanel.SendToBack();

            string[] dirs = Directory.GetDirectories(ConfigurationManager.AppSettings["emulatorpath"], "*");
            int y = 30;
            foreach(string dir in dirs)
            {
                
                string name = dir.Split('\\').Last();
                if(ignoreDirs.Contains(name))
                {
                    continue;
                }
                Label emu = new Label();
                emu.Text = name;
                emu.Size = new Size(400,30);
                emu.TextAlign = ContentAlignment.MiddleCenter;
                emu.Location = new Point(0,y);
                emu.ForeColor = Color.White;
                emu.BackColor = infoBg;
                emu.Font = title;
                y += 50;
                gamePanel.Controls.Add(emu);

                string[] roms = Directory.GetFiles(ConfigurationManager.AppSettings["emulatorpath"] + @"\" + name + @"\Roms\", "*");
                foreach (string rom in roms)
                {
                    string[] splitRomName = rom.Split('.');
                    if (ignoreFileExt.Contains(splitRomName[splitRomName.Length-1]))
                    {
                        continue;
                    }
                    string gameName = rom.Split('\\').Last().Split('.')[0];
                    try
                    {
                        PictureBox pb = new PictureBox();
                       
                        string imgPath = ConfigurationManager.AppSettings["emulatorpath"] + @"\Rom Images\" + gameName + ".gif";
                        Bitmap img = new Bitmap(imgPath);
                        pb.Image = img;
                        pb.Name = rom;
                        pb.Size = new Size(32, 32);
                        pb.SizeMode = PictureBoxSizeMode.StretchImage;
                        pb.Location = new Point(0, y);

                        gamePanel.Controls.Add(pb);
                    } catch
                    {
                        Console.WriteLine("Img not found");
                    }

                    Button romButton = new Button();
                    romButton.Text = gameName;
                    romButton.Name = name;
                    romButton.Click += new EventHandler(OpenGame);
                    romButton.Size = new Size(350,30);
                    romButton.Cursor = Cursors.Hand;
                    romButton.Location = new Point(40, y);
                    romButton.ForeColor = Color.White;
                    romButton.FlatStyle = FlatStyle.Flat;
                    romButton.FlatAppearance.BorderSize = 0;
                    gamePanel.Controls.Add(romButton);
                    y += 40;
                }
            }
            logo = new PictureBox();
            logo.Size = new Size(100,100);
            logo.SizeMode = PictureBoxSizeMode.StretchImage;
            logo.Location = new Point(50,50);
            infoPanel.Controls.Add(logo);

            
            gameTitle.Text = "";
            gameTitle.Size = new Size(ClientRectangle.Width - 450, 60);
            gameTitle.Location = new Point(175, 80);
            gameTitle.ForeColor = Color.White;
            gameTitle.Font = h1;
            infoPanel.Controls.Add(gameTitle);
        }

        private void OpenGame(object sender, EventArgs args) {
            Button gameBtn = (sender as Button);
            string game = gameBtn.Text;

            Console.WriteLine(game);

            gameTitle.Text = game;
            try
            {
                logo.Image = new Bitmap(ConfigurationManager.AppSettings["emulatorpath"] + "\\Rom Images\\" + game + ".gif");
            } catch
            {
                Console.WriteLine("Img not found");
            }

            launchBtn.Text = "Play";
            launchBtn.Size = new Size(100, 50);
            launchBtn.Location = new Point(50, 175);
            launchBtn.BackColor = Color.FromArgb(74, 132, 194);
            launchBtn.ForeColor = Color.White;
            launchBtn.Cursor = Cursors.Hand;
            launchBtn.Font = title;
            launchBtn.Name = game + "/" + gameBtn.Name;
            launchBtn.FlatStyle = FlatStyle.Flat;
            launchBtn.FlatAppearance.BorderSize = 0;
            infoPanel.Controls.Add(launchBtn);

            if(!ptm.Saves.ContainsKey(game.Replace(" ", "")))
            {
                ptm.AddGame(game.Replace(" ", ""));
            }

            playTime.Text = GeneratePlayTime(ptm.Saves[game.Replace(" ", "")]);
            playTime.Size = new Size(500,50);
            playTime.Location = new Point(150, 188);
            playTime.ForeColor = Color.White;
            playTime.Font = title;
            infoPanel.Controls.Add(playTime);

            Label achLabel = FormUtils.CreateLabel("Achievements", 400, 30, 50, infoPanel.Height - 725, Color.White);
            achLabel.Font = title;
            infoPanel.Controls.Add(achLabel);

            achievementPanel.Size = new Size(infoPanel.Width - 100, infoPanel.Height - 350);
            achievementPanel.Location = new Point(50, 300);
            achievementPanel.BackColor = listBg;
            infoPanel.Controls.Add(achievementPanel);

            List<Achievement> achievements = AchievementManager.LoadAchievements(game);
            if (achievements.Count == 0)
            {
                achievementPanel.Controls.Clear();
                return;
            }

            int x = 10;
            int y = 10;
            int i = 1;

            foreach (Achievement a in achievements)
            {
                PictureBox achImage = new PictureBox();
                achImage.Size = new Size(50,50);
                achImage.SizeMode = PictureBoxSizeMode.StretchImage;
                achImage.Location = new Point(x, y);
                achImage.Cursor = Cursors.Hand;

                Label achName = new Label();
                achName.MaximumSize = new Size(100, 40);
                achName.ForeColor = Color.White;
                achName.Text = a.Name;
                achName.AutoSize = true;
                achName.Location = new Point(x + 52, y + 10);
                achName.AutoSize = true;
                if (a.Completed)
                {
                    try
                    {
                        achImage.Image = new Bitmap(ConfigurationManager.AppSettings["emulatorpath"] + @"\Achievements\Images\" + a.Name + "^" + a.Game + ".png");
                    } catch
                    {
                        achImage.Image = new Bitmap(ConfigurationManager.AppSettings["emulatorpath"] + @"\Achievements\Images\NoImage.png");
                    }
                } else
                {
                    achImage.Image = new Bitmap(ConfigurationManager.AppSettings["emulatorpath"] + @"\Achievements\Images\Locked.png");
                }
                achievementPanel.Controls.Add(achName);
                achievementPanel.Controls.Add(achImage);

                x += 150;
                i++;
                if (i > 7)
                {
                    i = 1;
                    y += 70;
                    x = 10;
                }
            }
        }

        private string GeneratePlayTime(int time) {
            string final = "Play Time: ";
            if(time >= 120)
            {
                final += (time / 60) + " Hours";
            } else
            {
                final += time + " Minutes";
            }

            return final;
        }

        private void LaunchGame(object sender, EventArgs e) {
            Console.WriteLine("Launching");
            Button game = (sender as Button);
            string[] gameEmu = game.Name.Split('/');
            string romName = gameEmu[0];
            string emuName = gameEmu[1];
            string pathToExe = ConfigurationManager.AppSettings["emulatorpath"] + "\\" + emuName + "\\" + emuName + ".exe";
            string pathToRom = ConfigurationManager.AppSettings["emulatorpath"] + "\\" + emuName + "\\Roms\\" + romName + ConfigurationManager.AppSettings[emuName+"ext"];
            Console.WriteLine(pathToExe);
            Console.WriteLine(pathToRom);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = pathToExe;
            startInfo.Arguments = "\"" + pathToRom + "\"";

            try
            {
                Process.Start(startInfo);
                ptm.Game = romName.Replace(" ", "");
                ptm.Emulator = emuName;
                ptm.Start();
                return;
            } catch
            {
                Console.WriteLine("Error");
            }
        }

        private void SetPath(object sender, EventArgs e) {

        }
    }
}
