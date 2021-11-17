
using System;
using System.Collections.Generic;

public class TestActionProvider : IActionProvider
{
    
    private List<Tuple<string, string>> _gestActions = new List<Tuple<string, string>>();

    private void Awake()
    {
      /*  _gestActions.Add(new Tuple<string, string>("", "GoLeft"));
        _gestActions.Add(new Tuple<string, string>("", "GoLd"));
        _gestActions.Add(new Tuple<string, string>("Droite", "GoRight"));
        _gestActions.Add(new Tuple<string, string>("take", "Jump"));
        _gestActions.Add(new Tuple<string, string>("prendrepoignet", "Crouch"));*/
    }

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override List<Tuple<string, string>> GetMapAction()
    {
        return _gestActions;
    }

    public override void UpdateMap(List<Tuple<string, string>> map)
    {
        _gestActions = map;
    }

}
