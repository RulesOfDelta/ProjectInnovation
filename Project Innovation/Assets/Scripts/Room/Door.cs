using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    private Collider coll;
    private Room2 room;

    [EventRef, SerializeField] private string doorActivationSound;
    [EventRef, SerializeField] private string doorLocalizationSound;
    private EventInstance doorLocInstance;

    private void Start()
    {
        coll = GetComponent<Collider>();
        coll.isTrigger = false;
        doorLocInstance = doorLocalizationSound.CreateSound();
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
        doorLocInstance.start();
    }

    private void OnDestroy()
    {
        doorLocInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        doorLocInstance.release();
    }
}