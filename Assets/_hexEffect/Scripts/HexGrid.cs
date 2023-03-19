using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _hexEffect.Scripts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class HexVector
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
    public Action GridFilled;
    [SerializeField] private HexView hexViewPrefab;
    [SerializeField] private float innerSize = .9f;
    [SerializeField] private float outterSize = 1f;
    [SerializeField] private float height = 0.1f;
    [SerializeField] private float spaceBetween = 0.1f;

    private readonly HexVector _upRight = new HexVector(-1, 1);
    private readonly HexVector _upLeft = new HexVector(-1, -1);
    private readonly HexVector _downRight = new HexVector(1, 1);
    private readonly HexVector _downLeft = new HexVector(1, -1);
    private readonly HexVector _left = new HexVector(0, -2);
    private readonly HexVector _right = new HexVector(0, 2);
    private List<HexVector> _directions;
    private List<HexView> _hexViews;
    private HexModel[,] _grid;
    private int _rows;
    private int _cols;
    private bool _isPointy;

    // Start is called before the first frame updat
    public void Initialize(int rows, int cols, bool isPointy = true)
    {
        this._isPointy = isPointy; //future work for flat hexagons
        this._rows = rows;
        this._cols = cols;

        InitializeDirections();
        CreateGrid();
        ResetGrid();
        CreateViews();
        
    }

    
    private void InitializeDirections()
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
    }

    char GenerateRandomChar()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int index = Random.Range(0, chars.Length);
        return chars[index];
    }

    public int RemainingOpenSpots
    {
        get { return _grid.Cast<HexModel>().Count(hex => hex != null && hex.State == HexState.Empty); }
    }


    public void ResetGrid()
    {
        if (_grid == null || _grid.Length == 0)
        {
            Debug.Log("Grid is Null or Empty --> Can't be Reseted");
        }

        foreach (var hex in _grid)
        {
            if (hex != null)
            {
                hex.ResetModel();
            }
        }
    }

    private List<HexModel> GetFreeHexs()
    {
        return _grid.Cast<HexModel>().Where(hex => hex != null && hex.State == HexState.Empty).ToList();
    }


    private HexModel GetStartingPosForWord()
    {
        if (RemainingOpenSpots <= 0)
        {
            Debug.Log("Hex Grid --> No Free Spots To Place Letter");
        }

        var freeHexes = GetFreeHexs();
        var freeHexIndex = Random.Range(0, freeHexes.Count());
        return freeHexes[freeHexIndex];
    }


    private HexModel GetStartingPosForWord2()
    {
        if (RemainingOpenSpots <= 0)
        {
            Debug.Log("Hex Grid --> No Free Spots To Place Letter");
        }

        HexModel hex = null;
        bool isValid = false;
        while (!isValid)
        {
            var randomRow = Random.Range(0, _rows);
            var randomCol = Random.Range(0, _cols * 2);
            hex = _grid[randomRow, randomCol];
            if (hex != null && hex.State == HexState.Empty)
            {
                isValid = true;
            }
        }

        return hex;
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
        Debug.Log("Placing Words...");

        if (words.Count == 0)
        {
            Debug.LogError("HexGrid --> Error No words were found !");
            return;
        }

        if (words.Sum(w => w.Length) > RemainingOpenSpots)
        {
            //double safe just for the game not to crash
            Debug.Log("List of words exceed the ammount of free spots");
            return;
        }

        bool allWordsPlaced = false;
        List<string> placedWords;
        List<HexModel> visitedHexs = new List<HexModel>();
        int maxAttemps = 1000;
        int currentAttempt = 0;
        int maxWordAttempts = 200; // 100 attempts for each word
        while (!allWordsPlaced && currentAttempt < maxAttemps)
        {
            allWordsPlaced = true;
            currentAttempt++;

            foreach (string word in words)
            {
                bool placedWord = false;
                int currentWordAttempt = 0;
                visitedHexs.Clear();

                var startHex = GetStartingPosForWord2();
                if (!visitedHexs.Contains(startHex))
                {
                    visitedHexs.Add(startHex);
                    var startPos = new HexVector(startHex.row, startHex.col);
                    placedWord = TryPlaceWordOnGrid(word, words.IndexOf(word), startPos);
                    //yield return new WaitForSeconds(.1f);
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
            }
        }

        if (!allWordsPlaced)
        {
            Debug.LogError("HexGrid --> Not solution was found to put words!");
            return;
        }
    }


    public bool TryPlaceWordOnGrid(string word, int wordIndex, HexVector currentPos,
        int currentCharIndex = 0)
    {
        if (word.Length == 0)
        {
            // all letters have been placed successfully
            return true;
        }

        if (currentPos.row < 0 || currentPos.row >= _rows || currentPos.col < 0 || currentPos.col >= _cols * 2)
        {
            // out of bounds so return false
            return false;
        }

        var currentHex = _grid[currentPos.row, currentPos.col];
        if (currentHex == null)
        {
            // hex is blocked or doesn't exist
            return false;
        }

        if (currentHex.State == HexState.Filled)
        {
            return false;
        }


        currentHex.SetLetter(word[0], currentCharIndex, wordIndex);
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
            if (TryPlaceWordOnGrid(remainingCharacters, wordIndex, newCellPos, ++currentCharIndex))
            {
                return true;
            }
        }

        //couldn't fill the rest of the word using the current cell so we reset and return fasle
        currentHex.ResetModel();
        return false;
    }


    private void CreateGrid()
    {
        //we use double coordinates with offset to make it easier to manage the hex grid
        // (col+row)%2==0 constraint
        //meaning we will have the double of columns / or rows depending on the point up

        int rowStep = 1;
        int colStep = 2;


        //rows, cols
        _grid = new HexModel[_rows * rowStep, _cols * colStep];
        transform.position = Vector3.zero;


        for (var row = 0; row < _rows * rowStep; row += rowStep)
        {
            //careful we are using double cordinates.

            int rowHexagons = ((int) (_cols) - (int) Mathf.Abs(Mathf.Floor(_cols / 2f) - row)); // 5 / 2  0 - 1 - 2 


            //find every 4th column ( centered pattern) so the others can be positioned relative to those columns
            //  handle grids with odd numbers of rows and even numbers of columns. 

            int xStart = (_cols - 3) % 4 == 0
                ? (int) Mathf.Ceil((_cols - (rowHexagons / 2f * colStep)))
                : (int) Mathf.Floor((_cols - (rowHexagons / 2f * colStep)));


            int xEnd = xStart + rowHexagons * colStep;

            for (var col = xStart; col < xEnd; col += colStep)
            {
                var hexModel = new HexModel();
                hexModel.InnerSize = this.innerSize;
                hexModel.Height = this.height;
                hexModel.isPointy = this._isPointy;
                hexModel.OuterSize = this.outterSize;
                hexModel.row = row;
                hexModel.col = col;
                _grid[row, col] = hexModel;
            }
        }
    }

    public void CreateViews()
    {
        if (_grid == null || _grid.Length == 0)
        {
            Debug.Log("No elements in the Grid");
            return;
        }

        if (_hexViews == null)
        {
            _hexViews = new List<HexView>();

            foreach (var hexModel in _grid)
            {
                if (hexModel == null) continue;
                Debug.Log("Hex Model Found");
                var newHexView = Instantiate(hexViewPrefab);
                newHexView.Initialize(hexModel);
                var newHexTransformChached = newHexView.transform;
                newHexTransformChached.position = GetHexViewPositionFromGridPos(hexModel.row, hexModel.col);
                newHexTransformChached.SetParent(transform, true);
                _hexViews.Add(newHexView);
            }
        }
    }

    public IEnumerator DisplayGrid()
    {
        if (_hexViews == null)
        {
            yield break;
        }

        foreach (var hexView in _hexViews)
        {
            if (hexView == null) continue;
            yield return new WaitForSeconds(.05f);
            hexView.gameObject.SetActive(true);

        }

        yield return null;
    }


    public IEnumerator HideGrid()
    {
        foreach (var hexView in _hexViews)
        {
            if (hexView == null) continue;
            hexView.UnSelect();
            hexView.gameObject.SetActive(false);

        }
        yield return null;
    }


    public Vector3 GetHexViewPositionFromGridPos(int row, int col)
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

        var auxCol = _isPointy ? col / 2f : col;
        var auxRow = _isPointy ? row : row / 2f;

        if (_isPointy)
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

    public List<HexModel> currentHexSelection = new List<HexModel>();

    public bool AreHexsAdjacent(HexModel hexA, HexModel hexB)
    {
        var startPos = new HexVector(hexA.row, hexA.col);
        foreach (var direction in _directions)
        {
            var row = startPos.row + direction.row;
            var col = startPos.col + direction.col;
            if (row < 0 || row >= _rows || col < 0 || col >= _cols * 2)
            {
                continue;
            }

            var neighbour = _grid[row, col];
            if (neighbour != null && neighbour == hexB)
            {
                return true;
            }
        }

        return false;
    }

    public void DeselectAllHexViews()
    {
        foreach (var hex in _hexViews)
        {
            hex.UnSelect();
        }
    }

    
    public void SelectHex(HexView hex,Color color)
    {
        hex.Select(color);
    }


    // i want states to update views.
    public void UnselectHex(HexModel target)
    {
        foreach (var hex in _hexViews)
        {
            if (target == hex.Model && target.State != HexState.Blocked)
            {
                hex.UnSelect();
            }
        }
    }
}