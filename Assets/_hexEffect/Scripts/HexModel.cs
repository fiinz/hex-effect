using System;
using UnityEngine;

namespace _hexEffect.Scripts
{
    [System.Serializable]
    public class HexModel
    {
        public int row;
        public int col;
        public event Action StateChanged;
        private  bool _isPointy;
        private float _height;
        private Char _char;
        private HexState _state;
        private float _innerSize;
        private float _outerSize;
        private int _wordIndex;
        private int _charIndex;
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

  
        public char Char
        {
            get => _char;
            set {
                _char = value;
                StateChanged?.Invoke();
                
            } 
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

        public int WordIndex
        {
            get => _wordIndex;
            set => _wordIndex = value;
        }

        public int CharIndex
        {
            get => _charIndex;
            set => _charIndex = value;
        }

        public void ResetModel()
        {
            Char = '\0';
            CharIndex = -1;
            WordIndex = -1;
            State = HexState.Empty;
    
        }

        public void SetLetter(char c, int currentCharIndex, int wordIndex)
        {
            Char = c;
            CharIndex =currentCharIndex;
            WordIndex =wordIndex;
            State = HexState.Filled;
        }
    }
}