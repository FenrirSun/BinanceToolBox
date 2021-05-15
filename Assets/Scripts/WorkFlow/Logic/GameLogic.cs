

using System;
using GameEvents;

public class GameLogic : LogicBase
{
    private LinkUpWorld world;
    
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void RegisterEvents()
    {
        var ec = GetEventComp();
        ec.Listen<GameStart>(evt =>
        {
            StartOneGame();
        });
    }

    private void StartOneGame()
    {
        if(world == null)
            world = new LinkUpWorld();
        
        world.Start();
    }

    private void Update()
    {
        if(world != null && world.isPlaying)
            world.Update();
    }
}
