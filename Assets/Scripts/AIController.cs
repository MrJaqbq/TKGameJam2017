﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : IController
{
    public bool DrawDebug;
    public GameObject PawnPrefab;
    public AIBehaviour Behaviour;
    public List<Transform> StartPoints = new List<Transform>();
    public Transform WorldOrigin;

    public Pawn Pawn { get; private set; }

    private bool died = false;

    public override void OnStart()
    {
        SpawnPawn();
        
        Pawn.OnStart(this);
        Behaviour.OnStart(MainGame.Instance.Player.Pawn, Pawn);
    }

    public void SpawnPawn()
    {
        if (Pawn)
            return;

        var startPoint = StartPoints[Random.Range(0, StartPoints.Count)];
        var go = Instantiate(PawnPrefab, startPoint.position, startPoint.rotation) as GameObject;

        var dir = startPoint.position - WorldOrigin.position;

        Pawn = go.GetComponent<Pawn>();
        Pawn.WorldOrigin = WorldOrigin;
        Pawn.Gravity.DefaultGravityDir = -dir.normalized;
    }

    public override void OnUpdate()
    {
        if (!Pawn)
            return;

        Pawn.DrawDebug = DrawDebug;

        Behaviour.OnBehave();
        
        Pawn.MovementDirection(Behaviour.GetDesiredMovement());
        Pawn.OnUpdate();

        if (!Pawn.IsAlive)
        {
            DestroyObject(Pawn.gameObject);
            Pawn = null;
            died = true;
        }
    }

    public bool IsAlive()
    {
        if (Pawn)
            return Pawn.IsAlive;
        return !died;
    }

    public override void StartPromptUsage(ActionObject obj)
    {
        obj.OnActivate(this, Pawn);
    }
}
