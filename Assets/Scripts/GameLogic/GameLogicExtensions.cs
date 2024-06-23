using Cysharp.Threading.Tasks;
using System.ComponentModel;

public static class GameLogicExtensions
{
    public static async UniTask LoadAllData(this GameLogic gameLogic, int playerId)
    {
        await gameLogic.LoadPlayerData(playerId);
        await gameLogic.LoadMails(playerId);
        await gameLogic.LoadGuilds();
        await gameLogic.LoadFriends(playerId);
        await gameLogic.LoadPlayerWeapons(playerId);
        await gameLogic.LoadPlayerSkills(playerId);
        await gameLogic.LoadPlayerBlessings(playerId);
        await gameLogic.LoadMissionProgress(playerId);
        await gameLogic.LoadRewards();
    }

    public static void SubscribeToPropertyChanged(this GameLogic gameLogic, PropertyChangedEventHandler handler)
    {
        gameLogic.PropertyChanged += handler;
    }

    public static void UnsubscribeFromPropertyChanged(this GameLogic gameLogic, PropertyChangedEventHandler handler)
    {
        gameLogic.PropertyChanged -= handler;
    }
}