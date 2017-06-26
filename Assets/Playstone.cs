using System;
using UnityEngine;

public class Playstone : MonoBehaviour
{
    private Quaternion InitialRotation;

    private int LocalX { get; set; }
    private int LocalY { get; set; }

    public int DecreasedTimes { get; set; }

    public Vector2 Position
    {
        get
        {

            Group g = this.GetComponentInParent<Group>();

            float posX = LocalX;
            float posY = LocalY;

            for (int i = 0; i < g.Rotation / 90; i++)
            {
                float temp = posX;
                posX = -posY;
                posY = temp;
            }

            return new Vector2(g.Position.x + posX, g.Position.y + posY);
        }
    }
    
    void Start()
    {
        Group g = this.GetComponentInParent<Group>();

        InitialRotation = transform.rotation;

        LocalX = (int)Math.Round(transform.position.x - g.RotationRoot.transform.position.x);
        LocalY = (int)Math.Round(transform.position.y - g.RotationRoot.transform.position.y);
    }
    
    void Update()
    {
        Group g = this.GetComponentInParent<Group>();

        transform.localPosition = new Vector3(0, Position.y - DecreasedTimes, 0);

        transform.rotation = InitialRotation;
        transform.RotateAround(new Vector3(10, 0, 0), new Vector3(0, 1, 0), Position.x / g.GameController.w * 270);
    }
}
