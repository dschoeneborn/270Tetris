using Assets;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Repräsentiert ein Tetris Block. Ist eine Gruppe von gekrümmten Blöcken.
/// </summary>
public class Group : MonoBehaviour
{
    public Grid GameController;
    public Playstone RotationRoot;

    private bool registered;

    AudioSource audioSource;

    public AudioClip rotateSFX { get; set; }
    public AudioClip downSFX { get; set; }
    public AudioClip failSFX { get; set; }
    public AudioClip failRotateSFX { get; set; }

    public int Rotation { get; private set; }

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
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update ()
    {
#pragma warning disable 612
        transform.position = new Vector3(0, 0, 0);
#pragma warning restore 612
        if (!registered)
        {
            if(GameController.RegisterMovingObject(this))
            {
                registered = true;
            }
        }
    }
   
    /// <summary>
    /// Bewegt ein Block, standardmäßig mit Sound, innerhalb des Grids in eine Richtung.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="playsound"></param>
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

    /// <summary>
    /// Rotiert ein Block um 90 Grad
    /// </summary>
    public void Rotate()
    {
        if(CanRotateOneTime())
        {
            Rotation += 90;

            audioSource.PlayOneShot(rotateSFX);
        }
        else
        {
            audioSource.PlayOneShot(failSFX);
        }
    }

    
    /// <summary>
    /// Prüft ob ein Block sich in eine Richtung bewegen kann
    /// </summary>
    /// <param name="direction"></param>
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

    /// <summary>
    /// Prüft ob sich der Block um 90 Grad drehen kann.
    /// Ist dies nicht der Fall wird ein Fail Sound ausgegeben.
    /// </summary>
    /// <returns></returns>
    private bool CanRotateOneTime()
    {
        Rotation += 90;

        List<Playstone> childs = GetChildPlaystones();

        foreach (Playstone child in childs)
        {
            if (!GameController.IsInsideBorder(new Vector2(child.Position.x, child.Position.y)))
            {
                Rotation -= 90;
                audioSource.PlayOneShot(failRotateSFX);
                return false;
            }
        }

        Rotation -= 90;
        return true;
    }

    /// <summary>
    /// Gibt die Einzel-Blöcke, aus denen eine Gruppe besteht, als Liste zurück.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Gibt die neue Position eines Blockes im Grid zurück.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="gameBlock"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Setzt den GameController, falls dieser noch nicht gesetzt wurde
    /// </summary>
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
