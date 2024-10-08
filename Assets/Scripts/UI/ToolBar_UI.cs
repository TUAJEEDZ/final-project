using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBar_UI : MonoBehaviour
{
    [SerializeField] private List<Slot_UI> toolbarSlots = new List<Slot_UI>();

    private Slot_UI selectedSlot;

    private void Start()
    {
        SelectSlot(0);   //already select first slot when start game
    }
    private void Update()
    {
        CheckAlphaNumericKey();
    }

    public void SelectSlot(Slot_UI slot)
    {
        SelectSlot(slot.slotID);
    }

    public void SelectSlot(int index)
    {
        if(toolbarSlots.Count == 9)
        {
            if(selectedSlot != null)
            {
                selectedSlot.SetHighlight(false); // if slot isn't selected it will not show highlight
            }
            selectedSlot = toolbarSlots[index];
            selectedSlot.SetHighlight(true);                    //highlight selected slot
            //Debug.Log("Selected Slot: " + selectedSlot.name);

            GameManager.instance.player.inventoryManager.toolbar.SelectSlot(index);
        }
    }
    private void CheckAlphaNumericKey()   //setkey
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectSlot(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectSlot(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectSlot(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectSlot(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectSlot(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SelectSlot(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SelectSlot(8);
        }
    }

 
}
 

