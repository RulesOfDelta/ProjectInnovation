using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    private Room2 room;
    
    public void InsertRoom(Room2 r)
    {
        room = r;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playerMask.Contains(other.gameObject.layer)) return;
        // This is a player -> generate a new room
        if(room)
            room.Generate();
    }
}