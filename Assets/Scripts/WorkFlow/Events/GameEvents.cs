namespace GameEvents
{
    public class GameStart : GameEventBase<GameStart> {}

    public class OnEntityEliminate : GameEventBase<GameStart>
    {
        public ElementEntity mainEntity;
        public ElementEntity viceEntity;

        public static OnEntityEliminate Create(ElementEntity _main, ElementEntity _vice)
        {
            var result = new OnEntityEliminate();
            result.mainEntity = _main;
            result.viceEntity = _vice;
            return result;
        }
    }
}