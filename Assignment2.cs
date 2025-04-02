using System;
using System.Collections.Generic;

public abstract class Character{ // Base class for all characters
    public string Name { get; } // Character's name (immutable)
    public Race Race { get; } // Character's race
    private int _hp;
    private double _experienceLevel;
    private static Random random = new Random();

    public int HP{
        get => _hp;
        protected set => _hp = Math.Clamp(value, 0, Race.InitialHP); // Allows modification within derived classes
    }
    public void Heal(int amount){   //healing logic
        HP = Math.Min(Race.InitialHP, HP + amount);
    }

    public double ExperienceLevel{
        get => _experienceLevel;
        private set => _experienceLevel = Math.Clamp(value, InitialXP, 10); // Ensure XP is between initial XP and 10
    }

    protected abstract double InitialXP { get; }

    public void GainXP(double amount){  //XP gain logic
        ExperienceLevel = Math.Max(InitialXP, Math.Min(ExperienceLevel + amount, 10));
    }

    protected Character(string name, Race race){ // Character constructor
        Name = name;
        Race = race;
        HP = race.InitialHP; // Set HP to race default
        ExperienceLevel = InitialXP;
    }

    public abstract int OnAttack(); // Handles attack logic
    public abstract int OnDefense(); // Handles defense logic

    protected abstract double AttackFormula(); 
    protected abstract double DefenseFormula();

    public override string ToString(){ // Returns character details
        return $"{Name} ({GetType().Name}) - Race: {Race.GetType().Name}, Exp: {ExperienceLevel:F2}, HP: {HP}";
    }

    public void TakeDamage(int damage){ // Handles taking damage
        HP = Math.Max(0, HP - damage);
    }
}



public class Race{ // Base class for all races
    public int Strength { get; }
    public int Agility { get; }
    public int Intelligence { get; }
    public int InitialHP { get; }
    public List<string> VictoryMessages { get; }

    public Race(int strength, int agility, int intelligence, int initialHP, List<string> messages){ // Initializes race stats and victory messages
        Strength = strength;
        Agility = agility;
        Intelligence = intelligence;
        InitialHP = initialHP;
        VictoryMessages = messages;
    }

    public string GetRandomVictoryMessage(){ // Chooses a random victory message
        return VictoryMessages[new Random().Next(VictoryMessages.Count)];
    }
}

// Race Implementations
public class Fairy : Race{
    public Fairy() : base(2, 5, 9, 20, new List<string> { "The fairies rejoice!", "The fairies outsmart their foes!" }) { }
}

public class Orc : Race{
    public Orc() : base(10, 3, 2, 40, new List<string> { "Orcs dominate!", "Strength is supreme!" }) { }
}

public class Elf : Race{
    public Elf() : base(4, 7, 6, 30, new List<string> { "The wisdom of the elves prevails!", "The elves never miss their mark!" }) { }
}

// Character Categories
public class Warrior : Character{// Warrior Class

    protected override double InitialXP => 3.70;

    public Warrior(string name, Race race) : base(name, race) { }

    protected override double AttackFormula() => 0.6 * Race.Strength + 0.3 * Race.Agility + 0.1 * Race.Intelligence;
    protected override double DefenseFormula() => 0.3 * Race.Strength + 0.3 * Race.Agility + 0.2 * Race.Intelligence;

    public override int OnAttack(){
        Console.WriteLine($"{Name} swings a mighty sword!");
        return (int)(AttackFormula() * (new Random().NextDouble() * ExperienceLevel)); //return value directly :))
    }

    public override int OnDefense(){
        Console.WriteLine($"{Name} blocks with a shield!");
        return (int)(DefenseFormula() * (new Random().NextDouble() * ExperienceLevel));
    }
}

public class Mage : Character{// Mage Class
    protected override double InitialXP => 2.75;

    public Mage(string name, Race race) : base(name, race) { }

    protected override double AttackFormula() => 0.2 * Race.Strength + 0.2 * Race.Agility + 1.0 * Race.Intelligence;
    protected override double DefenseFormula() => 0.1 * Race.Strength + 0.4 * Race.Agility + 0.8 * Race.Intelligence;

    public override int OnAttack(){
        Console.WriteLine($"{Name} casts a fireball!");
        return (int)(AttackFormula() * (new Random().NextDouble() * ExperienceLevel));
    }

    public override int OnDefense(){
        Console.WriteLine($"{Name} creates a magical barrier!");
        return (int)(DefenseFormula() * (new Random().NextDouble() * ExperienceLevel));
    }
}

public class Archer : Character{// Archer Class
    protected override double InitialXP => 3.15;

    public Archer(string name, Race race) : base(name, race) { }

    protected override double AttackFormula() => 0.3 * Race.Strength + 0.7 * Race.Agility + 0.2 * Race.Intelligence;
    protected override double DefenseFormula() => 0.2 * Race.Strength + 0.7 * Race.Agility + 0.4 * Race.Intelligence;

    public override int OnAttack(){
        Console.WriteLine($"{Name} shoots an arrow!");
        return (int)(AttackFormula() * (new Random().NextDouble() * ExperienceLevel));
    }

