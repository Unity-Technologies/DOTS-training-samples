using System.Collections;
using System.Collections.Generic;
using JumpTheGun;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

public class FollowPlayer : MonoBehaviour
{
    public Vector3 offsetFromPlayer = new Vector3(0, 13, -7);
    public float zoom = 1.0f;
    public float zoomMin = 0.8f, zoomMax = 1.3f;
    private const float ZOOM_SPEED = 0.2f;
    private PlayerBounceSystem _playerBounceSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerBounceSystem = World.Active.GetOrCreateSystem<PlayerBounceSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.DownArrow)) {
            zoom += ZOOM_SPEED * Time.unscaledDeltaTime;
        } else if (Input.GetKey(KeyCode.UpArrow)) {
            zoom -= ZOOM_SPEED * Time.unscaledDeltaTime;
        }
        zoom = math.clamp(zoom, zoomMin, zoomMax);
        Vector3 playerPosOnGround = _playerBounceSystem.PlayerPosition;
        playerPosOnGround.y = 0;
        transform.position = playerPosOnGround + zoom * offsetFromPlayer;
    }
}
