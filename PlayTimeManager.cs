using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;

namespace EmulatorManager {
    class PlayTimeManager {

        private string game;
        private string emulator;
        public Dictionary<string, int> saves = new Dictionary<string, int>();
        Timer timer = new Timer();
        private string path = ConfigurationManager.AppSettings["emulatorpath"] + @"\Time.txt";
        public PlayTimeManager() {
            string[] data = File.ReadAllLines(path);
            foreach (string save in data)
            {
                string[] split = save.Split(':');
                saves.Add(split[0], Int32.Parse(split[1]));
            }
        }

        public void Start() {
            timer.Interval = 60000;
            timer.Elapsed += Loop;
            timer.Start();
        }

        private void Loop(object sender, ElapsedEventArgs e) {
            saves[game]++;

            using (StreamWriter file = new StreamWriter(path))
            {
                foreach(string key in saves.Keys)
                {
                    file.WriteLine(key+":"+saves[key]);
                }
            }
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (p.ProcessName == emulator)
                {
                    return;
                }
            }
            timer.Stop();
        }

        public void AddGame(string game) {
            saves.Add(game, 0);
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(game.Replace(" ", "") + ":0");
            }
        }

        public string Game {
            set { game = value; }
        }

        public string Emulator {
            set { emulator = value; }
        }

        public Dictionary<string, int> Saves {
            get { return saves; }
        }

    }
}
