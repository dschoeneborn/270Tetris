using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    private const int FRAMES_BETWEEN_KEY_UPDATE = 10;
    private const int FRAMES_BETWEEN_MOVING_UPDATE = 40;

    public int w = 10;
    public int h = 20;

    public Text PointsCounter;

    private GameObject[][] grid;

    private Group movingGroup;

    private long lastUpdatedDown;
    private long lastUpdatedKeys;
    private long actualFrame;

    private bool lastFramespawnedItem = false;
    private int points = 0;

    // Use this for initialization
    void Start ()
    {
        grid = new GameObject[h][];

		for(int y = 0; y < h; y++)
        {
            grid[y] = new GameObject[w];
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        actualFrame++;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movingGroup.MoveOneBlock(Direction.LEFT);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            movingGroup.MoveOneBlock(Direction.RIGHT);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            movingGroup.MoveOneBlock(Direction.DOWN);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            movingGroup.Rotate();
        }

        if (lastUpdatedKeys + FRAMES_BETWEEN_KEY_UPDATE < actualFrame)
        {
            if (PointsCounter != null)
            {
                PointsCounter.text = "Points: " + points;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                movingGroup.MoveOneBlock(Direction.LEFT);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                movingGroup.MoveOneBlock(Direction.RIGHT);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                movingGroup.MoveOneBlock(Direction.DOWN);
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                movingGroup.Rotate();
            }

            lastUpdatedKeys = actualFrame;
        }

        if (lastUpdatedDown + FRAMES_BETWEEN_MOVING_UPDATE < actualFrame)
        {
            if (IsMovingObjectOnBottom())
            {
                ResolveMovingObject();
                DeleteFullRows();

                if (lastFramespawnedItem)
                {
                    Destroy(this);
                }

                FindObjectOfType<Spawner>().spawnNext();
                lastFramespawnedItem = true;
            }
            else
            {
                lastFramespawnedItem = false;
            }

            movingGroup.MoveOneBlock(Direction.DOWN, false);

            lastUpdatedDown = actualFrame;
        }
    }

    public bool RegisterMovingObject(Group newGroup)
    {
        if (movingGroup == null)
        {
            movingGroup = newGroup;
            return true;
        }

        return false;
    }

    private bool IsMovingObjectOnBottom()
    {
        return (!movingGroup.CanMoveOneBlock(Direction.DOWN));
    }

    private void ResolveMovingObject()
    {
        List<Transform> childs = new List<Transform>();

        for (int i = 0; i < movingGroup.transform.childCount; i++)
        {
            childs.Add(movingGroup.transform.GetChild(i));
        }

        foreach(Transform child in childs)
        {
            int rowNumber = (int)Math.Round(child.position.y);
            int columnNumber = (int)Math.Round(child.position.x);
            grid[rowNumber][columnNumber] = child.gameObject;
        }
        movingGroup = null;
    }

    public bool IsInsideBorder(Vector2 pos)
    {
        return ((int)pos.x >= 0 &&
                (int)pos.x < w &&
                (int)pos.y >= 0 &&
                (int)pos.y < h);
    }

    public bool IsValidPosition(int y, int x)
    {
        return (IsInsideBorder(new Vector2(x, y)) &&
            grid[y][x] == null);
    }

    private void DeleteRow(int y)
    {
        for(int x = 0; x<w; x++)
        {
            Destroy(grid[y][x].gameObject);
            grid[y][x] = null;
        }
    }

    private void DecreaseRow(int y)
    {
        for(int x = 0; x<w; ++x)
        {
            if(grid[y][x] != null)
            {
                grid[y][x].transform.position += new Vector3(0, -1, 0);
                grid[y - 1][x] = grid[y][x];
                grid[y][x] = null;
            }
        }
    }

    public void DecreseRowsAbove(int y)
    {
        for(int i = y; i<grid.Length; i++)
        {
            DecreaseRow(i);
        }
    }

    public bool IsRowFull(int y)
    {
        for(int x=0; x<w; x++)
        {
            if(grid[y][x] == null)
            {
                return false;
            }
        }
        return true;
    }

    public void DeleteFullRows()
    {
        for(int y=0; y<h; ++y)
        {
            if(IsRowFull(y))
            {
                points += 100;
                DeleteRow(y);
                DecreseRowsAbove(y + 1);
                --y;
            }
        }
    }
}
