using System.Drawing;
using System.Windows.Forms;
using PixelMagic.Helpers;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace PixelMagic.Rotation
{
    public class DKFrostMGFmflex : CombatRoutine
    {
        public override string Name
        {
            get { return "Frost DK"; }
        }

        public override string Class
        {
            get { return "Deathknight"; }
        }

        private static readonly Stopwatch coolDownStopWatch = new Stopwatch();
        public override Form SettingsForm { get; set; }
        public SettingsForm2 SettingsForm2 { get; set; }
        public bool useChainofIce = false;

        public override void Initialize()
        {
            WoW.Speak("Welcome to F m Flex Frost");
            Log.Write("Welcome to the Frost DK by Fmflex", Color.Green);
            SettingsForm2 = new SettingsForm2();
            SettingsForm = SettingsForm2;

            SettingsForm2.btnHotkeysFrostOffensiveCooldowns.Text = cooldownHotKeyString;
            SettingsForm2.checkTalentHornOfWinter.Checked = isTalentHornofWinter;
            SettingsForm2.checkTalentFrostScythe.Checked = isTalentFrostscythe;
            SettingsForm2.checkTalentOblitaration.Checked = isTalentOblitaration;
            SettingsForm2.checkTalentBoS.Checked = isTalentBoS;
            SettingsForm2.checkTalentGlacialAdvance.Checked = isTalentGlacialAdvance;

            SettingsForm2.checkHotkeysFrostOffensiveErW.Checked = isCheckHotkeysFrostOffensiveErW;
            SettingsForm2.checkHotkeysFrostOffensivePillarofFrost.Checked = isCheckHotkeysFrostOffensivePillarofFrost;
            SettingsForm2.checkHotkeysFrostAntiMagicShield.Checked = isCheckHotkeysFrostAntiMagicShield;
            SettingsForm2.checkHotkeysFrostIceboundFortitude.Checked = isCheckHotkeysFrostIceboundFortitude;
            SettingsForm2.checkHotkeysFrostIFPercent.Text = FrostIceboundHPPercent.ToString();
            SettingsForm2.checkHotkeysFrostAMSPercent.Text = FrostAMSHPPercent.ToString();
            try
            {
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\KickSettings.ini"))
                    throw new FileNotFoundException();
                using (System.IO.StreamReader file =
                new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\KickSettings.ini"))
                {
                    while (file.Peek() >= 0)
                    {
                        SettingsForm2.spellList.Items.Add(file.ReadLine());
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Log.Write("KickSettings.ini doesn't exist yet");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            SettingsForm2.checkTalentFrostScythe.CheckedChanged += isTalentFrostscythe_Click;
            SettingsForm2.checkTalentHornOfWinter.CheckedChanged += isTalentHornofWinter_Click;
            SettingsForm2.checkTalentOblitaration.CheckedChanged += isTalentOblitaration_Click;
            SettingsForm2.checkTalentBoS.CheckedChanged += isTalentBoS_Click;
            SettingsForm2.checkTalentGlacialAdvance.CheckedChanged += isTalentGlacialAdvance_Click;
            SettingsForm2.checkHotkeysFrostIceboundFortitude.CheckedChanged += isCheckHotkeysFrostIceboundFortitude_Click;
            SettingsForm2.checkHotkeysFrostIFPercent.TextChanged += isCheckHotkeysFrostIFPercent_Click;
            SettingsForm2.checkHotkeysFrostAntiMagicShield.CheckedChanged += isCheckHotkeysFrostAntiMagicShield_Click;
            SettingsForm2.checkHotkeysFrostAMSPercent.TextChanged += isCheckHotkeysFrostAMSPercent_Click;
            SettingsForm2.checkHotkeysFrostOffensivePillarofFrost.CheckedChanged += isCheckHotkeysFrostOffensivePillarofFrost_Click;
            SettingsForm2.checkHotkeysFrostOffensiveErW.CheckedChanged += isCheckHotkeysFrostOffensiveErW_Click;
            SettingsForm2.btnHotkeysFrostOffensiveCooldowns.KeyDown += KeyDown;
            SettingsForm2.btnaddspell.Click += btnaddspell_Click;
            SettingsForm2.btnremovespell.Click += btnremovespell_Click;
        }

        private void btnaddspell_Click(object sender, EventArgs e)
        {
            if(!SettingsForm2.spellText.Text.Equals("") && Regex.IsMatch(SettingsForm2.spellText.Text, @"^\d+$"))
                SettingsForm2.spellList.Items.Add(SettingsForm2.spellText.Text);
            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory+ @"\KickSettings.ini",false))
            {
                foreach (string line in SettingsForm2.spellList.Items)
                {
                        file.WriteLine(line);
                }
            }
        }
        private void btnremovespell_Click(object sender, EventArgs e)
        {
            SettingsForm2.spellList.Items.Remove(SettingsForm2.spellList.SelectedItem);
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\KickSettings.ini", false))
            {
                foreach (string line in SettingsForm2.spellList.Items)
                {
                    file.WriteLine(line);
                }
            }
        }

        void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey)
                return;
            SettingsForm2.btnHotkeysFrostOffensiveCooldowns.Text = "Hotkey : ";
            if (e.Shift)
            {
                cooldownModifier = (int)Keys.ShiftKey;
                SettingsForm2.btnHotkeysFrostOffensiveCooldowns.Text += Keys.Shift + " + ";
            }
            else if (e.Alt)
            {
                cooldownModifier = (int)Keys.Menu;
                SettingsForm2.btnHotkeysFrostOffensiveCooldowns.Text += Keys.Alt + " + ";
            }
            else if (e.Control)
            {
                cooldownModifier = (int)Keys.ControlKey;
                SettingsForm2.btnHotkeysFrostOffensiveCooldowns.Text += Keys.Control + " + ";
            }
            else cooldownModifier = -1;
            cooldownKey = (int)e.KeyCode;
            SettingsForm2.btnHotkeysFrostOffensiveCooldowns.Text +=e.KeyCode;
            cooldownHotKeyString = SettingsForm2.btnHotkeysFrostOffensiveCooldowns.Text;
            SettingsForm2.checkHotkeysFrostIFPercentLabel.Focus();
        }
        private void isCheckHotkeysFrostIceboundFortitude_Click(object sender, EventArgs e)
        {
            isCheckHotkeysFrostIceboundFortitude = SettingsForm2.checkHotkeysFrostIceboundFortitude.Checked;
        }
        private void isCheckHotkeysFrostIFPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsForm2.checkHotkeysFrostIFPercent.Text, out userVal)&& userVal >= 0 && userVal <= 100)
            {
                FrostIceboundHPPercent =userVal;
            }
            else
            {
                SettingsForm2.checkHotkeysFrostIFPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box",Color.DarkRed);
            }
        }
        private void isCheckHotkeysFrostAntiMagicShield_Click(object sender, EventArgs e)
        {
            isCheckHotkeysFrostAntiMagicShield = SettingsForm2.checkHotkeysFrostAntiMagicShield.Checked;
        }
        private void isCheckHotkeysFrostAMSPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsForm2.checkHotkeysFrostAMSPercent.Text, out userVal)&& userVal >= 0 && userVal <= 100)
            {
                    FrostAMSHPPercent = userVal;
               
            }
            else
            {
                SettingsForm2.checkHotkeysFrostAMSPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void isCheckHotkeysFrostOffensivePillarofFrost_Click(object sender, EventArgs e)
        {
            isCheckHotkeysFrostOffensivePillarofFrost = SettingsForm2.checkHotkeysFrostOffensivePillarofFrost.Checked;
        }
        private void isCheckHotkeysFrostOffensiveErW_Click(object sender, EventArgs e)
        {
            isCheckHotkeysFrostOffensiveErW = SettingsForm2.checkHotkeysFrostOffensiveErW.Checked;
        }
        private void isTalentFrostscythe_Click(object sender, EventArgs e)
        {
            isTalentFrostscythe = SettingsForm2.checkTalentFrostScythe.Checked;

        }
        private void isTalentHornofWinter_Click(object sender, EventArgs e)
        {
            isTalentHornofWinter = SettingsForm2.checkTalentHornOfWinter.Checked;
        }
        private void isTalentBoS_Click(object sender, EventArgs e)
        {
            isTalentBoS = SettingsForm2.checkTalentBoS.Checked;
        }
        private void isTalentOblitaration_Click(object sender, EventArgs e)
        {
            isTalentOblitaration = SettingsForm2.checkTalentOblitaration.Checked;
        }
        private void isTalentGlacialAdvance_Click(object sender, EventArgs e)
        {
            isTalentGlacialAdvance = SettingsForm2.checkTalentGlacialAdvance.Checked;
        }

        public override void Stop()
        {
        }

        public static bool CanCastInRange(string spell)
        {
            return WoW.CanCast(spell, true, true, true, false, true);
        }
        public static bool IsMelee()
        {
            return WoW.CanCast("Frost Strike", false, false, true, false, false);
        }

        public override void Pulse()
        {
            if (!coolDownStopWatch.IsRunning ||coolDownStopWatch.ElapsedMilliseconds > 60000)
                coolDownStopWatch.Restart();
            if (DetectKeyPress.GetKeyState(cooldownKey) < 0 && (cooldownModifier==-1 || cooldownModifier != -1 && DetectKeyPress.GetKeyState(cooldownModifier) < 0))
            {
                if (coolDownStopWatch.ElapsedMilliseconds > 1000)
                {
                    combatRoutine.UseCooldowns = !combatRoutine.UseCooldowns;
                    WoW.Speak("Cooldowns " + (combatRoutine.UseCooldowns ? "On" : "Off"));
                    coolDownStopWatch.Restart();
                }
            }
            if (WoW.TargetIsCasting && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= 40 && CanCastInRange("Mind Freeze") && isCastingListedSpell())
            {
                WoW.CastSpell("Mind Freeze");
            }
            if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
            {
                if (WoW.CanCast("Anti-Magic Shell") && WoW.HealthPercent <= FrostAMSHPPercent && !WoW.IsSpellOnCooldown("Anti-Magic Shell") && isCheckHotkeysFrostIceboundFortitude)
                {
                    WoW.CastSpell("Anti-Magic Shell");
                }
                if (WoW.CanCast("Icebound Fortitude") && WoW.HealthPercent < FrostIceboundHPPercent && !WoW.IsSpellOnCooldown("Icebound Fortitude") && isCheckHotkeysFrostAntiMagicShield)
                {
                    WoW.CastSpell("Icebound Fortitude");
                }
                if (useChainofIce && CanCastInRange("ChainofIce") && !IsMelee() && !WoW.TargetHasDebuff("ChainofIce") && WoW.CurrentRunes >= 1)
                {
                    WoW.CastSpell("ChainofIce");
                    return;
                }
                if(isTalentBoS)
                {
                    BreathRotation();
                }
                else
                MGRotation();
            }
        }
        public void BreathRotation()
        {
            if (combatRoutine.UseCooldowns && !WoW.IsSpellOnCooldown("PillarofFrost") && IsMelee() && WoW.PlayerHasBuff("Breath") && isCheckHotkeysFrostOffensivePillarofFrost)
            {
                WoW.CastSpell("PillarofFrost");
            }
            if (combatRoutine.UseCooldowns && !WoW.IsSpellOnCooldown("HEmpower Rune") && WoW.PlayerHasBuff("Breath") && WoW.RunicPower<=30 && isCheckHotkeysFrostOffensiveErW )
            {
                WoW.CastSpell("HEmpower Rune");
            }
            if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave) // Do Single Target Stuff here
            {
                if (combatRoutine.UseCooldowns && WoW.CanCast("Breath") && IsMelee() && WoW.CurrentRunes >= 2 && WoW.RunicPower >= 70)
                {
                    WoW.CastSpell("Breath");
                    return;
                }
                if (combatRoutine.UseCooldowns && WoW.CanCast("Breath") && IsMelee() && WoW.RunicPower >= 70)
                {
                    return;
                }
                if (WoW.CanCast("Howling Blast", true, false, true, false, true) && !WoW.IsSpellOnCooldown("Howling Blast") && !WoW.TargetHasDebuff("Frost Fever")
                    && WoW.CurrentRunes >= 1 && !WoW.PlayerHasBuff("Breath"))
                {
                    WoW.CastSpell("Howling Blast");
                    return;
                }
                if (WoW.CanCast("Frost Strike") && IsMelee() && WoW.RunicPower >= 70 && !WoW.PlayerHasBuff("Breath"))
                {
                    WoW.CastSpell("Frost Strike");
                    return;
                }
                if (WoW.CanCast("Howling Blast", true, false, true, false, true) && WoW.PlayerHasBuff("Rime") && ((WoW.RunicPower>60 && WoW.PlayerHasBuff("Breath")) || !WoW.PlayerHasBuff("Breath")))
                {
                    WoW.CastSpell("Howling Blast");
                    return;
                }
                if (WoW.CanCast("Remorseless Winter") && IsMelee() && WoW.CurrentRunes >= 1 && (!combatRoutine.UseCooldowns ||(combatRoutine.UseCooldowns && WoW.SpellCooldownTimeRemaining("Breath") >= 12)))
                {
                    WoW.CastSpell("Remorseless Winter");
                    return;
                }
                if (WoW.CanCast("Frostscythe") && IsMelee() && WoW.PlayerHasBuff("Killing Machine") && !WoW.PlayerHasBuff("Breath") && WoW.CurrentRunes >= 1 && isTalentFrostscythe)
                {
                    WoW.CastSpell("Frostscythe");
                    return;
                }

                if (WoW.CanCast("Obliterate") && IsMelee() && WoW.CurrentRunes >= 2 && (!WoW.PlayerHasBuff("Breath")|| (WoW.PlayerHasBuff("Breath") && (WoW.RunicPower <=70 ||WoW.PlayerHasBuff("Breath") && WoW.CurrentRunes>3))))
                {
                    WoW.CastSpell("Obliterate");
                    return;
                }
                if (WoW.CanCast("Frost Strike") && IsMelee() && WoW.RunicPower >= 25 && !WoW.PlayerHasBuff("Breath") && (!combatRoutine.UseCooldowns || (combatRoutine.UseCooldowns && WoW.SpellCooldownTimeRemaining("Breath") >= 12)))
                {
                    WoW.CastSpell("Frost Strike");
                    return;
                }
                if (isTalentHornofWinter && !WoW.IsSpellOnCooldown("Horn") && !WoW.PlayerHasBuff("HEmpower Rune") && (WoW.PlayerHasBuff("Breath") ||(!WoW.PlayerHasBuff("Breath") && WoW.SpellCooldownTimeRemaining("Breath")>=30)) && WoW.CurrentRunes <= 4 && WoW.RunicPower <= 70)
                {
                    WoW.CastSpell("Horn");
                }
                if (WoW.CanCast("Death Strike") && IsMelee() && WoW.PlayerHasBuff("Free DeathStrike") && !WoW.PlayerHasBuff("Breath"))
                {
                    WoW.CastSpell("Death Strike");
                    return;
                }
            }
            if (combatRoutine.Type == RotationType.AOE)
            {
                if (combatRoutine.UseCooldowns && WoW.CanCast("Breath") && IsMelee() && WoW.CurrentRunes >= 2 && WoW.RunicPower >= 70)
                {
                    WoW.CastSpell("Breath");
                    return;
                }
                if (combatRoutine.UseCooldowns && WoW.CanCast("Breath") && IsMelee() && WoW.RunicPower >= 70)
                {
                    return;
                }
                if (WoW.CanCast("Howling Blast", true, false, true, false, true) && !WoW.IsSpellOnCooldown("Howling Blast") && !WoW.TargetHasDebuff("Frost Fever")
                    && WoW.CurrentRunes >= 1 && !WoW.PlayerHasBuff("Breath"))
                {
                    WoW.CastSpell("Howling Blast");
                    return;
                }
                if (WoW.CanCast("Frost Strike") && IsMelee() && WoW.RunicPower >= 70 && !WoW.PlayerHasBuff("Breath"))
                {
                    WoW.CastSpell("Frost Strike");
                    return;
                }
                if (WoW.CanCast("Howling Blast", true, false, true, false, true) && WoW.PlayerHasBuff("Rime") && ((WoW.RunicPower >= 60 && WoW.PlayerHasBuff("Breath")) || !WoW.PlayerHasBuff("Breath")))
                {
                    WoW.CastSpell("Howling Blast");
                    return;
                }
                if (WoW.CanCast("Frostscythe") && IsMelee() && WoW.PlayerHasBuff("Killing Machine") && !WoW.PlayerHasBuff("Breath") && WoW.CurrentRunes >= 1 && isTalentFrostscythe)
                {
                    WoW.CastSpell("Frostscythe");
                    return;
                }
                if (WoW.CanCast("Remorseless Winter") && IsMelee() && WoW.CurrentRunes >= 1 && (!combatRoutine.UseCooldowns || (combatRoutine.UseCooldowns && WoW.SpellCooldownTimeRemaining("Breath") >= 12)))
                {
                    WoW.CastSpell("Remorseless Winter");
                    return;
                }
                if (WoW.CanCast("Obliterate") && IsMelee() && WoW.CurrentRunes >= 2 && (!WoW.PlayerHasBuff("Breath") && !isTalentFrostscythe || (WoW.PlayerHasBuff("Breath") && (WoW.RunicPower <= 70 || WoW.PlayerHasBuff("Breath") && WoW.CurrentRunes > 3))))
                {
                    WoW.CastSpell("Obliterate");
                    return;
                }
                if (WoW.CanCast("Frostscythe") && IsMelee() && WoW.CurrentRunes >= 1 && isTalentFrostscythe)
                {
                    WoW.CastSpell("Frostscythe");
                    return;
                }
                if (WoW.CanCast("Frost Strike") && IsMelee() && WoW.RunicPower >= 25 && !WoW.PlayerHasBuff("Breath") && WoW.SpellCooldownTimeRemaining("Breath") >= 12 && (!combatRoutine.UseCooldowns || (combatRoutine.UseCooldowns && WoW.SpellCooldownTimeRemaining("Breath") >= 12)))
                {
                    WoW.CastSpell("Frost Strike");
                    return;
                }
                if (combatRoutine.UseCooldowns && isTalentHornofWinter && !WoW.IsSpellOnCooldown("Horn") && !WoW.PlayerHasBuff("HEmpower Rune") && IsMelee() && WoW.PlayerHasBuff("Breath") && WoW.CurrentRunes <= 4 && WoW.RunicPower <= 70)
                {
                    WoW.CastSpell("Horn");
                }
                if (WoW.CanCast("Death Strike") && IsMelee() && WoW.PlayerHasBuff("Free DeathStrike") && !WoW.PlayerHasBuff("Breath"))
                {
                    WoW.CastSpell("Death Strike");
                    return;
                }
            }
        }
        public void MGRotation()
        {
            if (combatRoutine.UseCooldowns && !WoW.IsSpellOnCooldown("PillarofFrost") && IsMelee() && isCheckHotkeysFrostOffensivePillarofFrost)
            {
                WoW.CastSpell("PillarofFrost");
            }
            if (combatRoutine.UseCooldowns && !WoW.IsSpellOnCooldown("Empower Rune") && IsMelee() && WoW.PlayerHasBuff("PillarofFrost") && WoW.CurrentRunes == 0 && isCheckHotkeysFrostOffensiveErW)
            {
                WoW.CastSpell("Empower Rune");
            }
            if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave) // Do Single Target Stuff here
            {
                if (CanCastInRange("Frost Strike") && IsMelee() && (!WoW.PlayerHasBuff("Icy Talons") || WoW.PlayerBuffTimeRemaining("Icy Talons") <= 1) && WoW.RunicPower >= 25 &&
                    !(combatRoutine.UseCooldowns && WoW.CanCast("Obliteration") && isTalentOblitaration) && (!isTalentOblitaration || (isTalentOblitaration && !WoW.PlayerHasBuff("Obliteration"))))
                {
                    WoW.CastSpell("Frost Strike");
                    return;
                }
                if (WoW.CanCast("Death Strike") && IsMelee() && WoW.HealthPercent <= 50 && WoW.PlayerHasBuff("Free DeathStrike")
                    && (!isTalentOblitaration || (isTalentOblitaration && !WoW.PlayerHasBuff("Obliteration"))))
                {
                    WoW.CastSpell("Death Strike");
                    return;
                }
                if (WoW.CanCast("Howling Blast", true, false, true, false, true) && !WoW.IsSpellOnCooldown("Howling Blast") && !WoW.TargetHasDebuff("Frost Fever")
                    && WoW.CurrentRunes >= 1 && (!isTalentOblitaration || (isTalentOblitaration && !WoW.PlayerHasBuff("Obliteration"))))
                {
                    WoW.CastSpell("Howling Blast");
                    return;
                }
                if (WoW.CanCast("Frost Strike") && IsMelee() && WoW.RunicPower >= 80 && !(combatRoutine.UseCooldowns && WoW.CanCast("Obliteration") && isTalentOblitaration) && (!isTalentOblitaration || (isTalentOblitaration && !WoW.PlayerHasBuff("Obliteration"))))
                {
                    WoW.CastSpell("Frost Strike");
                    return;
                }
                if ((WoW.CanCast("Howling Blast", true, false, true, false, true) && WoW.PlayerHasBuff("Rime"))
                    && (!isTalentOblitaration || (isTalentOblitaration && !WoW.PlayerHasBuff("Obliteration"))))
                {
                    WoW.CastSpell("Howling Blast");
                    return;
                }

                if (combatRoutine.UseCooldowns && WoW.CanCast("Obliteration") && IsMelee() && WoW.CurrentRunes >= 2 && WoW.RunicPower >= 25 && isTalentOblitaration)
                {
                    WoW.CastSpell("Obliteration");
                    return;
                }
                if (WoW.CanCast("Frost Strike") && IsMelee() && WoW.RunicPower >= 25 && isTalentOblitaration && WoW.PlayerHasBuff("Obliteration") && !WoW.PlayerHasBuff("Killing Machine"))
                {
                    WoW.CastSpell("Frost Strike");
                    return;
                }

                if (WoW.CanCast("Obliterate") && IsMelee() && WoW.PlayerHasBuff("Killing Machine") && WoW.CurrentRunes >= 1 && isTalentOblitaration && WoW.PlayerHasBuff("Obliteration"))
                {
                    WoW.CastSpell("Obliterate");
                    return;
                }
                if (WoW.CanCast("Frostscythe") && IsMelee() && WoW.PlayerHasBuff("Killing Machine") && WoW.CurrentRunes >= 1 && isTalentFrostscythe)
                {
                    WoW.CastSpell("Frostscythe");
                    return;
                }

                if (WoW.CanCast("Obliterate") && IsMelee() && WoW.CurrentRunes >= 2)
                {
                    WoW.CastSpell("Obliterate");
                    return;
                }
                if (WoW.CanCast("Glacial Advance") && IsMelee() && WoW.CurrentRunes >= 1 && isTalentGlacialAdvance && (!isTalentOblitaration || (isTalentOblitaration && !WoW.PlayerHasBuff("Obliteration"))))
                {
                    WoW.CastSpell("Glacial Advance");
                    return;
                }
                if (WoW.CanCast("Remorseless Winter") && IsMelee() && WoW.CurrentRunes >= 1 && (!isTalentOblitaration || (isTalentOblitaration && !WoW.PlayerHasBuff("Obliteration"))))
                {
                    WoW.CastSpell("Remorseless Winter");
                    return;
                }
                if (WoW.CanCast("Death Strike") && IsMelee() && WoW.PlayerHasBuff("Free DeathStrike") && (!isTalentOblitaration || (isTalentOblitaration && !WoW.PlayerHasBuff("Obliteration"))))
                {
                    WoW.CastSpell("Death Strike");
                    return;
                }
            }
            if (combatRoutine.Type == RotationType.AOE)
            {
                if (CanCastInRange("Frost Strike") && IsMelee() && (!WoW.PlayerHasBuff("Icy Talons") || WoW.PlayerBuffTimeRemaining("Icy Talons") <= 2)
                    && WoW.RunicPower >= 25)
                {
                    WoW.CastSpell("Frost Strike");
                    return;
                }
                if (WoW.CanCast("Death Strike") && IsMelee() && WoW.HealthPercent <= 50 && WoW.PlayerHasBuff("Free DeathStrike"))
                {
                    WoW.CastSpell("Death Strike");
                    return;
                }
                if (WoW.CanCast("Howling Blast", true, false, true, false, true) && !WoW.IsSpellOnCooldown("Howling Blast") && !WoW.TargetHasDebuff("Frost Fever")
                    && WoW.CurrentRunes >= 1)
                {
                    WoW.CastSpell("Howling Blast");
                    return;
                }
                if ((WoW.CanCast("Howling Blast", true, false, true, false, true) && WoW.PlayerHasBuff("Rime")))
                {
                    WoW.CastSpell("Howling Blast");
                    return;
                }
                if (WoW.CanCast("Remorseless Winter") && IsMelee() && WoW.CurrentRunes >= 1 && !isTalentFrostscythe)
                {
                    WoW.CastSpell("Remorseless Winter");
                    return;
                }
                if (WoW.CanCast("Frost Strike") && IsMelee() && WoW.RunicPower >= 80)
                {
                    WoW.CastSpell("Frost Strike");
                    return;
                }
                if (WoW.CanCast("Obliterate") && IsMelee() && WoW.CurrentRunes >= 2 && !isTalentFrostscythe)
                {
                    WoW.CastSpell("Obliterate");
                    return;
                }
                if (WoW.CanCast("Frostscythe") && IsMelee() && WoW.PlayerHasBuff("Killing Machine") && WoW.CurrentRunes >= 1 && isTalentFrostscythe)
                {
                    WoW.CastSpell("Frostscythe");
                    return;
                }
                if (WoW.CanCast("Glacial Advance") && IsMelee() && WoW.CurrentRunes >= 1 && isTalentGlacialAdvance)
                {
                    WoW.CastSpell("Glacial Advance");
                    return;
                }
                if (WoW.CanCast("Remorseless Winter") && IsMelee() && WoW.CurrentRunes >= 1)
                {
                    WoW.CastSpell("Remorseless Winter");
                    return;
                }
                if (WoW.CanCast("Frostscythe") && IsMelee() && WoW.CurrentRunes >= 1 && isTalentFrostscythe)
                {
                    WoW.CastSpell("Frostscythe");
                    return;
                }

                if (WoW.CanCast("Frost Strike") && IsMelee() && WoW.PlayerBuffStacks("Icy Talons") < 3 && WoW.RunicPower >= 25)
                {
                    WoW.CastSpell("Frost Strike");
                    return;
                }
                if (WoW.CanCast("Death Strike") && IsMelee() && WoW.PlayerHasBuff("Free DeathStrike"))
                {
                    WoW.CastSpell("Death Strike");
                    return;
                }
            }
        }
        public bool isCastingListedSpell()
        {
            foreach (string spellid in SettingsForm2.spellList.Items)
            {
                var spellidint = Int32.Parse(spellid);
                if (WoW.TargetCastingSpellID == spellidint)
                {
                    return true;
                }
            }
            return false;
        }

        public static int cooldownKey
        {
            get
            {
                var cooldownKey = ConfigFile.ReadValue("DKFrost", "cooldownKey").Trim();
                if (cooldownKey != "")
                {
                    return Convert.ToInt32(cooldownKey);
                }

                return -1;
            }
            set
            {
                ConfigFile.WriteValue("DkFrost", "cooldownKey", value.ToString());
            }
        }
        public static int cooldownModifier
        {
            get
            {
                var cooldownModifier = ConfigFile.ReadValue("DKFrost", "cooldownModifier").Trim();
                if (cooldownModifier != "")
                {
                    return Convert.ToInt32(cooldownModifier);
                }

                return -1;
            }
            set
            {
                ConfigFile.WriteValue("DkFrost", "cooldownModifier", value.ToString());
            }
        }

        public static string cooldownHotKeyString
        {
            get
            {
                var cooldownHotKeyString = ConfigFile.ReadValue("DkFrost", "cooldownHotKeyString").Trim();

                if (cooldownHotKeyString != "")
                {
                    return cooldownHotKeyString;
                }

                return "Click to Set";
            }
            set { ConfigFile.WriteValue("DkFrost", "cooldownHotKeyString", value); }
        }
        public static bool isTalentOblitaration
        {
            get
            {
                var isOblitaration = ConfigFile.ReadValue("DkFrost", "isOblitaration").Trim();

                if (isOblitaration != "")
                {
                    return Convert.ToBoolean(isOblitaration);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isOblitaration", value.ToString()); }
        }
        public static bool isTalentBoS
        {
            get
            {
                var isBoS = ConfigFile.ReadValue("DkFrost", "isBoS").Trim();

                if (isBoS != "")
                {
                    return Convert.ToBoolean(isBoS);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isBoS", value.ToString()); }
        }

        public static bool isTalentGlacialAdvance
        {
            get
            {
                var isTalentGlacialAdvance = ConfigFile.ReadValue("DkFrost", "isTalentGlacialAdvance").Trim();

                if (isTalentGlacialAdvance != "")
                {
                    return Convert.ToBoolean(isTalentGlacialAdvance);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isTalentGlacialAdvance", value.ToString()); }
        }

        public static bool isTalentFrostscythe
        {
            get
            {
                var isTalentFrostscythe = ConfigFile.ReadValue("DkFrost", "isTalentFrostscythe").Trim();

                if (isTalentFrostscythe != "")
                {
                    return Convert.ToBoolean(isTalentFrostscythe);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isTalentFrostscythe", value.ToString()); }
        }
        public static bool isTalentHornofWinter
        {
            get
            {
                var isHornofWinter = ConfigFile.ReadValue("DkFrost", "isHornofWinter").Trim();

                if (isHornofWinter != "")
                {
                    return Convert.ToBoolean(isHornofWinter);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isHornofWinter", value.ToString()); }
        }
        public static bool isCheckHotkeysFrostIceboundFortitude
        {
            get
            {
                var isCheckHotkeysFrostIceboundFortitude = ConfigFile.ReadValue("DkFrost", "isCheckHotkeysFrostIceboundFortitude").Trim();

                if (isCheckHotkeysFrostIceboundFortitude != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysFrostIceboundFortitude);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isCheckHotkeysFrostIceboundFortitude", value.ToString()); }
        }

        public static int FrostIceboundHPPercent
        {
            get
            {
                var FrostIceboundHPPercent = ConfigFile.ReadValue("DKFrost", "FrostIceboundHPPercent").Trim();
                if (FrostIceboundHPPercent != "")
                {
                    return Convert.ToInt32(FrostIceboundHPPercent);
                }

                return -1;
            }
            set
            {
                ConfigFile.WriteValue("DkFrost", "FrostIceboundHPPercent", value.ToString());
            }
        }
        public static bool isCheckHotkeysFrostAntiMagicShield
        {
            get
            {
                var isCheckHotkeysFrostAntiMagicShield = ConfigFile.ReadValue("DkFrost", "isCheckHotkeysFrostAntiMagicShield").Trim();

                if (isCheckHotkeysFrostAntiMagicShield != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysFrostAntiMagicShield);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isCheckHotkeysFrostAntiMagicShield", value.ToString()); }
        }

        public static int FrostAMSHPPercent
        {
            get
            {
                var FrostAMSHPPercent = ConfigFile.ReadValue("DKFrost", "FrostAMSHPPercent").Trim();
                if (FrostAMSHPPercent != "")
                {
                    return Convert.ToInt32(FrostAMSHPPercent);
                }

                return -1;
            }
            set
            {
                ConfigFile.WriteValue("DkFrost", "FrostAMSHPPercent", value.ToString());
            }
        }
        public static bool isCheckHotkeysFrostOffensiveErW
        {
            get
            {
                var isCheckHotkeysFrostOffensiveErW = ConfigFile.ReadValue("DkFrost", "isCheckHotkeysFrostOffensiveErW").Trim();

                if (isCheckHotkeysFrostOffensiveErW != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysFrostOffensiveErW);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isCheckHotkeysFrostOffensiveErW", value.ToString()); }
        }
        public static bool isCheckHotkeysFrostOffensivePillarofFrost
        {
            get
            {
                var isCheckHotkeysFrostOffensivePillarofFrost = ConfigFile.ReadValue("DkFrost", "isCheckHotkeysFrostOffensivePillarofFrost").Trim();

                if (isCheckHotkeysFrostOffensivePillarofFrost != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysFrostOffensivePillarofFrost);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isCheckHotkeysFrostOffensivePillarofFrost", value.ToString()); }
        }
    }

    public class SettingsForm2 : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public SettingsForm2()
        {
            this.tabPageKick = new System.Windows.Forms.TabPage();
            this.groupBoxKick = new System.Windows.Forms.GroupBox();
            this.spellList = new System.Windows.Forms.ListBox();
            this.spellText = new System.Windows.Forms.TextBox();
            this.spellIdLabel = new System.Windows.Forms.Label();
            this.btnaddspell = new System.Windows.Forms.Button();
            this.btnremovespell = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.checkTalentHornOfWinter = new System.Windows.Forms.CheckBox();
            this.checkTalentGlacialAdvance = new System.Windows.Forms.CheckBox();
            this.checkTalentBoS = new System.Windows.Forms.CheckBox();
            this.checkTalentOblitaration = new System.Windows.Forms.CheckBox();
            this.checkTalentFrostScythe = new System.Windows.Forms.CheckBox();
            this.checkHotkeysFrostIceboundFortitude = new System.Windows.Forms.CheckBox();
            this.checkHotkeysFrostAntiMagicShield = new System.Windows.Forms.CheckBox();
            this.checkHotkeysFrostIFPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysFrostIFPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysFrostAMSPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysFrostAMSPercentLabel = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.checkHotkeysFrostOffensiveErW = new System.Windows.Forms.CheckBox();
            this.checkHotkeysFrostOffensivePillarofFrost = new System.Windows.Forms.CheckBox();
            this.btnHotkeysFrostOffensiveCooldowns = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPageKick.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabControl3.SuspendLayout();
            // 
            // tabPageKick
            // 
            this.tabPageKick.Controls.Add(this.groupBoxKick);
            this.tabPageKick.Location = new System.Drawing.Point(4, 22);
            this.tabPageKick.Name = "tabPageKick";
            this.tabPageKick.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageKick.Size = new System.Drawing.Size(582, 406);
            this.tabPageKick.TabIndex = 6;
            this.tabPageKick.Text = "Kick Settings";
            this.tabPageKick.UseVisualStyleBackColor = true;
            // 
            // groupBoxKick
            // 
            this.groupBoxKick.Controls.Add(this.spellIdLabel);
            this.groupBoxKick.Controls.Add(this.spellText);
            this.groupBoxKick.Controls.Add(this.spellList);
            this.groupBoxKick.Controls.Add(this.btnaddspell);
            this.groupBoxKick.Controls.Add(this.btnremovespell);
            this.groupBoxKick.Location = new System.Drawing.Point(8, 8);
            this.groupBoxKick.Name = "groupBoxKick";
            this.groupBoxKick.Size = new System.Drawing.Size(561, 380);
            this.groupBoxKick.TabIndex = 2;
            this.groupBoxKick.TabStop = false;
            this.groupBoxKick.Text = "Spells to Kick";
            /*
            this.spellIdLabel = new System.Windows.Forms.Label();
            this.btnaddspell = new System.Windows.Forms.Button();
            this.btnremovespell
            */
            // 
            // btnaddspell
            // 
            this.btnaddspell.Location = new System.Drawing.Point(110, 50);
            this.btnaddspell.Name = "btnaddspell";
            this.btnaddspell.Size = new System.Drawing.Size(28, 48);
            this.btnaddspell.TabIndex = 1;
            this.btnaddspell.Text = "+";
            this.btnaddspell.UseVisualStyleBackColor = true;

            // 
            // btnremovespell
            // 
            this.btnremovespell.Location = new System.Drawing.Point(110, 100);
            this.btnremovespell.Name = "btnremovespell";
            this.btnremovespell.Size = new System.Drawing.Size(28, 48);
            this.btnremovespell.TabIndex = 1;
            this.btnremovespell.Text = "-";
            this.btnremovespell.UseVisualStyleBackColor = true;
            // 
            // spellIdLabel
            // 
            this.spellIdLabel.AutoSize = true;
            this.spellIdLabel.Location = new System.Drawing.Point(28, 28);
            this.spellIdLabel.Name = "spellIdLabel";
            this.spellIdLabel.Size = new System.Drawing.Size(28, 28);
            this.spellIdLabel.TabIndex = 9;
            this.spellIdLabel.Text = "Spell ID:";
            // 
            // spellText
            // 
            this.spellText.AutoSize = true;
            this.spellText.Location = new System.Drawing.Point(28, 50);
            this.spellText.Name = "spellText";
            this.spellText.Size = new System.Drawing.Size(80, 28);
            this.spellText.TabIndex = 9;
            this.spellText.Text = "";

            // 
            // spellList
            // 
            this.spellList.AutoSize = false;
            this.spellList.Location = new System.Drawing.Point(28, 75);
            this.spellList.Name = "spellList";
            this.spellList.Size = new System.Drawing.Size(80, 290);
            this.spellList.TabIndex = 9;
            
            this.spellList.Text = "spellList";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.groupBox12);
            this.tabPage5.Controls.Add(this.groupBox13);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(582, 406);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Hotkeys";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.checkTalentGlacialAdvance);
            this.groupBox22.Controls.Add(this.checkTalentBoS);
            this.groupBox22.Controls.Add(this.checkTalentOblitaration);
            this.groupBox22.Controls.Add(this.checkTalentFrostScythe);
            this.groupBox22.Controls.Add(this.checkTalentHornOfWinter);
            this.groupBox22.Location = new System.Drawing.Point(8, 8);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(561, 120);
            this.groupBox22.TabIndex = 2;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "Frost Talents";
            // 
            // checkTalentGlacialAdvance
            // 
            this.checkTalentGlacialAdvance.AutoSize = true;
            this.checkTalentGlacialAdvance.Location = new System.Drawing.Point(400, 84);
            this.checkTalentGlacialAdvance.Name = "checkTalentGlacialAdvance";
            this.checkTalentGlacialAdvance.Size = new System.Drawing.Size(200, 28);
            this.checkTalentGlacialAdvance.TabIndex = 9;
            this.checkTalentGlacialAdvance.Text = "Glacial Advance";
            this.checkTalentGlacialAdvance.UseVisualStyleBackColor = true;
            // 
            // checkTalentBoS
            // 
            this.checkTalentBoS.AutoSize = true;
            this.checkTalentBoS.Location = new System.Drawing.Point(200, 84);
            this.checkTalentBoS.Name = "checkTalentGlacialAdvance";
            this.checkTalentBoS.Size = new System.Drawing.Size(200, 28);
            this.checkTalentBoS.TabIndex = 9;
            this.checkTalentBoS.Text = "Breath of Sindra";
            this.checkTalentBoS.UseVisualStyleBackColor = true;
            // 
            // checkTalentOblitaration
            // 
            this.checkTalentOblitaration.AutoSize = true;
            this.checkTalentOblitaration.Location = new System.Drawing.Point(8, 84);
            this.checkTalentOblitaration.Name = "checkTalentOblitaration";
            this.checkTalentOblitaration.Size = new System.Drawing.Size(100, 28);
            this.checkTalentOblitaration.TabIndex = 9;
            this.checkTalentOblitaration.Text = "Oblitaration";
            this.checkTalentOblitaration.UseVisualStyleBackColor = true;
            // 
            // checkTalentFrostScythe
            // 
            this.checkTalentFrostScythe.AutoSize = true;
            this.checkTalentFrostScythe.Location = new System.Drawing.Point(8, 56);
            this.checkTalentFrostScythe.Name = "checkTalentFrostScythe";
            this.checkTalentFrostScythe.Size = new System.Drawing.Size(100, 28);
            this.checkTalentFrostScythe.TabIndex = 9;
            this.checkTalentFrostScythe.Text = "Frostscythe";
            this.checkTalentFrostScythe.UseVisualStyleBackColor = true;
            // 
            // checkTalentHornOfWinter
            // 
            this.checkTalentHornOfWinter.AutoSize = true;
            this.checkTalentHornOfWinter.Location = new System.Drawing.Point(8, 28);
            this.checkTalentHornOfWinter.Name = "checkTalentFrostScythe";
            this.checkTalentHornOfWinter.Size = new System.Drawing.Size(100, 28);
            this.checkTalentHornOfWinter.TabIndex = 9;
            this.checkTalentHornOfWinter.Text = "Horn of Winter";
            this.checkTalentHornOfWinter.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.checkHotkeysFrostIceboundFortitude);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostIFPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostIFPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostAntiMagicShield);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostAMSPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostAMSPercentLabel);

            this.groupBox12.Location = new System.Drawing.Point(8, 100);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(561, 80);
            this.groupBox12.TabIndex = 2;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Defensive Cooldowns";
            // 
            // checkHotkeysFrostIceboundFortitude
            // 
            this.checkHotkeysFrostIceboundFortitude.AutoSize = true;
            this.checkHotkeysFrostIceboundFortitude.Location = new System.Drawing.Point(151, 28);
            this.checkHotkeysFrostIceboundFortitude.Name = "checkHotkeysFrostIceboundFortitude";
            this.checkHotkeysFrostIceboundFortitude.Size = new System.Drawing.Size(100, 28);
            this.checkHotkeysFrostIceboundFortitude.TabIndex = 9;
            this.checkHotkeysFrostIceboundFortitude.Text = "Icebound Fortitude";
            this.checkHotkeysFrostIceboundFortitude.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysFrostIFPercent
            // 
            this.checkHotkeysFrostIFPercent.AutoSize = true;
            this.checkHotkeysFrostIFPercent.Location = new System.Drawing.Point(300, 28);
            this.checkHotkeysFrostIFPercent.Name = "checkHotkeysFrostIFPercent";
            this.checkHotkeysFrostIFPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostIFPercent.TabIndex = 9;
            this.checkHotkeysFrostIFPercent.Text = "50";
            // 
            // checkHotkeysFrostIFPercent
            // 
            this.checkHotkeysFrostIFPercentLabel.AutoSize = true;
            this.checkHotkeysFrostIFPercentLabel.Location = new System.Drawing.Point(321, 30);
            this.checkHotkeysFrostIFPercentLabel.Name = "checkHotkeysFrostIFPercentLabel";
            this.checkHotkeysFrostIFPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostIFPercentLabel.TabIndex = 9;
            this.checkHotkeysFrostIFPercentLabel.Text = "% HP";

            // 
            // checkHotkeysFrostAntiMagicShield
            // 
            this.checkHotkeysFrostAntiMagicShield.AutoSize = true;
            this.checkHotkeysFrostAntiMagicShield.Location = new System.Drawing.Point(151, 50);
            this.checkHotkeysFrostAntiMagicShield.Name = "checkHotkeysFrostAntiMagicShield";
            this.checkHotkeysFrostAntiMagicShield.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysFrostAntiMagicShield.TabIndex = 8;
            this.checkHotkeysFrostAntiMagicShield.Text = "Anti-Magic Shield";
            this.checkHotkeysFrostAntiMagicShield.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysFrostAMSPercent
            // 
            this.checkHotkeysFrostAMSPercent.AutoSize = true;
            this.checkHotkeysFrostAMSPercent.Location = new System.Drawing.Point(300, 50);
            this.checkHotkeysFrostAMSPercent.Name = "checkHotkeysFrostIFPercent";
            this.checkHotkeysFrostAMSPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostAMSPercent.TabIndex = 9;
            this.checkHotkeysFrostAMSPercent.Text = "50";
            // 
            // checkHotkeysFrostAMSPercentLabel
            // 
            this.checkHotkeysFrostAMSPercentLabel.AutoSize = true;
            this.checkHotkeysFrostAMSPercentLabel.Location = new System.Drawing.Point(321, 52);
            this.checkHotkeysFrostAMSPercentLabel.Name = "checkHotkeysFrostAMSPercentLabel";
            this.checkHotkeysFrostAMSPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostAMSPercentLabel.TabIndex = 9;
            this.checkHotkeysFrostAMSPercentLabel.Text = "% HP";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.checkHotkeysFrostOffensiveErW);
            this.groupBox13.Controls.Add(this.checkHotkeysFrostOffensivePillarofFrost);
            this.groupBox13.Controls.Add(this.btnHotkeysFrostOffensiveCooldowns);
            this.groupBox13.Location = new System.Drawing.Point(8, 8);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(556, 90);
            this.groupBox13.TabIndex = 3;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Offensive Cooldowns";

            // 
            // checkHotkeysFrostOffensiveErW
            // 
            this.checkHotkeysFrostOffensiveErW.AutoSize = true;
            this.checkHotkeysFrostOffensiveErW.Location = new System.Drawing.Point(151, 60);
            this.checkHotkeysFrostOffensiveErW.Name = "checkHotkeysFrostOffensiveErW";
            this.checkHotkeysFrostOffensiveErW.Size = new System.Drawing.Size(48, 17);
            this.checkHotkeysFrostOffensiveErW.TabIndex = 3;
            this.checkHotkeysFrostOffensiveErW.Text = "ErW";
            this.checkHotkeysFrostOffensiveErW.UseVisualStyleBackColor = true;

            // 
            // checkHotkeysFrostOffensivePillarofFrost
            // 
            this.checkHotkeysFrostOffensivePillarofFrost.AutoSize = true;
            this.checkHotkeysFrostOffensivePillarofFrost.Location = new System.Drawing.Point(151, 32);
            this.checkHotkeysFrostOffensivePillarofFrost.Name = "checkHotkeysFrostOffensivePillarofFrost";
            this.checkHotkeysFrostOffensivePillarofFrost.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeysFrostOffensivePillarofFrost.TabIndex = 2;
            this.checkHotkeysFrostOffensivePillarofFrost.Text = "Pillar of Frost";
            this.checkHotkeysFrostOffensivePillarofFrost.UseVisualStyleBackColor = true;

            // 
            // btnHotkeysFrostOffensiveCooldowns
            // 
            this.btnHotkeysFrostOffensiveCooldowns.Location = new System.Drawing.Point(18, 28);
            this.btnHotkeysFrostOffensiveCooldowns.Name = "btnHotkeysFrostOffensiveCooldowns";
            this.btnHotkeysFrostOffensiveCooldowns.Size = new System.Drawing.Size(113, 23);
            this.btnHotkeysFrostOffensiveCooldowns.TabIndex = 1;
            this.btnHotkeysFrostOffensiveCooldowns.Text = "Click to Set";
            this.btnHotkeysFrostOffensiveCooldowns.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox22);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(582, 406);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Frost Options";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tabPage5);
            this.tabControl3.Controls.Add(this.tabPage2);
            this.tabControl3.Controls.Add(this.tabPageKick);
            this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl3.Location = new System.Drawing.Point(0, 0);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(590, 432);
            this.tabControl3.TabIndex = 0;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 432);
            this.Controls.Add(this.tabControl3);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.tabPageKick.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPageKick;
        private System.Windows.Forms.GroupBox groupBoxKick;
        public System.Windows.Forms.ListBox spellList;
        private System.Windows.Forms.Label spellIdLabel;
        public System.Windows.Forms.Button btnaddspell;
        public System.Windows.Forms.Button btnremovespell;
        public System.Windows.Forms.TextBox spellText;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.GroupBox groupBox12;
        public System.Windows.Forms.CheckBox checkHotkeysFrostIceboundFortitude;
        public System.Windows.Forms.TextBox checkHotkeysFrostIFPercent;
        public System.Windows.Forms.Label checkHotkeysFrostIFPercentLabel;
        public System.Windows.Forms.CheckBox checkHotkeysFrostAntiMagicShield;
        public System.Windows.Forms.TextBox checkHotkeysFrostAMSPercent;
        private System.Windows.Forms.Label checkHotkeysFrostAMSPercentLabel;
        private System.Windows.Forms.GroupBox groupBox13;
        public System.Windows.Forms.CheckBox checkHotkeysFrostOffensiveErW;
        public System.Windows.Forms.CheckBox checkHotkeysFrostOffensivePillarofFrost;
        public System.Windows.Forms.Button btnHotkeysFrostOffensiveCooldowns;
        public System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.GroupBox groupBox22;
        public System.Windows.Forms.CheckBox checkTalentHornOfWinter;
        public System.Windows.Forms.CheckBox checkTalentGlacialAdvance;
        public System.Windows.Forms.CheckBox checkTalentBoS;
        public System.Windows.Forms.CheckBox checkTalentOblitaration;
        public System.Windows.Forms.CheckBox checkTalentFrostScythe;

    }
}

/*
[AddonDetails.db]
AddonAuthor=FmFlex
AddonName=tyahelper
WoWVersion=Legion - 70100
[SpellBook.db]
Spell,49143,Frost Strike,D1
Spell,207230,Frostscythe,D2
Spell,49184,Howling Blast,D3
Spell,49020,Obliterate,D4
Spell,196770,Remorseless Winter,D5
Spell,194913,Glacial Advance,D6
Spell,49998,Death Strike,D8
Spell,48707,Anti-Magic Shell,F4
Spell,48792,Icebound Fortitude,F5
Spell,51271,PillarofFrost,F11
Spell,45524,ChainofIce,F8
Spell,47568,Empower Rune,D7
Spell,207127,HEmpower Rune,D7
Spell,207256,Obliteration,D9
Spell,47528,Mind Freeze,F
Spell,152279,Breath,D0
Spell,57330,Horn,D6
Aura,51124,Killing Machine
Aura,94879,Icy Talons
Aura,55095,Frost Fever
Aura,59057,Rime
Aura,101568,Free DeathStrike
Aura,51271,PillarofFrost
Aura,45524,ChainofIce
Aura,207256,Obliteration
Aura,152279,Breath
Aura,207127,HEmpower Rune
*/
