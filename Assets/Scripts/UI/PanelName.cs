namespace UI
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PanelName
    {
        // 全局
        public const string MainMenuPanel = "MainMenuPanel";
        public const string LoadingPanel = "LoadingPanel";
        public const string MainLobbyPanel = "MainLobbyPanel";
        public const string NavigationPanel = "NavigationPanel";
        public const string SettingsPanel = "SettingsPanel";
        public const string InventoryPanel = "InventoryPanel";
        public const string CharacterDetailPanel = "CharacterDetailPanel";
        public const string CharacterSkillDetailPanel = "CharacterSkillDetailPanel";
        public const string CharacterRosterPanel = "CharacterRosterPanel";
        public const string CharacterUpgradePanel = "CharacterUpgradePanel";
        
        // 战斗
        public const string BattlePanel = "BattlePanel";
        public const string BattleDeployPanel = "BattleDeployPanel";
        public const string BattleInfoPanel = "BattleInfoPanel";
        public const string BattleDialoguePanel = "BattleDialoguePanel";
        public const string BattleCharacterControlPanel = "BattleCharacterControlPanel";
        public const string BattleSkillReleasePanel = "BattleSkillReleasePanel";
        public const string BattleVictoryPanel = "BattleVictoryPanel";
        public const string BattleTurnTipPanel = "BattleTurnTipPanel";

        public static string[] GetAllPanelNames()
        {
            return new[]
            {
                MainMenuPanel,
                LoadingPanel,
                MainLobbyPanel,
                NavigationPanel,
                SettingsPanel,
                InventoryPanel,
                CharacterDetailPanel,
                CharacterSkillDetailPanel,
                CharacterRosterPanel,
                CharacterUpgradePanel,
                
                BattlePanel,
                BattleDeployPanel,
                BattleInfoPanel,
                BattleDialoguePanel,
                BattleCharacterControlPanel,
                BattleSkillReleasePanel,
                BattleVictoryPanel,
                BattleTurnTipPanel
            };
        }
    }
}