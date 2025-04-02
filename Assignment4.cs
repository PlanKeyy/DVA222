//Code by Samuel Worku
using System;
using System.Collections.Generic;
using System.Linq;

// (Abstract) base class for sliding puzzle games
// generick: T so it can be int for number game and console color for color game
public abstract class SlidingPuzzleBase<T>{ 
    protected int Size;     // adjustable size
    protected T[,] Board;
    protected (int row, int col) EmptyTile;
    protected int Moves;    //Move counter
    protected static Random random = new Random();

    protected T EmptyValue; // the empty user tile

    public SlidingPuzzleBase(int size, T emptyValue){   //Constructor/Builder-BOB
        Size = size;
        EmptyValue = emptyValue;
        Board = new T[size, size];
        InitializeBoard();
    }

    // Board is made solvable, by shuffling from an already solved solution
    protected void ShuffleByMoves(int steps){
        var keys = new[] { ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.UpArrow, ConsoleKey.DownArrow };
        for (int i = 0; i < steps; i++){
            MoveTile(keys[random.Next(keys.Length)]);
        }
    }

    //Subclass logiccc, implemented by each game variant
    protected abstract void InitializeBoard();  
    protected abstract bool IsSolved();
    protected abstract void PrintTile(T value);

    public void Play(){ //game loop
        ShuffleByMoves(Size * Size * 10);
        Moves = 0;
        while (true){
            Console.Clear();
            PrintBoard();

            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Escape) break;

            if (MoveTile(key)) Moves++;

            if (IsSolved()){
                Console.Clear();
                PrintBoard();
                Console.WriteLine($"You solved the puzzle in {Moves} moves!");
                break;
            }
        }
    }

    protected bool MoveTile(ConsoleKey key){    // Movement of empty tile
        (int newRow, int newCol) = key switch{
            ConsoleKey.UpArrow => (EmptyTile.row - 1, EmptyTile.col),
            ConsoleKey.DownArrow => (EmptyTile.row + 1, EmptyTile.col),
            ConsoleKey.LeftArrow => (EmptyTile.row, EmptyTile.col - 1),
            ConsoleKey.RightArrow => (EmptyTile.row, EmptyTile.col + 1),
            _ => EmptyTile
        };

        // Ckeck for staying within boundries board
        if (newRow >= 0 && newRow < Size && newCol >= 0 && newCol < Size){
            
            // Swap empty tile with neighbor
            (Board[EmptyTile.row, EmptyTile.col], Board[newRow, newCol]) = (Board[newRow, newCol], Board[EmptyTile.row, EmptyTile.col]);
            EmptyTile = (newRow, newCol);
            return true;
        }
        return false;
    }

    protected void PrintBoard(){
        Console.WriteLine();
        for (int r = 0; r < Size; r++){
            for (int c = 0; c < Size; c++){
                Console.Write(" ");
                PrintTile(Board[r, c]);
                Console.Write(" ");
            }
            Console.WriteLine("\n");
        }
        Console.ResetColor();
    }
}

public class SlidingPuzzleNumberGame : SlidingPuzzleBase<int>{
    public SlidingPuzzleNumberGame(int size) : base(size, 0) { }

    // Filling board from 1 to size from left to right, top to bottom, last is empty user tile
    protected override void InitializeBoard(){
        int counter = 1;
        for (int r = 0; r < Size; r++){
            for (int c = 0; c < Size; c++){
                if (r == Size - 1 && c == Size - 1){
                    Board[r, c] = EmptyValue;
                    EmptyTile = (r, c);
                }
                else{
                    Board[r, c] = counter++;
                }
            }
        }
    }

    // win condition checking for ascending orderr, excluding movment tile
    protected override bool IsSolved(){
        int expected = 1;
        for (int r = 0; r < Size; r++){
            for (int c = 0; c < Size; c++){
                if (r == Size - 1 && c == Size - 1)
                    return true;

                if (!Board[r, c].Equals(expected))
                    return false;

                expected++;
            }
        }
        return true;
    }

    protected override void PrintTile(int value){
        if (value == 0){
            Console.Write("[  ]");  // Empty/movment tile 
        }
        else if (value > 9){
            Console.Write($"[{value}]");    //doubble digit
        }
        else{
            Console.Write($"[ {value}]");   //single digit
        }
    }
}

