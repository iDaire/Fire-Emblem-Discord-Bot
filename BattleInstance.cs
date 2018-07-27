// What Has Yet To Be Done: Magic vs Magic, Ranged Weaponry, Skills

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

            WeaponInstance Arcthunder = new WeaponInstance("Arcthunder", "Anima", "None", true);
            Arcthunder.setAllStats(new int[] {7, 9, 70, 15, 1});

            CharacterInstance backupCharacter = new CharacterInstance("Ike", Ragnell);
            backupCharacter.setAllStats(new int[] {65, 37, 9, 35, 40, 22, 32, 15});

            CharacterInstance exampleCharacter = new CharacterInstance("Soren", Arcthunder);
            exampleCharacter.setAllStats(new int[] {50, 23, 40, 32, 34, 30, 24, 36});

            CharacterInstance enemyCharacter = new CharacterInstance("Black Knight", Alondite);
            enemyCharacter.setAllStats(new int[] {70, 38, 18, 30, 40, 20, 35, 25});

            Console.WriteLine("{0} and {1} will now fight...", exampleCharacter.name, enemyCharacter.name);
            Console.ReadLine();

            // This Is The Main Battle Process

            int turnLimit = 20;
            CharacterInstance winningCharacter = B.doBattle(exampleCharacter, enemyCharacter, turnLimit);
            
            // Later on, we're going to determine the winner by who has lower health, but for now, this works.
            if (winningCharacter != null) Console.WriteLine("The winner is {0}!", winningCharacter.name);
            else Console.WriteLine("There was no winner...", winningCharacter.name);


            Console.ReadLine(); // Holding the console window open.
        } 

    }

    public class BattleFunctionHolder {
        public Random randomNumberGenerator = new Random();

        public CharacterInstance doBattle (CharacterInstance exampleCharacter, CharacterInstance enemyCharacter, int turnLimit) {
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
                Because there is a for loop inside a for loop, we have to be careful with break positioning. */
                Boolean isBattleOver = false;

                // A character gets to move twice in a round if they have four more speed than the enemy. 
                int numMoves = (calculateAttackSpeed(exampleCharacter) - 4) >= enemyCharacter.stats["SPD"] ? 2 : 1;
                for (int a = 0; a < numMoves; a++) {
                    isBattleOver = doCharacterTurn(exampleCharacter, enemyCharacter);
                    if (isBattleOver) {
                        winningCharacter = exampleCharacter;
                        isBattleOver = true;
                        break;
                    }
                }
                if (isBattleOver) break;
                Console.ReadLine();

                // Let's do this twice.
                numMoves = (calculateAttackSpeed(enemyCharacter) - 4) >= exampleCharacter.stats["SPD"] ? 2 : 1;
                for (int a = 0; a < numMoves; a++) {
                    isBattleOver = doCharacterTurn(enemyCharacter, exampleCharacter);
                    if (isBattleOver) {
                        winningCharacter = enemyCharacter;
                        isBattleOver = true;
                        break;
                    }
                }
                if (isBattleOver) break;
                Console.ReadLine();
            }
            return winningCharacter;
        }

        public Boolean doCharacterTurn (CharacterInstance doingCharacter, CharacterInstance targetCharacter) {
            // The weapon advantages and the hit rate equation were taken from Fire Emblem 7 with tweaks, but the weapon weight affecting speed was taken from Fire Emblem 9
            int hitRate = ((doingCharacter.stats["SKL"]) * 2) + doingCharacter.equippedWeapon.stats["HIT"] + ((doingCharacter.stats["LCK"] - (targetCharacter.stats["LCK"] / 2)) / 2);
            hitRate += 15 * calculateWeaponEffectiveness(doingCharacter.equippedWeapon, targetCharacter.equippedWeapon);
            if (hitRate < 0) hitRate = 0;
            Console.WriteLine(hitRate);

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

            // We're going to need to know if the weapon is magical. MUST ACCOUNT FOR SHOCKSTICKS AND LEVIN SWORDS LATER.
            String attackStat = doingCharacter.equippedWeapon.isMagic ? "INT" : "STR";
            String defenseStat = doingCharacter.equippedWeapon.isMagic ? "RES" : "DEF";

            // Let's calculate the potential damage and also add in weapon advantages again.
            int potentialDamage = (doingCharacter.stats[attackStat] + doingCharacter.equippedWeapon.stats["MGT"]) - targetCharacter.stats[defenseStat]; 
            potentialDamage += 1 * calculateWeaponEffectiveness(doingCharacter.equippedWeapon, targetCharacter.equippedWeapon);
            if (potentialDamage < 0) potentialDamage = 0;

            // Roll two dice, one for hit rate and one for crit rate.
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
                // Let's check if they're a part of the physical weapon triangle.
                if (doingType.Equals("Sword")) {
                    if (targetType.Equals("Lance")) valueToReturn = -1;
                    else if (targetType.Equals("Axe")) valueToReturn = 1;
                    else valueToReturn = 0;
                } else if (doingType.Equals("Axe")) {
                    if (targetType.Equals("Sword")) valueToReturn = -1;
                    else if (targetType.Equals("Lance")) valueToReturn = 1;
                    else valueToReturn = 0;
                } else if (doingType.Equals("Lance")) {
                    if (targetType.Equals("Axe")) valueToReturn = -1;
                    else if (targetType.Equals("Sword")) valueToReturn = 1;
                    else valueToReturn = 0;
                }

                // Well then they might be a part of the magic triangle.
                if (doingType.Equals("Anima")) {
                    if (targetType.Equals("Dark")) valueToReturn = -1;
                    else if (targetType.Equals("Light")) valueToReturn = 1;
                    else valueToReturn = 0;
                } else if (doingType.Equals("Dark")) {
                    if (targetType.Equals("Light")) valueToReturn = -1;
                    else if (targetType.Equals("Anima")) valueToReturn = 1;
                    else valueToReturn = 0;
                } else if (doingType.Equals("Light")) {
                    if (targetType.Equals("Anima")) valueToReturn = -1;
                    else if (targetType.Equals("Dark")) valueToReturn = 1;
                    else valueToReturn = 0;
                }
            }

            valueToReturn *= activeMultiplier;
            return valueToReturn;
        }
    }
}