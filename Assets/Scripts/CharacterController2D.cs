using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MoveMode
{
    WALKING,
    RIDING,
    CLIMBING
}

public class CharacterController2D : MonoBehaviour
{
    [Header("Physics Settings")]
    public LayerMask collisionMask;
    public float collisionDist = 0.01f;
    public float moveSpeed = 2f;
    public float jumpSpeed = 6f;
    public float gravity = -1f;
    public float terminalVelocity = -10f;
    public GameObject mount;
    public MoveMode moveMode = MoveMode.WALKING;

    public static CharacterController2D instance;

    private new CapsuleCollider2D collider;
    Vector2 speed = new Vector2();
    bool hasJumped = false;
    float radius;
    float height;
    

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        collider = GetComponent<CapsuleCollider2D>();
        radius = collider.size.x / 2;
        height = collider.size.y / 2;

        EventBus.GenerationEvents.OnGenerationEnd += StartPhysics;
    }

    bool physics = false;

    void StartPhysics(object caller, params object[] parameters)
    {
        physics = true;
    }

    void StopPhysics(object caller, params object[] parameters)
    {
        physics = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(physics)
        {
            switch (moveMode)
            {
                case MoveMode.WALKING:
                    {
                        bool down = CheckCollision(Vector2.down, collisionDist);
                        bool up = CheckCollision(Vector2.up, collisionDist);
                        bool left = CheckCollision(Vector2.left, collisionDist);
                        bool right = CheckCollision(Vector2.right, collisionDist);
                        bool shouldInvokeLand = false;

                        speed.x = InputManager.instance.move.ReadValue<Vector2>().x * moveSpeed * Time.fixedDeltaTime;

                        if (Mathf.Sign(speed.x) == -1 && left)
                        {
                            speed.x = 0;
                        }
                        else if (Mathf.Sign(speed.x) == 1 && right)
                        {
                            speed.x = 0;
                        }

                        if (down)
                        {
                            if (InputManager.instance.jump.ReadValue<float>() > 0 && !hasJumped && !up)
                            {
                                speed.y = jumpSpeed * Time.fixedDeltaTime;
                                hasJumped = true;
                                EventBus.PlayerEvents.OnPlayerJump?.Invoke(this);
                            }
                            else
                            {
                                speed.y = 0;
                                hasJumped = false;
                                if (shouldInvokeLand)
                                {
                                    shouldInvokeLand = false;
                                    EventBus.PlayerEvents.OnPlayerLand?.Invoke(this);
                                }
                            }
                        }
                        else
                        {
                            shouldInvokeLand = true;
                            speed.y = Mathf.Clamp(speed.y + gravity * Time.fixedDeltaTime, terminalVelocity, Mathf.Infinity);

                            if (hasJumped && up)
                            {
                                speed.y = gravity * Time.fixedDeltaTime;
                            }
                        }

                        transform.position += (Vector3)speed;
                    }
                    break;

                case MoveMode.RIDING:
                    {
                        transform.position = mount.transform.position + Vector3.up * 0.2f;

                        if (InputManager.instance.jump.ReadValue<float>() > 0)
                        {
                            moveMode = MoveMode.WALKING;
                            EventBus.PlayerEvents.OnPlayerStopRiding?.Invoke(this, mount);
                            mount = null;
                        }
                    }
                    break;
            }
        }
    }

    public void Mount(GameObject mount)
    {
        moveMode = MoveMode.RIDING;
        this.mount = mount;
        EventBus.PlayerEvents.OnPlayerStartRiding?.Invoke(this, mount);
    }

    bool CheckCollision(Vector2 dir, float dist)
    {
        return Physics2D.CapsuleCast(transform.position, collider.size - Vector2.one * collisionDist, CapsuleDirection2D.Vertical, 0, dir, dist, collisionMask);
    }
}
