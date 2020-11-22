using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmulatorManager {
    class Achievement {
        private string name;
        private string gameBelongingTo;
        private string description;
        private bool completed;
        private List<string> rules;
        private Dictionary<string, bool> parts;

        public Achievement(string name, string game, string description, bool completed, List<string> rules, Dictionary<string, bool> parts) {
            this.name = name;
            gameBelongingTo = game;
            this.description = description;
            this.completed = completed;
            this.rules = rules;
            this.parts = parts;
        }

        public bool Completed {
            get { return completed; }
        }

        public string Name { 
            get { return name; }
        }

        public string Game {
            get { return gameBelongingTo.Replace(" ",""); }
        }
    }
}