// Color-based sliding puzzle
public class SlidingPuzzleColorGame : SlidingPuzzleBase<ConsoleColor>{
    private List<ConsoleColor> usedColors;  // track to avoid to many dupes
    private ConsoleColor[] flatGrid;    // used for win condition check

    public SlidingPuzzleColorGame(int size) : base(size, ConsoleColor.Black){
        flatGrid = new ConsoleColor[Size * Size];
        usedColors = new List<ConsoleColor>();
    }

    //fills board with shuffled pairs (one un-paired in odd-size boards)
    protected override void InitializeBoard(){
        ConsoleColor[] availableColors ={
            ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Green,
            ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Magenta,
            ConsoleColor.Gray, ConsoleColor.White, ConsoleColor.DarkBlue,
            ConsoleColor.DarkRed, ConsoleColor.DarkGreen, ConsoleColor.DarkYellow
        };

        List<ConsoleColor> tiles = new List<ConsoleColor>();
        int totalTiles = Size * Size - 1;

        // adds pairs of colors for groupabilityyy
        int colorIndex = 0;
        while (tiles.Count + 2 <= totalTiles){
            var color = availableColors[colorIndex % availableColors.Length];
            tiles.Add(color);
            tiles.Add(color);
            colorIndex++;
        }

        if (tiles.Count < totalTiles){  // leftover color in odd-sized boards
            tiles.Add(availableColors[colorIndex % availableColors.Length]);
        }

        // shuffles tiles with empty tile at the end
        tiles = tiles.OrderBy(_ => random.Next()).ToList();
        tiles.Add(EmptyValue);

        for (int r = 0, i = 0; r < Size; r++){  // placing shuffled tiles
            for (int c = 0; c < Size; c++, i++){
                Board[r, c] = tiles[i];
                if (Board[r, c].Equals(EmptyValue)) EmptyTile = (r, c);
            }
        }
    }

    //checks same-colored tiles are adjacent
    protected override bool IsSolved(){
        FlattenBoard();

        for (int i = 0; i < flatGrid.Length - 1; i++){
            var current = flatGrid[i];
            var next = flatGrid[i + 1];
            if (!current.Equals(next)){
                for (int j = i + 2; j < flatGrid.Length; j++){
                    if (flatGrid[j].Equals(current))
                        return false;
                }
            }
        }
        // empty tile must be in bottom right
        return EmptyTile.row == Size - 1 && EmptyTile.col == Size - 1;
    }

    // flattens board for linear color grouping check
    private void FlattenBoard(){
        int index = 0;
        for (int r = 0; r < Size; r++){
            for (int c = 0; c < Size; c++){
                flatGrid[index++] = Board[r, c];
            }
        }
    }

    // prints each tile using its background color
    protected override void PrintTile(ConsoleColor color){
        if (color.Equals(EmptyValue)){
            Console.Write("[ ]");
            return;
        }

        Console.BackgroundColor = color;
        Console.Write("   ");   // becomes solid block of color
        Console.ResetColor();
    }
}

// Game entry point
class Program{
    static void Main(){
        while (true){
            Console.Clear();
            Console.WriteLine("Sliding Puzzle Games");
            Console.WriteLine("Choose game mode: ");
            Console.WriteLine("1. Number Puzzle");
            Console.WriteLine("2. Color Puzzle");
            Console.Write("Enter your choice (1 or 2): ");
            string? modeInput = Console.ReadLine();

            // Input validator
            if (modeInput != "1" && modeInput != "2"){
                Console.WriteLine("Invalid choice. Press any key to try again.");
                Console.ReadKey();
                continue;
            }

            int size = 0;
            while (size < 3){
                Console.Write("Enter board size (e.g., 3 for 3x3): ");
                if (!int.TryParse(Console.ReadLine(), out size) || size < 3){
                    Console.WriteLine("Invalid size. Please enter an integer >= 3.");
                }
            }

            // launch selected game mode
            if (modeInput == "1"){
                SlidingPuzzleBase<int> game = new SlidingPuzzleNumberGame(size);
                game.Play();
            }
            else{
                SlidingPuzzleBase<ConsoleColor> game = new SlidingPuzzleColorGame(size);
                game.Play();
            }

            // wanna play again ?
            Console.WriteLine("Play again? (y/n): ");
            if (Console.ReadKey().KeyChar != 'y')
                break;
        }
    }
}
