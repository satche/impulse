using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenusController : MonoBehaviour
{
    [Tooltip("All menus in the game")]
    public List<Canvas> menus;

    void Start()
    {
        // Initially, only the first menu (main menu) is enabled
        SwitchMenu(0);
    }

    public void SwitchMenu(int index)
    {
        // Disable all menus
        foreach (Canvas menu in menus)
        {
            menu.enabled = false;
        }

        // Enable the selected menu
        if (index >= 0 && index < menus.Count)
        {
            menus[index].enabled = true;
        }
    }
}
