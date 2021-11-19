using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStructs;

public class Grid : MonoBehaviour
{
    // BREDD = 16, HÖJD = 9
    Camera cam;
    static Vector3[,] gridPositions = new Vector3[17, 10];
    [SerializeField] GameObject dotPrefab;
    static float gridLength;

    public static Vector3[,] GridPositions { get => gridPositions; set => gridPositions = value; }
    public static float GridLength { get => gridLength; set => gridLength = value; }

    void Start()
    {
        cam = Camera.main;
        dotPrefab.transform.localScale = new Vector3(0.1f, 0.1f, 1);
        CreateGrid();

    }

    void Update()
    {
        UpdateGridPositions();
    }
    private void UpdateGridPositions()
    {
        Vector3 min = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.gameObject.transform.position.z));
        Vector3 maxX = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, cam.gameObject.transform.position.z));

        float widthWorldUnits = Vector3.Distance(min, maxX);

        float yPos = min.y;
        gridLength = widthWorldUnits / 16;
        for (int y = 0; y < 10; yPos += gridLength, y++) // höjd
        {
            float xPos = min.x;
            for (int x = 0; x < 17; xPos += gridLength, x++) //bredd
            {
                GridPositions[x, y] = new Vector3(xPos, yPos);
            }
        }
    }
    private void CreateGrid()
    {
        Vector3 min = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.gameObject.transform.position.z));
        Vector3 maxX = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, cam.gameObject.transform.position.z));
        float widthWorldUnits = Vector3.Distance(min, maxX);

        float yPos = min.y;
        gridLength = widthWorldUnits / 16;
        for (int y = 0; y < 10; yPos += gridLength, y++) // höjd
        {
            float xPos = min.x;
            for (int x = 0; x < 17; xPos += gridLength, x++) //bredd
            {
                GridPositions[x, y] = new Vector3(xPos, yPos);
            }
        }
    }
    public static Point FindClosestGridPosition(Vector3 inPosition)
    {
        float closestDistance = float.MaxValue;
        Point closestPoint = new Point();

        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 17; x++)
            {
                float distance = Vector3.Distance(inPosition, gridPositions[x,y]);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint.x = x;
                    closestPoint.y = y;
                }
            }
        }
        return closestPoint;
    }
}
