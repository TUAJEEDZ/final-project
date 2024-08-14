using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   public Inventory inventory;

   private void Awake()
   {
	   inventory = new Inventory(27);
   }
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.E)) // set plow keybind
		{
			Vector3Int position = new Vector3Int((int)transform.position.x, // plow position
				(int)transform.position.y -1 , 0);

			if (GameManager.instance.tileManager.IsInteractable(position)) // check for interactable tile
			{
				Debug.Log("Tile is interactable"); //output if selected tile is interactable
				GameManager.instance.tileManager.SetInteracted(position);
			}
		}
	}

	public void DropItem(Item item)
	{
		Vector2 spawnLocation = transform.position;

		Vector2 spawnOffset = Random.insideUnitCircle * 1.75f; // random position

		Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, 
			Quaternion.identity);

		droppedItem.rb2d.AddForce(spawnOffset * 2f, ForceMode2D.Impulse);
    }
}