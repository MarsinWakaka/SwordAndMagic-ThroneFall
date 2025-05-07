namespace Events
{
    // 事件类型枚举（可按需扩展）
    public static class GameEvent
    {
        public const string LoadGridRequest = "LoadGridRequest";
        public const string UnitSpawnRequest = "UnitSpawnRequest";
        public const string LoadDialogueRequest = "LoadDialogueRequest";
        
        public const string UnitSpawn = "UnitSpawn";
        public const string UnitDespawn = "UnitDespawn";
        public const string UnitMove = "CharacterMove";
        public const string CharacterAttack = "CharacterAttack";
        public const string CharacterDie = "CharacterDie";
        
        /// 通过对话文件的ID字符串去加载对话文件，对应的事件类名为 LoadDialogueEvent
        public const string DialogueRequest = "DialogueRequest";
        public const string CharacterExpressionRequest = "CharacterExpressionRequest";
        public const string DialogueImageRequest = "DialogueImageRequest";
        
        public const string OnBattleStart = "OnBattleStart";
        public const string OnTurnStart = "OnTurnStart";
        public const string OnTurnEnd = "OnTurnEnd";
        
        public const string CalculateMovableAreaRequest = "CalculateMovableAreaRequest";
        public const string CalculateAttackableAreaRequest = "CalculateAttackableAreaRequest";
        public const string ShowAreaEvent = "ShowAreaEvent";
        public const string HideAreaEvent = "HideAreaEvent";
        public const string ShowPathWayEvent = "ShowPathWayEvent";
        public const string HidePathWayEvent = "HidePathWayEvent";
        
        // 循环事件
        public const string TriggerSceneEvent = "TriggerSceneEvent";
        public const string OnCharacterActionStart = "OnCharacterActionStart";
        public const string OnActionEnd = "OnCharacterActionEnd";
        public const string OnPreHit = "OnPreHit";
        public const string OnHit = "OnHit";
        public const string OnHitEnd = "OnHitEnd";
        public const string OnPreAttack = "OnPreAttack";
        public const string OnAttack = "OnAttack";
        public const string OnAttackEnd = "OnAttackEnd";
        public const string OnPreMove = "OnPreMove";
        public const string OnMove = "OnMove";
        public const string OnBuffApplied = "OnBuffApplied";
        public const string OnBuffRemoved = "OnBuffRemoved";
        public const string OnCreateGridEffect = "OnCreateGridEffect";
        public const string OnRemoveGridEffect = "OnRemoveGridEffect";
        
        public const string GameOver = "GameOver";
        
        // Skill
        public const string OnSkillSlotSelected = "SkillSelectedUpdateUI";
        public const string OnSkillTargetSelected = "OnSkillTargetSelected";
        public const string SkillReleaseConfirmInput = "SkillReleaseConfirmInput";
        public const string CancelSkillSelectedInput = "CancelSkillSelectedInput";
    }
}