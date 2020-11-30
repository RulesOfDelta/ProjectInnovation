using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    private Collider coll;
    private Room2 room;

    // [FMODUnity.EventRef, SerializeField] private string doorSound;
    // private FMOD.Studio.EventInstance doorSoundInstance;

    private void Start()
    {
        coll = GetComponent<Collider>();
        coll.isTrigger = false;

        // doorSoundInstance = FMODUnity.RuntimeManager.CreateInstance(doorSound);
        // doorSoundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        // doorSoundInstance.setParameterByName("HumVolume", 0f);
    }

    private void OnDestroy()
    {
        // doorSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        // doorSoundInstance.release();
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
        }
    }

    public void OnAllEnemiesClear()
    {
        coll.isTrigger = true;
        // doorSoundInstance.start();
        // doorSoundInstance.setParameterByName("HumVolume", 1f);
    }
}