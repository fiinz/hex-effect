using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _hexEffect.Scripts
{
    public enum HexState
    {
        Selected,
        Filled,
        Empty,
        Blocked
    }


    public class Hex:MonoBehaviour
    {
        public event Action HexClicked;
        [SerializeField] private Canvas canvas;
        [SerializeField] private BoxCollider colider;
        [SerializeField] private HexRenderer hexRenderer;
        [SerializeField] private TMP_Text letterTMP;
        

        private bool Initialized;
        public HexModel Model { get; set; }
        
    

        public void Initialize(HexModel model)
        {
            this.Model = model;
            Initialized = true;
            hexRenderer.Initialize(model);
            hexRenderer.DrawMesh();
            hexRenderer.AnimateHue();
            var canvasTransform = canvas.transform;
            var canvasPosition = canvasTransform.localPosition;
            canvasTransform.localPosition = new Vector3(canvasPosition.x, model.Height * 1.1f, canvasPosition.z);
            Model.State = HexState.Empty;
            letterTMP.text = String.Empty;
   

        }

       

        
        public void SetLetter(Char c)
        {
            letterTMP.text=c.ToString();
            Model.State = HexState.Filled;

        }

        public void ChangeColorText(Color color)
        {
            letterTMP.color = color;
        }

        public void Reset()
        {
            letterTMP.text=String.Empty;
            Model.State = HexState.Empty;
            
        }
    }
}