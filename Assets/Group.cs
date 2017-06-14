using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    public Grid GameController;

    private bool registered;

    AudioSource audioSource;

    public AudioClip rotateSFX { get; set; }
    public AudioClip downSFX { get; set; }
    public AudioClip failSFX { get; set; }

    private int X { get; set; }
    private int Y { get; set; }

    [Obsolete]
    public new Transform transform
    {
        get
        {
            return base.transform;
        }
    }


    public Vector2 Position
    {
        get
        {
            return new Vector2(X, Y);
        }
        set
        {
            X = (int)Math.Round(value.x);
            Y = (int)Math.Round(value.y);
        }
    }

    // Use this for initialization
    void Start()
    {
        FindGamecontrollerIfNull();

        if (GameController == null)
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update ()
    {
#pragma warning disable 612
        transform.position = new Vector3(Position.x, Position.y, 10);
#pragma warning restore 612
        if (!registered)
        {
            if(GameController.RegisterMovingObject(this))
            {
                registered = true;
            }
        }
    }
   
    public void MoveOneBlock(Direction direction, bool playsound=true)
    {
        if (CanMoveOneBlock(direction))
        {
            Vector2 pos = Position;

            if (direction == Direction.DOWN)
            {
                pos.y -= 1;

                if(playsound)
                {
                    audioSource.PlayOneShot(downSFX);
                }
            }
            else if(direction == Direction.LEFT)
            {
                pos.x -= 1;
            }
            else
            {
                pos.x += 1;
            }

            Position = pos;
        }
    }

    public void Rotate()
    {
        List<Playstone> childs = GetChildPlaystones();

        foreach(Playstone child in childs)
        {
            child.transform.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
        }
        
        audioSource.PlayOneShot(rotateSFX);
        
    }

    /// <summary>
    /// Move Element one block
    /// </summary>
    /// <returns></returns>
    public bool CanMoveOneBlock(Direction direction)
    {
        List<Playstone> childs = GetChildPlaystones() ;

        foreach (Playstone child in childs)
        {
            Vector3 newPosition = GetNewPosition(direction, child);

            if((int)Math.Round(newPosition.x) == child.Position.x &&
                (int)Math.Round(newPosition.y) == child.Position.y)
            {
                audioSource.PlayOneShot(failSFX);
                return false;
            }
        }

        return true;
    }

    private List<Playstone> GetChildPlaystones()
    {
        List<Playstone> childs = new List<Playstone>();

#pragma warning disable CS0612 // Type or member is obsolete
        for (int i = 0; i < transform.childCount; i++)
        {
            childs.Add(transform.GetChild(i).GetComponent<Playstone>());
        }
#pragma warning restore CS0612 // Type or member is obsolete

        return childs;
    }

    private Vector2 GetNewPosition(Direction direction, Playstone gameBlock)
    {
        Vector2 expectedPosition = new Vector2(gameBlock.Position.x, gameBlock.Position.y);
        if (direction == Direction.DOWN)
        {
            expectedPosition += new Vector2(0, -1);
        }
        else if (direction == Direction.LEFT)
        {
            expectedPosition += new Vector2(-1, 0);
        }
        else
        {
            expectedPosition += new Vector2(1, 0);
        }

        int expectedPositionY = (int)Math.Round(expectedPosition.y);
        int expectedPositionX = (int)Math.Round(expectedPosition.x);

        if (GameController.IsValidPosition(expectedPositionY, expectedPositionX))
        {
            return expectedPosition;
        }
        else
        {
            return new Vector2(gameBlock.Position.x, gameBlock.Position.y);
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
