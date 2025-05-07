using Events;
using Events.Battle;
using MyFramework.Utilities;
using Test;
using UI;
using UI.ConcretePanel.Battle;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(UIManagerTest))]
    public class UIManagerTestEditor : UnityEditor.Editor
    {
        private SerializedProperty _panelNameProp;
        private SerializedProperty _openStrategyProp;
        
        // 缓存用户输入的字符串（非序列化字段，仅运行时存在）
        private string _cachedDialogueText = "1-1-1";

        private void OnEnable()
        {
            _panelNameProp = serializedObject.FindProperty("panelName");
            _openStrategyProp = serializedObject.FindProperty("openStrategy");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector(); // 绘制默认Inspector

            // 动态扩展部分：仅对BattleDialoguePanel显示输入框
            if (_panelNameProp.stringValue == PanelName.BattleDialoguePanel)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Dialogue Parameters", EditorStyles.boldLabel);
                
                // 使用普通GUI.TextField缓存输入（不绑定SerializedProperty）
                _cachedDialogueText = EditorGUILayout.TextField("Dialogue Text", _cachedDialogueText);
            }

            // 公共按钮逻辑
            EditorGUILayout.Space();
            if (GUILayout.Button("Open Panel"))
            {
                OpenPanelWithCurrentParams();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OpenPanelWithCurrentParams()
        {
            var panelName = _panelNameProp.stringValue;
            var openStrategy = (OpenStrategy)_openStrategyProp.enumValueIndex;
            object data = null;

            if (panelName == PanelName.BattleDialoguePanel)
            {
                EventBus.Channel(Channel.Gameplay).Publish(new LoadDialogueEvent(
                    _cachedDialogueText, () =>
                    {
                        Debug.Log($"对话加载完成 : {_cachedDialogueText}");
                    }));
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPanel(panelName, openStrategy, data);
            }
            else
            {
                Debug.LogError("UIManager instance not found!");
            }
        }
    }
}