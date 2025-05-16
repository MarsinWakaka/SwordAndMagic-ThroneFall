using Events.Battle;
using GameLogic.Unit;
using MyFramework.Utilities;
using MyFramework.Utilities.Extensions;
using SoundSystem;
using UI;
using UI.ConcretePanel.Battle;
using UnityEngine;

namespace GameLogic.Battle
{
    public class TurnManager : MonoBehaviour
    {
        // 遍历顺序
        // (回合开始) -> 场景事件 -> 玩家回合 -> 敌人回合 -> 场景物体 -> (回合结束)

        protected void Awake()
        {
            CurRound = 0;
        }

        public void OnEnable()
        {
            EventBus.Channel(Channel.Gameplay).Subscribe<StartBattleEvent>(HandleBattleStart);
        }

        public void OnDisable()
        {
            EventBus.Channel(Channel.Gameplay).Unsubscribe<StartBattleEvent>(HandleBattleStart);
        }

        private int CurRound { get; set; }

        private void HandleBattleStart(StartBattleEvent eventArgs)
        {
            CurRound = eventArgs.Round;
            SoundManager.Instance.PlayBGM(BGM.BattleBGM);
            StartTurn(CurRound);
        }

        private void StartTurn(int round)
        {
            CurRound = round;
            $"[Cur Turn {CurRound}]".LogWithColor(Color.cyan);
            
            // 场景事件 (通知场景事件)
            "[Scene Event]".LogWithColor(Color.cyan);
            EventBus.Channel(Channel.Gameplay).Publish(
                new TriggerSceneEventArgs(CurRound, () =>
                {
                    // TODO : Clear Undo Stack
                    // 玩家回合 (通知玩家回合)
                    "[Player Turn]".LogWithColor(Color.cyan);
                    SoundManager.Instance.PlaySFXOneShot(SFX.TurnStart);
                    UIManager.Instance.ShowPanel(PanelName.BattleTurnTipPanel, OpenStrategy.Additive, 
                        new TurnTipPanelParam(
                            "玩家回合",
                            () =>
                            {
                                UIManager.Instance.ClosePanel(PanelName.BattleTurnTipPanel);
                                EventBus.Channel(Channel.Gameplay).Publish(
                                    new StartTurnEvent(CurRound, Faction.Player, () =>
                                    {
                                        // TODO : Clear Undo Stack
                                        // 敌人回合 (通知敌人回合)
                                        "[Enemy Turn]".LogWithColor(Color.cyan);
                                        SoundManager.Instance.PlaySFXOneShot(SFX.TurnStart);
                                        UIManager.Instance.ShowPanel(PanelName.BattleTurnTipPanel, OpenStrategy.Additive,
                                            new TurnTipPanelParam(
                                                "敌人回合",
                                                () =>
                                                {
                                                    UIManager.Instance.ClosePanel(PanelName.BattleTurnTipPanel);
                                                    EventBus.Channel(Channel.Gameplay).Publish(
                                                        new StartTurnEvent(CurRound, Faction.Enemy, () =>
                                                        {
                                                            // 回合结束 (通知回合结束)
                                                            "[End Turn]".LogWithColor(Color.cyan);
                                                            StartTurn(CurRound + 1);
                                                        }));
                                                }));
                                    }));
                            }));
                }));
        }
    }
}