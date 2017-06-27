using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ermöglicht das Spawnen von Tetris-Blöcken
/// </summary>
public class Spawner : MonoBehaviour {
    public Group[] groups;
    public Grid controller;

    public AudioClip rotateSFX;
    public AudioClip downSFX;
    public AudioClip failSFX;
    public AudioClip failRotateSFX;

    // Use this for initialization
    void Start () {
        spawnNext();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Lässt einen zufälligen Block auf dem Grid erscheinen
    /// </summary>
    public void spawnNext()
    {
        int i = Random.Range(0, groups.Length);

        Group created = Instantiate(groups[i], transform.position, Quaternion.identity);

        created.Position = transform.position;

        created.GameController = controller;
        created.failSFX = failSFX;
        created.downSFX = downSFX;
        created.rotateSFX = rotateSFX;
        created.failRotateSFX = failRotateSFX;
    }
}
