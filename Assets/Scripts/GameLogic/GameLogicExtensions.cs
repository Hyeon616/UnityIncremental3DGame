using Cysharp.Threading.Tasks;
using System;
using System.ComponentModel;

public static class GameLogicExtensions
{
    public static async UniTask<bool> LoadAllData(this GameLogic gameLogic, int playerId)
    {
        try
        {
            
            var tasks = new[]
            {
                ResourceManager.Instance.LoadPlayerData(playerId),
                ResourceManager.Instance.LoadMails(playerId),
                ResourceManager.Instance.LoadGuilds(),
                ResourceManager.Instance.LoadFriends(playerId),
                ResourceManager.Instance.LoadPlayerWeapons(playerId),
                ResourceManager.Instance.LoadPlayerSkills(playerId),
                ResourceManager.Instance.LoadMissionProgress(playerId),
                ResourceManager.Instance.LoadAllStages(),
                ResourceManager.Instance.LoadMonsterData(),
                ResourceManager.Instance.LoadCurrentStage(playerId)
        };

            await UniTask.WhenAll(tasks);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
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