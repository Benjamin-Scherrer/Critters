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
    [field: SerializeField] public EventReference UI_Open {get; private set;}


    [field: Header ("Ambient")]
    [field: SerializeField] public EventReference IntroExplosion {get; private set;}
    [field: SerializeField] public EventReference Sandstorm {get; private set;}


    [field: Header ("Mechanics Door")]
    [field: SerializeField] public EventReference Door_Slide_Close {get; private set;}
    [field: SerializeField] public EventReference Door_Slide_Open {get; private set;}


    [field: Header ("Mechanics Energy Core")]
    [field: SerializeField] public EventReference Energy_Core_Sizzle {get; private set;}


    [field: Header ("Mechanics Healing Pod")]
    [field: SerializeField] public EventReference HealingPod_Activation {get; private set;}
    [field: SerializeField] public EventReference HealingPod_HealingBoop {get; private set;}


    [field: Header ("Mechanics Holobridge")]
    [field: SerializeField] public EventReference Holobridge_Activation {get; private set;}
    [field: SerializeField] public EventReference Holobridge_BuildBoop {get; private set;}


    [field: Header ("Mechanics Item Pickup")]
    [field: SerializeField] public EventReference Item_Corepickup {get; private set;}
    [field: SerializeField] public EventReference Item_Dashpickup {get; private set;}
    [field: SerializeField] public EventReference Item_Shieldpickup {get; private set;}


    [field: Header ("Mechanics Stones")]
    [field: SerializeField] public EventReference Stone_Break {get; private set;}
    
    
    [field: Header ("Enemy All")]
    [field: SerializeField] public EventReference Enemy_Death {get; private set;}
    [field: SerializeField] public EventReference Enemy_Alert {get; private set;}


    [field: Header ("Enemy Bullets")]
    [field: SerializeField] public EventReference Bullet_Impact_General {get; private set;}
    [field: SerializeField] public EventReference Bullet_Impact_Player {get; private set;}
    [field: SerializeField] public EventReference Bullet_Impact_Shield {get; private set;}
    [field: SerializeField] public EventReference Bullet_Shot {get; private set;}


    [field: Header ("Enemy Charger")]
    [field: SerializeField] public EventReference Charger_Alert {get; private set;}
    [field: SerializeField] public EventReference Charger_Start {get; private set;}
    [field: SerializeField] public EventReference Charger_Death {get; private set;}


    [field: Header ("Enemy Driller")]
    [field: SerializeField] public EventReference Driller_Drilling {get; private set;}
    [field: SerializeField] public EventReference Driller_NextSegment {get; private set;}
    [field: SerializeField] public EventReference Driller_Startup {get; private set;}
    [field: SerializeField] public EventReference Driller_Death {get; private set;}


    [field: Header ("Enemy Drone")]
    [field: SerializeField] public EventReference Drone_Move {get; private set;}
    [field: SerializeField] public EventReference Drone_Death {get; private set;}


    [field: Header ("Enemy Turret")]
    [field: SerializeField] public EventReference Turret_Dig_Down {get; private set;}
    [field: SerializeField] public EventReference Turret_Dig_Up {get; private set;}
    [field: SerializeField] public EventReference Turret_Hitswivel {get; private set;}
    [field: SerializeField] public EventReference Turret_Transform {get; private set;}
    [field: SerializeField] public EventReference Turret_Hit {get; private set;}
    [field: SerializeField] public EventReference Turret_Death {get; private set;}


    [field: Header ("Player Disk")]
    [field: SerializeField] public EventReference Disk_Autoreset {get; private set;}
    [field: SerializeField] public EventReference Disk_Crash {get; private set;}
    [field: SerializeField] public EventReference Disk_EnemyHit {get; private set;}
    [field: SerializeField] public EventReference Disk_Flight {get; private set;}
    [field: SerializeField] public EventReference Disk_Playercatch {get; private set;}


    [field: Header ("Player Player")]
    [field: SerializeField] public EventReference Player_Block {get; private set;}
    [field: SerializeField] public EventReference Player_Catch {get; private set;}
    [field: SerializeField] public EventReference Player_Dash {get; private set;}
    [field: SerializeField] public EventReference Player_Roll {get; private set;}
    [field: SerializeField] public EventReference Player_Death {get; private set;}
    [field: SerializeField] public EventReference Player_Hit {get; private set;}
    [field: SerializeField] public EventReference Player_Shot {get; private set;}
    [field: SerializeField] public EventReference Player_Walk {get; private set;}

    [field: Header ("Music")]
    [field: SerializeField] public EventReference Music1 { get; private set;}
    [field: SerializeField] public EventReference Music2 { get; private set;}
    [field: SerializeField] public EventReference Music3 { get; private set;}
    [field: SerializeField] public EventReference Music4 { get; private set;}
    [field: SerializeField] public EventReference Music5 { get; private set;}
    [field: SerializeField] public EventReference Music6 { get; private set;}
    [field: SerializeField] public EventReference MusicShutoff { get; private set;}
    
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
