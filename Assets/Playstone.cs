using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playstone : MonoBehaviour
{

    public Vector2 Position
    {
        get
        {

            Group g = this.GetComponentInParent<Group>();


            return new Vector2(g.Position.x + transform.localPosition.x, g.Position.y + transform.localPosition.y);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
