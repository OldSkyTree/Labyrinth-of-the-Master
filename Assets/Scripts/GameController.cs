using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject figurePrefab;
    public GameObject chipPrefab;

    private List<Player> players;
    private List<Chip> chips;

    private bool isInitialized = false;
    private CellsHolder cellsHolder;

    private GameMode mode;

    public enum GameMode
    {
        NotStarted,
        Started,
        LineChoose,
        RotationChoose,
        FigureMoving,
        Ended
    }

    void Start()
    {
        cellsHolder = CellsHolder.GetCellsHolder();
        players = new List<Player>();
        chips = new List<Chip>();
        InitializeField();
        mode = GameMode.FigureMoving;
    }

    public static GameController GetGameController()
    {
        return GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public GameMode GetGameMode()
    {
        return mode;
    }
    public void SetGameMode(int gameMode)
    {
        mode = (GameMode)gameMode;
    }
    public void IncreaseGameMode()
    {
        mode++;
    }

    public void InitializeField()
    {
        if (isInitialized)
            return;
        for (int i = 0; i < 4; i++)
        {
            players.Add(CreateFigure(i));
        }
        cellsHolder.InitializeCells();
        CreateChips();
        isInitialized = true;
    }

    Color CreateColorFromIntRGB(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    Player CreateFigure(int playerId)
    {
        Vector3 figurePosition = new Vector3();
        Color figureColor = new Color();

        switch (playerId)
        {
            case 0:
                figurePosition = new Vector3(-2.05f, 0, 2.05f);
                figureColor = CreateColorFromIntRGB(204, 6, 5);
                break;
            case 1:
                figurePosition = new Vector3(2.05f, 0, 2.05f);
                figureColor = CreateColorFromIntRGB(6, 57, 113);
                break;
            case 2:
                figurePosition = new Vector3(2.05f, 0, -2.05f);
                figureColor = CreateColorFromIntRGB(225, 204, 79);
                break;
            case 3:
                figurePosition = new Vector3(-2.05f, 0, -2.05f);
                figureColor = CreateColorFromIntRGB(250, 244, 227);
                break;
        }
        Quaternion figureRotation = Quaternion.identity;
        GameObject figure = Instantiate<GameObject>(figurePrefab, figurePosition, figureRotation);
        figure.GetComponent<MeshRenderer>().material.color = figureColor;
        figure.name = "Player " + playerId;
        return new Player(playerId, figure);
    }

    public Player GetCurrentPlayer()
    {
        if (players.Count > 0)
            return players[0];
        return null;
    }
    
    void CreateChips()
    {
        List<int> idHolder = new List<int>();
        
        int chipId = 0;
        int arrayCount = 0;
        for (int i = 1; i < 6; i++)
        {
            for (int j = 1; j < 6; j++)
            {
                if ((i == 2 && j == 2) || (i == 2 && j == 4) || (i == 4 && j == 4) || (i == 4 && j == 2))
                    continue;
                Cell cell = cellsHolder.GetCell(i, j);
                do
                {
                    chipId = Random.Range(0, 21);
                }
                while (idHolder.Contains(chipId));
                idHolder.Add(chipId);
                chips.Add(CreateChip(chipId, cell));
                arrayCount++;
            }
        }
    }
    Chip CreateChip(int chipId, Cell cell)
    {
        float chipLength = 401;
        int textureLength = 2048;

        GameObject chip;
        Quaternion chipRotation = Quaternion.identity;
        Vector3 chipPosition = Vector3.zero;
        chip = Instantiate(chipPrefab, chipPosition, chipRotation, cell.GetGameObject().transform);
        chip.transform.localPosition = Vector3.zero;
        chip.name = "Chip " + (chipId + 1);

        Vector2[] array = chip.GetComponent<MeshFilter>().mesh.uv;
        int x = chipId / 5;
        int y = chipId % 5;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].x += (float)chipLength / textureLength * y;
            array[i].y -= (float)chipLength / textureLength * x;
        }
        chip.GetComponent<MeshFilter>().mesh.uv = array;
        cell.AssignChip(new Chip(chipId, chip));
        return cell.GetChip();
    }

    public static int ZToI(float z)
    {
        return 3 - Mathf.RoundToInt(z / 2.05f);
    }
    public static int XToJ(float x)
    {
        return Mathf.RoundToInt(x / 2.05f) + 3;
    }
    public static float IToZ(int i)
    {
        return (3 - i) * 2.05f;
    }
    public static float JToX(int j)
    {
        return (j - 3) * 2.05f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

public class Player
{
    private int id;
    private string name;
    private GameObject gameObject;
    private List<Chip> collectedChips;
    private int wandCount;

    private List<int> bonusChips;

    private const int WAND_COST = 3;
    private const int BONUS_CHIP_COST = 20;

    public Player(int playerId, GameObject gameObject) : 
        this(playerId, gameObject, "Player " + (playerId + 1))
    {
    }
    public Player(int playerId, GameObject gameObject, string playerName) : 
        this(playerId, gameObject, playerName, new List<int> { -1, -1, -1 })
    {
    }
    public Player(int playerId, GameObject gameObject, string playerName, List<int> bonus)
    {
        id = playerId;
        this.gameObject = gameObject;
        collectedChips = new List<Chip>();
        name = playerName;
        wandCount = 3;
        bonusChips = bonus;
    }

    public void CollectChip(Chip chip)
    {
        collectedChips.Add(chip);
    }

    public int GetId()
    {
        return id;
    }
    public string GetName()
    {
        return name;
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public int GetScore()
    {
        int score = 0;
        foreach (Chip chip in collectedChips)
        {
            score += chip.Cost;
            if (bonusChips.Contains(chip.Cost))
                score += BONUS_CHIP_COST;
        }
        score += wandCount * WAND_COST;
        return score;
    }
    public Vector3 GetPosition
    {
        get { return gameObject.transform.position; }
    }
}

public class Chip
{
    private int cost;
    private GameObject gameObject;

    public Chip(int cost, GameObject gameObject)
    {
        this.cost = cost;
        this.gameObject = gameObject;
    }

    public int Cost
    {
        get { return cost; }
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}

public class Point
{
    private Vector2Int current;
    private Point previous;
    private Cell cell;

    public Point(Cell cell, Vector2Int curr, Point prev)
    {
        this.cell = cell;
        current = curr;
        previous = prev;
    }

    public Cell GetCell
    {
        get { return cell; }
    }
    public Vector2Int GetCurrentPosition
    {
        get { return current; }
    }
    public Point GetPreviousPoint
    {
        get { return previous; }
    }
}
