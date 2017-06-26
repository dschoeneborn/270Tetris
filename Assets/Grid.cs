using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    private const int TIME_BETWEEN_MOVING_UPDATE = 1; //In seconds

    public int w = 10;
    public int h = 20;

    public Text PointsCounter;
    public RawImage GameOverScreen;
    public RawImage DebugItem;
    public bool ShowDebugInformation;

    private Playstone[][] grid;
    private Group movingGroup;

    private float lastUpdatedDown;

    private bool lastFramespawnedItem = false;
    private int points = 0;

    private float lastX;
    private float lastY;
    

    // Use this for initialization
    void Start ()
    {
        GameOverScreen.enabled = false;
        grid = new Playstone[h][];

		for(int y = 0; y < h; y++)
        {
            grid[y] = new Playstone[w];
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        MoveIfButtonPressed();

        if (lastUpdatedDown + TIME_BETWEEN_MOVING_UPDATE < Time.time)
        {
            if (ShowDebugInformation)
            {
                RemoveOldDebugInformation();
                DrawNewDebugInformation();
            }

            if (IsMovingObjectOnBottom())
            {
                ResolveMovingObject();
                DeleteFullRows();

                if (lastFramespawnedItem)
                {
                    PointsCounter.enabled = false;
                    GameOverScreen.enabled = true;
                    Destroy(this);
                }

                FindObjectOfType<Spawner>().spawnNext();
                lastFramespawnedItem = true;
            }
            else
            {
                lastFramespawnedItem = false;
            }

            if (movingGroup != null)
            {
                movingGroup.MoveOneBlock(Direction.DOWN, false);
            }

            lastUpdatedDown = Time.time;
        }
    }

    private void DrawNewDebugInformation()
    {
        foreach (RawImage obj in FindObjectsOfType(typeof(RawImage)))
        {
            if (obj.tag.Equals("DebugImage"))
            {
                for (int x = 0; x < h; x++)
                {
                    for (int y = 0; y < w; y++)
                    {
                        if (grid[x][y] != null)
                        {
                            float positionY = obj.rectTransform.rect.width * x;
                            float positionX = obj.rectTransform.rect.height * y;

                            RawImage instantiated = Instantiate(obj, FindDebugUI().transform);
                            instantiated.tag = "DebugInformationImage";
                            instantiated.transform.position = new Vector2(positionX, positionY);
                        }
                    }
                }
            }
        }
    }

    private void RemoveOldDebugInformation()
    {
        foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.tag.Equals("DebugInformationImage"))
            {
                Destroy(obj);
            }
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

    private void MoveIfButtonPressed()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || (IsAxisAvailable("DpadX") && Input.GetAxis("DpadX") == -1 && lastX != -1))
        {
            lastX = -1;
            movingGroup.MoveOneBlock(Direction.LEFT);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || (IsAxisAvailable("DpadX") && Input.GetAxis("DpadX") == 1 && lastX != 1))
        {
            lastX = 1;
            movingGroup.MoveOneBlock(Direction.RIGHT);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || (IsAxisAvailable("DpadY") && Input.GetAxis("DpadY") == -1 && lastY != -1))
        {
            lastY = -1;
            movingGroup.MoveOneBlock(Direction.DOWN);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || (Input.GetKeyDown("joystick button 0")))
        {
            movingGroup.Rotate();
        }

        if (IsAxisAvailable("DpadX") && Input.GetAxis("DpadX") == 0)
        {
            lastX = 0;
        }

        if (IsAxisAvailable("DpadY") && Input.GetAxis("DpadY") == 0)
        {
            lastY = 0;
        }
    }

    private GameObject FindDebugUI()
    {
        foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.tag.Equals("DebugUI"))
            {
                return obj;
            }
        }

        return null;
    }

    private bool IsMovingObjectOnBottom()
    {
        return (movingGroup != null && !movingGroup.CanMoveOneBlock(Direction.DOWN));
    }

    private void ResolveMovingObject()
    {
        List<Playstone> childs = new List<Playstone>();

#pragma warning disable CS0612
        for (int i = 0; i < movingGroup.transform.childCount; i++)
        {
            childs.Add(movingGroup.transform.GetChild(i).GetComponent<Playstone>());
        }
#pragma warning restore CS0612

        foreach (Playstone child in childs)
        {
            int rowNumber = (int)Math.Round(child.Position.y);
            int columnNumber = (int)Math.Round(child.Position.x);
            grid[rowNumber][columnNumber] = child;
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
        for(int y=0; y<h; y++)
        {
            if(IsRowFull(y))
            {
                points += 100;
                DeleteRow(y);
                DecreaseAllRowsAbove(y);
                y--;
            }
        }
    }

    private void DecreaseAllRowsAbove(int above)
    {
        for (int x = above; x < h - 1; x++)
        {
            grid[x] = grid[x + 1];
            grid[x + 1] = new Playstone[w];

            for (int y = 0; y < w ; y++)
            {
                if(grid[x][y] != null)
                {
                    grid[x][y].DecreasedTimes += 1;
                }
            }
        }
    }

    private bool IsAxisAvailable(string axisName)
    {
        try
        {
            Input.GetAxis(axisName);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
