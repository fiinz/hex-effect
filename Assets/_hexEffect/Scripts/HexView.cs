using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _hexEffect.Scripts
{
    public enum HexState
    {
        Selected,
        UnSelected,
        Filled,
        Empty,
        Blocked
    }

    public  class HexViewPool:MonoBehaviour
    {
        private HexViewPool _instance; //future pool of views.

    }


    public class HexView:MonoBehaviour
    {
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private BoxCollider colider;
        [SerializeField] private HexRenderer hexRenderer;
        [SerializeField] private TMP_Text letterTMP;
        [SerializeField] public CanvasGroup canvasGroup;
        [SerializeField] private Color hexDefaultColor;
        [SerializeField] private Color letterDefaultColor;

        private bool Initialized;
        public HexModel Model { get; set; }
        
        public void Initialize(HexModel model)
        {
            this.Model = model;
            hexRenderer.Initialize(model);
            hexRenderer.DrawMesh();
            hexRenderer.SetColor(hexDefaultColor);
            letterTMP.color = letterDefaultColor;
            //hexRenderer.AnimateHue();
            var canvasTransform = canvas.transform;
            var canvasPosition = canvasTransform.localPosition;
            canvasTransform.localPosition = new Vector3(canvasPosition.x, Model.Height*1.2f, canvasPosition.z);
            this.gameObject.SetActive(false);
            Initialized = true;
            UpdateState();
        }

        public void OnEnable()
        {
            if (Model == null) return;
            Model.StateChanged += UpdateState;
            UpdateState();
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, .3f);

        }

        public void UpdateState()
        {
//            Debug.Log("State Updated");
            SetLetter(Model.Char);
        }


        public void OnDisable()
        {
            Model.StateChanged -= UpdateState;
        }
        
        public void SetLetter(Char c)
        {
            letterTMP.text=c.ToString().ToUpper();

        }

        public void SetLetterColor(Color color)
        {
          
            letterTMP.color = color;
         

        }

        public void ChangeHexColor(Color color)
        {
            hexRenderer.SetColor(color);

        }
        public void Select(Color color)
        {
            letterTMP.color = color;
            ChangeHexColor(color);
            Model.State = HexState.Selected;
        }
        public void UnSelect()
        {
            //hexRenderer.UnFill();
            
            hexRenderer.SetColor(this.hexDefaultColor);
            letterTMP.color = this.letterDefaultColor;
            Model.State = HexState.UnSelected;
        }
    }
}