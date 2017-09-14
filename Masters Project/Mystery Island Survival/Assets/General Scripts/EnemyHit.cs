using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHit : MonoBehaviour {

    public Slider playerHealthSlider;
    public Transform player;

    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip[] playerAudio;

    void OnTriggerEnter(Collider other)
    {
        playerHealthSlider.value -= 20; 
        Debug.Log("Hit Player");
        PlayClipAt(playerAudio[0], player.position);                  // Play sound
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
    {
        GameObject tempGO = new GameObject("TempAudio");          // create the temp object
        tempGO.transform.position = pos;                          // set its position
        AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.clip = clip;                                      // define the clip
        aSource.spatialBlend = 1.0f;                               // set to 3D sound
        aSource.minDistance = 0.2f;
        aSource.maxDistance = 0.3f;
        aSource.Play();                                           // start the sound
        Destroy(tempGO, clip.length);                             // destroy object after clip duration
        return aSource;                                           // return the AudioSource reference
    }
}
