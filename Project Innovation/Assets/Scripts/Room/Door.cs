using FMODUnity;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    private Collider coll;
    private Room2 room;

    [EventRef, SerializeField] private string doorActivationSound;

    private void Start()
    {
        coll = GetComponent<Collider>();
        coll.isTrigger = false;
    }

    public void InsertRoom(Room2 r)
    {
        room = r;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playerMask.Contains(other.gameObject.layer)) return;
        // This is a player -> generate a new room
        if (room)
        {
            room.Generate();
            Highscore.AddToHighscore(50);
            var sound = doorActivationSound.CreateSound();
            sound.start();
            sound.release();
        }
    }

    public void OnAllEnemiesClear()
    {
        coll.isTrigger = true;
        // doorSoundInstance.start();
        // doorSoundInstance.setParameterByName("HumVolume", 1f);
    }
}