using System;
using System.Collections.Generic;

namespace FireEmblemDiscordBot {    
    public class CharacterInstance {
        // Using a dictionary to hold the stats in because it's simple.
        public Dictionary<String, int> stats = new Dictionary<String, int> () {
            {"HP", 0},
            {"STR", 0},
            {"INT", 0},
            {"SPD", 0},
            {"SKL", 0},
            {"LCK", 0},
            {"DEF", 0},
            {"RES", 0}
        };
        public String name;
        public CharacterInstance (String inputName) {
            name = inputName;
        }

        public void setAllStats (int[] statArray) {
            if (statArray == null || statArray.Length != 8) return;
            stats["HP"] = statArray[0];
            stats["STR"] = statArray[1];
            stats["INT"] = statArray[2];
            stats["SPD"] = statArray[3];
            stats["SKL"] = statArray[4];
            stats["LCK"] = statArray[5];
            stats["DEF"] = statArray[6];
            stats["RES"] = statArray[7];
        }
    }
}