﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Person : MonoBehaviour
{
    //----------------------------------------------------------
    public enum State
    {
        Invalid,
        Listening,
        Talking
    }

    public enum ListeningState
    {
        Invalid,
        Understands,    // 
        Asleep,         // don't receive information
        Confused,       // receive information reduced
    }

    public enum TalkingState
    {
        Invalid,
        Idle,
        Slow,
        Fast,
    }

    //----------------------------------------------------------
    public Dictionary<GameState.Department, float> DeparmentQualities = new Dictionary<GameState.Department, float>();
    public State CurrentState = State.Invalid;
    public ListeningState CurrentListeningState = ListeningState.Invalid;
    public TalkingState CurrentTalkingState = TalkingState.Invalid;

    //----------------------------------------------------------
    private void Start ()
    {
        this.StartListeningState();
    }

    //----------------------------------------------------------
    private void Update ()
    {
        if (this.CurrentState == State.Talking)
        {
            if (this.CurrentTalkingState == TalkingState.Slow)
            {
                
            }
        }
	}

    //----------------------------------------------------------
    public void Talk()
    {
        this.StartListeningState();
    }

    //----------------------------------------------------------
    public void StopTalk()
    {
        this.StartListeningState();
    }

    //----------------------------------------------------------
    private void StartTalkState()
    {
        this.CurrentState = State.Talking;
        this.CurrentTalkingState = TalkingState.Idle;
    }

    //----------------------------------------------------------
    private void StartListeningState()
    {
        this.CurrentState = State.Listening;
        this.CurrentListeningState = ListeningState.Understands;
    }

    //----------------------------------------------------------
}