using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenusController : MonoBehaviour
{
    [Tooltip("All menus in the game")]
    public List<Canvas> menus;

    void Start()
    {
        SwitchMenu("Main");
    }

    /// <summary>
    /// Switches to the menu at the given index.
    /// </summary>
    /// <param name="index">The index of the menu to switch to</param>
    public void SwitchMenu(string name)
    {
        foreach (Canvas menu in menus)
        {
            menu.enabled = false;
            if (menu.name == name)
            {
                menu.enabled = true;
            }
        }
    }

    /// <summary>
    /// Loads the scene with the given name.
    /// </summary>
    /// <param name="sceneName">Name of the scene</param>
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void OnSliderChange(Slider slider)
    {
        // Floor the value to one decimal
        slider.value = Mathf.Floor(slider.value * 10) / 10;

        // Update the text
        TextMeshProUGUI textValue = slider.transform.Find("Handle Slide Area/Handle/Slider Value").GetComponent<TextMeshProUGUI>();
        textValue.text = slider.value.ToString();

        // Store the value in the PlayerPrefs
        PlayerPrefs.SetFloat(slider.name, slider.value);
    }
}
