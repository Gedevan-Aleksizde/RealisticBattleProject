// CunningLords.Interaction.CunningLordsMenuViewModel
using JetBrains.Annotations;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace RBMConfig
{
    internal class RBMConfigViewModel : ViewModel
    {
        public TextViewModel ArmorStatusUIEnabledText { get; }
        public SelectorVM<SelectorItemVM> ArmorStatusUIEnabled { get; }

        public TextViewModel RealisticArrowArcText { get; }
        public SelectorVM<SelectorItemVM> RealisticArrowArc { get; }

        public TextViewModel PostureSystemEnabledText { get; }
        public SelectorVM<SelectorItemVM> PostureSystemEnabled { get; }

        public TextViewModel PlayerPostureMultiplierText { get; }
        public SelectorVM<SelectorItemVM> PlayerPostureMultiplier { get; }

        public TextViewModel PostureGUIEnabledText { get; }
        public SelectorVM<SelectorItemVM> PostureGUIEnabled { get; }

        public TextViewModel VanillaCombatAiText { get; }
        public SelectorVM<SelectorItemVM> VanillaCombatAi { get; }

        public TextViewModel ActiveTroopOverhaulText { get; }
        public SelectorVM<SelectorItemVM> ActiveTroopOverhaul { get; }

        public TextViewModel RangedReloadSpeedText { get; }
        public SelectorVM<SelectorItemVM> RangedReloadSpeed { get; }

        public TextViewModel PassiveShoulderShieldsText { get; }
        public SelectorVM<SelectorItemVM> PassiveShoulderShields { get; }

        public TextViewModel BetterArrowVisualsText { get; }
        public SelectorVM<SelectorItemVM> BetterArrowVisuals { get; }

        public SelectorVM<SelectorItemVM> RBMCombatEnabled { get; }

        public SelectorVM<SelectorItemVM> RBMAIEnabled { get; }

        public SelectorVM<SelectorItemVM> RBMTournamentEnabled { get; }

        [DataSourceProperty]
        public string LabelRBMConfiguration => new TextObject("{=RBM.Conf000}RBM Configuration").ToString();
        [DataSourceProperty]
        public string LabelRBMCombat=> new TextObject("{=RBM.Conf200}RBM Combat").ToString();
        [DataSourceProperty]
        public string LabelRBMAI => new TextObject("{=RBM.Conf201}RBM AI").ToString();
        [DataSourceProperty]
        public string LabelRBMTourney => new TextObject("{=RBM.Conf202}RBM Tournament").ToString();
        [DataSourceProperty]
        public string LabelModuleStatus => new TextObject("{=RBM.Conf203}Module Status").ToString();
        [DataSourceProperty]
        public string LabelTroopOverhaul => new TextObject("{=RBM.Conf001}Troop Overhaul").ToString();
        [DataSourceProperty]
        public string LabelRangedreloadspeed => new TextObject("{=RBM.Conf002}Ranged reload speed").ToString();
        [DataSourceProperty]
        public string LabelPassiveShoulderShields => new TextObject("{=RBM.Conf003}Passive Shoulder Shields").ToString();
        [DataSourceProperty]
        public string LabelBetterArrowVisuals => new TextObject("{=RBM.Conf004}Better Arrow Visuals").ToString();
        [DataSourceProperty]
        public string LabelArmorStatusGUI => new TextObject("{=RBM.Conf005}Armor Status GUI").ToString();
        [DataSourceProperty]
        public string LabelRealisticArrowArc => new TextObject("{=RBM.Conf006}Realistic Arrow Arc").ToString();
        [DataSourceProperty]
        public string LabelPostureSystem => new TextObject("{=RBM.Conf007}Posture System").ToString();
        [DataSourceProperty]
        public string LabelPlayerPostureMultiplier => new TextObject("{=RBM.Conf008}Player Posture Multiplier").ToString();
        [DataSourceProperty]
        public string LabelPostureGUI => new TextObject("{=RBM.Conf009}Posture GUI").ToString();
        [DataSourceProperty]
        public string LabelVanillaAIBlock_Parry_Attack => new TextObject("{=RBM.Conf010}Vanilla AI Block/Parry/Attack").ToString();
        [DataSourceProperty]
        public string LabelRBMTournament => new TextObject("{=RBM.Conf202}RBM Tournament").ToString();
        [DataSourceProperty]
        public string CancelText => new TextObject("{=RBM.Conf101}Cancel").ToString();

        [DataSourceProperty]
        public string DoneText => new TextObject("{=RBM.Conf100}Done").ToString();

        public RBMConfigViewModel()
        {
            TextObject _default = new("{=RBM.Val004} (Default)");
            TextObject _recommended = new("{=RBM.Val003} (Recommended)");
            RefreshValues();
            //RbmConfigData data;
            List<TextObject> troopOverhaulOnOff = new() { new("{=RBM.Val001}Inactive{status}"), new TextObject("{=RBM.Val002}Active{status}").SetTextVariable("status", _recommended) };
            ActiveTroopOverhaulText = new (new("{=RBM.Conf001}Troop Overhaul"));
            ActiveTroopOverhaul = new SelectorVM<SelectorItemVM>(troopOverhaulOnOff, 0, null);

            List<TextObject> rangedReloadSpeed = new () { new("{=RBM.Val005}Vanilla{status}"), new("{=RBM.Val006}Realistic{status}"), new("{=RBM.Val007}Semi-Realistic{status}") };
            RangedReloadSpeedText = new (new("{=RBM.Conf002}Ranged reload speed"));
            RangedReloadSpeed = new SelectorVM<SelectorItemVM>(rangedReloadSpeed, 0, null);

            List<TextObject> passiveShoulderShields = new () { new TextObject("{=RBM.Val001}Inactive{status}").SetTextVariable("status", _default), new("{=RBM.Val002}Active{status}") };
            PassiveShoulderShieldsText = new (new("{=RBM.Conf003}Passive Shoulder Shields"));
            PassiveShoulderShields = new SelectorVM<SelectorItemVM>(passiveShoulderShields, 0, null);

            List<TextObject> betterArrowVisuals = new () { new("{=RBM.Val008}Disabled{status}"), new TextObject("{=RBM.Val009}Enabled{status}").SetTextVariable("status", _default) };
            BetterArrowVisualsText = new TextViewModel(new("{=RBM.Conf004}Better Arrow Visuals"));
            BetterArrowVisuals = new SelectorVM<SelectorItemVM>(betterArrowVisuals, 0, null);

            List<TextObject> armorStatusUIEnabled = new () { new("{=RBM.Val008}Disabled{status}"), new TextObject("{=RBM.Val009}Enabled{status}").SetTextVariable("status", _default) };
            ArmorStatusUIEnabledText = new TextViewModel(new("{=RBM.Conf005}Armor Status GUI"));
            ArmorStatusUIEnabled = new SelectorVM<SelectorItemVM>(armorStatusUIEnabled, 0, null);

            List<TextObject> realisticArrowArc = new () { new TextObject("{=RBM.Val008}Disabled{status}").SetTextVariable("status", _default), new("{=RBM.Val009}Enabled{status}") };
            RealisticArrowArcText = new TextViewModel(new("{=RBM.Conf006}Realistic Arrow Arc"));
            RealisticArrowArc = new SelectorVM<SelectorItemVM>(realisticArrowArc, 0, null);

            if (RBMConfig.troopOverhaulActive)
            {
                ActiveTroopOverhaul.SelectedIndex = 1;
            }
            else
            {
                ActiveTroopOverhaul.SelectedIndex = 0;
            }

            if (RBMConfig.realisticRangedReload.Equals("0"))
            {
                RangedReloadSpeed.SelectedIndex = 0;
            }
            else if (RBMConfig.realisticRangedReload.Equals("1"))
            {
                RangedReloadSpeed.SelectedIndex = 1;
            }
            else if (RBMConfig.realisticRangedReload.Equals("2"))
            {
                RangedReloadSpeed.SelectedIndex = 2;
            }

            if (RBMConfig.passiveShoulderShields)
            {
                PassiveShoulderShields.SelectedIndex = 1;
            }
            else
            {
                PassiveShoulderShields.SelectedIndex = 0;
            }

            if (RBMConfig.betterArrowVisuals)
            {
                BetterArrowVisuals.SelectedIndex = 1;
            }
            else
            {
                BetterArrowVisuals.SelectedIndex = 0;
            }

            if (RBMConfig.armorStatusUIEnabled)
            {
                ArmorStatusUIEnabled.SelectedIndex = 1;
            }
            else
            {
                ArmorStatusUIEnabled.SelectedIndex = 0;
            }

            if (RBMConfig.realisticArrowArc)
            {
                RealisticArrowArc.SelectedIndex = 1;
            }
            else
            {
                RealisticArrowArc.SelectedIndex = 0;
            }

            List<TextObject> postureOptions = new () { new("{=RBM.Val008}Disabled{status}"), new TextObject("{=RBM.Val009}Enabled{status}").SetTextVariable("status", _default) };
            PostureSystemEnabledText = new TextViewModel(new("{=RBM.Conf007}Posture System"));
            PostureSystemEnabled = new SelectorVM<SelectorItemVM>(postureOptions, 0, null);

            List<TextObject> playerPostureMultiplierOptions = new () { new TextObject("{=RBM.Val010}1x{status}").SetTextVariable("status", _default), new("{=RBM.Val011}1.5x{status}"), new("{=RBM.Val012}2x{status}") };
            PlayerPostureMultiplierText = new TextViewModel(new("{=RBM.Conf008}Player Posture Multiplier"));
            PlayerPostureMultiplier = new SelectorVM<SelectorItemVM>(playerPostureMultiplierOptions, 0, null);

            List<TextObject> postureGUIOptions = new () { new("{=RBM.Val008}Disabled{status}"), new TextObject("{=RBM.Val009}Enabled{status}").SetTextVariable("status", _default) };
            PostureGUIEnabledText = new TextViewModel(new("{=RBM.Conf009}Posture GUI"));
            PostureGUIEnabled = new SelectorVM<SelectorItemVM>(postureGUIOptions, 0, null);

            List<TextObject> vanillaCombatAiOptions = new () { new TextObject("{=RBM.Val008}Disabled{status}").SetTextVariable("status", _default), new("{=RBM.Val009}Enabled{status}") };
            VanillaCombatAiText = new TextViewModel(new("{=RBM.Conf010}Vanilla AI Block/Parry/Attack"));
            VanillaCombatAi = new SelectorVM<SelectorItemVM>(vanillaCombatAiOptions, 0, null);

            if (RBMConfig.playerPostureMultiplier == 1f)
            {
                PlayerPostureMultiplier.SelectedIndex = 0;
            }
            else if (RBMConfig.playerPostureMultiplier == 1.5f)
            {
                PlayerPostureMultiplier.SelectedIndex = 1;
            }
            else if (RBMConfig.playerPostureMultiplier == 2f)
            {
                PlayerPostureMultiplier.SelectedIndex = 2;
            }

            if (RBMConfig.postureEnabled)
            {
                PostureSystemEnabled.SelectedIndex = 1;
            }
            else
            {
                PostureSystemEnabled.SelectedIndex = 0;
            }

            if (RBMConfig.postureGUIEnabled)
            {
                PostureGUIEnabled.SelectedIndex = 1;
            }
            else
            {
                PostureGUIEnabled.SelectedIndex = 0;
            }

            if (RBMConfig.vanillaCombatAi)
            {
                VanillaCombatAi.SelectedIndex = 1;
            }
            else
            {
                VanillaCombatAi.SelectedIndex = 0;
            }

            List<TextObject> rbmCombatEnabledOptions = new () { new("{=RBM.Val008}Disabled{status}"), new TextObject("{=RBM.Val009}Enabled{status}").SetTextVariable("status", _default) };
            RBMCombatEnabled = new SelectorVM<SelectorItemVM>(rbmCombatEnabledOptions, 0, null);

            List<TextObject> rbmAiEnabledOptions = new () { new("{=RBM.Val008}Disabled{status}"), new TextObject("{=RBM.Val009}Enabled{status}").SetTextVariable("status", _default) };
            RBMAIEnabled = new SelectorVM<SelectorItemVM>(rbmAiEnabledOptions, 0, null);

            List<TextObject> rbmTournamentEnabledOptions = new () { new("{=RBM.Val008}Disabled{status}"), new TextObject("{=RBM.Val009}Enabled{status}").SetTextVariable("status", _default) };
            RBMTournamentEnabled = new SelectorVM<SelectorItemVM>(rbmTournamentEnabledOptions, 0, null);

            if (RBMConfig.rbmCombatEnabled)
            {
                RBMCombatEnabled.SelectedIndex = 1;
            }
            else
            {
                RBMCombatEnabled.SelectedIndex = 0;
            }

            if (RBMConfig.rbmAiEnabled)
            {
                RBMAIEnabled.SelectedIndex = 1;
            }
            else
            {
                RBMAIEnabled.SelectedIndex = 0;
            }

            if (RBMConfig.rbmTournamentEnabled)
            {
                RBMTournamentEnabled.SelectedIndex = 1;
            }
            else
            {
                RBMTournamentEnabled.SelectedIndex = 0;
            }
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
        }

        private void ExecuteDone()
        {
            if (ActiveTroopOverhaul.SelectedIndex == 0)
            {
                RBMConfig.troopOverhaulActive = false;
            }
            if (ActiveTroopOverhaul.SelectedIndex == 1)
            {
                RBMConfig.troopOverhaulActive = true;
            }

            if (RangedReloadSpeed.SelectedIndex == 0)
            {
                RBMConfig.realisticRangedReload = "0";
            }
            else if (RangedReloadSpeed.SelectedIndex == 1)
            {
                RBMConfig.realisticRangedReload = "1";
            }
            else if (RangedReloadSpeed.SelectedIndex == 2)
            {
                RBMConfig.realisticRangedReload = "2";
            }

            if (PassiveShoulderShields.SelectedIndex == 0)
            {
                RBMConfig.passiveShoulderShields = false;
            }
            if (PassiveShoulderShields.SelectedIndex == 1)
            {
                RBMConfig.passiveShoulderShields = true;
            }

            if (BetterArrowVisuals.SelectedIndex == 0)
            {
                RBMConfig.betterArrowVisuals = false;
            }
            if (BetterArrowVisuals.SelectedIndex == 1)
            {
                RBMConfig.betterArrowVisuals = true;
            }

            if (ArmorStatusUIEnabled.SelectedIndex == 0)
            {
                RBMConfig.armorStatusUIEnabled = false;
            }
            if (ArmorStatusUIEnabled.SelectedIndex == 1)
            {
                RBMConfig.armorStatusUIEnabled = true;
            }

            if (RealisticArrowArc.SelectedIndex == 0)
            {
                RBMConfig.realisticArrowArc = false;
            }
            if (RealisticArrowArc.SelectedIndex == 1)
            {
                RBMConfig.realisticArrowArc = true;
            }

            if (PostureSystemEnabled.SelectedIndex == 0)
            {
                RBMConfig.postureEnabled = false;
            }
            if (PostureSystemEnabled.SelectedIndex == 1)
            {
                RBMConfig.postureEnabled = true;
            }

            if (PlayerPostureMultiplier.SelectedIndex == 0)
            {
                RBMConfig.playerPostureMultiplier = 1f;
            }
            else if (PlayerPostureMultiplier.SelectedIndex == 1)
            {
                RBMConfig.playerPostureMultiplier = 1.5f;
            }
            else if (PlayerPostureMultiplier.SelectedIndex == 2)
            {
                RBMConfig.playerPostureMultiplier = 2f;
            }

            if (PostureGUIEnabled.SelectedIndex == 0)
            {
                RBMConfig.postureGUIEnabled = false;
            }
            if (PostureGUIEnabled.SelectedIndex == 1)
            {
                RBMConfig.postureGUIEnabled = true;
            }

            if (VanillaCombatAi.SelectedIndex == 0)
            {
                RBMConfig.vanillaCombatAi = false;
            }
            if (VanillaCombatAi.SelectedIndex == 1)
            {
                RBMConfig.vanillaCombatAi = true;
            }

            if (RBMCombatEnabled.SelectedIndex == 0)
            {
                RBMConfig.rbmCombatEnabled = false;
            }
            if (RBMCombatEnabled.SelectedIndex == 1)
            {
                RBMConfig.rbmCombatEnabled = true;
            }

            if (RBMAIEnabled.SelectedIndex == 0)
            {
                RBMConfig.rbmAiEnabled = false;
            }
            if (RBMAIEnabled.SelectedIndex == 1)
            {
                RBMConfig.rbmAiEnabled = true;
            }

            if (RBMTournamentEnabled.SelectedIndex == 0)
            {
                RBMConfig.rbmTournamentEnabled = false;
            }
            if (RBMTournamentEnabled.SelectedIndex == 1)
            {
                RBMConfig.rbmTournamentEnabled = true;
            }

            RBMConfig.saveXmlConfig();
            //RBMConfig.parseXmlConfig();
            TaleWorlds.ScreenSystem.ScreenManager.PopScreen();
        }

        private void ExecuteCancel()
        {
            TaleWorlds.ScreenSystem.ScreenManager.PopScreen();
        }
    }
}