using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour {
    float lastFall = 0;

    public Grid GameController;

    private bool registered;

    // Use this for initialization
    void Start()
    {
        FindGamecontrollerIfNull();

        if (GameController == null)
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update ()
    {
		if(!registered)
        {
            if(GameController.RegisterMovingObject(this))
            {
                registered = true;
            }
        }
    }

    public void MoveOneBlock(Direction direction)
    {
        if (CanMoveOneBlock(direction))
        {
            Vector3 pos = transform.position;

            if (direction == Direction.DOWN)
            {
                pos.y -= 1;
            }
            else if(direction == Direction.LEFT)
            {
                pos.x -= 1;
            }
            else
            {
                pos.x += 1;
            }

            transform.position = pos;
        }
    }

    public void Rotate()
    {
        transform.Rotate(0, 0, 90);
    }

    /// <summary>
    /// Move Element one block
    /// </summary>
    /// <returns></returns>
    public bool CanMoveOneBlock(Direction direction)
    {
        List<Transform> childs = new List<Transform>();

        for(int i = 0; i < transform.childCount; i++)
        {
            childs.Add(transform.GetChild(i));
        }

        foreach(Transform child in childs)
        {
            Vector3 newPosition = GetNewPosition(direction, child);

            if((int)Math.Round(newPosition.x) == (int)Math.Round(child.position.x) &&
                (int)Math.Round(newPosition.y) == (int)Math.Round(child.position.y) &&
                (int)Math.Round(newPosition.z) == (int)Math.Round(child.position.z))
            {
                return false;
            }
        }

        return true;
    }

    private Vector3 GetNewPosition(Direction direction, Transform gameBlock)
    {
        Vector3 expectedPosition = gameBlock.position;
        if (direction == Direction.DOWN)
        {
            expectedPosition += new Vector3(0, -1, 0);
        }
        else if (direction == Direction.LEFT)
        {
            expectedPosition += new Vector3(-1, 0, 0);
        }
        else
        {
            expectedPosition += new Vector3(1, 0, 0);
        }

        int expectedPositionY = (int)Math.Round(expectedPosition.y);
        int expectedPositionX = (int)Math.Round(expectedPosition.x);

        if (GameController.IsValidPosition(expectedPositionY, expectedPositionX))
        {
            return expectedPosition;
        }
        else
        {
            return gameBlock.position;
        }
    }

    private void FindGamecontrollerIfNull()
    {
        if (GameController == null)
        {
            Grid temp = GetComponentInParent<Grid>();

            if (temp.tag == "GameController")
            {
                GameController = temp;
            }
        }
    }
}
