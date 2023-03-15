using System;
using TMPro;
using UnityEngine;

namespace _hexEffect.Scripts
{
    public enum HexState
    {
        Selected,
        Active,
        Inactive,
    }

    
    public class HexModel
    {
        public Action StateChanged;
        private  bool _isPointy;
        private float _height;
        private Char _letter;
        private HexState _state;
        private float _innerSize;
        private float _outerSize;

  

        public float InnerSize
        {
            get => _innerSize;
            set { _innerSize = value; StateChanged?.Invoke(); }
        }

        public float OuterSize
        {
            get => _outerSize;
            set
            {
                _outerSize = value;
                StateChanged?.Invoke();
            }
        }

        public HexState State
        {
            get => _state;
            set {_state = value; StateChanged?.Invoke();} 
        }

        public char Letter
        {
            get => _letter;
            set { _letter = value; StateChanged?.Invoke();} 
        }

        public float Height
        {
            get => _height;
            set
            {
             _height = value; StateChanged?.Invoke();
             
            } 
        }

        public bool isPointy
        {
            get => _isPointy;
            set => _isPointy = value;
        }
    }
    public class Hex:MonoBehaviour
    {
        [SerializeField] private HexRenderer hexRenderer;
        [SerializeField] private TMP_Text letterTMP;
        private bool Initialized;
        public HexModel Model { get; set; }
        public void Initialize(HexModel model)
        {
            this.Model = model;
            Initialized = true;
            
        }
        public void OnEnable()
        {
            
        }
    }
}