using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlaceMinecart : MonoBehaviour
{
    public GameObject prefab;

    private void Start()
    {
        InputManager.instance.click.started += _ => OnClick();
    }

    void OnClick()
    {
        //raycast to check for existing minecart
        RaycastHit2D hit = Physics2D.Raycast(InputManager.instance.GetClickPos(), Vector2.up, 0.01f);

        if(hit && hit.collider.CompareTag("Minecart"))
        {
            if(CharacterController2D.instance.moveMode == MoveMode.WALKING)
            {
                GameObject cart = hit.collider.gameObject;
                CharacterController2D.instance.Mount(cart);
            }
        }
        else
        {
            //if none, check for rails
            if (TilemapManager.GetTile(TileLayer.RAILS, InputManager.instance.GetClickPos()) != null)
            {
                Vector2 pos = InputManager.instance.GetClickPos();

                pos.x = Mathf.FloorToInt(pos.x) + 0.5f;
                pos.y = Mathf.FloorToInt(pos.y) + 0.5f;

                Minecart minecart = Instantiate(prefab, pos, Quaternion.identity).GetComponent<Minecart>();

                float vec1 = minecart.transform.position.x - transform.position.x;

                if(vec1 >= 0)
                {
                    minecart.direction = Direction.RIGHT;
                }
                else
                {
                    minecart.direction = Direction.LEFT;
                }

                EventBus.MinecartEvents.OnMinecartPlaced?.Invoke(this, minecart);
            }
        }
    }
}
