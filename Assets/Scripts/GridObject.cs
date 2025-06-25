using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public GridManager gridManager;
    public bool isOccupied;
    public LayerMask gridLayer;
    public Transform plugSocketHolder;

    public Socket socket;
    public Plug plug;
    public Renderer gridRenderer;
    private Color defaultColor;
    public Color highlightColor = Color.yellow;
    // public bool isDealingDamage;

    private void Awake()
    {
        defaultColor = gridRenderer.material.color;

    }

    public void Highlight()
    {
        gridRenderer.material.color = highlightColor;
    }

    public void ResetHighlight()
    {
        gridRenderer.material.color = defaultColor;
    }


  
    public void ReleaseToGrid(Plug plug)
    {
        plug.transform.parent = plugSocketHolder;
        
        isOccupied = true;
        this.plug = plug;
        plug.PlaceOnGrid(this);
    }


}