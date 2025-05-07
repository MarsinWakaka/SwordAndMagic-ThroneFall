using Events;
using Events.Battle;
using MyFramework.Utilities;
using UI;
using UI.ConcretePanel.Battle;
using UnityEngine;

namespace GameLogic.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        // 1. 按功能分类
        // 类型	                    作用	                        示例
        // 教程关 (Tutorial)	        引导玩家学习基础操作和规则	    T_1-新手教学、T_Combat
        // 主线关 (Main Story)	    推动核心剧情发展的关键关卡	    MS_2-3-沙漠遗迹
        // 支线关 (Side Quest)	    可选内容，提供额外奖励或背景故事	SQ_1-失踪的商人
        // Boss关 (Boss Fight)	    高强度战斗或解谜挑战	        B_3-5-机械巨龙、Boss_Final
        // 隐藏关 (Secret Level)	    需要特殊条件解锁的关卡	        SECRET_1-彩虹桥
        // 挑战关 (Challenge Mode)	限时、分数或特殊规则挑战	    CH_TimeAttack_1

        private const string DialogueDataPath = "Dialogues/";

        private void OnEnable()
        {
            EventBus.Channel(Channel.Gameplay)
                .Subscribe<LoadDialogueEvent>(OnDialogueRequest);
        }

        private void OnDisable()
        {
            EventBus.Channel(Channel.Gameplay)
                .Unsubscribe<LoadDialogueEvent>(OnDialogueRequest);
        }
        
        private LoadDialogueEvent _loadDialogueEvent;

        private void OnDialogueRequest(LoadDialogueEvent args)
        {
            if (args.DialogueId.Trim() == string.Empty)
            {
                args.DialogueComplete.Invoke();
                return;
            }

            _loadDialogueEvent = args;

            Debug.Log($"对话请求: {args.DialogueId}");
            var dialogueData = Resources.Load<Dialogue>($"{DialogueDataPath}{args.DialogueId}");
            if (dialogueData != null)
            {
                UIManager.Instance.ShowPanel(PanelName.BattleDialoguePanel, OpenStrategy.Additive,
                    new BattleDialogueParams(dialogueData, HandleDialogueEnd));
            }
            else
            {
                Debug.LogError($"对话数据加载失败: {args.DialogueId}");
                args.DialogueComplete.Invoke();
            }
        }

        private void HandleDialogueEnd()
        {
            UIManager.Instance.ClosePanel(PanelName.BattleDialoguePanel);
            _loadDialogueEvent.DialogueComplete();
        }
    }
}