using System;
using FireEmblemDiscordBot;

namespace FireEmblemDiscordBot {
    class BattleInstance {
        public static void Main (String[] args) {
            // Stats Go In This Order: HP, STR, INT, SPD, SKL, LCK, DEF, RES

            CharacterInstance exampleCharacter = new CharacterInstance("Lyndis");
            exampleCharacter.setAllStats(new int[] {16, 4, 1, 7, 9, 5, 2, 0});

            CharacterInstance enemyCharacter = new CharacterInstance("Batta");
            enemyCharacter.setAllStats(new int[] {21, 5, 0, 1, 3, 2, 3, 0});

            Console.WriteLine("{0} and {1} will now fight...", exampleCharacter.name, enemyCharacter.name);
            Console.ReadLine();

            // This Is The Main Battle Process

            int turnLimit = 20;
            for (int i = 0; i < turnLimit; i++) {
                doCharacterTurn(exampleCharacter, enemyCharacter);
                Console.ReadLine();
                doCharacterTurn(enemyCharacter, exampleCharacter);
                Console.ReadLine();
            }

            Console.ReadLine(); // Holding the console window open.
        } 

        public static void doCharacterTurn (CharacterInstance doingCharacter, CharacterInstance targetCharacter) {
            // 90 is a placeholder here for the weapon rate, which needs to be changed later. Alongside that, we need to add weapon advantages later as well.
            int hitRate = ((doingCharacter.stats["SKL"] - targetCharacter.stats["SKL"]) * 2) + 90 + ((doingCharacter.stats["LCK"] - (targetCharacter.stats["LCK"] / 2)) / 2);
            // We're using regular speed here, but we have to substitute it later for attack speed.
            int dodgeRate = (targetCharacter.stats["SPD"] * 2) + targetCharacter.stats["LCK"];
            int hitChance = hitRate - dodgeRate;
            // Again, we're using substitutes here. 20 is a placeholder for the weapon crit.
            int critRate = 20 + (doingCharacter.stats["SKL"] / 2);
            // No substitutes here. This is how it actually is.
            int critDodgeRate = targetCharacter.stats["LCK"];
            int critChance = critRate - critDodgeRate;

            // We're not using true hit, meaning we're taking two dice, rolling both of them, and using the average of the two in order to determine RNG.
            // This results in a type of bell curve. 80% hitrate is closer to 92.5% and 20% is closer to 8.5%.

            Random randomNumberGenerator = new Random();
            int diceOne = randomNumberGenerator.Next(0, 100), diceTwo = randomNumberGenerator.Next(0, 100), diceThree = randomNumberGenerator.Next(0, 100);
            float obtainedHitChance = (diceOne + diceTwo) / 2;
            if (obtainedHitChance <= hitChance) { 
                Console.WriteLine("{0} has hit {1} (HIT RATE: {2}%)", doingCharacter.name, targetCharacter.name, hitChance);
                if (diceThree <= critChance) Console.WriteLine("{0} has crit {1} (CRIT RATE: {2}%)", doingCharacter.name, targetCharacter.name, critChance);
                else Console.WriteLine("{0} has not crit {1} (CRIT RATE: {2}%)", doingCharacter.name, targetCharacter.name, critChance);
            } else Console.WriteLine("{0} has not hit {1} (HIT RATE: {2}%) (CRIT RATE: {3}%)", doingCharacter.name, targetCharacter.name, hitChance, critChance);

        }


    }
}