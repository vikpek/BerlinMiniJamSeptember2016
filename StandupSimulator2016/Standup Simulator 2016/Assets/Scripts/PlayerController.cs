using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController i { private set; get; }

    public PlayerState playerState;

    public enum PlayerState
    {
        Idle,
        ActiveExplain,
        ActiveWakeup
    };

    public void Awake()
    {
        if (i == null)
            i = this;
    }

    public void Start()
    {
        playerState = PlayerState.Idle;
    }

    // Get current speaker from gamestate

    public void Explain()
    {
        playerState = PlayerState.ActiveExplain;
    }

    public void Wakeup()
    {
        playerState = PlayerState.ActiveWakeup;
    }

    public void BackToIdle()
    {
        playerState = PlayerState.Idle;
    }

    public void Skip()
    {
        Person currSpeaker = GameState.Instance.CurrentPersonSpeaking;
    }
}
