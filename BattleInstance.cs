using System;
using FireEmblemDiscordBot;

namespace FireEmblemDiscordBot {
    class BattleInstance {
        public static void Main (String[] args) {
            // Stats Go In This Order For Characters: HP, Strength, Magic / Intellect, Speed, Skill, Luck, Defense, Resistance
            // Stats Go In This Order For Weapons: Might, Weight, Hit, Crit, Range

            WeaponInstance Alondite = new WeaponInstance("Alondite", "Sword");
            Alondite.setAllStats(new int[] {18, 20, 80, 5, 1});

            WeaponInstance Ragnell = new WeaponInstance("Ragnell", "Sword");
            Alondite.setAllStats(new int[] {18, 20, 80, 5, 1});

            CharacterInstance exampleCharacter = new CharacterInstance("Ike");
            exampleCharacter.setAllStats(new int[] {65, 37, 9, 35, 40, 22, 32, 15});

            CharacterInstance enemyCharacter = new CharacterInstance("Black Knight");
            enemyCharacter.setAllStats(new int[] {70, 38, 18, 30, 40, 20, 35, 25});

            Console.WriteLine("{0} and {1} will now fight...", exampleCharacter.name, enemyCharacter.name);
            Console.ReadLine();

            // This Is The Main Battle Process

            int turnLimit = 20;
            CharacterInstance winningCharacter = null;
            for (int i = 0; i < turnLimit; i++) {
                // This all is just displaying the information from each round onto the console in different colors.
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Round {0}", i + 1);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} has {1} HP left.", exampleCharacter.name, exampleCharacter.stats["HP"]);
                Console.WriteLine("{0} has {1} HP left.\n", enemyCharacter.name, enemyCharacter.stats["HP"]);
                Console.ForegroundColor = ConsoleColor.White;

                /* This is to check if the battle is over. 
                * Because there is a for loop inside a for loop, we have to be careful with break positioning. */
                Boolean isBattleOver = false;

                // A character gets to move twice in a round if they have four more speed than the enemy. 
                int numMoves = (calculateAttackSpeed(exampleCharacter) - 4) >= enemyCharacter.stats["SPD"] ? 2 : 1;
                for (int a = 0; a < numMoves; a++) {
                    if (doCharacterTurn(exampleCharacter, enemyCharacter)) {
                        winningCharacter = exampleCharacter;
                        isBattleOver = true;
                        break;
                    }
                }
                if (isBattleOver) break;
                Console.ReadLine();

                numMoves = (calculateAttackSpeed(enemyCharacter) - 4) >= exampleCharacter.stats["SPD"] ? 2 : 1;
                for (int a = 0; a < numMoves; a++) {
                    if (doCharacterTurn(enemyCharacter, exampleCharacter)) {
                        winningCharacter = enemyCharacter;
                        isBattleOver = true;
                        break;
                    }
                }
                if (isBattleOver) break;
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

        public static int calculateAttackSpeed (CharacterInstance inputCharacter) {
            // If a weapon weighs more than a character is able to handle, their speed gets reduced.
            int valueToReturn = 0;
            if (inputCharacter.equippedWeapon != null && (inputCharacter.equippedWeapon.stats["WGT"] > inputCharacter.stats["STR"]))
                valueToReturn = inputCharacter.stats["SPD"] - (inputCharacter.equippedWeapon.stats["WGT"] - inputCharacter.stats["STR"]);
            else valueToReturn = inputCharacter.stats["SPD"];
            return valueToReturn; 
        }
    }
}