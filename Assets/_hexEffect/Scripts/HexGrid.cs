using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _hexEffect.Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public struct HexVector
{
    public int row;
    public int col;

    public HexVector(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
}

public class HexGrid : MonoBehaviour
{
    [FormerlySerializedAs("hex")] [SerializeField]
    private Hex startHex;

    [SerializeField] private int rows;
    [SerializeField] private int cols;
    [SerializeField] private bool isPointy;
    [SerializeField] private float innerSize = 1;
    [SerializeField] private float outterSize = 1.5f;
    [SerializeField] private float height = 1.5f;
    [SerializeField] private bool comb = true;
    [SerializeField] private float spaceBetween = 0.1f;
    public Hex[,] Grid;

    //were using row, cols
    private readonly HexVector _upRight = new HexVector(-1, 1);
    private readonly HexVector _upLeft = new HexVector(-1, -1);
    private readonly HexVector _downRight = new HexVector(1, 1);
    private readonly HexVector _downLeft = new HexVector(1, -1);
    private readonly HexVector _left = new HexVector(0, -2);
    private readonly HexVector _right = new HexVector(0, 2);
    private List<HexVector> _directions;

    // Start is called before the first frame updat
    public void Initialize()
    {
        _directions = new List<HexVector>
        {
            _upLeft,
            _upRight,
            _downLeft,
            _downRight,
            _left,
            _right
        };
        if (isPointy)
            CreateGrid();
    }

    char GenerateRandomChar()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int index = Random.Range(0, chars.Length);
        return chars[index];
    }

    public int RemainingOpenSpots
    {
        get { return Grid.Cast<Hex>().Count(hex => hex != null && hex.Model.State == HexState.Empty); }
    }


    public void ResetGrid()
    {
        if (Grid == null || Grid.Length == 0)
        {
            Debug.Log("Grid is Null or Empty --> Can't be Reseted");
        }

        foreach (var hex in Grid)
        {
            if (hex != null)
            {
                hex.Reset();
            }
        }
    }

    private List<Hex> GetFreeHexs()
    {
        return Grid.Cast<Hex>().Where(hex => hex != null && hex.Model.State == HexState.Empty).ToList();
    }


    private HexVector GetStartingPosForWord()
    {
        if (RemainingOpenSpots <= 0)
        {
            Debug.Log("Hex Grid --> No Free Spots To Place Letter");
        }

        var freeHexes = GetFreeHexs();
        var freeHexIndex = Random.Range(0, freeHexes.Count());
        return new HexVector(freeHexes[freeHexIndex].Model.row, freeHexes[freeHexIndex].Model.col);
    }

    public void ShuffleWordList(List<string> words)
    {
        for (int i = 0; i < words.Count; i++)
        {
            int j = Random.Range(i, words.Count);
            string temp = words[i];
            words[i] = words[j];
            words[j] = temp;
        }
    }

    public void PlaceWordsOnGrid(List<string> words)
    {
        if (words.Sum(w => w.Length) > RemainingOpenSpots)
        {
            Debug.Log("List of words exceed the ammount of free spots");
        }

        Debug.Log("Remaining Spots" + RemainingOpenSpots);
        bool allWordsPlaced = false;
        List<string> placedWords;

        int maxAttemps = 100;
        int currentAttempt = 0;

        int maxWordAttempts = 100; // 100 attempts for each word
        while (currentAttempt < maxAttemps && !allWordsPlaced)
        {
            allWordsPlaced = true;
            currentAttempt++;

            foreach (string word in words)
            {
                bool placedWord = false;
                int currentWordAttempt = 0;

                while (!placedWord && currentWordAttempt < maxWordAttempts)
                {
                    currentWordAttempt++;
                    var startPos = GetStartingPosForWord();
                    var color = Random.ColorHSV(0,1,.5f,1,.5f,1);
                    placedWord = TryPlaceWordOnGrid(word, startPos,color);

                }


                if (!placedWord)
                {
                    allWordsPlaced = false;
                    break;
                }
            }

            if (!allWordsPlaced)
            {
                ResetGrid();
                ShuffleWordList(words);
            }
        }
    }

    public bool TryPlaceWordOnGrid(string word, HexVector currentPos,Color color)
    {
        if (word.Length == 0)
        {
            Debug.Log("Hex Grid --> Sucess Putting word");

            // all letters have been placed successfully
            return true;
        }

        if (RemainingOpenSpots < word.Length)
        {
            //  no free spots
            Debug.Log("Hex Grid --> No free Spots to Put this word");
            return false;
        }

        if (currentPos.row < 0 || currentPos.row >= rows || currentPos.col < 0 || currentPos.col >= cols * 2)
        {
            // out of bounds so return false
            return false;
        }

        var currentHex = Grid[currentPos.row, currentPos.col];
        if (currentHex == null)
        {
            // hex is blocked or doesn't exist
            return false;
        }

        if (currentHex.Model.State == HexState.Filled)
        {
            return false;
        }

        currentHex.ChangeColorText(color);
        currentHex.SetLetter(word[0]);
        var possibleDirections = new List<HexVector>(this._directions); // it will
        while (possibleDirections.Count > 0)
        {
            var dir = possibleDirections[Random.Range(0, possibleDirections.Count)];
            int nextCol = currentPos.col + (int) dir.col;
            int nextRow = currentPos.row + (int) dir.row;
            HexVector newCellPos = new HexVector(nextRow, nextCol);
            possibleDirections.Remove(dir); // we remove so it doesn't try in the same direction
            // check if the next hexagon is within the bounds of the grid
            var remainingCharacters = word.Substring(1);
            if (TryPlaceWordOnGrid(remainingCharacters, newCellPos,color))
            {
                return true;
            }
        }

        //couldn't fill the rest of the word using the current cell so we reset and return fasle
        currentHex.Reset();
        return false;
    }


    /*

    public bool PutWordOnGrid(string word)
    {
    
        if (RemainingOpenSpots < word.Length)
        {
            Debug.Log("Hex Grid --> No free Spots to Put this word");
            return false;
        }

        int startCol = Random.Range(0, cols*2);
        int startRow = Random.Range(0, rows);
        List<Hex> placedLettersList = new List<Hex>();
        while (Grid[startRow,startCol] == null || (Grid[startRow,startCol] != null && (Grid[startRow,startCol].Model.State!=HexState.Inactive))){ //we check nulls because of the double coordinates
            startCol = Random.Range(0, cols*2);
            startRow = Random.Range(0, rows);
        }

        int currentCol = startCol;
        int currentRow = startRow;
        Hex currentHex = startHex;
        
        currentHex = Grid[currentRow,currentCol];
        currentHex.ChangeColorText(Color.white);
        currentHex.SetLetter(word[0]);
        
        
        for (int c = 1; c < word.Length; c++)
        {
            bool letterIsPlaced = false;
            int numTries = 0;
            int maxTries = 50; // this needs to be bigger because the direction is always random, and we don't want sequentially 
            while (!letterIsPlaced && numTries<maxTries)
            {
                var dir = _directions[Random.Range(0, _directions.Count)];
                int nextCol = currentCol + (int)dir.y;
                int nextRow = currentRow + (int)dir.x;
                // check if the next hexagon is within the bounds of the grid
                if (nextRow >= 0 && nextRow < rows && nextCol >= 0 && nextCol < cols * 2)
                {
                    Hex nextHex = Grid[nextRow, nextCol];
                    if (nextHex != null && nextHex.Model.State == HexState.Inactive &&
                        !placedLettersList.Contains(nextHex))
                    {
                        currentHex = nextHex;
                        currentCol = nextCol;
                        currentRow = nextRow;
                        currentHex.ChangeColorText(Color.white);
                        currentHex.SetLetter(word[c]);
                        placedLettersList.Add(currentHex);
                        letterIsPlaced = true;
                    }

                }

                numTries++;
            }
            if (!letterIsPlaced)
            {
                // remove the placed letters from the grid
                foreach (var hex in placedLettersList)
                {
                    hex.Reset();
                }
                return false;
            }

        }

        return true;
    }
    */

    private void CreateGrid()
    {
        //we use double coordinates with offset to make it easier to manage the hex grid
        // (col+row)%2==0 constraint
        //meaning we will have the double of columns / or rows depending on the point up

        int rowStep = 1;
        int colStep = 2;


        //rows, cols
        Grid = new Hex[rows * rowStep, cols * colStep];
        transform.position = Vector3.zero;


        for (var row = 0; row < rows * rowStep; row += rowStep)
        {
            //careful we are using double cordinates.

            int rowHexagons = ((int) (cols) - (int) Mathf.Abs(Mathf.Floor(cols / 2f) - row)); // 5 / 2  0 - 1 - 2 


            //find every 4th column ( centered pattern) so the others can be positioned relative to those columns
            //  handle grids with odd numbers of rows and even numbers of columns. 

            int xStart = (cols - 3) % 4 == 0
                ? (int) Mathf.Ceil((cols - (rowHexagons / 2f * colStep)))
                : (int) Mathf.Floor((cols - (rowHexagons / 2f * colStep)));


            int xEnd = xStart + rowHexagons * colStep;

            for (var col = xStart; col < xEnd; col += colStep)
            {
                var hexModel = new HexModel();
                hexModel.InnerSize = this.innerSize;
                hexModel.Height = this.height;
                hexModel.isPointy = this.isPointy;
                hexModel.OuterSize = this.outterSize;
                hexModel.row = row;
                hexModel.col = col;
                var newHex = Instantiate(startHex);
                newHex.gameObject.SetActive(true);
                newHex.Initialize(hexModel);
                var newHexTransformChached = newHex.transform;
                newHexTransformChached.position = GetPositionFromGridPos(row, col);
                newHexTransformChached.SetParent(transform, true);
                Grid[row, col] = newHex;
            }
        }
    }


    public Vector3 GetPositionFromGridPos(int row, int col)
    {
        float width;
        float height;
        float xPosition = 0f;
        float zPosition = 0f;
        bool shouldOfset;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float radius = outterSize; //outersize is the external radius

        var auxCol = isPointy ? col / 2f : col;
        var auxRow = isPointy ? row : row / 2f;

        if (isPointy)
        {
            shouldOfset = auxRow % 2 == 0;
            width = Mathf.Sqrt(3f) * radius; // these can be found on  https://www.redblobgames.com/grids/hexagons/
            height = 2f * radius;
            horizontalDistance = width;
            verticalDistance = height * (3f / 4f);
        }
        else
        {
            shouldOfset = auxCol % 2 == 0;
            height = Mathf.Sqrt(3f) * radius; // these can be found on  https://www.redblobgames.com/grids/hexagons/
            width = 2f * radius;
            verticalDistance = height;
            horizontalDistance = width * (3f / 4f);
        }

        xPosition = (auxCol * (horizontalDistance + spaceBetween));
        zPosition = -(auxRow * (verticalDistance + spaceBetween));
        return new Vector3(xPosition, 0, zPosition);
    }
}