using System;
using UnityEngine;

public class Playstone : MonoBehaviour
{
    private Quaternion InitialRotation;
    private bool FirstUpdate = true;

    public int LocalX { get; set; }
    public int LocalY { get; set; }

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

    // Use this for initialization
    void Start()
    {
        Group g = this.GetComponentInParent<Group>();

        InitialRotation = transform.rotation;

        LocalX = (int)Math.Round(transform.position.x - g.RotationRoot.transform.position.x);
        LocalY = (int)Math.Round(transform.position.y - g.RotationRoot.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        Group g = this.GetComponentInParent<Group>();

        transform.localPosition = new Vector3(0, Position.y, 0);

        transform.rotation = InitialRotation;
        transform.RotateAround(new Vector3(10, 0, 0), new Vector3(0, 1, 0), Position.x / g.GameController.w * 270);

        if(FirstUpdate)
        {
            //SetBending();
            FirstUpdate = false;
        }
    }

    private void SetBending()
    {
        Quaternion actualRotation = Quaternion.LookRotation(new Vector3(10, 0, 0) - transform.position);


        float yRotation = actualRotation.eulerAngles.y / 2;
        yRotation = 10;

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            for (int j = 0; j < child.transform.childCount; j++)
            {
                GameObject grandchild = child.transform.GetChild(i).gameObject;

                if (grandchild.gameObject.tag.Equals("Left"))
                {
                    grandchild.transform.Rotate(new Vector3(0, 0, -yRotation));
                }
                else if (grandchild.gameObject.tag.Equals("Right"))
                {
                    grandchild.transform.Rotate(new Vector3(-yRotation, 0, 0));
                }
            }
        }
    }
}
