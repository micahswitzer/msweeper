namespace MSweeper.Core
{
    public interface IGamePlayer
    {
        TilePosition ChooseTile(Game game);
    }
}
