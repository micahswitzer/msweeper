namespace MSweeper.Core
{
    public interface IGamePlayer
    {
        int ChoicesMade { get; }
        int GuessesMade { get; }
        TilePosition ChooseTile(Game game);
    }
}
