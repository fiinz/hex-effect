using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public struct Face
{
    public List<Vector3> vertices { get; private set; }
    public List<int> triangles { get; private set; }
    public List<Vector2> uvs { get; private set; }

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
        
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexRenderer : MonoBehaviour
{
    public  bool isPointy=true;
    public  float innerSize = 1;
    public  float outterSize = 1.5f;
    public  float height = 1.5f;

    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private List<Face> _faces;
    // Start is called before the first frame update
    void Awake()
    {
        _mesh = new Mesh();
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _mesh.name = "Hex";
        _meshFilter.mesh = _mesh;
    }

    public void OnValidate()
    {
       DrawMesh();
    }

    public void SetMaterial(Material material)
    {
        _meshRenderer.material = material;

    }

    public void DrawMesh()
    {
        DrawFaces(); // draws each individual triangle
        CombineFaces(); // merges the triangle
    }

    private void CombineFaces()
    {
        var vertices = new List<Vector3>();
        var tris = new List<int>();
        var uvs = new List<Vector2>();
        for (var i = 0; i < _faces.Count; i++)
        {
            vertices.AddRange(_faces[i].vertices);
            uvs.AddRange(_faces[i].uvs);
            var offset = (4 * i); //ofset ???
            foreach (var triangle in _faces[i].triangles)
            {
                tris.Add(triangle+offset);
            }
        }

        if (_mesh == null)
        {
            Debug.Log("Mesh is null");
        }
        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = tris.ToArray();
        _mesh.uv = uvs.ToArray();
        

    }

    private void DrawFaces()
    {
        _faces = new List<Face>();
        
        
        //top faces
        for (var point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(innerSize,outterSize,height,height,point));
             
        }   
        
        //bottom faces
        for (var point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(innerSize,outterSize,0,0,point,true));
             
        }   
        
        //outer faces
        
        for (var point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(outterSize,outterSize,height,0f,point, true));
             
        }
        
        for (var point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(innerSize,innerSize,height,0 ,point,false));
             
        }   
     
    }

    //reverse ist used to flip normals 
    private Face CreateFace(float innerRad, float outerRad, float outerHeight, float innerHeight, int point,
        bool reserve = false)
    {
        var pointA = GetPoint(innerRad, innerHeight, point);
        var pointB = GetPoint(innerRad, innerHeight, point<5?point+1:0); //points to the next point so it can draw the triangle, except the last one
        var pointC = GetPoint(outerRad, outerHeight, point<5?point+1:0);
        var pointD = GetPoint(outerRad, outerHeight, point);
        var vertices = new List<Vector3>() {pointA, pointB, pointC, pointD};
        var triangles = new List<int>() {0, 1, 2, 2, 3, 0};
        var uvs = new List<Vector2>()
        { 
            //planar mapping so we can project any image on top, however side geometry will not be be considered
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,1),
            
        };
        if (reserve)
        {
            vertices.Reverse();
        }
        //a hex is created from a circle
        
        return new Face(vertices,triangles,uvs);
    }

    
    //using polar coordinates to get a point of the hex
    //height is the actual height of the hexagon
    private Vector3 GetPoint(float size, float height, int point)
    {
        float angleStep =  (2*Mathf.PI / 6.0f);
        float adjustAnglePointIsUp = isPointy ? angleStep / 2.0f : 0;
        float angle = (angleStep * point)-adjustAnglePointIsUp;
        float ax = size * Mathf.Cos(angle);
        float az = size * Mathf.Sin(angle);
        return new Vector3(ax,height,az);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
