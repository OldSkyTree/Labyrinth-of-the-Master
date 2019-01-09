using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject figurePrefab;
    public GameObject chipPrefab;

    private List<Player> players;
    private Cell[,] cells = new Cell[7, 7];
    private List<Chip> chips;

    private bool isInitialized = false;

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
        FillField();
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
                figurePosition = new Vector3(-2.05f, 0.2f, 2.05f);
                figureColor = CreateColorFromIntRGB(204, 6, 5);
                break;
            case 1:
                figurePosition = new Vector3(2.05f, 0.2f, 2.05f);
                figureColor = CreateColorFromIntRGB(6, 57, 113);
                break;
            case 2:
                figurePosition = new Vector3(2.05f, 0.2f, -2.05f);
                figureColor = CreateColorFromIntRGB(225, 204, 79);
                break;
            case 3:
                figurePosition = new Vector3(-2.05f, 0.2f, -2.05f);
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

    void FillField()
    {
        Vector3 cellPosition = new Vector3();

        CreateBasicCell();

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (i % 2 == 0 && j % 2 == 0)
                    continue;
                float z = (3 - i) * 2.05f;
                float x = (j - 3) * 2.05f;
                cellPosition = new Vector3(x, 0.1f, z);
                int cellType = Random.Range(0, 3);
                cells[i, j] = CreateCell(cellType);
                cells[i, j].GetGameObject().transform.position = cellPosition;
            }
        }
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                cells[i, j].GetGameObject().name = "Cell " + (i * 7 + j);
            }
        }
    }
    void CreateBasicCell()
    {
        Vector3 cellPosition = new Vector3();

        int turn;

        for (int i = 0; i < 7; i += 6)
        {
            turn = 0;
            for (int j = 0; j < 7; j += 6)
            {
                if (i == 6)
                    turn--;
                float z = (3 - i) * 2.05f;
                float x = (j - 3) * 2.05f;
                cellPosition = new Vector3(x, 0.1f, z);
                cells[i, j] = CreateCell(0, turn);
                cells[i, j].GetGameObject().transform.position = cellPosition;
                if (i == 0)
                    turn++;
            }
        }

        for (int i = 2; i < 5; i += 2)
        {
            turn = -1;
            for (int j = 0; j < 7; j += 6)
            {
                float z = (3 - i) * 2.05f;
                float x = (j - 3) * 2.05f;
                cellPosition = new Vector3(x, 0.1f, z);
                cells[i, j] = CreateCell(1, turn);
                cells[i, j].GetGameObject().transform.position = cellPosition;
                turn = 1;
            }
        }

        turn = 0;
        for (int i = 0; i < 7; i += 6)
        {
            for (int j = 2; j < 5; j += 2)
            {
                float z = (3 - i) * 2.05f;
                float x = (j - 3) * 2.05f;
                cellPosition = new Vector3(x, 0.1f, z);
                cells[i, j] = CreateCell(1, turn);
                cells[i, j].GetGameObject().transform.position = cellPosition;
            }
            turn = 2;
        }

        for (int i = 2; i < 5; i += 2)
        {
            turn = -1;
            for (int j = 2; j < 5; j += 2)
            {
                if (i == 4)
                    turn--;
                float z = (3 - i) * 2.05f;
                float x = (j - 3) * 2.05f;
                cellPosition = new Vector3(x, 0.1f, z);
                cells[i, j] = CreateCell(1, turn);
                cells[i, j].GetGameObject().transform.position = cellPosition;
                if (i == 2)
                    turn++;
            }
        }

    }
    Cell CreateCell(int type)
    {
        int randomTurnCount = Random.Range(0, 4);
        return CreateCell(type, randomTurnCount);
    }
    Cell CreateCell(int type, int turnNumber)
    {
        Vector3 cellPosition = Vector3.zero;
        Quaternion cellRotation = Quaternion.Euler(0, turnNumber * 90, 0);
        GameObject cell = Instantiate(cellPrefab, cellPosition, cellRotation);

        Vector2[] array = cell.GetComponent<MeshFilter>().mesh.uv;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].x += (float)300 / 1024 * type;
        }
        cell.GetComponent<MeshFilter>().mesh.uv = array;

        return new Cell(type, cell, turnNumber);
    }

    public Cell GetCell(int i, int j)
    {
        return cells[i, j];
    }

    void CreateChips()
    {
        List<int> idHolder = new List<int>();

        Vector3 chipPosition = new Vector3();
        int chipId = 0;
        int arrayCount = 0;
        for (float x = -4.1f; x <= 4.1f; x += 2.05f)
        {
            for (float z = 4.1f; z >= -4.1f; z -= 2.05f)
            {
                if (Mathf.Abs(x) == 2.05f && Mathf.Abs(z) == 2.05f)
                    continue;
                chipPosition = new Vector3(x, 0.2f, z);
                do
                {
                    chipId = Random.Range(0, 21);
                }
                while (idHolder.Contains(chipId));
                idHolder.Add(chipId);
                chips.Add(CreateChip(chipId));
                chips[arrayCount].GetGameObject().name = "Chip " + (chipId + 1);
                chips[arrayCount].GetGameObject().transform.position = chipPosition;
                arrayCount++;
            }
        }
    }
    Chip CreateChip(int chipId)
    {
        float chipLength = 401;
        int textureLength = 2048;

        GameObject chip;
        Quaternion chipRotation = Quaternion.identity;
        Vector3 chipPosition = Vector3.zero;
        chip = Instantiate(chipPrefab, chipPosition, chipRotation);

        Vector2[] array = chip.GetComponent<MeshFilter>().mesh.uv;
        int x = chipId / 5;
        int y = chipId % 5;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].x += (float)chipLength / textureLength * y;
            array[i].y -= (float)chipLength / textureLength * x;
        }
        chip.GetComponent<MeshFilter>().mesh.uv = array;
        return new Chip(chipId, chip);
    }

    public bool IsMoveAvailable(Vector2Int From, Vector2Int To)
    {
        if (From.x < 0 || From.y < 0 || To.x < 0 || To.y < 0 || From.x > 6 || From.y > 6 || To.x > 6 || To.y > 6)
            return false;
        
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        List<Cell> visitedCells = new List<Cell>();
        
        queue.Enqueue(From);

        while (queue.Count != 0)
        {
            Vector2Int current = queue.Dequeue();

            visitedCells.Add(cells[current.x, current.y]);
            
            //---------------------------------------------UP
            if (cells[current.x, current.y].Up && current.x > 0 && cells[current.x - 1, current.y].Down)
            {
                Vector2Int nextCell = current + Vector2Int.left;
                if (nextCell == To)
                    return true;
                else 
                if (!visitedCells.Contains(cells[nextCell.x, nextCell.y]))
                    queue.Enqueue(nextCell);
            }
            //---------------------------------------------RIGHT
            if (cells[current.x, current.y].Right && current.y < 6 && cells[current.x, current.y + 1].Left)
            {
                Vector2Int nextCell = current + Vector2Int.up;
                if (nextCell == To)
                    return true;
                else
                if (!visitedCells.Contains(cells[nextCell.x, nextCell.y]))
                    queue.Enqueue(nextCell);
            }
            //---------------------------------------------DOWN
            if (cells[current.x, current.y].Down && current.x < 6 && cells[current.x + 1, current.y].Up)
            {
                Vector2Int nextCell = current + Vector2Int.right;
                if (nextCell == To)
                    return true;
                else
                if (!visitedCells.Contains(cells[nextCell.x, nextCell.y]))
                    queue.Enqueue(nextCell);
            }
            //---------------------------------------------LEFT
            if (cells[current.x, current.y].Left && current.y > 0 && cells[current.x, current.y - 1].Right)
            {
                Vector2Int nextCell = current + Vector2Int.down;
                if (nextCell == To)
                    return true;
                else
                if (!visitedCells.Contains(cells[nextCell.x, nextCell.y]))
                    queue.Enqueue(nextCell);
            }
        }
        return false;
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

public class Cell
{
    private bool up;
    private bool right;
    private bool down;
    private bool left;
    private GameObject gameObject;

    public Cell(int type, GameObject gameObject)
    {
        switch (type)
        {
            case 0:
                up = false; right = true; down = true; left = false;
                break;
            case 1:
                up = false; right = true; down = true; left = true;
                break;
            case 2:
                up = false; right = true; down = false; left = true;
                break;
        }
        this.gameObject = gameObject;
    }
    public Cell(int type, GameObject gameObject, int turnCount) : this(type, gameObject)
    {
        if (turnCount > 0)
            RotateRight(turnCount);
        else if (turnCount < 0)
            RotateLeft(-turnCount);
    }

    void RotateRight(int turnNumber)
    {
        for (int i = 0; i < turnNumber; i++)
        {
            bool upTemp = up;
            up = left;
            left = down;
            down = right;
            right = upTemp;
        }
    }
    void RotateLeft(int turnNumber)
    {
        for (int i = 0; i < turnNumber; i++)
        {
            bool upTemp = up;
            up = right;
            right = down;
            down = left;
            left = upTemp;
        }
    }

    public bool Up
    {
        get { return up; }
        private set { up = value; }
    }
    public bool Right
    {
        get { return right; }
        private set { right = value; }
    }
    public bool Down
    {
        get { return down; }
        private set { down = value;}
    }
    public bool Left
    {
        get { return left; }
        private set { left = value; }
    }
    public GameObject GetGameObject()
    {
        return gameObject;
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
