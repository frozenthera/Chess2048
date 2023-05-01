using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    //FIXME_Later move to Singleton<>
    public static GameManager Inst;

    public PlayerController localPlayer;
    private bool spawnDone = false;

    public Checker[,] boardState = new Checker[4,4];
    public PlayerEnum[,] boardPlayerState;
    public PieceEnum[,] boardPieceState;

    public Coordinate curSelected = Coordinate.none;
    public List<Coordinate> curMovable;
    public List<PieceEnum> spawnList;

    [SerializeField] private Transform pieces;
    public Transform Pieces => pieces;
    public bool isGameOver = false;
    
    public NetworkVariable<int> WHITE_Idx = new NetworkVariable<int>(0);
    public NetworkVariable<int> BLACK_Idx = new NetworkVariable<int>(0);
    public NetworkVariable<bool> PlayerActed = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isServerBlack = new NetworkVariable<bool>(true);
    public NetworkVariable<PlayerEnum> curPlayer = new NetworkVariable<PlayerEnum>(PlayerEnum.BLACK);

    public PlayerEnum LocalPlayerSide
    {
        get => IsServer
        ? isServerBlack.Value ? PlayerEnum.BLACK : PlayerEnum.WHITE
        : isServerBlack.Value ? PlayerEnum.WHITE : PlayerEnum.BLACK
        ;
    }

    public bool IsMyTurn
    {
        get => IsServer 
        ? isServerBlack.Value && curPlayer.Value == PlayerEnum.BLACK || !isServerBlack.Value && curPlayer.Value == PlayerEnum.WHITE
        : !isServerBlack.Value && curPlayer.Value == PlayerEnum.BLACK || isServerBlack.Value && curPlayer.Value == PlayerEnum.WHITE
        ;
    }

    //FIXME
    private void Awake()
    {
        Inst = this;
    }

    private void LateUpdate() 
    {
        if(!IsServer) return;
        if(spawnDone)
        {
            Board.Inst.ResetPaintedClientRpc();
            spawnDone = false;
        }    
    }

    public override void OnNetworkSpawn()
    {
        curPlayer.OnValueChanged += OnCurPlayerChanged;
        PlayerActed.OnValueChanged += OnPlayerActedChanged;
        NetworkManager.OnClientConnectedCallback += InitializeGame;
    }

    public void InitializeGame(ulong clientID)
    {
        if(!IsServer) return;
        StartCoroutine(_InitializeGame());
    }

    public IEnumerator _InitializeGame()
    {
        yield return StartCoroutine(Initialize());
        yield return StartCoroutine(Board.Inst.Initialize());

    }

    public IEnumerator Initialize()
    {
        boardState = new Checker[4,4];
        boardPlayerState = new PlayerEnum[4,4];
        boardPieceState = new PieceEnum[4,4];

        bool temp = Random.Range(0,2) == 0;
        isServerBlack.Value = temp;
        yield return null;
        UIManager.Inst.InitializeClientRpc(temp);

        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                boardPlayerState[i, j] = PlayerEnum.EMPTY;
                boardPieceState[i, j] = PieceEnum.NONE;
            }
        }
    }

    public void OnCurPlayerChanged(PlayerEnum past, PlayerEnum cur)
    {
        UIManager.Inst.UpdateTurnEndButton();
        Board.Inst.ResetPainted();
    }

    public void OnPlayerActedChanged(bool pre, bool cur)
    {
        if(IsServer && cur)
        {
            SpawnAtRndPoint();
            SwapTurn();
        }
    }

    public void SwapTurn()
    {
        if(curPlayer.Value == PlayerEnum.WHITE) 
            curPlayer.Value = PlayerEnum.BLACK;
        else curPlayer.Value = PlayerEnum.WHITE;

        PlayerActed.Value = false;
        curSelected = Coordinate.none;
    }

    public bool isTherePieceWithOppo(Coordinate coord, PlayerEnum compare)
    {
        return boardPlayerState[coord.X, coord.Y] != compare && boardPlayerState[coord.X, coord.Y] != PlayerEnum.EMPTY; 
    }

    public bool isTherePiece(Coordinate coord)
    {
        return boardPlayerState[coord.X, coord.Y] != PlayerEnum.EMPTY;
    }

    public void GameOver(PlayerEnum winner)
    {
        isGameOver = true;
        Debug.Log($"{winner.ToString()} WINS!!");
        UIManager.Inst.SetResultPanel(winner);
    }

    public void ResetGame()
    {
        curSelected = Coordinate.none;
        curMovable = null;
        WHITE_Idx.Value = 0;
        BLACK_Idx.Value = 0;
        PlayerActed.Value = false;
        curPlayer.Value = PlayerEnum.BLACK;

        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                boardPlayerState[i, j] = PlayerEnum.EMPTY;
                boardPieceState[i, j] = PieceEnum.NONE;
            }
        }

        // should reset at client level
        UIManager.Inst.ResetUI();
        isGameOver = false;
    }

    public PlayerEnum GetPlayerState(Coordinate coord)
    {
        return boardPlayerState[coord.X, coord.Y];
    }

    public PieceEnum GetPieceState(Coordinate coord)
    {
        return boardPieceState[coord.X, coord.Y];
    }

    public bool isSelectedAvailable()
    {
        return curSelected.X > -1 && curSelected.X < 4 && curSelected.Y > -1 && curSelected.Y < 4;
    }

    //Should only call from Server-Side
    public void RemovePiece(Vector2Int cor, bool isMove = true)
    {
        if(boardPieceState[cor.x, cor.y] == PieceEnum.KING && !isMove)
        {
            EndGameClientRpc(boardPlayerState[cor.x, cor.y] == PlayerEnum.WHITE ? PlayerEnum.BLACK : PlayerEnum.WHITE);
        }

        boardState[cor.x, cor.y].player.Value = PlayerEnum.EMPTY;
        boardState[cor.x, cor.y].piece.Value = PieceEnum.NONE;  
        boardPieceState[cor.x, cor.y] = PieceEnum.NONE;
        boardPlayerState[cor.x, cor.y] = PlayerEnum.EMPTY;

        
    }

    public void SetPiece(Vector2Int cor, PlayerEnum player, PieceEnum piece)
    {
        boardState[cor.x, cor.y].player.Value = player;
        boardState[cor.x, cor.y].piece.Value = piece; 
        boardPlayerState[cor.x, cor.y] = player;
        boardPieceState[cor.x, cor.y] = piece;
    }

    [ClientRpc]
    public void EndGameClientRpc(PlayerEnum playerEnum)
    {
        Debug.Log(playerEnum.ToString() + " WINS!!!");
        UIManager.Inst.SetResultPanel(playerEnum);
    }

    public void SpawnAtRndPoint()
    {
        List<Coordinate> spawnableList = new();
        List<Coordinate> unsafeCoordinateList = new();

        for(int i=0; i<4; i++)
        for(int j=0; j<4; j++)
        {
            if(boardPlayerState[i,j] == PlayerEnum.EMPTY) spawnableList.Add(new Coordinate(i,j));
        }

        for(int i=0; i<4; i++)
        for(int j=0; j<4; j++)
        {
            if(boardPlayerState[i,j] != PlayerEnum.EMPTY && boardPlayerState[i,j] != curPlayer.Value)
            {
                foreach(var item in Piece.ReachableCoordinate(new Coordinate(i,j), boardPlayerState[i,j], boardPieceState[i,j], true))
                {
                    if(spawnableList.Contains(item))
                    {
                        spawnableList.Remove(item);
                        unsafeCoordinateList.Add(item);
                    }
                }
            }
        }

        int rnd;
        if(spawnableList.Count < 1)
        {
            if(unsafeCoordinateList.Count < 1) return;
            rnd = Random.Range(0, unsafeCoordinateList.Count);
            // Debug.Log(unsafeCoordinateList.Count + ", Spawn at unsafe coord of" + unsafeCoordinateList[rnd].ToString());
            SpawnPieceServerRpc(unsafeCoordinateList[rnd]);
        }
        else
        {
            rnd = Random.Range(0, spawnableList.Count);
            // Debug.Log(spawnableList.Count + ", Spawn at " + spawnableList[rnd].ToString());
            SpawnPieceServerRpc(spawnableList[rnd]);
        }
        
    }

    [ServerRpc]
    public void SpawnPieceServerRpc(Coordinate cor)
    {
        //Piece spawn procedure
        if(boardPlayerState[cor.X, cor.Y] == PlayerEnum.EMPTY)
        {
            if(curPlayer.Value == PlayerEnum.WHITE)
            {
                if(WHITE_Idx.Value > 15) return;
                SetPiece(new Vector2Int(cor.X, cor.Y), PlayerEnum.WHITE, spawnList[WHITE_Idx.Value++]);
                spawnDone = true;
            }
            else
            {
                if(BLACK_Idx.Value > 15) return;
                SetPiece(new Vector2Int(cor.X, cor.Y), PlayerEnum.BLACK, spawnList[BLACK_Idx.Value++]);
                spawnDone = true;
            }
            StartCoroutine(EUpdateNextPiece());    
        }
    }

    public IEnumerator EUpdateNextPiece()
    {
        yield return null;
        UIManager.Inst.UpdateNextPieceClientRpc();
    }
}