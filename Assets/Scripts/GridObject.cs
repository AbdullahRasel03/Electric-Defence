using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public int row, col;
    public bool isOccupied;
    public bool isSelected;
    public GameObject cell;
    public LayerMask gridLayer, cubeLayer;
    public bool isCoinActive;
    public GameObject mysteryBox;
    public int coinValue = 1;
    public ParticleSystem[] blastParticles;
    private int coinFlyDirection = 1;
    public List<GridObject> rowGrids, columnGrids;


    // public bool isDealingDamage;

    private void Awake()
    {
        

    }

    public void SelectGrid()
    {
        isSelected = true;
        mysteryBox.transform.DOKill();
        
    }
    public void Deselect()
    {
        isSelected = false;
        mysteryBox.transform.DOLocalMoveY(1f, 1f).SetLoops(-1, LoopType.Yoyo);
       
    }
    public void ActivateCoin()
    {
        if (!isCoinActive)
        {
            mysteryBox.SetActive(true);
            mysteryBox.transform.localScale = Vector3.zero;
            isCoinActive = true;
            mysteryBox.transform.DOScale(Vector3.one, 0.5f).OnComplete(() =>
            {

                mysteryBox.transform.DOLocalMoveY(1.5f, 1f).SetLoops(-1, LoopType.Yoyo);

            });

        }
    }

    //public void MoveCoin()
    //{
    //    GameObject _coin = gridManager.pooledCoins[0];
    //    gridManager.pooledCoins.Remove(_coin);
    //    _coin.SetActive(true);


    //    _coin.transform.position = cell.transform.position + Vector3.up;

    //    _coin.GetComponent<Rigidbody>().isKinematic = false;
    //    _coin.GetComponent<Rigidbody>().AddForce(Vector3.one * 2 + Vector3.right * Random.Range(-1f, 1f) + Vector3.forward * Random.Range(-1f, 1f) + Vector3.up * Random.Range(3, 5), ForceMode.Impulse);
    //    _coin.GetComponent<Rigidbody>().AddTorque(Vector3.up * 0.2f, ForceMode.Impulse);
    //    Vector3 initialPosition = transform.position;
    //    Vector3 targetPosition = gridManager.coinsTarget.transform.position;
    //    Vector3 middlePoint = (initialPosition + targetPosition) / 2;
    //    middlePoint.x -= (Random.Range(0.5f, 1.5f) * coinFlyDirection);

    //    Vector3[] path = new Vector3[] { middlePoint, targetPosition };

    //    // Coin movement with path tween
    //    _coin.transform.DOPath(path, 0.6f, PathType.CatmullRom)
    //        .SetDelay(2)
    //        .SetEase(Ease.Linear)
    //        .OnStart(() => {
    //            _coin.GetComponent<Rigidbody>().isKinematic = true;

    //        })
    //        .OnComplete(() =>
    //        {
    //            EconomyManager.instance.UpdateCoinCount(1);
    //            _coin.SetActive(false);
    //            _coin.transform.position = initialPosition;
    //            gridManager.pooledCoins.Add(_coin);
    //        });


    //    coinFlyDirection *= (-1);

    //}

   
    /////Power Section................................................................................................................... 

}