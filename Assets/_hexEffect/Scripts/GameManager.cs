using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _hexEffect.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private List<string> words;
        private List<string> _foundWords;

        [SerializeField] private int _minWordSize;
        [SerializeField] private int _maxWordSize;
        [SerializeField] private int rows;
        [SerializeField] private int cols;
        [SerializeField] private int maxTime;
        [SerializeField] private int bonusTimePerWordGuessed;
        [SerializeField] private Color[] selectionColorsPallete;
        [SerializeField] private HexGrid _gridManager;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private VolumeProfile _volumeProfile;

        private float _nextRayCast = 0.0f;
        private float nextRayCastFreqency = 0.005f;
        private readonly List<HexModel> _currentHexSelection = new List<HexModel>();
        private Color _currentSelectionColor;
        private Bloom _bloom;
        private int _currentLevel;

        private Coroutine _timerCoroutine;
       [SerializeField] private float _remainingTime;
       [SerializeField] private float _timeStep;
        private readonly WaitForSeconds _oneSecond = new WaitForSeconds(1);

        private void Start()
        {
            _currentLevel = 1;
            _nextRayCast = Time.time + nextRayCastFreqency;
            WordBuilder.Initialize();
            _gridManager.Initialize(rows, cols);
            _uiManager.Initialize(_minWordSize, _maxWordSize, _currentLevel, maxTime);
            _uiManager.StartGameClicked += OnStartGameClicked;
            _uiManager.TryAgainClicked += OnTryAgainClicked;
            _currentSelectionColor = selectionColorsPallete[0];
            _volumeProfile.TryGet<Bloom>(out _bloom);
            _bloom.intensity.value = 2f;
            _uiManager.ShowStartMenuPanel();
        }

        private void OnTryAgainClicked()
        {
            _uiManager.HideLostPanel();
            StartCoroutine(HandleStartNewGame());
        }

        private void OnStartGameClicked()
        {
            StartCoroutine(HandleStartNewGame());
        }

        public IEnumerator DisplayGrid()
        {
            yield return new WaitForSeconds(.5f);
            StartCoroutine(_gridManager.DisplayGrid());
        }


        private bool touch = false;

        public bool IsHexAlreadySelected(HexModel hexModel)
        {
            return _currentHexSelection.Contains(hexModel);
        }

        public HexModel LastSelectedHex()
        {
            if (_currentHexSelection.Count > 0)
            {
                return _currentHexSelection.Last();
            }

            return null;
        }

        public void AddHexModelToSelection(HexModel hexModel)
        {
            if (IsHexAlreadySelected(hexModel))
            {
                return;
            }

            _currentHexSelection.Add(hexModel);
            _bloom.intensity.value = 2f + (_currentHexSelection.Count * 0.25f);
        }

        public void CheckInputs()
        {
#if UNITY_WEBGL
            if (Input.GetKeyUp(KeyCode.Space))
            {
                StartCoroutine(HandleLevelComplete());
            }


            if (Input.GetMouseButtonDown(0))
            {
                touch = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                touch = false;
            }
#elif UNITY_ANDROID
if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {

                touch = true;


        }
            if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
        {

                touch = false;

        }

#endif
        }


        private void GenerateNextLevel()
        {
            GridReady = false;

            _foundWords = new List<string>();
            _currentSelectionColor = selectionColorsPallete[0];
            _gridManager.ResetGrid();
            var spots = _gridManager.RemainingOpenSpots;

            words = WordBuilder.GetListOfWords(spots, _minWordSize, _maxWordSize);

            while (words.Sum(w => w.Length) > _gridManager.RemainingOpenSpots)
            {
                //preventing a crash on putting the words 
                words = WordBuilder.GetListOfWords(spots, _minWordSize, _maxWordSize);
            }

            words = words.OrderByDescending(w => w.Length).ToList();
            _gridManager.PlaceWordsOnGrid(words);
            GridReady = true;
        }


        public void ClearSelection()
        {
            foreach (var hex in _currentHexSelection)
            {
                if (hex.State != HexState.Blocked)
                {
                    _gridManager.UnselectHex(hex);
                }
            }

            _currentHexSelection.Clear();
            _uiManager.UpdateSelection(_currentHexSelection, _currentSelectionColor);
        }


        public void Update()
        {
            CheckInputs();

            if (Time.time < _nextRayCast)
            {
                return;
            }

            _nextRayCast = Time.time + nextRayCastFreqency;


            if (touch)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                //hexRenderer.UnFill();
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    var hex = hit.collider.GetComponent<HexView>();
                    var hexModel = hex.Model;
                    if (hexModel.State == HexState.Blocked)
                    {
                        touch = false;
                        return;
                    }

                    if (hexModel.State == HexState.Selected && _currentHexSelection.Count > 0 &&
                        hexModel == _currentHexSelection.Last())
                    {
                        return;
                    }

                    if (hexModel.State == HexState.Selected && _currentHexSelection.Count > 0)
                    {
                        RemoveLastHexFromSelection();
                        return;
                    }

                    if (_currentHexSelection.Count > 0 &&
                        !_gridManager.AreHexsAdjacent(hexModel, _currentHexSelection.Last()))
                    {
                        touch = false;
                        return;
                    }

                    if (_currentHexSelection.Count > _maxWordSize)
                    {
                        return;
                    }

                    AddHexModelToSelection(hexModel);
                    _uiManager.UpdateSelection(_currentHexSelection, _currentSelectionColor);
                    _gridManager.SelectHex(hex, _currentSelectionColor);
                }
            }
            else
            {
                if (_currentHexSelection.Count > _maxWordSize)
                {
                    ClearSelection();
                }
                else if (_currentHexSelection.Count > 0)
                {
                    ProcessSelection();
                    ClearSelection();
                }
            }


            //move to grid Manager , just to test
        }

        private void RemoveLastHexFromSelection()
        {
            var hex = _currentHexSelection.Last();
            _gridManager.UnselectHex(hex);
            _currentHexSelection.Remove(hex);
            _uiManager.UpdateSelection(_currentHexSelection, _currentSelectionColor);
            _bloom.intensity.value = 1.5f + _currentHexSelection.Count;
        }

        private void ClearSubsequentHexsFromSelection(int index)
        {
            for (int i = index; i < _currentHexSelection.Count; i++)
            {
                var hex = _currentHexSelection[i];
                _gridManager.UnselectHex(hex);
                _currentHexSelection.RemoveAt(i);
            }
        }

        private void StartTimer()
        {
            if (_timerCoroutine != null)
            {
               StopTimer();
               return;
            }

            _remainingTime = maxTime;
            _timerCoroutine= StartCoroutine(TimerCoroutine());
        }

        private void StopTimer()
        {
            if (_timerCoroutine != null)
            { 
                
                StopCoroutine(_timerCoroutine);

            }
            _timerCoroutine = null;

        }

        private void AddBonusTime(int value)
        {
            _remainingTime += value;
            _remainingTime = Mathf.Clamp(_remainingTime, 0, maxTime);
            _uiManager.UpdateTimer(value);
        }


        IEnumerator TimerCoroutine()
        {
           
            _timeStep = Time.deltaTime + (Time.deltaTime * ((_currentLevel-1) * 0.1f)); // timer becomes faster and faster
            _remainingTime -= _timeStep;
            yield return null;
            if (_remainingTime <= 0)
            {
                StartCoroutine(HandleGameOver());
                yield break;
            }
            if (_timerCoroutine == null)
            {
                yield break;
            }
            _uiManager.UpdateTimer(_remainingTime);
            _timerCoroutine=StartCoroutine(TimerCoroutine());
        }

        private void HandleFoundWord(List<HexModel> currentHexSelection, string word)
        {
            _foundWords.Add(word);
            _uiManager.UpdateWordsLengthPanel(word, _currentSelectionColor);
            _currentSelectionColor = selectionColorsPallete[_foundWords.Count];
            Debug.Log(" Found Word - " + word);

            foreach (var hexModel in currentHexSelection)
            {
                hexModel.State = HexState.Blocked; // goes for GridManager
            }

            AddBonusTime(bonusTimePerWordGuessed);
        }

        private void CheckIfAllWordWereFound()
        {
            if (_foundWords.Count == words.Count)
            {
                StartCoroutine(HandleLevelComplete());
            }
        }

        private bool GridReady = false;

        public IEnumerator HandleLevelComplete()
        {
            StopTimer();
            PerformSuccessFeedback();
            yield return new WaitForSeconds(.5f);
            yield return StartCoroutine(_gridManager.HideGrid());
            GenerateNextLevel();
            _currentLevel++;
            _uiManager.UpdateLevel(_currentLevel);
            _uiManager.SetWordsLengthPanel(words);
            yield return new WaitUntil(() => GridReady);
            yield return DisplayGrid();
            yield return StartCoroutine(DisplayGrid());
            yield return new WaitForSeconds(1);
            _remainingTime = maxTime;
            StartTimer();
        }

        public IEnumerator HandleGameOver()
        {
            _uiManager.HideGameUI();
            yield return StartCoroutine(_gridManager.HideGrid());
            yield return new WaitForSeconds(.5f);
            _uiManager.ShowLostPanel(_currentLevel);
        }


        public IEnumerator HandleStartNewGame()
        {
            _currentLevel = 1;
            _uiManager.UpdateLevel(_currentLevel);
            _uiManager.UpdateTimer(maxTime);
            _uiManager.HideStartMenu();
            _uiManager.ShowGameUI();
            GenerateNextLevel();
            yield return new WaitUntil(() => GridReady);
            _uiManager.SetWordsLengthPanel(words);
            yield return StartCoroutine(DisplayGrid());
            yield return new WaitForSeconds(1);
            StartTimer();
        }


        private void PerformSuccessFeedback()
        {
            float baseBloom = 15f;
            _bloom.intensity.value = baseBloom;

            DOTween.To(() => baseBloom, x => baseBloom = x, 2f, .5f)
                .OnUpdate(() => { _bloom.intensity.value = baseBloom; });
        }


        private bool DoesAllSelectedLettersBelongToSameWord()
        {
            int firstSelectedLetterWordIndex = _currentHexSelection[0].WordIndex;

            for (int i = 1; i < _currentHexSelection.Count; i++)
            {
                if (_currentHexSelection[i].WordIndex != firstSelectedLetterWordIndex)
                {
                    //selection with letters belonging different words, so not valid
                    return false;
                }
            }

            return true;
        }


        private bool IsCurrentSelectedWordInWordList()
        {
            //probably redundant
            var selectedWord = String.Empty;


            for (var i = 0; i < _currentHexSelection.Count; i++)
            {
                selectedWord += _currentHexSelection[i].Char;
            }


            if (!WordBuilder.ContainsCaseInsensitive(words, selectedWord))
            {
                return false;
            }

            return true;
        }

        private bool IsWordSelectionSequenceValid()
        {
            int wordIndex = _currentHexSelection[0].WordIndex;
            string word = words[wordIndex];
            if (_currentHexSelection.Count != word.Length)
            {
                return false;
            }

            for (int i = 0; i < _currentHexSelection.Count; i++)
            {
                if (_currentHexSelection[i].Char != word[i])
                {
                    return false;
                }
            }

            return true;
        }


        private void ProcessSelection()
        {
            if (!DoesAllSelectedLettersBelongToSameWord())
            {
                Debug.Log("Not same word");
                return;
            }

            if (!IsCurrentSelectedWordInWordList())
            {
                Debug.Log("Word not in list");

                //it means that if all selected letters belong to the same word but the word doesn't exist it means that the player is close
                return;
            }


            if (!IsWordSelectionSequenceValid())
            {
                // we need to check if the sequence was correct but also taking in consideration palindromic words
                return;
            }


            int firstSelectedLetterWordIndex = _currentHexSelection[0].WordIndex;
            string foundWord = words[firstSelectedLetterWordIndex];
            PerformSuccessFeedback();
            HandleFoundWord(_currentHexSelection, foundWord);
            CheckIfAllWordWereFound();
        }
    }
}