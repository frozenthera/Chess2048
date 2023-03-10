using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;
    [SerializeField] private List<GameObject> piecePrefabs = new();
    private Dictionary<PieceEnum, GameObject> pieceDict = new();

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        foreach(var item in piecePrefabs)
        {
            pieceDict.Add(item.GetComponent<Piece>().pieceClass, item);
        }
    }

    public GameObject GetObjectByPieceEnum(PieceEnum pieceEnum)
    {
        if(pieceEnum == PieceEnum.EMPTY) return null;
        return pieceDict[pieceEnum];
    }

}
