using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIBase : MonoBehaviour
{

    [Header("Roaming")]
    public float MinWaitTime;
    public float MaxWaitTime;
    public int AllowedRetryHotspotRequests = 3;
    public float LookAtSpeed = 5f;


    private Animator anim;
    public HotspotManager.HotspotRoom DesiredRoom = HotspotManager.HotspotRoom.ANY;

    private bool _shouldRoam = true;
    private bool _shouldLookAtPOI = false;

    NavMeshAgent agent;
    Hotspot currentHotspot = null;
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (HotspotManager.RequestHotspotByRoom(HotspotManager.HotspotRoom.ANY, null, out currentHotspot))
        {
            agent.SetDestination(currentHotspot.Position);
        }

        StartCoroutine(LookAtPoi());
        StartCoroutine(Roam());
    }



    IEnumerator LookAtPoi()
    {
        while (true)
        {
            while (_shouldLookAtPOI)
            {
                Vector3 direction = (currentHotspot.PointOfInterest - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * LookAtSpeed);

                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator Roam()
    {
        while (true)
        {
            while (_shouldRoam)
            { // Roam around forever
                if (HasReachedDestination())
                {
                    anim.SetBool("IsWalking", false);
                    _shouldLookAtPOI = true;
                    yield return new WaitForSeconds(UnityEngine.Random.Range(MinWaitTime, MaxWaitTime));
                    int  RequestRetries = 0;
                    
                    while(RequestRetries < AllowedRetryHotspotRequests)
                    {
                        if (HotspotManager.RequestHotspotByRoom(DesiredRoom, currentHotspot, out currentHotspot))
                        {
                            _shouldLookAtPOI = true;
                            agent.SetDestination(currentHotspot.Position);
                            anim.SetBool("IsWalking", true);
                            break;
                        }
                    }


                }


                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.5f);
        }

    }

    private bool HasReachedDestination()
    {
        bool hasReachedDestination = false;

        // Based on https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
        if (_shouldRoam)
        {
            if (agent.pathPending == false)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (agent.hasPath == false || agent.velocity.sqrMagnitude == 0f)
                    {
                        hasReachedDestination = true;
                    }
                }
            }

        }

        return hasReachedDestination;
    }


}
