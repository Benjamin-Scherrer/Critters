using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{

    [field: Header ("TEST")]
    [field: SerializeField] public EventReference TestSoundtrack {get; private set;}
    [field: SerializeField] public EventReference TestConfirm {get; private set;}

    [field: Header ("UI")]
    [field: SerializeField] public EventReference UI_Confirm {get; private set;}
    [field: SerializeField] public EventReference UI_Close {get; private set;}

    [field: Header ("Ambient")]
    [field: SerializeField] public EventReference Ambience {get; private set;}

    [field: Header ("Item Pickup")]
    [field: SerializeField] public EventReference Item_Pickup {get; private set;}
    [field: SerializeField] public EventReference Collisions {get; private set;}
    [field: SerializeField] public EventReference Grabbing {get; private set;}
    [field: SerializeField] public EventReference Breaking {get; private set;}
    [field: SerializeField] public EventReference Touch {get; private set;}
    [field: SerializeField] public EventReference Jellyfish_Movement {get; private set;}

    [field: Header ("Music")]
    [field: SerializeField] public EventReference Music1 { get; private set;}
    [field: SerializeField] public EventReference Music2 { get; private set;}
    [field: SerializeField] public EventReference Music3 { get; private set;}
    [field: SerializeField] public EventReference Music4 { get; private set;}
    [field: SerializeField] public EventReference Music5 { get; private set;}
    [field: SerializeField] public EventReference Music6 { get; private set;}
    [field: SerializeField] public EventReference Music_Shutoff { get; private set;}
    
    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.LogError("Found more than one Fmod Events instance in the scene.");
        }
        instance = this;
    }

}
