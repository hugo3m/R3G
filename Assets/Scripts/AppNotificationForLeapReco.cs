using System.Collections.Generic;
using Recognizer;
using Recognizer.StrategyFusion;
using UnityEngine;

public abstract class AppNotificationForLeapReco : MonoBehaviour
{
    /// <summary>
    /// L'event à lancer quand un geste est reconnu
    /// </summary>
    public abstract System.Action<string> GetGestureRecognizedAction();
    
    /// <summary>
    /// Donne les scores des classifeurs
    /// </summary>
    public abstract System.Action<ResultStrategy> GetClassDetailedScoresAction();
    
    
    /// <summary>
    /// Event pour notifier lorsqu'il y a un problème avec la reconnaissance (interaction avec le moteur)
    /// Est lancé si aucun résultat du moteur n'est reçu pendant deux secondes
    /// </summary>
    public abstract System.Action<RecoManager.StatusReco> GetStatusRecoChangeAction();
}

