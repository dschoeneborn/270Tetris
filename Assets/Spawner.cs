﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public Group[] groups;
    public Grid controller;

    public AudioClip rotateSFX;
    public AudioClip downSFX;
    public AudioClip failSFX;

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

        Group created = Instantiate(groups[i], transform.position, Quaternion.identity);

        created.GameController = controller;
        created.failSFX = failSFX;
        created.downSFX = downSFX;
        created.rotateSFX = rotateSFX;
    }
}
