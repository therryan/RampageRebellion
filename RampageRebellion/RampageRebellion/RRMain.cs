using System;

/// <summary>
/// The class used to run the game
/// </summary>
static class ProgramMain
{

#if WINDOWS || XBOX
    static void Main(string[] args)
    {
        using (RampageRebellion game = new RampageRebellion())
        {
#if !DEBUG
            game.IsFullScreen = true;
#endif
            game.Run();
        }
    }
#endif
}
