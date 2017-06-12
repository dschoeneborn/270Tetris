using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int w = 10;
    public int h = 20;

    private GameObject[][] grid;

    private Group movingGroup;

    private long lastUpdated;
    private long actualFrame;

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

        if(lastUpdated + 25 < actualFrame)
        {
            if (Input.GetKey(KeyCode.J))
            {
                movingGroup.MoveOneBlock(Direction.LEFT);
            }
            else if (Input.GetKey(KeyCode.L))
            {
                movingGroup.MoveOneBlock(Direction.RIGHT);
            }
            else if (Input.GetKey(KeyCode.K))
            {
                movingGroup.MoveOneBlock(Direction.DOWN);
            }

            if (IsMovingObjectOnBottom())
            {
                ResolveMovingObject();
                DeleteFullRows();
                FindObjectOfType<Spawner>().spawnNext();
            }

            lastUpdated = actualFrame;
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
        }
    }

    private void DecreaseRow(int y)
    {
        for(int x = 0; x<w; ++x)
        {
            if(grid[y][x] != null)
            {
                grid[y][x].transform.position += new Vector3(0, -1, 0);
            }
        }
    }

    public void DecreseRowsAbove(int y)
    {
        for(int i = y; i<grid.Length; i++)
        {
            DecreaseRow(i);

            if(i != grid.Length -1)
            {
                grid[i] = grid[i + 1];
            }
            else
            {
                grid[i] = new GameObject[w];
            }
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
                DeleteRow(y);
                DecreseRowsAbove(y + 1);
                --y;
            }
        }
    }
}