    public override int OnDefense(){
        Console.WriteLine($"{Name} dodges swiftly!");
        return (int)(DefenseFormula() * (new Random().NextDouble() * ExperienceLevel));
    }
}

public class Thief : Character{// Thief Class (Custom)
    protected override double InitialXP => 3.40;

    public Thief(string name, Race race) : base(name, race) { }

    protected override double AttackFormula() => 0.2 * Race.Strength + 0.6 * Race.Agility + 0.5 * Race.Intelligence;
    protected override double DefenseFormula() => 0.1 * Race.Strength + 0.8 * Race.Agility + 0.3 * Race.Intelligence;

    public override int OnAttack(){
        Console.WriteLine($"{Name} throws a dagger!");
        return (int)(AttackFormula() * (new Random().NextDouble() * ExperienceLevel));
    }

    public override int OnDefense(){
        Console.WriteLine($"{Name} dodges swiftly!");
        return (int)(DefenseFormula() * (new Random().NextDouble() * ExperienceLevel));
    }
}


// Tournament Class
public class Tournament{
    private List<Character> participants; //list of participants
    private Random random = new Random(); //random fighter selection

    public Tournament(List<Character> characters){ // Initialising tournament constructor
        participants = new List<Character>(characters);
    }

    public void Start(){
        Console.WriteLine("Tournament begins!");

        while (participants.Count > 1){ //loop until 1 fighter is left
            Character fighter1 = participants[random.Next(participants.Count)];
            Character fighter2;

            do{ //makes sure that fighter2 is != fighter1
                fighter2 = participants[random.Next(participants.Count)];
            } while (fighter1 == fighter2);

            Fight(fighter1, fighter2); //fight start
        }

        Console.WriteLine($"\nTournament Winner: {participants[0].Name}");
        Console.WriteLine(participants[0].Race.GetRandomVictoryMessage());
    }

    private void Fight(Character c1, Character c2){
        Console.WriteLine($"\n{c1.Name} VS {c2.Name}");

        int round = 1;

        // Fights until one fighter's health is 0 or 10 rounds (max) is reached
        while (c1.HP > 0 && c2.HP > 0 && round <= 10){
            Console.WriteLine($"\nRound {round}");

            //PROPERLY RETURNS ATTACK AND DEFENCE POINTS FOR DEALING DAMAGE

            // Fighter 1 attacks Fighter 2
            int attack1 = c1.OnAttack(); // For fighter 1 
            int defense2 = c2.OnDefense(); // For fighter 2
            int damageTo2 = Math.Max(0, attack1 - defense2);
            c2.TakeDamage(damageTo2); // HP reduction
            Console.WriteLine($"{c1.Name} deals {damageTo2} damage! {c2.Name} has {c2.HP} HP left.");

            if (c2.HP <= 0) break;

            //PROPERLY RETURNS ATTACK AND DEFENCE POINTS FOR DEALING DAMAGE

            // Fighter 2 attacks Fighter 1
            int attack2 = c2.OnAttack(); // For fighter 2
            int defense1 = c1.OnDefense(); // For fighter 1
            int damageTo1 = Math.Max(0, attack2 - defense1);
            c1.TakeDamage(damageTo1); // HP reduction
            Console.WriteLine($"{c2.Name} deals {damageTo1} damage! {c1.Name} has {c1.HP} HP left.");
            round++;
        }

        if (c1.HP > 0 && c2.HP > 0){
            // Fight ended in a draw
            c1.GainXP(0.25);
            c2.GainXP(0.25);
            Console.WriteLine($"{c1.Name} and {c2.Name} fought to a draw!");
            Console.WriteLine($"{c1.Name} and {c2.Name} gain 0.25 XP each!");
        }
        else{
            // Determine winner and loser
            Character winner = c1.HP > 0 ? c1 : c2;
            Character loser = c1.HP <= 0 ? c1 : c2;

            // Announce winner and loser
            Console.WriteLine($"\n{winner.Name} wins the match against {loser.Name}!");
            Console.WriteLine($"{winner.Name} gains 0.50 XP!");

            // Apply HP regeneration
            double healingFactor = 0.8; //(80 % of lost HP)
            int lostHP = winner.Race.InitialHP - winner.HP;
            int healedHP = (int)(lostHP * healingFactor);
            winner.Heal(healedHP);
            Console.WriteLine($"{winner.Name} heals {healedHP} HP!");

            // Remove the defeated participant
            participants.Remove(loser);
        }
    }
}


// Main Program
class Program{
    static void Main(){
        List<Character> characters = new List<Character> {
            new Warrior("Laios", new Orc()),
            new Mage("Marcille", new Elf()),
            new Archer("Chilchuck", new Fairy()),
            new Warrior("Senshi", new Elf()),
            new Mage("Thistle", new Fairy()),
            new Archer("Kabru", new Orc()),
            new Thief("Izutsumi", new Elf())
        };
        Tournament tournament = new Tournament(characters);
        tournament.Start();
    }
}
