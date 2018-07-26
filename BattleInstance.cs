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
            CharacterInstance winningCharacter = null;
            for (int i = 0; i < turnLimit; i++) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Round {0}", i + 1);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} has {1} HP left.", exampleCharacter.name, exampleCharacter.stats["HP"]);
                Console.WriteLine("{0} has {1} HP left.\n", enemyCharacter.name, enemyCharacter.stats["HP"]);
                Console.ForegroundColor = ConsoleColor.White;

                if (doCharacterTurn(exampleCharacter, enemyCharacter)) {
                    winningCharacter = exampleCharacter;
                    break;
                }
                Console.ReadLine();
                if (doCharacterTurn(enemyCharacter, exampleCharacter)) {
                    winningCharacter = enemyCharacter;
                    break;
                }
                Console.ReadLine();
            }
            // Later on, we're going to determine the winner by who has lower health, but for now, this works.
            if (winningCharacter != null) Console.WriteLine("The winner is {0}!", winningCharacter.name);
            else Console.WriteLine("There was no winner...", winningCharacter.name);


            Console.ReadLine(); // Holding the console window open.
        } 

        public static Boolean doCharacterTurn (CharacterInstance doingCharacter, CharacterInstance targetCharacter) {
            // 90 is a placeholder here for the weapon rate, which needs to be changed later. Alongside that, we need to add weapon advantages later as well.
            int hitRate = ((doingCharacter.stats["SKL"]) * 2) + 90 + ((doingCharacter.stats["LCK"] - (targetCharacter.stats["LCK"] / 2)) / 2);
            // We're using regular speed here, but we have to substitute it later for attack speed.
            int dodgeRate = (targetCharacter.stats["SPD"] * 2) + targetCharacter.stats["LCK"];
            int hitChance = hitRate - dodgeRate;
            // Again, we're using substitutes here. 20 is a placeholder for the weapon crit.
            int critRate = 20 + (doingCharacter.stats["SKL"] / 2);
            // No substitutes here. This is how it actually is.
            int critDodgeRate = targetCharacter.stats["LCK"];
            int critChance = critRate - critDodgeRate;
            // Let's calculate the potential damage here to be able to use it later. We need to add weapon might to this later just like everything else.
            int potentialDamage = doingCharacter.stats["STR"] - targetCharacter.stats["DEF"]; 

            // We're not using true hit, meaning we're taking two dice, rolling both of them, and using the average of the two in order to determine RNG.
            // This results in a type of bell curve. 80% hitrate is closer to 92.5% and 20% is closer to 8.5%.

            Random randomNumberGenerator = new Random();
            int diceOne = randomNumberGenerator.Next(0, 100), diceTwo = randomNumberGenerator.Next(0, 100);
            if (diceOne <= hitChance) { 
                if (diceTwo <= critChance) {
                    potentialDamage *= 3;
                    Console.WriteLine("{0} has critical hit {1}, dealing {4} damage (HIT RATE: {2}%) (CRIT RATE: {3}%)", doingCharacter.name, targetCharacter.name, hitChance, critChance, potentialDamage);
                }
                else Console.WriteLine("{0} has hit {1}, dealing {4} damage (HIT RATE: {2}%) (CRIT RATE: {3}%)", doingCharacter.name, targetCharacter.name, hitChance, critChance, potentialDamage);
                targetCharacter.stats["HP"] -= potentialDamage;
            } else Console.WriteLine("{0} has not hit {1} (HIT RATE: {2}%) (CRIT RATE: {3}%)", doingCharacter.name, targetCharacter.name, hitChance, critChance);

            Boolean isTargetDead = false;
            if (targetCharacter.stats["HP"] <= 0) {
                targetCharacter.stats["HP"] = 0;
                Console.WriteLine("{0} has fallen.", targetCharacter.name);
                isTargetDead = true;
            }
            return isTargetDead;
        }


    }
}