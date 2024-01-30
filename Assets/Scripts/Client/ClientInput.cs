using Shared;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Client
{
    public class ClientInput : MonoBehaviour
    {
        // Update is called once per frame
        private void Update()
        {
            // This makes it so that we don't attempt navigation if we wanted to click on a UI element
            if (EventSystem.current.IsPointerOverGameObject()){ return; }

            var leftClick = Input.GetMouseButton(0);
            var rightClick = Input.GetMouseButton(1);
            // Check to see if the user made any kind of mouse click
            if (leftClick || rightClick)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out var hitInfo))
                {
                    Debug.Log($"ScreenToPointRay raycast hit {hitInfo.collider.name}");
                    // Movement will be handled via right clicks
                    if(rightClick)
                    {
                        // Check for the nearest position on the nav mesh within 1f meter of where the user clicked
                        var navPoint = NavMesh.SamplePosition(hitInfo.point, out var navHit, 1f, NavMesh.AllAreas);
                        if (navPoint)
                        {
                            Player.LocalPlayer.ClickedNavMesh(navHit.position);
                        }
                        else
                        {
                            Debug.Log("Click point is not on navmesh.");
                        }
                    }
                }
            }
        }
    }
}
