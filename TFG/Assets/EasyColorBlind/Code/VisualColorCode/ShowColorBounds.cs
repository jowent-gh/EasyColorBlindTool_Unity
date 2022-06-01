using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class ShowColorBounds : MonoBehaviour {
    
    [SerializeField]
    Color m_color;
    
    [SerializeField]
    Transform m_root; // if skinned mesh, bounds relative to root
    
    [SerializeField]
    [Range(0, 1)] float m_figureSize = .5f; 
    
    Renderer m_renderer;
    Transform m_transform;
    MaterialPropertyBlock m_mpb;

    float m_bounds2DSize;
    Vector2 m_bounds2DCenter;
    
    void OnEnable() {
        m_renderer = GetComponent<Renderer>();
        m_transform = m_root != null ? m_root : transform;
        m_mpb = new MaterialPropertyBlock();
        m_mpb.SetFloat("_HasColorFigure", 1);
        m_mpb.SetFloat("_HasColorFigure", 1);
    }

    private void OnDisable() {
        m_mpb.SetFloat("_HasColorFigure", 0);
        m_renderer.SetPropertyBlock(m_mpb);
    }

    private void Update() {
        UpdateBoundsCoordinates();
        UpdateColorToFigure();
        m_renderer.SetPropertyBlock(m_mpb);
    }


    private void UpdateBoundsCoordinates(){
        // Transform each vertex from the AABB to Screen Points
        List<Vector3> l_points = BoundsPointList(m_renderer.bounds); 
        l_points = WorldToScreenPoints(l_points);

        // Gets minimums and maximums on both 2D Coordinates
        Vector2 l_minCoords = GetMinCoords(l_points);
        Vector2 l_maxCoords = GetMaxCoords(l_points);

        // Center From the margins
        m_bounds2DCenter = new Vector2((l_minCoords.x + l_maxCoords.x) / 2.0f, (l_minCoords.y + l_maxCoords.y) / 2.0f);
        
        // Size
        m_bounds2DSize = Mathf.Min(l_maxCoords.y - l_minCoords.y, l_maxCoords.x - l_minCoords.x);

        Vector4 l_size01 = new Vector4();
        float m_fixedSize = m_bounds2DSize * m_figureSize / 2 ;
        l_size01.x = (m_bounds2DCenter.x - m_fixedSize) / Screen.width; 
        l_size01.y = (m_bounds2DCenter.y - m_fixedSize) / Screen.height; 
        l_size01.z = (m_bounds2DCenter.x + m_fixedSize) / Screen.width; 
        l_size01.w = (m_bounds2DCenter.y + m_fixedSize) / Screen.height;


        // Apply parameter material shader
        m_mpb.SetVector("_FigureCoords", l_size01);
        // m_renderer.SetPropertyBlock(m_mpb);
    }

    private void UpdateColorToFigure(){
        ColorFigure l_colorFigures = EasyColorblindFuncions.ColorToFigure(m_color);
        Vector4 l_nodes = new Vector4(){
            x = l_colorFigures.red,
            y = l_colorFigures.green,
            z = l_colorFigures.blue,
            w = l_colorFigures.sv
        };
        m_mpb.SetVector("_FigureNodes", l_nodes);
    }


    private void OnDrawGizmosSelected() {
        m_transform = m_root != null ? m_root : transform;
        foreach(Vector3 p in BoundsPointList(GetComponent<Renderer>().bounds)){
            Gizmos.DrawWireSphere(p, .1f);
        }
    }

    private Vector2 GetMaxCoords(List<Vector3> _points){
        Vector2 l_maxCoords = new Vector2();

        for(int i = 0; i< _points.Count; i++){
            if(l_maxCoords.x < _points[i].x)
                l_maxCoords.x = _points[i].x;
            if(l_maxCoords.y < _points[i].y)
                l_maxCoords.y = _points[i].y;
        }

        return l_maxCoords;
    }

    private Vector2 GetMinCoords(List<Vector3> _points){
        Vector2 l_minCoords = new Vector2(Screen.width, Screen.height);

        for(int i = 0; i< _points.Count; i++){
            if(l_minCoords.x > _points[i].x)
                l_minCoords.x = _points[i].x;
            if(l_minCoords.y > _points[i].y)
                l_minCoords.y = _points[i].y;
        }

        return l_minCoords;
    }


    private List<Vector3> WorldToScreenPoints(List<Vector3> _points){
        List<Vector3> l_list = new List<Vector3>();

        foreach(Vector3 p in _points){
            l_list.Add(Camera.main.WorldToScreenPoint(p));
        }

        return l_list;
    }

    private List<Vector3> BoundsPointList(Bounds _bounds){
        List<Vector3> l_pointList = new List<Vector3>();

        Vector3 l_point;

        // x,y,z
        l_point = _bounds.center + m_transform.right * _bounds.extents.x
                                 + m_transform.up * _bounds.extents.y
                                 + m_transform.forward * _bounds.extents.z;
        l_pointList.Add(l_point);
        
        // x,y,-z
        l_point = _bounds.center + m_transform.right * _bounds.extents.x
                                 + m_transform.up * _bounds.extents.y
                                 - m_transform.forward * _bounds.extents.z;
        l_pointList.Add(l_point);
        
        // x,-y,z
        l_point = _bounds.center + m_transform.right * _bounds.extents.x
                                 - m_transform.up * _bounds.extents.y
                                 + m_transform.forward * _bounds.extents.z;
        l_pointList.Add(l_point);
        
        // x,-y,-z
        l_point = _bounds.center + m_transform.right * _bounds.extents.x
                                 - m_transform.up * _bounds.extents.y
                                 - m_transform.forward * _bounds.extents.z;
        l_pointList.Add(l_point);
        
        // -x,y,z
        l_point = _bounds.center - m_transform.right * _bounds.extents.x
                                 + m_transform.up * _bounds.extents.y
                                 + m_transform.forward * _bounds.extents.z;
        l_pointList.Add(l_point);
        
        // -x,y,-z
        l_point = _bounds.center - m_transform.right * _bounds.extents.x
                                 + m_transform.up * _bounds.extents.y
                                 - m_transform.forward * _bounds.extents.z;
        l_pointList.Add(l_point);
        
        // -x,-y,z
        l_point = _bounds.center - m_transform.right * _bounds.extents.x
                                 - m_transform.up * _bounds.extents.y
                                 + m_transform.forward * _bounds.extents.z;
        l_pointList.Add(l_point);
        
        // -x,-y,-z
        l_point = _bounds.center - m_transform.right * _bounds.extents.x
                                 - m_transform.up * _bounds.extents.y
                                 - m_transform.forward * _bounds.extents.z;
        l_pointList.Add(l_point);

        return l_pointList;
    } 
}
