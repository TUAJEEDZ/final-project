using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float dropRange = 1f;
    public InventoryManager inventoryManager;
    private TileManager tileManager;
    public Transform dropPoint;

    private Animator animator;
    private Movement movement;

    private void Start()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
        tileManager = GameManager.instance.tileManager;
    }
    private void Awake()
    {
        inventoryManager = GetComponent<InventoryManager>();
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        // Get the direction the player is facing
        float horizontal = animator.GetFloat("Horizontal");
        float vertical = animator.GetFloat("Vertical");

        Vector2 direction = new Vector2(horizontal, vertical).normalized;

        if (Input.GetButtonDown("Fire1")) // set plow keybind
        {
            if (tileManager != null && direction != Vector2.zero)
            {
                // Calculate the position in front of the player based on the direction
                Vector3Int position = new Vector3Int(
                    Mathf.RoundToInt(transform.position.x - 1 + direction.x),
                    Mathf.RoundToInt(transform.position.y -1 + direction.y),
                    0
                );

                string tileName = tileManager.GetTileName(position);

                if (!string.IsNullOrWhiteSpace(tileName))
                {
                    if (tileName == "Interactable" && inventoryManager.toolbar.selectedSlot.itemName == "Hoe")
                    {
                        animator.SetTrigger("isPlowing");
                        StartCoroutine(DelayedInteraction(position));
                    }
                }
            }
        }
    }

    private IEnumerator DelayedInteraction(Vector3Int position)
    {
        yield return new WaitForSeconds(0.5f); // Delay for 1 second
        tileManager.SetInteracted(position);
    }


    public void DropItem(Item item)
    {
        float horizontal = animator.GetFloat("Horizontal");
        float vertical = animator.GetFloat("Vertical");

        Vector2 direction = new Vector2(horizontal, vertical).normalized;

            if (direction != Vector2.zero)
            {
                dropPoint.localPosition = direction * dropRange; 
                dropPoint.right = direction;  
            }
        Vector2 spawnLocation = (Vector2)transform.position + direction; // Spawn item in front of player

        Vector2 spawnOffset = Random.insideUnitCircle * 0.5f; // Slight random offset

        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);

        droppedItem.rb2d.AddForce(direction * 2f + spawnOffset, ForceMode2D.Impulse);
    }

    public void DropItem(Item item, int numToDrop)
    {
        for (int i = 0; i < numToDrop; i++)
        {
            DropItem(item);
        }
    }
}
