using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Image ExplainButtonSprite;
    public Image WakeupButtonSprite;       

    public static PlayerController i { private set; get; }

    public PlayerState playerState;

    private GameState _gameState;

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
        _gameState = GameState.Instance;
        playerState = PlayerState.Idle;

        this.ExplainButtonSprite.color = Color.white;
        this.WakeupButtonSprite.color = Color.white;
    }

    public void Explain()
    {
        playerState = PlayerState.ActiveExplain;

        this.ExplainButtonSprite.color = Color.red;
        this.WakeupButtonSprite.color = Color.white;
    }

    public void Wakeup()
    {
        playerState = PlayerState.ActiveWakeup;

        this.ExplainButtonSprite.color = Color.white;
        this.WakeupButtonSprite.color = Color.red;
    }

    public void BackToIdle()
    {
        playerState = PlayerState.Idle;

        this.ExplainButtonSprite.color = Color.white;
        this.WakeupButtonSprite.color = Color.white;
    }

    public void Skip()
    {
        BackToIdle();

        _gameState.GoToNextPerson();
        //  From Game State - move to the next person!
    }
}
