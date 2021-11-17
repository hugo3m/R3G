using System.Collections;
using System.Collections.Generic;
using Menus;
using UnityEngine;

public class GoMenu : MonoBehaviour
{
    public AppMenuItem menu;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
    }
    
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            menu.Draw();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            menu.Undraw();
        }
    }
}
