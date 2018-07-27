using System;
using System.Collections.Generic;

namespace FireEmblemDiscordBot {    
    public class WeaponInstance {
        // Using a dictionary to hold the stats in because it's simple.
        public Dictionary<String, int> stats = new Dictionary<String, int> () {
            {"MGT", 0},
            {"WGT", 0},
            {"HIT", 0},
            {"CRT", 0},
            {"RNG", 1}
        };
        public String name, type, reave;
        public Boolean isMagic;
        public WeaponInstance (String inputName, String inputType, String inputReave = "None", Boolean isMagic = false) {
            name = inputName;
            type = inputType;
            reave = inputReave;
            this.isMagic = isMagic;
        }

        public void setAllStats (int[] statArray) {
            if (statArray == null || statArray.Length != 5) return;
            stats["MGT"] = statArray[0];
            stats["WGT"] = statArray[1];
            stats["HIT"] = statArray[2];
            stats["CRT"] = statArray[3];
            stats["RNG"] = statArray[4];
        }
    }
}