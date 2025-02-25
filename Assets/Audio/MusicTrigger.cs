using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    private AudioManager audioManager;

    // Set this to true if you want to trigger Song 1, false for other songs
    public bool isTriggerSong1 = false;
    public bool isTriggerSong2 = false;
    public bool isTriggerSong3 = false;
    public bool isTriggerSong4 = false;
    public bool isTriggerSong5 = false;
    public bool isTriggerSong6 = false;

    public bool isTriggerNoSongs = false;



    private void Start()
    {
        // Find the AudioManager instance in the scene
        audioManager = AudioManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainPlayer"))
        {
            if (isTriggerSong1 && !audioManager.isSong1Playing)
            {
                // Trigger Song 1 if it's not already playing
                SetSongStatus(true, false, false, false, false, false);
                audioManager.StopSongs();
                audioManager.PlaySongs();
                Destroy(gameObject);
            }
            else if (isTriggerSong2 && !audioManager.isSong2Playing)
            {
                // Trigger Song 2 if it's not already playing
                SetSongStatus(false, true, false, false, false, false);
                audioManager.StopSongs();
                audioManager.PlaySongs();
                Destroy(gameObject);
            }
            else if (isTriggerSong3 && !audioManager.isSong3Playing)
            {
                // Trigger Song 3 if it's not already playing
                SetSongStatus(false, false, true, false, false, false);
                audioManager.StopSongs();
                audioManager.PlaySongs();
                Destroy(gameObject);
            }
            else if (isTriggerSong4 && !audioManager.isSong4Playing)
            {
                // Trigger Song 4 if it's not already playing
                SetSongStatus(false, false, false, true, false, false);
                audioManager.StopSongs();
                audioManager.PlaySongs();
                Destroy(gameObject);
            }
            else if (isTriggerSong5 && !audioManager.isSong5Playing)
            {
                // Trigger Song 5 if it's not already playing
                SetSongStatus(false, false, false, false, true, false);
                audioManager.StopSongs();
                audioManager.PlaySongs();
                Destroy(gameObject);
            }
            else if (isTriggerSong6 && !audioManager.isSong6Playing)
            {
                SetSongStatus(false, false, false, false, false, true);
                audioManager.StopSongs();
                audioManager.PlaySongs();
                Destroy(gameObject);
            }
            else if (isTriggerNoSongs)
            {
                AudioManager.Instance.PlayOneShot(FMODEvents.instance.Music_Shutoff, this.transform.position);
                SetSongStatus(false, false, false, false, false, false);
                audioManager.StopSongs();
                Destroy(gameObject);
            }
        }
    }

    private void SetSongStatus(bool song1, bool song2, bool song3, bool song4, bool song5, bool song6)
    {
        audioManager.isSong1Playing = song1;
        audioManager.isSong2Playing = song2;
        audioManager.isSong3Playing = song3;
        audioManager.isSong4Playing = song4;
        audioManager.isSong5Playing = song5;
        audioManager.isSong6Playing = song6;
    }
}
