using System;
using FireEmblemDiscordBot;

namespace FireEmblemDiscordBot {
    class BattleInstance {
        public static void Main (String[] args) {
            Console.WriteLine("Write the name of a character...");
            string inputName = Console.ReadLine();

            CharacterInstance exampleCharacter = new CharacterInstance(inputName);
            exampleCharacter.setAllStats(new int[] {20, 15, 0, 5, 10, 5, 5, 0});

            Console.WriteLine(String.Format("{0} has {1} HP!", exampleCharacter.name, exampleCharacter.stats["HP"]));
            Console.ReadLine(); // Holding the console window open.
        }
    }
}