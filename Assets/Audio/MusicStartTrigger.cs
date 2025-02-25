using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStartTrigger : MonoBehaviour
{

    [SerializeField] private bool isStartScreenMusic = false;

    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {

        audioManager = AudioManager.Instance;

        if (isStartScreenMusic)
        {
            audioManager.isSong1Playing = false;
            audioManager.isSong2Playing = false;
            audioManager.isSong3Playing = false;
            audioManager.isSong4Playing = false;
            audioManager.isSong5Playing = false;
            audioManager.isSong6Playing = false;
            audioManager.StopSongs();
            audioManager.isSong1Playing = true;
            audioManager.isSong2Playing = false;
            audioManager.isSong3Playing = false;
            audioManager.isSong4Playing = false;
            audioManager.isSong5Playing = false;
            audioManager.isSong6Playing = false;
            audioManager.PlaySongs();
        }
        else
        {
            //No Sound
        }
    }
}
