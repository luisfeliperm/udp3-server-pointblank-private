namespace Core.models.enums
{
    public enum RoomState
    {
        Ready, // Partida nao iniciado
        CountDown,
        Loading,
        Rendezvous,
        PreBattle,
        Battle, // Partida iniciada
        BattleEnd,
        Empty,
    }
}