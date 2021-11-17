using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Sert d'intermédiaire entre les applications et les applications transversales
/// </summary>
public abstract class IActionProvider : MonoBehaviour
{

    /// <summary>
    /// permet d'obtenir les couples de (gest,actions)
    /// </summary>
    /// <returns>Les couples bijectifs gest->action</returns>
    public abstract List<Tuple<string, string>> GetMapAction();

    /// <summary>
    /// Met a jour les couples gestes-> actions
    /// </summary>
    /// <param name="map">les nouveaux couples</param>
    public abstract void UpdateMap(List<Tuple<string, string>> map);


}
