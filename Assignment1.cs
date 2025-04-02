using System; // Needed for Console.WriteLine and basic system functions

class Stack64U{
    private ulong[] stack;   // Array to store elements (64-bit unsigned int)
    private ulong[] min_stack; // Array to track the minimum value 
    private int size;       // Number of elements currently in the stack
    private int capacity;   // Maximum allocated size of the stack

    // Default constructor with initial capacity of 5
    public Stack64U(){
        capacity = 4;
        stack = new ulong[capacity];
        min_stack = new ulong[capacity];
        size = 0; 
    }

    public void Push(ulong value){
        if (size == stack.Length){
            Resize(stack.Length * 2); // Double capacity when full
        }

        stack[size] = value;

        // Handle minimum value tracking
        if (size == 0 || value < min_stack[size - 1]){
            min_stack[size] = value;
        }
        else{
            min_stack[size] = min_stack[size - 1];
        }

        size++;
    }

    public ulong Pop(){
        if (IsEmpty()){
            throw new Exception("Stack is empty");
        }

        size--; // Decrease size
        ulong value = stack[size]; // Retrieve popped value

        // Resize stack 
        if (size > 0 && size == stack.Length / 2){
            Resize(stack.Length / 2);
        }
        //################### NEW CHANGE ###################
        // removed if(size >0){min_stack[size - 1] = GetMin();}     since GetMin() already retrives the min_stack[size - 1]

        return value;
    }

    public ulong Peek(){ 
        if (IsEmpty()){
            throw new Exception("Stack is empty");
        }
        return stack[size - 1]; //Returnes twe top element without removing it

    }
    public int Count()  //################### NEW CHANGE ###################
    {
        return size; // Return the number of elements in the stack
    }

    public bool IsEmpty(){
        return size == 0;
    }

    public ulong GetMin(){
        if (IsEmpty()){
            throw new Exception("Stack is empty");
        }
        return min_stack[size - 1]; // Return the current minimum
    }

    public void PrintStack()
    {
        for (int i = 0; i < size; i++) {
            Console.WriteLine(stack[i]);
        }
    }

    private void Resize(int newSize){ //Resizes stack and min_stack
        //new arrays
        ulong[] newStack = new ulong[newSize];
        ulong[] newMinStack = new ulong[newSize];
        //copies existing stack to new resised stacks
        Array.Copy(stack, newStack, size);
        Array.Copy(min_stack, newMinStack, size);
        //"pointer" re-ajustment
        stack = newStack;
        min_stack = newMinStack;
    }
}

class List64U{
    private List<ulong> data;   // List to store elements (64-bit unsigned int)
    private List<ulong> min_stack;   // List to track the minimum value 

    // Constructor
    public List64U(){
        data = new List<ulong>();
        min_stack = new List<ulong>();
    }

    public void Push(ulong value){
        data.Add(value);

        // Handle minimum value tracking
        if (min_stack.Count == 0 || value < min_stack[min_stack.Count - 1]){
            min_stack.Add(value);
        }
        else{
            min_stack.Add(min_stack[min_stack.Count - 1]);
        }
    }
    public ulong Pop(){
        if (IsEmpty()){
            throw new Exception("Stack is empty");
        }

        int lastIndex = data.Count - 1;
        ulong value = data[lastIndex];
        data.RemoveAt(lastIndex); // Remove the last element

        // Update the min_stack array after popping
        min_stack.RemoveAt(lastIndex); // Remove the corresponding min value

        return value;
    }

    public ulong Peek(){
        if (IsEmpty()){
            throw new Exception("Stack is empty");
        }
        return data[data.Count - 1]; // Return the top element
    }

    public bool IsEmpty(){
        return data.Count == 0;
    }

    public ulong GetMin(){
        if (IsEmpty()){
            throw new Exception("Stack is empty");
        }
        return min_stack[min_stack.Count - 1]; // Return the current minimum
    }

    public int Count(){
        return data.Count; // Return the number of elements in the stack
    }

    public void PrintStack(){
        foreach (var elem in data){
            Console.WriteLine(elem);
        }
    }
}

class Program{ // Client
    static void Main(){
        Stack64U stack = new Stack64U();
                // Pick Stack or List before starting
        //List64U stack = new List64U();

        stack.Push(10);
        stack.Push(5);
        stack.Push(20);
        stack.Push(2);
        stack.Push(15);

        stack.PrintStack();

        Console.WriteLine("Peek: " + stack.Peek()); // Should print 15 (in this configuratoon)
        Console.WriteLine("Min: " + stack.GetMin()); // Should print 2 (in this configuration)

        stack.Pop();
        Console.WriteLine("Popped: " + stack.Peek()); // 2

        stack.Pop();
        Console.WriteLine("Popped: " + stack.Peek()); // 20

        stack.Pop();
        Console.WriteLine("Popped: " + stack.Peek()); // 5

        Console.WriteLine("Peek: " + stack.Peek()); // 5
        Console.WriteLine("Min: " + stack.GetMin()); // 5
    }
}
