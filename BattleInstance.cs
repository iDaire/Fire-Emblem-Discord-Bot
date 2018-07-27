// What Has Yet To Be Done: Magic vs Magic, Magic vs Phyiscal, Ranged Weaponry, Skills

using System;
using FireEmblemDiscordBot;

namespace FireEmblemDiscordBot {
    class BattleInstance {
        public static void Main (String[] args) {
            // Stats Go In This Order For Characters: HP, Strength, Magic / Intellect, Speed, Skill, Luck, Defense, Resistance
            // Stats Go In This Order For Weapons: Might, Weight, Hit, Crit, Range
            BattleFunctionHolder B = new BattleFunctionHolder();

            WeaponInstance Alondite = new WeaponInstance("Alondite", "Sword");
            Alondite.setAllStats(new int[] {18, 20, 80, 5, 1});

            WeaponInstance Ragnell = new WeaponInstance("Ragnell", "Sword");
            Ragnell.setAllStats(new int[] {18, 20, 80, 5, 1});

            CharacterInstance exampleCharacter = new CharacterInstance("Ike", Ragnell);
            exampleCharacter.setAllStats(new int[] {65, 37, 9, 35, 40, 22, 32, 15});

            CharacterInstance enemyCharacter = new CharacterInstance("Black Knight", Alondite);
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
                int numMoves = (B.calculateAttackSpeed(exampleCharacter) - 4) >= enemyCharacter.stats["SPD"] ? 2 : 1;
                for (int a = 0; a < numMoves; a++) {
                    if (B.doCharacterTurn(exampleCharacter, enemyCharacter)) {
                        winningCharacter = exampleCharacter;
                        isBattleOver = true;
                        break;
                    }
                }
                if (isBattleOver) break;
                Console.ReadLine();

                // Let's do this twice.
                numMoves = (B.calculateAttackSpeed(enemyCharacter) - 4) >= exampleCharacter.stats["SPD"] ? 2 : 1;
                for (int a = 0; a < numMoves; a++) {
                    if (B.doCharacterTurn(enemyCharacter, exampleCharacter)) {
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

    }

    public class BattleFunctionHolder {
        public Boolean doCharacterTurn (CharacterInstance doingCharacter, CharacterInstance targetCharacter) {
            // The weapon advantages and the hit rate equation were taken from Fire Emblem 7 with tweaks, but the weapon weight affecting speed was taken from Fire Emblem 9
            int hitRate = ((doingCharacter.stats["SKL"]) * 2) + doingCharacter.equippedWeapon.stats["HIT"] + ((doingCharacter.stats["LCK"] - (targetCharacter.stats["LCK"] / 2)) / 2);
            hitRate += 15 * calculateWeaponEffectiveness(doingCharacter.equippedWeapon, targetCharacter.equippedWeapon);
            if (hitRate < 0) hitRate = 0;

            // The dodge rate equation was taken from Fire Emblem 7 with no changes.
            int dodgeRate = (calculateAttackSpeed(targetCharacter) * 2) + targetCharacter.stats["LCK"];

            // We put them together to get the actual chance of hitting the opponent.
            int hitChance = hitRate - dodgeRate;

            // The weapon crit equation was taken from Fire Emblem 7 with no changes.
            int critRate = doingCharacter.equippedWeapon.stats["CRT"] + (doingCharacter.stats["SKL"] / 2);

            // The crit dodge equation was taken from Fire Emblem 7 with no changes.
            int critDodgeRate = targetCharacter.stats["LCK"];

            // Again, let's put it together to get the actual chance of doing a critical hit.
            int critChance = critRate - critDodgeRate;

            // Let's calculate the potential damage and also add in weapon advantages again. MUST MAKE MAGIC DAMAGE LATER.
            int potentialDamage = (doingCharacter.stats["STR"] + doingCharacter.equippedWeapon.stats["MGT"]) - targetCharacter.stats["DEF"]; 
            potentialDamage += 1 * calculateWeaponEffectiveness(doingCharacter.equippedWeapon, targetCharacter.equippedWeapon);
            if (potentialDamage < 0) potentialDamage = 0;

            // Roll two dice, one for hit rate and one for crit rate.
            Random randomNumberGenerator = new Random();
            int diceOne = randomNumberGenerator.Next(0, 100), diceTwo = randomNumberGenerator.Next(0, 100);

            // If there's a hit, we proceed to check for a crit. You can miss, hit, and crit hit.
            if (diceOne <= hitChance) { 
                if (diceTwo <= critChance) {
                    potentialDamage *= 3;
                    Console.WriteLine("{0} has critical hit {1}, dealing {4} damage (HIT RATE: {2}%) (CRIT RATE: {3}%)", doingCharacter.name, targetCharacter.name, hitChance, critChance, potentialDamage);
                }
                else Console.WriteLine("{0} has hit {1}, dealing {4} damage (HIT RATE: {2}%) (CRIT RATE: {3}%)", doingCharacter.name, targetCharacter.name, hitChance, critChance, potentialDamage);
                targetCharacter.stats["HP"] -= potentialDamage;
            } else Console.WriteLine("{0} has not hit {1} (HIT RATE: {2}%) (CRIT RATE: {3}%)", doingCharacter.name, targetCharacter.name, hitChance, critChance);

            // Let's check if the enemy is dead before continuing the round.
            Boolean isTargetDead = false;
            if (targetCharacter.stats["HP"] <= 0) {
                targetCharacter.stats["HP"] = 0;
                Console.WriteLine("{0} has fallen.", targetCharacter.name);
                isTargetDead = true;
            }
            return isTargetDead;
        }

        public int calculateAttackSpeed (CharacterInstance inputCharacter) {
            // If a weapon weighs more than a character is able to handle, their speed gets reduced.
            int valueToReturn = 0;
            if (inputCharacter.equippedWeapon != null && (inputCharacter.equippedWeapon.stats["WGT"] > inputCharacter.stats["STR"]))
                valueToReturn = inputCharacter.stats["SPD"] - (inputCharacter.equippedWeapon.stats["WGT"] - inputCharacter.stats["STR"]);
            else valueToReturn = inputCharacter.stats["SPD"];
            return valueToReturn; 
        }

        public int calculateWeaponEffectiveness (WeaponInstance doingWeapon, WeaponInstance targetWeapon) {
            // Swords beat Axes beat Lances beat Swords.
            int valueToReturn = 0, activeMultiplier = 1;
            String doingType, targetType;

            // If the weapon reaves something, double the multiplier.
            if (doingWeapon.reave != "None") {
                activeMultiplier *= 2;
                doingType = doingWeapon.reave;
            } else {
                doingType = doingWeapon.type;
            }
            // If the enemy weapon reaves something, double the multiplier. The multiplier can go up to 4.
            if (targetWeapon.reave != "None") {
                activeMultiplier *= 2;
                targetType = targetWeapon.reave;
            } else {
                targetType = targetWeapon.type;
            }

            // If the weapons are the same, there's no advantage to be had, but if not, we add advanatages up.
            if (doingType.Equals(targetType)) return valueToReturn;
            else {
                if (doingType.Equals("Sword")) {
                    if (targetType.Equals("Lance")) valueToReturn = -1;
                    else valueToReturn = 1;
                } else if (doingType.Equals("Axe")) {
                    if (targetType.Equals("Sword")) valueToReturn = -1;
                    else valueToReturn = 1;
                } else if (doingType.Equals("Lance")) {
                    if (targetType.Equals("Axe")) valueToReturn = -1;
                    else valueToReturn = 1;
                }
            }
            valueToReturn *= activeMultiplier;
            return valueToReturn;

        }
    }
}