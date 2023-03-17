using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public static class WordBuilder
{
    private static int minWordLengthInList;
    private static int maxWordLengthInList;

    public static List<int> WordSizes = new List<int> {2, 3, 4, 5, 6};
    public static string[] Words;
    public static List<List<string>> _wordLists = new List<List<string>>(); //will store with words by length

    public static List<string> SessionUsedWords = new List<string>();

    private const string fileName = "simplewords";
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
            var filteredDictionary = Words.Where(w => w.Trim().Length == i).ToList();
            if (filteredDictionary.Count > 0)
            {
                Debug.Log($"Found Words Sized {i} count:{filteredDictionary.Count}");
            }

            foreach (var word in filteredDictionary)
            {
                _wordLists[i].Add(word.Trim());
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
    

    //sums subsets algorithm, so we can determine the combinations of the number of lengths that sums up to the target number of characters
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
            var possibleValues=((int[])array.Clone()).ToList(); //make it random instead of sequentially
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
        var wordLengths = new List<int>();
        var combinationResult = new List<List<int>>();
        var wordList = new List<string>();
        
        for (var i = wordMinSize; i < wordMaxSize;i++)
        {
            wordLengths.Add(i);
        }
        

        CombinationSum(wordLengths.ToArray(), maxChars, new List<int>(), combinationResult, 0, 0);

        foreach (var length in combinationResult)
        {
            foreach (var entry in length)
            {
                bool foundCandidate = false;
                string candidateWord = String.Empty;
                while (!foundCandidate)
                {
                    int random = Random.Range(0, _wordLists[entry].Count);
                    candidateWord = _wordLists[entry][random].Trim();
                   
                    if (!SessionUsedWords.Contains(candidateWord) && !wordList.Contains(candidateWord))
                    {
                        foundCandidate = true;
                    }

                    wordList.Add(candidateWord);

                }
            }
        }

        return wordList;
    }
    
}