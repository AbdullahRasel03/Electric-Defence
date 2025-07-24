using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
[System.Serializable]
public class GridArray
{
    public GridObject[] grids;
}
public class GridManager : MonoBehaviour
{
    [SerializeField] int rowCount, columnCount;
    public List<GridArray> gridArrays;
    public TowerController[] towers;
    public Turret[] turrets;
    // Start is called before the first frame update
    void Start()
    {
        // Get all direct child GridObjects
        List<GridObject> allGrids = new List<GridObject>();
        foreach (Transform child in transform)
        {
            GridObject grid = child.GetComponent<GridObject>();
            if (grid != null)
            {
                allGrids.Add(grid);
            }
        }

        // Calculate column count (assuming even distribution)
        columnCount = allGrids.Count / rowCount;
        gridArrays = new List<GridArray>();

        for (int i = 0; i < rowCount; i++)
        {
            GridArray gridArray = new GridArray();
            gridArray.grids = new GridObject[columnCount];

            for (int j = 0; j < columnCount; j++)
            {
                int index = i * columnCount + j;
                if (index < allGrids.Count)
                {
                    gridArray.grids[j] = allGrids[index];
                    allGrids[index].gridManager = this;
                }
            }

            gridArrays.Add(gridArray);
        }
        gridArrays.Reverse();
    }

    public void CheckAllGridsPower()
    {
        StartCoroutine(CheckAllGridsPowerWithDelay());
    }

    private IEnumerator CheckAllGridsPowerWithDelay()
    {
        yield return new WaitForSeconds(0.1f);

        foreach (var item in towers)
        {
            item.CheckMultisOnPath();
            print("Here and there");
        }

        foreach (var item in turrets)
        {
            item.CheckMultisOnPath();
        }
        /*  foreach (var item in gridArrays)
          {
              yield return new WaitForSeconds(0.1f);
              foreach (var grid in item.grids)
              {
                  if (grid.socket != null)
                  {
                      grid.socket.CheckPowerActivation();
                      if (grid.socket.connectedPlug != null)
                      {
                          grid.socket.connectedPlug.CheckForSocketsUnderneath();
                      }
                  }

              }
          }*/
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
