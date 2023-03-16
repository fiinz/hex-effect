using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _hexEffect.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public List<string> words;
        
        [SerializeField] private int _maxWordSize;
        [SerializeField] private int _minWordSize;


        [SerializeField] private HexGrid _hexGridPrefab;
        private HexGrid _gridManager;
        private float nextRayCast=0.0f;
        private float nextRayCastFreqency=0.005f;
        public bool GridGenerated = false;
        public List<HexModel> currentHexSelection = new List<HexModel>();

        private void Start()
        {
            nextRayCast = Time.time + nextRayCastFreqency;
            WordBuilder.Initialize();
            /*  var words = WordBuilder.GetListOfWords(30,5);
              var count = 0;
              
               foreach (var word in words)
              {
                  Debug.Log("Words Inserted : "+word.ToString());
                  count += word.Length;
  
              }
               Debug.Log("Word Count: "+count.ToString()); */
            _gridManager = Instantiate(_hexGridPrefab);
            _gridManager.Initialize();
            _gridManager.ResetGrid();
            GridGenerated=false;
            var spots = _gridManager.RemainingOpenSpots;
            words = WordBuilder.GetListOfWords(spots, 3, 6);
          //  words = (new List<string>()
            //    {"JAIME", "MARIA", "DIMENSIONS", "MJD", "ALEX","OLA"});
            words = words.OrderByDescending(w => w.Length).ToList();
            _gridManager.PlaceWordsOnGrid(words);

            StartCoroutine(GenerateGrids());

            //10
            //5
            //5
            //4
            //newGridManager.PlaceWordsOnGrid(new List<string>(){"123456","123456","123456","123456","123456"});
            //newGridManager.PlaceWordsOnGrid(words);
        }

        public IEnumerator GenerateGrids()
        {
  
            yield return new WaitForSeconds(.5f);
            StartCoroutine(_gridManager.DisplayGrid());
            GridGenerated = true;

            //yield return null;
            // StartCoroutine(GenerateGrids());
        }

   

        private bool touch = false;

        public bool IsHexAlreadySelected(HexModel hexModel)
        {
            return currentHexSelection.Contains(hexModel);
        }
        public HexModel LastSelectedHex()
        {
            if (currentHexSelection.Count>0)
            {
                return currentHexSelection.Last();
            }
            return null;
        }
        public void AddHexModelToSelection(HexModel hexModel)
        {
            if (IsHexAlreadySelected(hexModel))
            {
                return;
            }
            currentHexSelection.Add(hexModel);
        }
        
        public void CheckInputs()
        {
#if UNITY_EDITOR

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
        
        public void SelectHex(HexView hex)
        {
            hex.Select();
        }

     
        public void ClearSelection()
        {
            foreach (var hex in currentHexSelection)
            {

                if (hex.State != HexState.Blocked)
                {


                    _gridManager.UnselectHex(hex);
                }


            }
            currentHexSelection.Clear();
        }

    
        public void Update()
        {
            CheckInputs();

            if (Time.time < nextRayCast)
            {
                return; 
                
            }
            nextRayCast = Time.time + nextRayCastFreqency;

        
            
            if (touch)
            {
                Debug.Log("Mouse Pressed");

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                //hexRenderer.UnFill();
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    var hex = hit.collider.GetComponent<HexView>();
                    var hexModel = hex._model;
                    if (hexModel.State == HexState.Blocked)
                    {
                        touch = false;
                        return;
                    }
                    if (hexModel.State == HexState.Selected && currentHexSelection.Count > 0 &&
                        hexModel == currentHexSelection.Last())
                    {
                        return;
                    }

                    if (hexModel.State==HexState.Selected && currentHexSelection.Count>0 )
                    {
                        RemoveLastHexFrom();
                        return;

                    }
                    if (currentHexSelection.Count>0 && !_gridManager.AreHexsAdjacent(hexModel,currentHexSelection.Last()))
                    {
                        touch = false;
                        return;
                    }
                    AddHexModelToSelection(hexModel);
                    SelectHex(hex);
                }
            }
            else
            {
                if (currentHexSelection.Count > 0)
                {
                    ProcessSelection();
                    ClearSelection();
                }
              
            }


            //move to grid Manager , just to test
           
        }

        private void RemoveLastHexFrom()
        {
            var hex=currentHexSelection.Last();
            _gridManager.UnselectHex(hex);
            currentHexSelection.Remove(hex);
        }

        private void ClearSubsequentHexsFromSelection(int index)
        {
            for (int i = index; i < currentHexSelection.Count; i++)
            {
                var hex = currentHexSelection[i];
                _gridManager.UnselectHex(hex);
                currentHexSelection.RemoveAt(i);
            }
            
        }

        private void HandleFoundWord(List<HexModel> currentHexSelection, string word)
        {
            
            Debug.Log(" Found Word - "+word);

            foreach (var hexModel in currentHexSelection)
            {

                hexModel.State = HexState.Blocked; // goes for GridManager
            }

            
        }

        private void ProcessSelection()
        {
            Debug.Log("----- Selected Letters  --- ");
            string selectedWord=String.Empty;
            int firstLetterWordIndex = currentHexSelection[0].WordIndex;
            foreach (var hexModel in currentHexSelection)
            {
                if (hexModel.WordIndex != firstLetterWordIndex)
                {
                    //different words
                    return;

                }
                selectedWord += hexModel.Char;
                
            }

            if (!words.Contains(selectedWord))
            {
                return;
            }
            HandleFoundWord(currentHexSelection, selectedWord);

            Debug.Log(" Selected Word - "+selectedWord);
        }
    }
}