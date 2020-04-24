using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class HotspotManager : MonoBehaviour
{
    public enum HotspotRoom
    {
        LIVING_ROOM, KITCHEN, LIBRARY, TOILET, ANY
    }


    [SerializeField]
    private List<Hotspot> hotspots;
    public HotspotRoom Room = HotspotRoom.ANY;

    public static List<HotspotManager> HotspotManagers = new List<HotspotManager>();

    public static bool RequestHotspotByRoom(HotspotRoom DesiredRoom, Hotspot currentHotspot, out Hotspot hotspot)
    {
        if(DesiredRoom == HotspotRoom.ANY)
        {
            return HotspotManagers.Where(man => man.hotspots.Any(hs => hs.IsFree)).OrderBy(_ => Random.Range(0, 100)).First().RequestHotspot(currentHotspot, out hotspot);
        } else
        {
            HotspotManager foundManager = HotspotManagers.Find(manager => manager.Room == DesiredRoom);
            if(foundManager != null)
            {
                return foundManager.RequestHotspot(currentHotspot, out hotspot);
            } else
            {
                hotspot = null;
                return false;
            }
        }


    }

    

    private bool RequestHotspot(Hotspot currentHotspot, out Hotspot returnHotspot)
    {
        Hotspot FoundHotspot = hotspots.Find(hotspot => hotspot.IsFree);

        if(FoundHotspot == null){
            returnHotspot = null;
            return false;
        } else
        {
            if(currentHotspot != null)
            currentHotspot.IsFree = true;
            returnHotspot = FoundHotspot;
            FoundHotspot.IsFree = false;
            return true;
        }
    }





    // Start is called before the first frame update
    void Awake()
    {
        HotspotManagers.Add(this);
        hotspots = transform.GetComponentsInChildren<Hotspot>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
