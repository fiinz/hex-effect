using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordBuilder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadDictionary()
    {
        
    }
    // Update is called once per frame
    public List<string> BuildWordList(List<string> dictionary, int maxChars)
    {
        var result = new List<string>();
        Shuffle(dictionary);
        var wordSizes = new List<int> { 2, 3, 4, 5, 6 };
        for (int i = 0; i < 1000; i++) // Try up to 1000 times to generate a valid word list
        {
            Shuffle(wordSizes);
            if (TryWordCombination(result, dictionary, maxChars, wordSizes))
            {
                return result;
            }
        }
        return result;
    }
    public  void Shuffle<T>( IList<T> list)
    {
        var rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    private  bool TryWordCombination(List<string> result, List<string> dictionary, int maxChars, List<int> wordSizes)
    {
        if (result.Sum(w => w.Length) == maxChars)
        {
            return true;
        }
        Shuffle(wordSizes);
        int wordSize = wordSizes[0];
        var filteredDictionary = dictionary.Where(w => w.Length == wordSize).ToList();
        Shuffle(filteredDictionary);
        foreach (var word in filteredDictionary)
        {
            if (result.Sum(w => w.Length) + word.Length > maxChars)
            {
                continue;
            }
            result.Add(word);
         
            if (TryWordCombination(result, dictionary, maxChars, wordSizes))
            {
                return true;
            }
            result.RemoveAt(result.Count - 1);
        }
        return false;
    }


    
    

}
