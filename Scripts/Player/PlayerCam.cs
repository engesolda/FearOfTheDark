using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public RectTransform menuRight;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        Player player = Player.GetInstance();
        //CEntralize the camera on the player
        Vector3 playerPos = player.transform.position;
        playerPos.z = -1;
        transform.position = playerPos;

        offset = transform.position - player.transform.position;

        RenderSettings.ambientLight = Color.black;
    }

    // Update is called once per frame
    void Update()
    {
        //Follow the player
        transform.position = Player.GetInstance().transform.position + offset;
    }
}
