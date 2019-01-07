using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject figurePrefab;
    public GameObject chipPrefab;
    
    private Dictionary<int, GameObject> playersContainer;
    private Cell[,] cells = new Cell[7, 7];
    private Dictionary<int, GameObject> chipsContainer;

    private bool isInitialized = false;

    void Start()
    {
        InitializeField();
    }
    
    public void InitializeField()
    {
        if (isInitialized)
            return;
        playersContainer = new Dictionary<int, GameObject>
        {
            { 0, CreateFigure(0, CreateColorFromIntRGB(204, 6, 5)) },
            { 1, CreateFigure(1, CreateColorFromIntRGB(6, 57, 113)) },
            { 2, CreateFigure(2, CreateColorFromIntRGB(225, 204, 79)) },
            { 3, CreateFigure(3, CreateColorFromIntRGB(250, 244, 227)) }
        };
        FillField();
        chipsContainer = CreateChips();
        isInitialized = true;
    }

    Color CreateColorFromIntRGB(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    GameObject CreateFigure(int playerId, Color color)
    {
        Vector3 figurePosition = new Vector3();
     
        switch (playerId)
        {
            case 0:
                figurePosition = new Vector3(-2, 0.3f, 2);
                break;
            case 1:
                figurePosition = new Vector3(2, 0.3f, 2);
                break;
            case 2:
                figurePosition = new Vector3(2, 0.3f, -2);
                break;
            case 3:
                figurePosition = new Vector3(-2, 0.3f, -2);
                break;
        }
        Quaternion figureRotation = Quaternion.identity;
        GameObject figure = Instantiate<GameObject>(figurePrefab, figurePosition, figureRotation);
        figure.GetComponent<MeshRenderer>().material.color = color;
        return figure;
    }

    void FillField()
    {
        Vector3 cellPosition = new Vector3();
        
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
                cells[i, j].GetGameObject().name = "Cell" + (i * 7 + j);
            }
        }
    }
    Cell CreateCell(int type)
    {
        Vector3 cellPosition = Vector3.zero;
        int randomTurnCount = Random.Range(0, 4);
        Quaternion cellRotation = Quaternion.Euler(0, randomTurnCount * 90, 0);
        GameObject cell = Instantiate(cellPrefab, cellPosition, cellRotation);

        Vector2[] array = cell.GetComponent<MeshFilter>().mesh.uv;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].x += (float)300 / 1024 * type;
        }
        cell.GetComponent<MeshFilter>().mesh.uv = array;

        return new Cell(type, cell, randomTurnCount);
    }

    Dictionary<int, GameObject> CreateChips()
    {
        Dictionary<int, GameObject> dictionary = new Dictionary<int, GameObject>();
        Vector3 chipPosition = new Vector3();
        GameObject chip;
        int chipId = 0;
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
                while (dictionary.ContainsKey(chipId));
                chip = CreateChip(chipId);
                chip.name = "Chip " + (chipId + 1);
                chip.transform.position = chipPosition;
                dictionary.Add(chipId, chip);
            }
        }
        return dictionary;
    }
    GameObject CreateChip(int chipId)
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
        return chip;
    }

    public Dictionary<int, GameObject> GetChips()
    {
        return chipsContainer;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

class Cell
{
    private int up;
    private int right;
    private int down;
    private int left;
    private GameObject gameObject;

    public Cell(int type, GameObject gameObject)
    {
        switch (type)
        {
            case 0:
                up = 0; right = 1; down = 1; left = 0;
                break;
            case 1:
                up = 0; right = 1; down = 1; left = 1;
                break;
            case 2:
                up = 0; right = 1; down = 0; left = 0;
                break;
        }
        this.gameObject = gameObject;
    }
    public Cell(int type, GameObject gameObject, int turnCount) : this(type, gameObject)
    {
        if (turnCount > 0)
            RotateRight(turnCount);
        else if (turnCount < 0)
            RotateLeft(turnCount);
    }

    void RotateRight(int turnNumber)
    {
        for (int i = 0; i < turnNumber; i++)
        {
            int upTemp = up;
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
            int upTemp = up;
            up = right;
            right = down;
            down = left;
            left = upTemp;
        }
    }

    public int Up
    {
        get { return up; }
        private set
        {
            up = (value > 1 || value < 0) ? 0 : value;
        }
    }
    public int Right
    {
        get { return right; }
        private set
        {
            right = (value > 1 || value < 0) ? 0 : value;
        }
    }
    public int Down
    {
        get { return down; }
        private set
        {
            down = (value > 1 || value < 0) ? 0 : value;
        }
    }
    public int Left
    {
        get { return left; }
        private set
        {
            left = (value > 1 || value < 0) ? 0 : value;
        }
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
