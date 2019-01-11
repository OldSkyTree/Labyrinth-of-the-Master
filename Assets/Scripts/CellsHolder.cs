using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellsHolder : MonoBehaviour
{
    public int fieldWidth, fieldHeight;
    public GameObject cellPrefab;
    public float aboveGround;

    private Cell[,] cells;

    private List<Cell> choosenCells;
    private bool isUp;

    private GameController gameController;

    void Start()
    {
        gameController = GameController.GetGameController();
    }

    public void ShowPath(GameObject rayReceiver)
    {
        if (gameController.GetGameMode() != GameController.GameMode.FigureMoving)
            return;
        int iFrom = GameController.ZToI(gameController.GetCurrentPlayer().GetPosition.z);
        int jFrom = GameController.XToJ(gameController.GetCurrentPlayer().GetPosition.x);

        int iTo = GameController.ZToI(rayReceiver.transform.position.z);
        int jTo = GameController.XToJ(rayReceiver.transform.position.x);

        List<Cell> list = GetVisitedCells(new Vector2Int(iFrom, jFrom), new Vector2Int(iTo, jTo));
        if (!IsUp)
            LiftCells(list);
        else
        {
            ResetLift();
            LiftCells(list);
        }
    }

    public void InitializeCells()
    {
        cells = new Cell[fieldHeight, fieldWidth];
        isUp = false;
        FillField();
    }

    public static CellsHolder GetCellsHolder()
    {
        return GameObject.FindGameObjectWithTag("CellsHolder").GetComponent<CellsHolder>();
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
                cellPosition = new Vector3(x, 0, z);
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
                float z = GameController.IToZ(i);
                float x = GameController.JToX(j);
                cellPosition = new Vector3(x, 0, z);
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
                float z = GameController.IToZ(i);
                float x = GameController.JToX(j);
                cellPosition = new Vector3(x, 0, z);
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
                float z = GameController.IToZ(i);
                float x = GameController.JToX(j);
                cellPosition = new Vector3(x, 0, z);
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
                float z = GameController.IToZ(i);
                float x = GameController.JToX(j);
                cellPosition = new Vector3(x, 0, z);
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
        GameObject cell = Instantiate(cellPrefab, cellPosition, cellRotation, this.transform);

        Vector2[] uvArray = cell.GetComponent<MeshFilter>().mesh.uv;
        for (int i = 0; i < uvArray.Length; i++)
        {
            uvArray[i].x += (float)300 / 1024 * type;
        }
        cell.GetComponent<MeshFilter>().mesh.uv = uvArray;

        return new Cell(type, cell, turnNumber);
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
    public List<Cell> GetVisitedCells(Vector2Int From, Vector2Int To)
    {
        if (From.x < 0 || From.y < 0 || To.x < 0 || To.y < 0 || From.x > 6 || From.y > 6 || To.x > 6 || To.y > 6)
            return new List<Cell>();

        if (From == To)
            return new List<Cell>() { cells[From.x, From.y] };

        Queue<Point> queue = new Queue<Point>();

        List<Point> visitedCells = new List<Point>();

        queue.Enqueue(new Point(cells[From.x, From.y], From, null));

        while (queue.Count != 0)
        {
            Point current = queue.Dequeue();
            Vector2Int nextPosition = Vector2Int.zero;
            Vector2Int currentPosition = current.GetCurrentPosition;

            visitedCells.Add(current);

            //---------------------------------------------UP
            nextPosition = currentPosition + Vector2Int.left;
            if (current.GetCell.Up && currentPosition.x > 0 && cells[nextPosition.x, nextPosition.y].Down)
            {
                if (nextPosition == To)
                    return RestorePath(new Point(cells[nextPosition.x, nextPosition.y], nextPosition, current));
                else
                if (!ContainsCell(visitedCells, cells[nextPosition.x, nextPosition.y]))
                    queue.Enqueue(new Point(cells[nextPosition.x, nextPosition.y], nextPosition, current));
            }
            //---------------------------------------------RIGHT
            nextPosition = currentPosition + Vector2Int.up;
            if (current.GetCell.Right && currentPosition.y < 6 && cells[nextPosition.x, nextPosition.y].Left)
            {
                if (nextPosition == To)
                    return RestorePath(new Point(cells[nextPosition.x, nextPosition.y], nextPosition, current));
                else
                if (!ContainsCell(visitedCells, cells[nextPosition.x, nextPosition.y]))
                    queue.Enqueue(new Point(cells[nextPosition.x, nextPosition.y], nextPosition, current));
            }
            //---------------------------------------------DOWN
            nextPosition = currentPosition + Vector2Int.right;
            if (current.GetCell.Down && currentPosition.x < 6 && cells[nextPosition.x, nextPosition.y].Up)
            {
                if (nextPosition == To)
                    return RestorePath(new Point(cells[nextPosition.x, nextPosition.y], nextPosition, current));
                else
                if (!ContainsCell(visitedCells, cells[nextPosition.x, nextPosition.y]))
                    queue.Enqueue(new Point(cells[nextPosition.x, nextPosition.y], nextPosition, current));
            }
            //---------------------------------------------LEFT
            nextPosition = currentPosition + Vector2Int.down;
            if (current.GetCell.Left && currentPosition.y > 0 && cells[nextPosition.x, nextPosition.y].Right)
            {
                if (nextPosition == To)
                    return RestorePath(new Point(cells[nextPosition.x, nextPosition.y], nextPosition, current));
                else
                if (!ContainsCell(visitedCells, cells[nextPosition.x, nextPosition.y]))
                    queue.Enqueue(new Point(cells[nextPosition.x, nextPosition.y], nextPosition, current));
            }
        }
        return new List<Cell>();
    }
    private bool ContainsCell(List<Point> list, Cell cell)
    {
        foreach (Point p in list)
        {
            if (p.GetCell.Equals(cell))
                return true;
        }
        return false;
    }
    private List<Cell> RestorePath(Point reachedPoint)
    {
        List<Cell> result = new List<Cell>() { reachedPoint.GetCell };

        Point temp = reachedPoint;

        while ((temp = temp.GetPreviousPoint) != null)
        {
            result.Insert(0, temp.GetCell);
        }

        return result;
    }

    public Cell GetCell(int i, int j)
    {
        return cells[i, j];
    }
    public Cell GetCellById(int id)
    {
        if (id < 0 && id > (fieldHeight - 1) * (fieldWidth - 1))
            return null;

        int i = Mathf.FloorToInt(id / fieldHeight);
        int j = Mathf.FloorToInt(id % fieldWidth);
        return cells[i, j];
    }
    public Cell ContainsChip(Chip chip)
    {
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
                if (cells[i, j].GetChip().Equals(chip))
                    return cells[i, j];
        return null;
    }

    public void LiftCells(List<Cell> list)
    {
        choosenCells = list;
        if (choosenCells != null)
        {
            for (int i = 0; i < fieldHeight; i++)
                for (int j = 0; j < fieldWidth; j++)
                    if (choosenCells.Contains(cells[i, j]))
                        cells[i, j].GetGameObject().transform.Translate(0, aboveGround, 0);
        }
        isUp = true;
    }
    public void ResetLift()
    {
        foreach (Cell cell in choosenCells)
        {
            cell.GetGameObject().transform.Translate(0, -aboveGround, 0);
        }
        choosenCells = null;
        isUp = false;
    }

    public void MoveLine(int lineNumber, int direction)
    {
        if (direction == 0)
            return;
        if (lineNumber < 0 || lineNumber >= fieldHeight || lineNumber % 2 == 0)
            return;

        direction /= Mathf.Abs(direction);
        
        if (direction > 0)
        {
            Cell temp = cells[lineNumber, fieldWidth - 1];
            for (int j = fieldWidth - 1; j > 0; j--)
            {
                cells[lineNumber, j] = cells[lineNumber, j - 1];
                cells[lineNumber, j - 1].MoveAt(lineNumber, j);
            }
            cells[lineNumber, 0] = temp;
            temp.MoveAt(lineNumber, 0);
        }
        else if (direction < 0)
        {
            Cell temp = cells[lineNumber, 0];
            for (int j = 0; j < fieldWidth - 1; j++)
            {
                cells[lineNumber, j] = cells[lineNumber, j + 1];
                cells[lineNumber, j + 1].MoveAt(lineNumber, j);
            }
            cells[lineNumber, fieldWidth - 1] = temp;
            temp.MoveAt(lineNumber, fieldWidth - 1);
        }

    }
    public void MoveColumn(int columnNumber, int direction)
    {
        if (direction == 0)
            return;
        if (columnNumber < 0 || columnNumber >= fieldHeight || columnNumber % 2 == 0)
            return;

        direction /= Mathf.Abs(direction);

        if (direction > 0)
        {
            Cell temp = cells[0, columnNumber];
            for (int i = 0; i < fieldHeight - 1; i++)
            {
                cells[i, columnNumber] = cells[i + 1, columnNumber];
                cells[i + 1, columnNumber].MoveAt(i, columnNumber);
            }
            cells[fieldHeight - 1, columnNumber] = temp;
            temp.MoveAt(fieldHeight - 1, columnNumber);
        }
        else if (direction < 0)
        {
            Cell temp = cells[fieldHeight - 1, columnNumber];
            for (int i = fieldHeight - 1; i > 0; i--)
            {
                cells[i, columnNumber] = cells[i - 1, columnNumber];
                cells[i - 1, columnNumber].MoveAt(i, columnNumber);
            }
            cells[0, columnNumber] = temp;
            temp.MoveAt(0, columnNumber);
        }
    }

    public bool IsUp
    {
        get { return isUp; }
    }
}

public class Cell
{
    private bool up;
    private bool right;
    private bool down;
    private bool left;
    private GameObject gameObject;
    private Chip chip;

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

    public void MoveAt(int i, int j)
    {
        float z = GameController.IToZ(i);
        float x = GameController.JToX(j);

        gameObject.transform.position = new Vector3(x, 0, z);
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
        private set { down = value; }
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

    public void AssignChip(Chip chip)
    {
        this.chip = chip;
    }
    public void RemoveChip()
    {
        chip = null;
    }
    public Chip GetChip()
    {
        return chip;
    }
}
