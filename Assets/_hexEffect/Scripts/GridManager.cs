using System;
using System.Collections;
using System.Collections.Generic;
using _hexEffect.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int rows;
    [SerializeField] private int cols;

    [FormerlySerializedAs("pointUp")] [SerializeField]
    private bool isPointy;

    [SerializeField] private float innerSize = 1;
    [SerializeField] private float outterSize = 1.5f;
    [SerializeField] private float height = 1.5f;
    [SerializeField] private bool comb = true;

    public Material material;
    private Vector2Int gridSize;

    // Start is called before the first frame update
    private void Start()
    {
        CreateGrid();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            //    CreateGrid();
        }
    }

    private HexRenderer[,] grid;

    private void CreateGrid()
    {
        //we use double coordinates with offset to make it easier to manage the hex grid
        // (col+row)%2==0 constraint
        //meaning we will have the double of columns / or rows depending on the point up

        int rowStep = 1;
        int colStep = 2;

        gridSize = new Vector2Int(cols * 2, rows);

        //rows, cols
        grid = new HexRenderer[gridSize.y, gridSize.x];


        if (!isPointy)
        {
            gridSize = new Vector2Int(cols, rows * 2);
            rowStep = 2;
            colStep = 1;
        }

        transform.position = Vector3.zero;

        var hexModel = new HexModel();
        hexModel.InnerSize = this.innerSize;
        hexModel.Height = this.height;
        hexModel.isPointy = this.isPointy;
        hexModel.OuterSize = this.outterSize;

        for (var row = 0; row < gridSize.y; row += rowStep)
        {
       
            
            //careful we are using double cordinates.
            int halfGrid = gridSize.x / 2; // we compensate double coordinates
            int rowHexagons = ((int) (cols) - (int) Mathf.Abs(Mathf.Floor(cols / 2f) - row)); // 5 / 2  0 - 1 - 2 
            
            //find every 4th column ( centered pattern) so the others can be positioned relative to those columns
            //  handle grids with odd numbers of rows and even numbers of columns. 
            int xStart = (cols - 3) % 4 == 0
                ? (int) Mathf.Ceil((halfGrid - (rowHexagons / 2f * colStep)))
                : (int) Mathf.Floor((halfGrid - (rowHexagons / 2f * colStep)));

            int xEnd = xStart + rowHexagons * colStep;

            for (var col = xStart; col < xEnd; col += colStep)
            {
                var tileGo = new GameObject($"Hex {row},{col}", typeof(HexRenderer));
                var hexRenderer = tileGo.GetComponent<HexRenderer>();
                var newMaterialInstance = Instantiate(material);
                hexRenderer.innerSize = hexModel.InnerSize;
                hexRenderer.outterSize = hexModel.OuterSize;
                hexRenderer.height = hexModel.Height;
                hexRenderer.isPointy = hexModel.isPointy;
                hexRenderer.SetMaterial(newMaterialInstance);
                hexRenderer.DrawMesh();
                tileGo.transform.position = GetPositionFromGridPos(row, col);
                tileGo.transform.SetParent(transform, true);
                grid[row, col] = hexRenderer;
            }
        }
    }

    public float space = 0.1f;

    public Vector3 GetPositionFromGridPos(int row, int col)
    {
        float width;
        float height;
        float xPosition = 0f;
        float zPosition = 0f;
        bool shouldOfset;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float size = outterSize; //outersize is the external radius

        if (isPointy)
        {
            width = Mathf.Sqrt(3f) * size; // these can be found on  https://www.redblobgames.com/grids/hexagons/
            height = 2f * size;
            horizontalDistance = width;
            verticalDistance = height * (3f / 4f);
            // offset = (shouldOfset) ? width / 2f : 0.0f;
            offset = 0f;
            xPosition = (col / 2f * (horizontalDistance + space));
            zPosition = -(row * (verticalDistance + space));
        }

        return new Vector3(xPosition, 0, zPosition);
    }

    /*
     public Vector2 DoubleWidthToAxial(int col, int row)
    {
        var q = col;
        var r = (row + col) *2;
        return new Vector2(q, r);
    }
  
    public void doublewidth_to_axial(hex){
    var q = (hex.col - hex.row) / 2
    var r = hex.row
        return Hex(q, r)
    }

    function axial_to_doublewidth(hex):
   
        return DoubledCoord(col, row)
        
*/
    public Vector3 GetPositionForHexFromArrayCoordinate(int row, int col)
    {
        float auxX = isPointy ? ((col / 2f)) : col;
        float auxY = isPointy ? row : ((row / 2f));
        // float horizontalSpacing=


        return new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
    }
}