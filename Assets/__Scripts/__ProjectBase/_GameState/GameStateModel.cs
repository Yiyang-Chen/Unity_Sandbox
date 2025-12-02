public class GameStateModel : IRuntimeModel
{
    public EGameState currentGameState = EGameState.GameDefaultState;

    public void SetGameState(EGameState s)
    {
        currentGameState = s;
    }
}