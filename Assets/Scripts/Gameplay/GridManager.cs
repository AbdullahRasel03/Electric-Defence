using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GridArray
{
    public GridObject[] grids;
}
public class GridManager : MonoBehaviour
{
    public GridArray[] gridArrays;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in gridArrays)
        {
            foreach (var grid in item.grids)
            {
                grid.gridManager = this;
            }
        }
    }

    public void CheckAllGridsPower()
    {
        StartCoroutine(CheckAllGridsPowerWithDelay());
    }

    private IEnumerator CheckAllGridsPowerWithDelay()
    {

        foreach (var item in gridArrays)
        {
            yield return new WaitForSeconds(0.1f);
            foreach (var grid in item.grids)
            {
                if (grid.socket != null)
                {
                    grid.socket.CheckPowerActivation();
                }
                else if (grid.plug != null)
                {
                    grid.plug.CheckForSocketsUnderneath();
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            foreach(var item in gridArrays)
            {
                foreach (var grid in item.grids)
                {
                    grid.ResetHighlight();
                }
            }
        }
    }
}
