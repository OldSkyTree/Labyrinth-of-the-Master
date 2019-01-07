using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject figurePrefab;
    public GameObject chipPrefab;
    
    private Dictionary<int, GameObject> playersContainer;
    private Dictionary<int, GameObject> cellsContainer;
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
        cellsContainer = FillField();
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

    Dictionary<int, GameObject> FillField()
    {
        Dictionary<int, GameObject> dictionary = new Dictionary<int, GameObject>();

        GameObject cell;
        Vector3 cellPosition = new Vector3();
        Quaternion cellRotation = Quaternion.identity;

        int cellId = 0;
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (i % 2 == 0 && j % 2 == 0)
                    continue;
                float x = (i - 3) * 2.05f;
                float z = (j - 3) * 2.05f;
                cellPosition = new Vector3(x, 0.1f, z);
                int cellType = Random.Range(0, 3);
                cell = CreateCell(cellType);
                cell.transform.position = cellPosition;
                dictionary.Add(cellId++, cell);
            }
        }
        return dictionary;
    }
    GameObject CreateCell(int type)
    {
        Vector3 cellPosition = Vector3.zero;
        Quaternion cellRotation = Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);
        GameObject cell = Instantiate(cellPrefab, cellPosition, cellRotation);

        Vector2[] array = cell.GetComponent<MeshFilter>().mesh.uv;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].x += (float)300 / 1024 * type;
        }
        cell.GetComponent<MeshFilter>().mesh.uv = array;
        return cell;
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
