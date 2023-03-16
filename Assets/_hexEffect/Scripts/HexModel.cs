using System;

namespace _hexEffect.Scripts
{
    public class HexModel
    {
        public int row;
        public int col;
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
}