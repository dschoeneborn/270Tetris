using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public Group[] groups;
    public Vector3 startPosition;
    public Grid controller;

	// Use this for initialization
	void Start () {
        spawnNext();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void spawnNext()
    {
        int i = Random.Range(0, groups.Length);

        Group created = Instantiate(groups[i], startPosition, Quaternion.identity);

        created.GameController = controller;
    }
}
