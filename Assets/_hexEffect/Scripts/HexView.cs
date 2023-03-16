using System;
using TMPro;
using Unity.VisualScripting;
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


    public class HexView:MonoBehaviour
    {
        public event Action HexClicked;
        [SerializeField] private Canvas canvas;
        [SerializeField] private BoxCollider colider;
        [SerializeField] private HexRenderer hexRenderer;
        [SerializeField] private TMP_Text letterTMP;

       [SerializeField] private Color defaultColor;
        private bool Initialized;
        public HexModel _model;


        public void Initialize(HexModel model)
        {
            this._model = model;
            hexRenderer.Initialize(model);
            hexRenderer.DrawMesh();
            hexRenderer.SetColor(defaultColor);
            //hexRenderer.AnimateHue();
            var canvasTransform = canvas.transform;
            var canvasPosition = canvasTransform.localPosition;
            canvasTransform.localPosition = new Vector3(canvasPosition.x, _model.Height*1.2f, canvasPosition.z);
            this.gameObject.SetActive(false);
            Initialized = true;
            UpdateState();
        }

        public void OnEnable()
        {
            if (_model == null) return;
            _model.StateChanged += UpdateState;
            UpdateState();

        }

        public void UpdateState()
        {
//            Debug.Log("State Updated");
            SetLetter(_model.Char);
        }


        public void OnDisable()
        {
            _model.StateChanged -= UpdateState;
        }
        
        public void SetLetter(Char c)
        {
            letterTMP.text=c.ToString().ToUpper();

        }

        public void ChangeLetterColor(Color color)
        {
            letterTMP.color = color;
        }

        public void ChangeHexColor(Color color)
        {
            hexRenderer.SetColor(color);

        }
        public void Select()
        {
            _model.State = HexState.Selected;
        }
        public void UnSelect()
        {
            //hexRenderer.UnFill();
            hexRenderer.SetColor(this.defaultColor);

            _model.State = HexState.Filled;
        }
    }
}