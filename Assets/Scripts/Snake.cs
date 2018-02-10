using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField]
    private float xSpeed;

    [SerializeField]
    private float zSpeed;

    [SerializeField]
    private GameObject head;

    [SerializeField]
    private Game game;

    private float xPos;
    private float zPos;

    private Vector3 currentDirection;

    private List<Food> body = new List<Food>();

    private Vector3[] positionHistory = new Vector3[30];

    void Start()
    {
        game = FindObjectOfType<Game>();
    }

    void Update()
    {
        UpdatePosition();
        UpdatePositionHistory();
        UpdateFoodPosition();
        DetectLost();
        ProcessKeyboard();
    }

    private void DetectLost()
    {
        foreach (var food in body)
        {
            if (food.transform.position == head.transform.position)
            {
                game.Lost();
                return;
            }
        }
    }

    private void UpdateFoodPosition()
    {
        for (int i = 0; i < body.Count; i++)
        {
            Food food = body[i];
            food.transform.position = positionHistory[i+1];
        }
        
        
    }

    private void UpdatePositionHistory()
    {
        if (positionHistory[0] != head.transform.position)
        {
            for (int i = positionHistory.Length - 1; i > 0; i--)
            {
                positionHistory[i] = positionHistory[i - 1];
            }

            positionHistory[0] =  head.transform.position;
        }        
    }

    public Vector3 HeadPosition()
    {
        return head.transform.position;
    }

    private void UpdatePosition()
    {
        var pos = head.transform.position;

        var xDelta = xSpeed * Time.deltaTime * currentDirection.x;
        var zDelta = zSpeed * Time.deltaTime * currentDirection.z;

        xPos += xDelta;
        zPos += zDelta;

        float halfGrid = game.GridScale / 2;

        xPos = Mathf.Clamp(xPos, -game.Size.x + halfGrid, game.Size.x - halfGrid * 2);
        zPos = Mathf.Clamp(zPos, -game.Size.z + halfGrid, game.Size.z - halfGrid * 2);

        pos.x = xPos >= 0 ? xPos + halfGrid : xPos;
        pos.z = zPos >= 0 ? zPos + halfGrid : zPos;

        pos.x = pos.x - pos.x % halfGrid;
        pos.z = pos.z - pos.z % halfGrid;

        head.transform.position = pos;
    }

    private void ProcessKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetDirection(new Vector3(-1, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetDirection(new Vector3(1, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetDirection(new Vector3(0, 0, 1));
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetDirection(new Vector3(0, 0, -1));
        }
    }

    private void SetDirection(Vector3 direction)
    {
        currentDirection = direction;
        var lookDirection = new Vector3(90 * direction.x, 0, 90 * direction.z);
        head.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
    }

    public void Grow(Food food)
    {
        if (body.Count >= positionHistory.Length)
        {
            game.Win();
            return;
        }
        
        body.Add(food);
        food.transform.SetParent(transform);
        food.transform.localPosition = new Vector3(0, 0, -0.5f * body.Count);
    }

    public void Reset()
    {
        head.transform.position = Vector3.zero;
        
        
        foreach (var food in body)
        {
            Destroy(food.gameObject);
        }

        for (int i = 0; i < positionHistory.Length; i++)
        {
            positionHistory[i] = Vector3.zero;
        }
        
        body.Clear();
    }
}