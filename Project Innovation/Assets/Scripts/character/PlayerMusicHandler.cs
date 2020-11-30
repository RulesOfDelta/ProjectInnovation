using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMusicHandler : MonoBehaviour
{
    [SerializeField] private MusicHandler musicHandler;
    private IReadOnlyList<Door> doors;

    public void InsertDoors(IReadOnlyList<Door> doorList)
    {
        doors = doorList;
    }

    public void OnClear()
    {
        musicHandler.State = MusicHandler.MusicState.Rest;
    }

    public void OnGenerate()
    {
        musicHandler.State = MusicHandler.MusicState.Fight;
    }

    private void Update()
    {
        if (musicHandler.State == MusicHandler.MusicState.Rest)
        {
            Vector3 minPos = Vector3.positiveInfinity;
            float recordDist = float.PositiveInfinity;
            bool hasClosest = false;
            foreach (var door in doors)
            {
                var dist = Vector3.Distance(door.transform.position, transform.position); 
                if (dist < recordDist)
                {
                    recordDist = dist;
                    minPos = door.transform.position;
                    hasClosest = true;
                }
            }

            if (hasClosest)
            {
                var dir = (minPos - transform.position).normalized;
                var val = Vector3.Dot(transform.forward, dir);
                val.RemapThis(-1f, 1f, 0f, 1f);
                musicHandler.SetRestVal(val);
            }
            else musicHandler.SetRestVal(0f);
        }
    }
}
