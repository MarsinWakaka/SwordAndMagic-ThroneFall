using Core.SceneSystem;
using Events.Battle;
using GameLogic.Grid;
using GameLogic.Map;
using GameLogic.Unit;
using MyFramework.Utilities;
using Player;
using Scene;
using SoundSystem;
using UI;
using UI.ConcretePanel.Battle;
using UnityEngine;

namespace GameLogic.Battle
{
    /// <summary>
    /// [function]
    /// [purpose]
    /// 用于控制战斗流程的系统
    /// </summary>
    public class BattleSystem : MonoBehaviour
    {
        [Header("服务注册区")]
        [SerializeField] private UnitManager unitManager;
        [SerializeField] private GridManager gridManager;

        private void OnEnable()
        {
            // 注册服务
            // TODO 完成其它服务的注册 
            ServiceLocator.Register<IGridManager>(gridManager);
            ServiceLocator.Register<IUnitManager>(unitManager);
            
            // 监听事件
            EventBus.Channel(Channel.Gameplay).Subscribe<LoadLevelEvent>(HandleLoadLevelEvent);
            EventBus.Channel(Channel.Gameplay).Subscribe<FactionWipeEvent>(HandleFactionDead);
        }

        private void OnDisable()
        {
            // 注销服务
            // TODO 完成其它服务的注销
            ServiceLocator.UnRegister<IGridManager>();
            ServiceLocator.UnRegister<IUnitManager>();
            
            // 取消事件订阅
            EventBus.Channel(Channel.Gameplay).Unsubscribe<LoadLevelEvent>(HandleLoadLevelEvent);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<FactionWipeEvent>(HandleFactionDead);
        }
        
#if UNITY_EDITOR
        [ContextMenu("LoadDefaultLevel")]
        private void LoadDefaultLevel()
        {
            HandleLoadLevelEvent(new LoadLevelEvent("1-1"));
        }
#endif
        
        private string _levelID;
        private int _reward;

        private void HandleLoadLevelEvent(LoadLevelEvent loadEvent)
        {
            // 调用MapLoader加载游戏场景
            if (!LevelDataLoader.TryLoadLevelData(loadEvent.LevelID, out var mapContext))
            {
                Debug.LogError($"关卡文件 {loadEvent.LevelID} 不存在");
                return;
            }

            _levelID = loadEvent.LevelID;
            _reward = mapContext.reward;
            
            // 事件通知
            var battleChannel = EventBus.Channel(Channel.Gameplay);

            battleChannel.Publish(new LoadDialogueEvent(mapContext.dialogueId, () =>
            {
                // 1.初始化地图
                battleChannel.Publish(new LoadGridRequest(mapContext.grids));
                // 2.单位管理器初始化
                // battleChannel.Publish(GameEvent.UnitSpawnRequest, new LoadUnitRequest(mapContext.obstacles));

                foreach (var spawnDataConfig in mapContext.characterSpawnData)
                {
                    battleChannel.Publish(new SpawnCharacterEvent(spawnDataConfig.ConvertToCharacterSpawnData()));
                }

                // 3.部署
                // 准备部署数据
                var charactersData = mapContext.PresetCharacters;
                if (mapContext.allowPlayerCharacters) charactersData.AddRange(PlayerDataManager.Instance.GetAllOwnedCharacters());
                // 发布部署事件
                battleChannel.Publish(new StartDeployEvent(
                    charactersData,
                    mapContext.deployableGrids, 
                    mapContext.maxDeployCount,
                    () =>
                {
                    // battleChannel.Publish(GameEvent.UnitSpawnRequest, new LoadUnitRequest(mapContext.obstacles));
                
                    // 开始战斗
                    battleChannel.Publish(new StartBattleEvent(1));
                }));
            }));
        }

        private void HandleFactionDead(FactionWipeEvent factionWipeEvent)
        {
            // TODO 处理阵营全灭事件
            // 1.显示战斗结果
            // 2.结束战斗
            // 3.保存战斗结果
            // 4.返回主菜单
            Debug.Log($"[Faction {factionWipeEvent.Faction} wiped out]");
            if (factionWipeEvent.Faction == Faction.Player)
            {
                HandleGameLose();
            }
            else
            {
                HandleGameWin();
            }
        }
        
        [ContextMenu("战斗胜利")]
        private void HandleGameWin()
        {
            // TODO 处理游戏胜利事件
            // 1.显示战斗结果
            // 2.结束战斗
            // 3.保存战斗结果
            if (_levelID == string.Empty)
            {
                Debug.Log("No level loaded, levelID is empty.");
                return;
            }
            SoundManager.Instance.PlayBGM(BGM.BattleVictoryBGM);
            EventBus.Channel(Channel.Gameplay).Publish(new LevelPassedEvent(_levelID, 3));
            // 4.返回主菜单
            Debug.Log("[Game Win]");
            // TODO 获取关卡胜利结果

            var winParam = new BattleVictoryParams(_reward, () =>
            {
                SceneLoader.LoadScene(new MainMenuScene());
            });
            UIManager.Instance.ShowPanel(PanelName.BattleVictoryPanel, OpenStrategy.PauseCurrent, winParam);
        }
        
        private void HandleGameLose()
        {
            // TODO 处理游戏失败事件
            // 1.显示战斗结果
            // 2.结束战斗
            // 3.保存战斗结果
            // 4.返回主菜单
            Debug.Log("[Game Lose]");

            var loseParam = new BattleVictoryParams(0, () =>
            {
                SceneLoader.LoadScene(new MainMenuScene());
            });
            UIManager.Instance.ShowPanel(PanelName.BattleVictoryPanel, OpenStrategy.PauseCurrent, loseParam);
        }
    }
}