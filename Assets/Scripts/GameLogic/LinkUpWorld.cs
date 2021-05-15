using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

public class LinkUpWorld
{
    public Camera camera;
    public bool isPlaying;
    public static Transform playerPointer;
    public static bool isPlayerTouch;

    
    public void Start()
    {
        if (isPlaying)
            return;

        isPlaying = true;
        camera = Camera.main;
    }

    public void Update()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = camera.ScreenPointToRay(touch.position);
                RaycastHit hitInfo;
                if(Physics.Raycast(ray,out hitInfo))
                {
                    // Debug.DrawLine(ray.origin, hitInfo.point);
                    GameObject gameObj = hitInfo.collider.gameObject;
                    var entity = gameObj.GetComponent<ElementEntity>();
                    if (entity)
                    {
                        entity.OnTouchStart();
                    }
                }
            }
        }
    }
}
