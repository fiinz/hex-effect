using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

static class SubsetSum
{
    private static Dictionary<int, bool> memo;
    private static Dictionary<int, KeyValuePair<int, int>> prev;

    static SubsetSum()
    {
        memo = new Dictionary<int, bool>();
        prev = new Dictionary<int, KeyValuePair<int, int>>();
    }

    public static bool Find(List<int> inputArray, int sum)
    {
        memo.Clear();
        prev.Clear();

        memo[0] = true;
        prev[0] = new KeyValuePair<int,int>(-1, 0);

        for (int i = 0; i < inputArray.Count; ++i)
        {
            int num = inputArray[i];
            for (int s = sum; s >= num; --s)
            {
                if (memo.ContainsKey(s - num) && memo[s - num] == true)
                {
                    memo[s] = true;

                    if (!prev.ContainsKey(s))
                    {
                        prev[s] = new KeyValuePair<int,int>(i, num);
                    }
                }
            }
        }

        return memo.ContainsKey(sum) && memo[sum];
    }

    public static IEnumerable<int> GetLastResult(int sum)
    {
        while (prev[sum].Key != -1)
        {
            yield return prev[sum].Key;
            sum -= prev[sum].Value;
        }
    }
}

public static class WordBuilder
{
    private static int minWordLengthInList;
    private static int maxWordLengthInList;

    public static List<int> WordSizes = new List<int> {2, 3, 4, 5, 6};
    public static string[] Words;
    public static List<List<string>> _wordLists = new List<List<string>>(); //will store with words by length

    public static List<string> UsedWords = new List<string>();

    private const string fileName = "words";
    // Start is called before the first frame update

    public static void Initialize()
    {
        LoadWordsFromFile(fileName);
        PreProcessSizes();
        CreateWordListsGroupedByLength();
        ShuffleWordLists();
    }

    public static void PreProcessSizes()
    {
        minWordLengthInList = int.MaxValue;
        maxWordLengthInList = int.MinValue;

        if (Words.Length == 0)
        {
            Debug.Log("World Builder --> Words not Loaded ");
            return;
        }

        foreach (string word in Words)
        {
            int length = word.Length;

            if (length < minWordLengthInList)
            {
                minWordLengthInList = length;
            }

            if (length > maxWordLengthInList)
            {
                maxWordLengthInList = length;
            }
        }
    }


    public static string[] LoadWordsFromFile(string file)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(file);

        if (textAsset != null)
        {
            // split the file contents into an array of words
            Words = textAsset.text.Split(',');
            Debug.Log("WordBuilder ---> Success ! Words Loaded");

            // add each word to the list
        }
        else
        {
            Debug.LogError("WordBuilder ---> Could not load file: " + file);
        }

        return Words;
    }

    private static void ShuffleWordLists()
    {
        // loop through each list in _wordLists
        for (int i = 0; i < _wordLists.Count; i++)
        {
            List<string> wordList = _wordLists[i];
            int n = wordList.Count;

            // perform the Fisher-Yates shuffle
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                string temp = wordList[k];
                wordList[k] = wordList[n];
                wordList[n] = temp;
            }
        }
    }

    public static void CreateWordListsGroupedByLength(int wordMinSize = 2, int wordMaxSize = 6)
    {
        for (int i = 0; i <= wordMaxSize; i++)
        {
            _wordLists.Add(new List<string>());
        }

        for (var i = wordMinSize; i <= wordMaxSize; i++)
        {
            var filteredDictionary = Words.Where(w => w.Length == i).ToList();
            if (filteredDictionary.Count > 0)
            {
                Debug.Log($"Found Words Sized {i} count:{filteredDictionary.Count}");
            }

            foreach (var word in filteredDictionary)
            {
                _wordLists[i].Add(word);
            }
        }
    }

    // Update is called once per frame
    public static List<string> GetListOfWords(int maxChars, int wordMinSize = 2, int wordMaxSize = 6)
    {
        wordMinSize = Mathf.Max(wordMinSize, minWordLengthInList);
        wordMaxSize = Mathf.Min(wordMaxSize, maxWordLengthInList);
        return TryWordCombination(maxChars, wordMinSize, wordMaxSize);
    }
    

    private static void CombinationSum(int[] array, int target, List<int> currentList, List<List<int>> results, int sum, int index){
        if (results.Count>0)
        {
            return;
        }
        if (sum > target){
            return;
        }
        else if (sum == target){
            if (!results.Contains(currentList)){
                List<int> newList = new List<int>();
                newList.AddRange(currentList);
                results.Add(newList); return;
            }
        }
        else
        {
            List<int> possibleValues=((int[])array.Clone()).ToList();
            Debug.Log("Possible Values"+possibleValues.Count());
            
            while (possibleValues.Count>0)
            {
                var r = Random.Range(0, possibleValues.Count);
                possibleValues.Remove(possibleValues[r]);
                currentList.Add(array[r]);
                CombinationSum(array, target, currentList, results, sum + array[r], r);
                currentList.Remove(array[r]);
                
            }
        }
    }
  
  
    private static List<string> TryWordCombination(int maxChars, int wordMinSize, int wordMaxSize)
    {
        List<int> values = new List<int>(){2,3,4,5,6};

        List<int> currentList = new List<int>();
        List<List<int>> results = new List<List<int>>();
        int sum = 0;
        int index = 0;
        bool solutionFound = false;

        CombinationSum(values.ToArray(), 50, currentList, results, 0, 0);

        List<string> result = new List<string>();

        foreach (var subset in results)
        {
            foreach (var entry in subset)
            {
                sum += entry;
                Debug.Log($"Entry {entry}");

            }
            Debug.Log($"Subset {sum}");

        }
        /*

        bool foundCanidate = false;
                string candidateWord = String.Empty;
                while (!foundCanidate)
                {
                    int random = Random.Range(0, _wordLists[entry].Count);
                    candidateWord = _wordLists[entry][random];
                    if (!UsedWords.Contains(candidateWord))
                    {
                        foundCanidate = true;
                    }
                }

                result.Add(candidateWord);
            }
                
*/
        return null;
    }

    /*
    private static bool TryWordCombination(List<string> result, int maxChars, int wordMinSize, int wordMaxSize)
    {
        var currentCharacters = result.Sum(w => w.Length);
        var remainingCharacters = maxChars - currentCharacters;
        if (result.Sum(w => w.Length) == maxChars)
        {
            //finished all words
            return true;
        }

        int maxWordMaxSize = Math.Min(remainingCharacters, wordMaxSize);
        int wordSize = Random.Range(wordMinSize, maxWordMaxSize + 1);
        Debug.Log($"wordSize random {wordSize}");

        bool foundCanidate = false;
        string candidateWord = String.Empty;

        while (!foundCanidate)
        {
            foundCanidate = true;
            int random = Random.Range(0, _wordLists[wordSize].Count);
            candidateWord = _wordLists[wordSize][random];
            if (UsedWords.Contains(candidateWord))
            {
                foundCanidate = false;
            }
        }

        if (result.Sum(w => w.Length)+candidateWord.Length > maxChars-wordMinSize)
        {
            return false;
        }

        result.Add(candidateWord);

        if (TryWordCombination(result, maxChars, wordMinSize, wordMaxSize))
        {
            return true;
        }

        result.Remove(candidateWord);
        return false;
    }*/
}