using System;
using System.Collections;
using System.Collections.Generic;

//Dictionary implementation with hash table (chaining)
public class MyDictionary<TKey, TValue> : IDictionary<TKey, TValue>{
    private List<KeyValuePair<TKey, TValue>>[] buckets;
    private int size; //number of buckets
    private int version;  // version to track modifications (in relation to enumeration)

    public MyDictionary(int size = 100){ //initialise dictionary with chosen size
        this.size = size;
        buckets = new List<KeyValuePair<TKey, TValue>>[size];
        for (int i = 0; i < size; i++){
            buckets[i] = new List<KeyValuePair<TKey, TValue>>();
        }
    }
    
    //adds key-value pair
    public void Add(TKey key, TValue value){
        if (key == null) throw new ArgumentNullException(nameof(key), "Key cannot be null.");

        version++;  // increases version on change to invalidate ongoing enumerations
        var index = GetIndex(key);
        var bucket = buckets[index];
        foreach (var pair in bucket){
            if (EqualityComparer<TKey>.Default.Equals(pair.Key, key))
                throw new ArgumentException("An element with the same key already exists.");
        }
        bucket.Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public bool ContainsKey(TKey key){ //checks if key already exists

        var index = GetIndex(key);
        foreach (var pair in buckets[index]){
            if (EqualityComparer<TKey>.Default.Equals(pair.Key, key))
                return true;
        }
        return false;
    }

    public ICollection<TKey> Keys{ //gets all keys
        get{
            var keys = new List<TKey>();
            foreach (var bucket in buckets){
                foreach (var pair in bucket){
                    keys.Add(pair.Key);
                }
            }
            return keys;
        }
    }

    public bool Remove(TKey key){     //removes key 
        if (key == null) throw new ArgumentNullException(nameof(key), "Key cannot be null.");

        version++;
        var index = GetIndex(key);
        var bucket = buckets[index];
        var initialCount = bucket.Count;
        bucket.RemoveAll(pair => EqualityComparer<TKey>.Default.Equals(pair.Key, key));
        return bucket.Count < initialCount;
    }

    public bool TryGetValue(TKey key, out TValue value){ //tries to get the value from a key
        var index = GetIndex(key);
        foreach (var pair in buckets[index]){
            if (EqualityComparer<TKey>.Default.Equals(pair.Key, key)){
                value = pair.Value;
                return true;
            }
        }
        value = default;
        return false;
    }

    public ICollection<TValue> Values{ //gets all values in dictionary
        get{
            var values = new List<TValue>();
            foreach (var bucket in buckets){
                foreach (var pair in bucket){
                    values.Add(pair.Value);
                }
            }
            return values;
        }
    }

    public TValue this[TKey key]{ //index for value acess by the key
        get{
            if (key == null) throw new ArgumentNullException(nameof(key), "Key cannot be null.");

            if (TryGetValue(key, out TValue value))
                return value;
            throw new KeyNotFoundException("The given key was not present in the dictionary.");
        }
        set{
            if (key == null) throw new ArgumentNullException(nameof(key), "Key cannot be null.");

            version++;
            var index = GetIndex(key);
            var bucket = buckets[index];
            for (int i = 0; i < bucket.Count; i++){
                if (EqualityComparer<TKey>.Default.Equals(bucket[i].Key, key)){
                    bucket[i] = new KeyValuePair<TKey, TValue>(key, value);
                    return;
                }
            }
            Add(key, value); // Add new key using central Add method ##############
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item)    {    // Adds key-value pair
        Add(item.Key, item.Value);
    }

    public void Clear(){ // Clears all items from the dictionary
        foreach (var bucket in buckets){
            bucket.Clear();
        }
        version++;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item){ // Checks for a specific KeyValuePair 
        var index = GetIndex(item.Key);
        foreach (var pair in buckets[index]){
            if (EqualityComparer<TKey>.Default.Equals(pair.Key, item.Key) &&
                EqualityComparer<TValue>.Default.Equals(pair.Value, item.Value))
                return true;
        }
        return false;
    }
    
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex){ // Copies all key-value pairs to an array from specified array index
        foreach (var bucket in buckets){
            foreach (var pair in bucket){
                if (arrayIndex >= array.Length) throw new ArgumentException("Array is too small.");
                array[arrayIndex++] = pair;
            }
        }
    }

    public int Count{ //Count of key-value pairs
        get{
            int count = 0;
            foreach (var bucket in buckets){
                count += bucket.Count;
            }
            return count;
        }
    }

    public bool IsReadOnly => false; // Indicates dictionary is read-only

    public bool Remove(KeyValuePair<TKey, TValue> item) { // Removes specific KeyValuePair
        var index = GetIndex(item.Key);
        var bucket = buckets[index];
        return bucket.Remove(new KeyValuePair<TKey, TValue>(item.Key, item.Value));
    }

    // Enumerator to iterate through all key-value pairs
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator(){
        int currentVersion = version;
        foreach (var bucket in buckets){
            foreach (var pair in bucket){
                if (currentVersion != version){
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                yield return pair;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator(){
        return GetEnumerator();
    }

    private int GetIndex(TKey key){ //bucket index based on key has code
        int hashCode = key.GetHashCode();
        int index = Math.Abs(hashCode % size);
        return index;
    }
}

public class GeoLocation{ //represent a location with lat. & long.
    public double Latitude { get; }
    public double Longitude { get; }

    public GeoLocation(double latitude, double longitude){
        Latitude = latitude;
        Longitude = longitude;
    }

    public override bool Equals(object obj){
        if (obj is GeoLocation other)
            return Latitude == other.Latitude && Longitude == other.Longitude;
        return false;
    }

    public override int GetHashCode(){
        return HashCode.Combine(Latitude, Longitude);
    }
}

class Program{
    static void Main(){
        // Instantiate a new MyDictionary
        var charMap = new MyDictionary<char, double>();

        //Adding elements
        charMap.Add('a', 1.1);
        charMap.Add('b', 2.2);
        Console.WriteLine("After adding elements:");
        PrintDictionary(charMap);

        //Updating an element
        charMap['b'] = 3.3;
        Console.WriteLine("\nAfter updating 'b':");
        PrintDictionary(charMap);

        //Retrieval and error handling for non-existent key
        try{
            Console.WriteLine("\nAttempt to retrieve value for key 'c':");
            double value = charMap['c'];  // This should "throw KeyNotFoundException"
        }
        catch (KeyNotFoundException){
            Console.WriteLine("Key 'c' not found as expected.");
        }

        // Removal of element
        bool isRemoved = charMap.Remove('a');
        Console.WriteLine($"\nAfter removing 'a': Removal successful? {isRemoved}");
        PrintDictionary(charMap);

        //non-existent key removal
        isRemoved = charMap.Remove('z');  // 'z' does not exist
        Console.WriteLine($"\nAttempt to remove non-existent key 'z': Removal successful? {isRemoved}");

        //Adding duplicate key
        try{
            charMap.Add('b', 4.4);
        }
        catch (ArgumentException ex){
            Console.WriteLine("\nAttempt to add duplicate key 'b': " + ex.Message);
        }

        // Display final state
        Console.WriteLine("\nFinal state of the dictionary:");
        PrintDictionary(charMap);
    }

    //print all keys and values from dictionary
    static void PrintDictionary(MyDictionary<char, double> map){
        foreach (var key in map.Keys){
            Console.WriteLine($"Key: {key}, Value: {map[key]}");
        }
    }
}
