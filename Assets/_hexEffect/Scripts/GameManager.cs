using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _hexEffect.Scripts
{
    public class GameManager:MonoBehaviour
    {
       [SerializeField] private HexGrid _hexGrid;

        private void Start()
        {

            WordBuilder.Initialize();
            var words = WordBuilder.GetListOfWords(30,5);
            var count = 0;
            
/*            foreach (var word in words)
            {
                Debug.Log("Word : "+word.ToString());
                count += word.Length;
                Debug.Log("Word Count: "+count.ToString());

            }*/
            var newGridManager = Instantiate(_hexGrid);
            newGridManager.Initialize();
            newGridManager.ResetGrid();
            //10
            //5
            //5
            //4
           // newGridManager.PlaceWordsOnGrid(new List<string>(){"123456","123456","123456","123456","123456"});
        }

      

        public void Update()
    {
        /*
        if (Time.time > nextRayCast)
        {
            //over
                
        }
            
            
        //move to grid Manager , just to test
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Clicked");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //hexRenderer.UnFill();
                

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.Log("Clicked on top of the game object!");

                    
                if (hit.collider.gameObject == gameObject)
                {
                        
                    Debug.Log("Clicked on top of the game object!");
                    hexRenderer.Fill();
                    HexClicked?.Invoke();
                }
                else
                {
                    hexRenderer.UnFill();
                }
            }
        }*/
    }
    }
}