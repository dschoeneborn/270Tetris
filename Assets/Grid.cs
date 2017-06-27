using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Bildet das Spielfeld ab</summary>
/// <remarks>
/// Beinhaltet alle Spielsteine im Spiel und übernimmt die Tätigkeit als Controller
/// </remarks>
public class Grid : MonoBehaviour
{
    private const int TIME_BETWEEN_MOVING_UPDATE = 1; //In seconds

    /// <summary>
    /// Breite des Spieldfelde
    /// </summary>
    public int w = 10;
    /// <summary>
    /// Höhe des Spielfeldes
    /// </summary>
    public int h = 20;

    public Text PointsCounter;
    public RawImage GameOverScreen;
    public RawImage DebugItem;
    public bool ShowDebugInformation;

    /// <summary>
    /// Array zum Speichern der im Spiel befindlichen Spielsteine
    /// </summary>
    private Playstone[][] grid;
    private Group movingGroup;

    private float lastUpdatedDown;

    private bool lastFramespawnedItem = false;
    private int points = 0;

    /// <summary>
    /// Hilfsvariable für Gamepadeingabe
    /// </summary>
    private float lastX;
    /// <summary>
    /// Hilfsvariable für Gamepadeingabe
    /// </summary>
    private float lastY;
    

    // Use this for initialization
    void Start ()
    {
        GameOverScreen.enabled = false;
        // Initialisierung des Spielfeldes
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


        //Prüfung, ob bereits ein neues Kommando entgegengenommen werden kann, oder noch weiter gewartet werden muss. Dadurch wird ein doppeltes Interpretieren von Kommandos vermieden
        if (lastUpdatedDown + TIME_BETWEEN_MOVING_UPDATE < Time.time)
        {
            if (ShowDebugInformation)
            {
                RemoveOldDebugInformation();
                DrawNewDebugInformation();
            }

            //Prüft, ob der Spielstein am Boden des Spielfelds angekommen ist
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

    /// <summary>
    /// Debuginformation als Hilfestellung bei der Analyse des Spiel.
    /// Blendet das Spielfeld als 2D-Version in der unteren linken Ecke ein
    /// </summary>
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

    /// <summary>
    /// Löscht die aktuelle Debug information
    /// </summary>
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


    /// <summary>
    /// Verarbeitet die Tastatur eingaben
    /// </summary>
    private void MoveIfButtonPressed()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || (IsAxisAvailable("Oculus_GearVR_DpadX") && Input.GetAxis("Oculus_GearVR_DpadX") == -1 && lastX != -1))
        {
            lastX = -1;
            movingGroup.MoveOneBlock(Direction.LEFT);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || (IsAxisAvailable("Oculus_GearVR_DpadX") && Input.GetAxis("Oculus_GearVR_DpadX") == 1 && lastX != 1))
        {
            lastX = 1;
            movingGroup.MoveOneBlock(Direction.RIGHT);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || (IsAxisAvailable("Oculus_GearVR_DpadY") && Input.GetAxis("Oculus_GearVR_DpadY") == -1 && lastY != -1))
        {
            lastY = -1;
            movingGroup.MoveOneBlock(Direction.DOWN);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || (Input.GetKeyDown("joystick button 0")))
        {
            movingGroup.Rotate();
        }

        if (IsAxisAvailable("Oculus_GearVR_DpadX") && Input.GetAxis("Oculus_GearVR_DpadX") == 0)
        {
            lastX = 0;
        }

        if (IsAxisAvailable("Oculus_GearVR_DpadY") && Input.GetAxis("Oculus_GearVR_DpadY") == 0)
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

    /// <summary>
    /// Prüft, ob sich ein Objekt am Boden des Spielfelds befindet
    /// </summary>
    /// <returns>true, wenn Objekt am Boden angekommen</returns>
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

    /// <summary>
    /// Prüft, ob sich ein Objekt innerhalb des Spielfeldes befindet
    /// </summary>
    /// <param name="pos"> Position des Objekts</param>
    /// <returns>true, wenn das Objekt innerhalb des Spielfeldes ist</returns>
    public bool IsInsideBorder(Vector2 pos)
    {
        return ((int)pos.x >= 0 &&
                (int)pos.x < w &&
                (int)pos.y >= 0 &&
                (int)pos.y < h);
    }

    /// <summary>
    /// Prüft, ob die angegebene Position im Spielfeld liegt und nicht von einem anderen Stein belegt ist.
    /// </summary>
    /// <param name="y"> y-Koordinate</param>
    /// <param name="x">x.Koordinate</param>
    /// <returns></returns>
    public bool IsValidPosition(int y, int x)
    {
        return (IsInsideBorder(new Vector2(x, y)) &&
            grid[y][x] == null);
    }


    /// <summary>
    /// Löscht Spielsteine auf der angegebenen Zeile aus dem Spiel
    /// </summary>
    /// <param name="y"> Koordinate der Reihe</param>
    private void DeleteRow(int y)
    {
        for(int x = 0; x<w; x++)
        {
            Destroy(grid[y][x].gameObject);
            grid[y][x] = null;
        }
    }


    /// <summary>
    /// Prüft ob eine Reihe voll ist
    /// </summary>
    /// <param name="y"> Koordinate der zu prüfenden Reihe</param>
    /// <returns>true, wenn Reihe voll ist</returns>
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


    /// <summary>
    /// Löscht alle vollen Reihen aus dem Spiel
    /// </summary>
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


    /// <summary>
    /// Setzt alle Reihen oberhalt von above um eine Reihe nach unten
    /// </summary>
    /// <param name="above">y-Koordinate ab der Reihen nach unten verschoben werden</param>
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
