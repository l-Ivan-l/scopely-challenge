using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerElementButton : MonoBehaviour, IPointerClickHandler
{
    private UIController uiController;
    private string playerName;

    void Awake()
    {
        uiController = GetComponentInParent<UIController>();
    }

    public void SetPlayerName(string _playerName)
    {
        playerName = _playerName;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        uiController.ActivatePlayerStatsPanel(playerName);
    }
}
