using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    
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


    public void ReleaseToGrid(Socket socket, bool isMainCube)
    {
        if (isMainCube)
        {
            socket.transform.parent = plugSocketHolder;
            socket.transform.DOLocalMove(Vector3.zero, 0.2f);
        }
        isOccupied = true;
        this.socket = socket;
    }

    public void ReleaseToGrid(Plug plug)
    {
        plug.transform.parent = plugSocketHolder;
        
        isOccupied = true;
        this.plug = plug;
        plug.PlaceOnGrid(this);
    }


}