using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EmulatorManager {
    class AchievementManager {

        public static List<Achievement> LoadAchievements(string game) {
            List<Achievement> achievements = new List<Achievement>();

            XDocument xmlDoc = XDocument.Load(ConfigurationManager.AppSettings["emulatorpath"]+ @"\Achievements\Achievements.xml");
            foreach (XElement el in xmlDoc.Root.Elements())
            {
                if(el.Attribute("name").Value == game)
                {
                    foreach(XElement ach in el.Elements())
                    {
                        string achName = ach.Attribute("name").Value;
                        string description = ach.Element("description").Value;
                        Boolean completed = bool.Parse(ach.Element("completed").Value);
                        List<string> rules = new List<string>();
                        foreach(XElement rule in ach.Element("rules").Elements())
                        {
                            rules.Add(rule.Value);
                        }

                        Dictionary<string, bool> parts = new Dictionary<string, bool>();
                        foreach (XElement part in ach.Element("parts").Elements())
                        {
                            parts.Add(part.Value, bool.Parse(part.Attribute("completed").Value));
                        }

                        achievements.Add(new Achievement(achName, game, description, completed, rules, parts));
                    }
                    return achievements;
                }
            }
            return achievements;
        }
    }
}
