// winifix@gmail.com
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertPropertyToExpressionBody

using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using PixelMagic.Helpers;
namespace PixelMagic.Rotation
{
    public class HunterBeastmastery : CombatRoutine
    {
        private readonly Stopwatch BeastCleave = new Stopwatch();

        public override string Name
        {
            get
            {
                return "Hunter BM";
            }
        }

        public override string Class
        {
            get
            {
                return "Hunter";
            }
        }

        public override void Initialize()
        {
            Log.Write("Welcome to Beastmastery Hunter", Color.Green);
            Log.Write("IN ORDER FOR THIS ROTATION TO WORK YOU WILL NEED TO DOWNLOAD AND INSTALL THE ADDON.", Color.Red);
            Log.Write("https://github.com/winifix/PixelMagicCR/tree/master/Hunter", Color.Blue);
        }

        public override void Stop()
        {
        }

        public override void Pulse()        // Updated for Legion (tested and working for single target)
        {
            AddonCreationPulse();
            PlayerStats();
            AoEStuff();
            if (WoW.IsInCombat && !WoW.PlayerHasBuff("Mount"))
            {
                SelectRotation();
            }

            //Healthstone - Potion
            if ((WoW.CanCast("Healthstone") || WoW.CanCast("Potion"))
                && (WoW.ItemCount("Healthstone") >= 1 || WoW.ItemCount("Potion") >= 1)
                && (!WoW.ItemOnCooldown("Healthstone") || !WoW.ItemOnCooldown("Potion"))
                && !WoW.PlayerHasBuff("Feign Death")
                && WoW.HealthPercent <= 30
                && !WoW.PlayerHasBuff("Mount"))
            {
                WoW.CastSpell("Healthstone");
                WoW.CastSpell("Potion");
                return;
            }

            //Exhilaration
            if (WoW.CanCast("Exhilaration")
                && WoW.HealthPercent <= 20
                && !WoW.PlayerHasBuff("Mount")
                && !WoW.PlayerHasBuff("Feign Death"))
            {
                WoW.CastSpell("Exhilaration");
                return;
            }

            //Call pet
            if (!WoW.HasPet 
                && !WoW.PlayerHasBuff("Mount")
                && !WoW.PlayerHasBuff("Feign Death"))
            {                
                WoW.CastSpell("Call Pet");
                return;
            }

            //Revive Pet
            if ((!WoW.HasPet || WoW.PetHealthPercent < 1)
                && !WoW.PlayerHasBuff("Mount")
                && !WoW.PlayerHasBuff("Feign Death"))
            {
                WoW.CastSpell("Heart of the Phoenix");
                WoW.CastSpell("Revive Pet");
                return;
            }            

            //Voley
            if (WoW.CanCast("Voley")
                && !WoW.PlayerHasBuff("Feign Death")
                && !WoW.PlayerHasBuff("Voley")
                && CharInfo.T6 == 3)
            {
                WoW.CastSpell("Voley");
                return;
            }

            if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.PlayerHasBuff("Mount") && !WoW.PlayerIsChanneling && !WoW.PlayerIsCasting && !WoW.PlayerHasBuff("Feign Death") && WoW.HealthPercent != 0)
            {
                //Stampede
                if (DetectKeyPress.GetKeyState(DetectKeyPress.Shift) < 0
                    && WoW.CanCast("Stampede")
                    && CharInfo.T7 == 1)
                {
                    WoW.CastSpell("Stampede");
                    return;
                }

                //Intimidation
                if (DetectKeyPress.GetKeyState(DetectKeyPress.Ctrl) < 0                    
                    && ((WoW.CanCast("Intimidation") && CharInfo.T5 == 3) || (WoW.CanCast("Binding Shot") && CharInfo.T5 == 1)))
                {
                    WoW.CastSpell("Binding Shot");
                    WoW.CastSpell("Intimidation");
                    return;
                }

                //Bestial Wrath
                if (WoW.CanCast("Bestial Wrath")
                    && !WoW.PlayerHasBuff("Aspect of the Turtle")
                    && WoW.IsSpellInRange("Cobra Shot")
                    && (WoW.Focus >= 107))
                {
                    WoW.CastSpell("Bestial Wrath");
                    return;
                }

                //Cooldowns
                if (UseCooldowns)
                {   
                    //Aspect of the Wild
                    if (WoW.CanCast("Aspect of the Wild")
                        && !WoW.PlayerHasBuff("Aspect of the Turtle")
                        && WoW.PlayerHasBuff("Bestial Wrath")
                        && WoW.PlayerBuffTimeRemaining("Bestial Wrath") >= 10)
                    {
                        WoW.CastSpell("Aspect of the Wild");
                        return;
                    }                    
                }                

                //SINGLE TARGET
                                       
                        //Legendary Trinket
                        if (WoW.CanCast("Kil'jaeden's Burning Wish")
                            && (((WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 15 && WoW.SpellCooldownTimeRemaining("Dire Beast") > 5)) || WoW.PlayerHasBuff("Bestial Wrath"))
                            && !WoW.ItemOnCooldown("Kil'jaeden's Burning Wish")
                            && WoW.IsSpellInRange("Cobra Shot"))
                        {
                            WoW.CastSpell("Kil'jaeden's Burning Wish");
                        }

                        //Titan's Thunder
                        if (combatRoutine.Type == RotationType.SingleTarget
                            && WoW.CanCast("Titan's Thunder")
                            && (WoW.PlayerBuffTimeRemaining("Dire Beast") >= 7)
                            && (WoW.PlayerHasBuff("Dire Beast") || CharInfo.T2 == 2))
                        {
                            WoW.CastSpell("Titan's Thunder");
                            return;
                        }

                        //A Murder of Crows
                        if (combatRoutine.Type == RotationType.SingleTarget
                            && WoW.SpellCooldownTimeRemaining("Bestial Wrath") >= 10
                            && WoW.CanCast("A Murder of Crows")
                            && WoW.IsSpellInRange("Cobra Shot")
                            && CharInfo.T6 == 1
                            && WoW.Focus >= 30)                            
                        {
                            WoW.CastSpell("A Murder of Crows");
                            return;
                        } 

                        // Dire beast
                        if (combatRoutine.Type == RotationType.SingleTarget
                            && WoW.CanCast("Dire Beast")
                            && WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 3
                            && WoW.IsSpellInRange("Cobra Shot")
 
                            && CharInfo.T2 != 2)
                        {
                            WoW.CastSpell("Dire Beast");
                            return;
                        }

                        //Dire Frenzy
                        if (combatRoutine.Type == RotationType.SingleTarget
                            && WoW.CanCast("Dire Frenzy")
                            && WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 6
                            && WoW.IsSpellInRange("Cobra Shot")
                            && CharInfo.T2 == 2)
                        {
                            WoW.CastSpell("Dire Frenzy");
                            return;
                        }                        

                        //Kill Command
                        if (combatRoutine.Type == RotationType.SingleTarget
                            && (WoW.CanCast("Kill Command") || WoW.SpellCooldownTimeRemaining("Kill Command") <= 0.7)
                            && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") > WoW.SpellCooldownTimeRemaining("Kill Command") || (WoW.SpellCooldownTimeRemaining("Bestial Wrath") >= 23 && WoW.SpellCooldownTimeRemaining("Dire Beast") > WoW.SpellCooldownTimeRemaining("Kill Command")))
                            && WoW.Focus >= 30)
                        {
                            WoW.CastSpell("Kill Command");
                            return;
                        }                        

                        //Chimaera Shot
                        if (combatRoutine.Type == RotationType.SingleTarget
                            && WoW.CanCast("Chimaera Shot")
                            && WoW.IsSpellInRange("Cobra Shot")
                            && CharInfo.T2 == 3
                            && WoW.Focus < 90)
                        {
                            WoW.CastSpell("Chimaera Shot");
                            return;
                        }

                        //Cobra Shot
                        if (combatRoutine.Type == RotationType.SingleTarget
                            && ((WoW.Focus >= 100) || (WoW.PlayerHasBuff("Bestial Wrath") && (WoW.Focus >= 40)))
                            && WoW.IsSpellInRange("Cobra Shot")
                            && WoW.CanCast("Cobra Shot")
                            && !WoW.CanCast("Bestial Wrath"))
                        {
                            WoW.CastSpell("Cobra Shot");
                            return;
                        }

                //AOE

                    //Bestial Wrath
                    if (combatRoutine.Type == RotationType.AOE
                        && WoW.CanCast("Bestial Wrath")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && !WoW.PlayerHasBuff("Aspect of the Turtle"))
                    {
                        WoW.CastSpell("Bestial Wrath");
                        return;
                    }

                    //Titan's Thunder
                    if (combatRoutine.Type == RotationType.AOE
                        && WoW.CanCast("Titan's Thunder")
                        && (WoW.PlayerBuffTimeRemaining("Dire Beast") >= 7)
                        && (WoW.PlayerHasBuff("Dire Beast") || CharInfo.T2 == 2))
                    {
                        WoW.CastSpell("Titan's Thunder");
                        return;
                    }

                    // Dire beast
                    if (combatRoutine.Type == RotationType.AOE
                        && WoW.CanCast("Dire Beast")
                        && WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 3
                        && WoW.IsSpellInRange("Cobra Shot")
                        && CharInfo.T2 != 2)
                    {
                        WoW.CastSpell("Dire Beast");
                        return;
                    }

                    //Dire Frenzy
                    if (combatRoutine.Type == RotationType.AOE
                        && WoW.CanCast("Dire Frenzy")
                        && WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 6
                        && WoW.IsSpellInRange("Cobra Shot")
                        && CharInfo.T2 == 2)
                    {
                        WoW.CastSpell("Dire Frenzy");
                        return;
                    }

                    //Barrage
                    if (combatRoutine.Type == RotationType.AOE
                        && WoW.CanCast("Barrage")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && CharInfo.T6 == 2
                        && (WoW.Focus >= 60))
                    {
                        WoW.CastSpell("Barrage");
                        return;
                    }                    

                    //Multishot
                    if (combatRoutine.Type == RotationType.AOE
                        && WoW.CanCast("Multi-Shot")
                        && WoW.IsSpellInRange("Multi-Shot")
                        && WoW.Focus >= 40)
                    {
                        WoW.CastSpell("Multi-Shot");
                        return;
                    }

                    //Chimaera Shot
                    if (combatRoutine.Type == RotationType.AOE
                        && WoW.CanCast("Chimaera Shot")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && CharInfo.T2 == 3
                        && WoW.Focus < 90)
                    {
                        WoW.CastSpell("Chimaera Shot");
                        return;
                    }

                //CLEAVE

                    //Titan's Thunder
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.CanCast("Titan's Thunder")
                        && (WoW.PlayerBuffTimeRemaining("Dire Beast") >= 7)
                        && (WoW.PlayerHasBuff("Dire Beast") || CharInfo.T2 == 2))
                    {
                        WoW.CastSpell("Titan's Thunder");
                        return;
                    }

                    //A Murder of Crows
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.SpellCooldownTimeRemaining("Bestial Wrath") >= 10
                        && WoW.CanCast("A Murder of Crows")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && CharInfo.T6 == 1
                        && WoW.Focus >= 30)
                    {
                        WoW.CastSpell("A Murder of Crows");
                        return;
                    }

                    //Multishot - Beast Cleave uptime
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.CanCast("Multi-Shot")
                        && (!WoW.PetHasBuff("BeastCleave") || WoW.PetBuffTimeRemaining("BeastCleave") <= 1)
                        && WoW.IsSpellInRange("Multi-Shot")
                        && !WoW.CanCast("Bestial Wrath")                       
                        && WoW.Focus >= 40)  
                    {
                        WoW.CastSpell("Multi-Shot");                        
                        return;
                    }

                    // Dire beast
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.CanCast("Dire Beast")
                        && WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 3
                        && WoW.IsSpellInRange("Cobra Shot")
                        && CharInfo.T2 != 2)
                    {
                        WoW.CastSpell("Dire Beast");
                        return;
                    }

                    //Dire Frenzy
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.CanCast("Dire Frenzy")
                        && WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 6
                        && WoW.IsSpellInRange("Cobra Shot")
                        && CharInfo.T2 == 2)
                    {
                        WoW.CastSpell("Dire Frenzy");
                        return;
                    }

                    //Barrage
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.CanCast("Barrage")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && CharInfo.T6 == 2
                        && (WoW.Focus >= 60))
                    {
                        WoW.CastSpell("Barrage");
                        return;
                    }

                    //Kill Command
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && (WoW.CanCast("Kill Command") || WoW.SpellCooldownTimeRemaining("Kill Command") <= 0.7)
                        && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") > WoW.SpellCooldownTimeRemaining("Kill Command") || (WoW.SpellCooldownTimeRemaining("Bestial Wrath") >= 23 && WoW.SpellCooldownTimeRemaining("Dire Beast") > WoW.SpellCooldownTimeRemaining("Kill Command")))
                        && WoW.Focus >= 30)
                    {
                        WoW.CastSpell("Kill Command");
                        return;
                    }                    

                    //Chimaera Shot
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.CanCast("Chimaera Shot")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && CharInfo.T2 == 3
                        && WoW.Focus < 90)
                    {
                        WoW.CastSpell("Chimaera Shot");
                        return;
                    }

                    //Cobra Shot
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && ((WoW.Focus >= 100) || (WoW.PlayerHasBuff("Bestial Wrath") && (WoW.Focus >= 40)))
                        && WoW.IsSpellInRange("Cobra Shot")
                        && WoW.CanCast("Cobra Shot")
                        && !WoW.CanCast("Bestial Wrath"))
                    {
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }

                    //Mend Pet
                    if (WoW.HasPet
                        && WoW.CanCast("Mend Pet")
                        && WoW.PetHealthPercent <= 70
                        && !WoW.PlayerHasBuff("Feign Death"))
                    {
                        WoW.CastSpell("Mend Pet");
                        return;
                    }
            }
        }


        public struct char_data
        {
            public int T1;
            public int T2;
            public int T3;
            public int T4;
            public int T5;
            public int T6;
            public int T7;
            public float Mana;
            public string Spec;
            public string Race;

            private char_data(int p1, int p2, int p3, int p4, int p5, int p6, int p7, float mana, string spec, string race)
            {
                T1 = p1;
                T2 = p2;
                T3 = p3;
                T4 = p4;
                T5 = p5;
                T6 = p6;
                T7 = p7;
                Mana = mana;
                Spec = spec;
                Race = race;
            }
        }

        public string[] Race = new string[] { "None", "Human", "Dwarf", "NightElf", "Gnome", "Dreanei", "Pandaren", "Orc", "Undead", "Tauren", "Troll", "BloodElf", "Goblin", "none" };
        public string[] Spec = new string[] { "None", "Blood", "Frost", "Unholy", "Havoc", "Vengeance", "Balance", "Feral", "Guardian", "Restoration", "Beast Mastery", "Marksmanship", "Survival", "Arcane", "Fire", "Frost", "Brewmaster", "Mistweaver", "Windwalker", "Holy", "Protection", "Retribution", "Discipline", "HolyPriest", "Shadow", "Assassination", "Outlaw", "Subtlety", "Elemental", "Enhancement", "RestorationShaman", "Affliction", "Arms", "Fury", "Protection", "none" };
        private int npcCount, players;
        private bool Nameplates = false;
        private Color pixelColor = Color.FromArgb(0);
        private double hastePct;
        private char_data CharInfo = new char_data();
        private bool AddonEmbeded = false;
        private bool RangeLib = false;

        private void PlayerStats()
        {
            // Playerstats start at row 1,  column 21
            // t1 t2 t3
            // t4 t5 t7
            // t7 +-haste hastePCT
            // Spec, Mana, Race
            int postive = 0;
            if ((Convert.ToDouble(pixelColor.R) == 255))
                hastePct = 0f;
            else
                hastePct = (Convert.ToSingle(pixelColor.R) * 100f / 255f);
            int spec, race;
            pixelColor = WoW.GetBlockColor(1, 21);
            CharInfo.T1 = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.R) * 100 / 255));
            CharInfo.T2 = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.G) * 100 / 255));
            CharInfo.T3 = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.B) * 100 / 255));
            pixelColor = WoW.GetBlockColor(2, 21);
            CharInfo.T4 = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.R) * 100 / 255));
            CharInfo.T5 = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.G) * 100 / 255));
            CharInfo.T6 = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.B) * 100 / 255));
            pixelColor = WoW.GetBlockColor(3, 21);
            CharInfo.T7 = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.R) * 100 / 255));
            spec = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.G) * 100 / 255));
            race = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.B) * 100 / 255));
            pixelColor = WoW.GetBlockColor(4, 21);
            CharInfo.Mana = (Convert.ToSingle(pixelColor.B) * 100 / 255);
            postive = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.G) / 255));
            if ((Convert.ToDouble(pixelColor.B) == 255))
                hastePct = 0f;
            else
                if (postive == 1)
                    hastePct = (Convert.ToSingle(pixelColor.R) * 100f / 255f) * (-1);
                else
                    hastePct = (Convert.ToSingle(pixelColor.G) * 100f / 255f);
            if (race > 13)
                race = 0;
            if (spec > 34)
                spec = 0;
            //Log.Write ("Char Race :" + race + " Spec : " + spec);
            CharInfo.Race = Race[race];
            CharInfo.Spec = Spec[spec];
            Log.Write(" T1 " + CharInfo.T1 + " T2 " + CharInfo.T2 + " T3 " + CharInfo.T3 + " T4 " + CharInfo.T4 + " T5 " + CharInfo.T5 + " T6 " + CharInfo.T6 + " T7 " + CharInfo.T7);
            //Log.Write("Char Haste " + hastePct + " Mana :" + CharInfo.Mana + " Race : " +CharInfo.Race + " Spec : "  +CharInfo.Spec ) ;

        }

        private void AoEStuff()
        {
            Color pixelColor = Color.FromArgb(0);
            pixelColor = WoW.GetBlockColor(11, 20);
            npcCount = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.G) * 100 / 255));
            if (Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.B) / 255)) == 1)
                Nameplates = true;
            else
                Nameplates = false;
        }

        private void SelectRotation()
        {  
            if (Nameplates)
            {
                if (npcCount >= 4 && !WoW.TargetIsPlayer)
                    combatRoutine.ChangeType(RotationType.AOE);
                if ((npcCount == 2 || npcCount == 3) && !WoW.TargetIsPlayer)
                    combatRoutine.ChangeType(RotationType.SingleTargetCleave);
                if (npcCount <= 1)
                    combatRoutine.ChangeType(RotationType.SingleTarget);
            }
        }

        private bool AddonEdited = false;
        private static string AddonName = ConfigFile.ReadValue("PixelMagic", "AddonName");
        private static string AddonEmbedName = "BossLib.xml";// Initialization variables		
        private static string LuaAddon = "Shaman.lua";
        public static string CustomLua
        {
            get
            {
                var customLua = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + LuaAddon);
                return customLua;
            }
        }
        public void RangeLibCopy()
        {
            try
            {
                string fileName = "text.txt";
                string sourcePath = string.Concat(AppDomain.CurrentDomain.BaseDirectory + "LibSpellRange-1.0\\");
                string sourcePathSub = string.Concat(AppDomain.CurrentDomain.BaseDirectory + "LibSpellRange-1.0\\lib\\LibStub\\");
                string targetPath = string.Concat("" + WoW.AddonPath + "\\" + AddonName + "\\lib\\LibSpellRange-1.0\\");
                string targetPathSub = string.Concat("" + WoW.AddonPath + "\\" + AddonName + "\\lib\\LibSpellRange-1.0\\lib\\LibStub\\");
                string destFile = "text.txt";

                // To copy a folder's contents to a new location:
                // Create a new target folder, if necessary.
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                    Log.Write("Base target:" + targetPath);
                }
                if (!Directory.Exists(targetPathSub))
                {
                    Log.Write("Sub target:" + targetPathSub);
                    Directory.CreateDirectory(targetPathSub);
                }
                if (!Directory.Exists(sourcePath))
                    Log.Write("Dirctory doesn't exist:" + sourcePath);
                if (!Directory.Exists(sourcePathSub))
                    Log.Write("Dirctory doesn't exist:" + sourcePathSub);
                if (Directory.Exists(sourcePath))
                {
                    string[] files = Directory.GetFiles(sourcePath);
                    foreach (string s in files)
                    {
                        Log.Write("Generating file" + s);
                        fileName = Path.GetFileName(s);
                        destFile = Path.Combine(targetPath, fileName);
                        File.Copy(s, destFile, true);
                    }
                }
                if (Directory.Exists(sourcePathSub))
                {
                    string[] files = Directory.GetFiles(sourcePathSub);

                    foreach (string s in files)
                    {
                        Log.Write("Generating Sub file" + s);
                        fileName = Path.GetFileName(s);
                        destFile = Path.Combine(targetPathSub, fileName);
                        File.Copy(s, destFile, true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex, "PixelMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Thread.Sleep(2000);
            RangeLib = true;
        }

        private void AddonEmbedEdit()
        {
            try
            {
                string addonlua = File.ReadAllText("" + WoW.AddonPath + "\\" + AddonName + "\\" + AddonEmbedName);
                Log.Write("Addon emedding Editing in progress");
                addonlua = addonlua.Replace("</Ui>",
                                            "<Script file=\"lib\\LibSpellRange-1.0\\LibSpellRange-1.0.lua\"/>"
                                            + Environment.NewLine + "<Script file = \"lib\\LibSpellRange-1.0\\LibSpellRange-1.0.xml\" />"
                                            + Environment.NewLine + "</Ui>");
                File.WriteAllText("" + WoW.AddonPath + "\\" + AddonName + "\\" + AddonEmbedName, addonlua);
                Log.Write("Addon Embedding complete");
                Thread.Sleep(2000);
                while (WoW.HealthPercent == 0 || WoW.HastePercent == 0)
                {
                    Thread.Sleep(2000);
                }
                AddonEmbeded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex, "PixelMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void AddonCreationPulse()
        {

            // Editing the addon
            if (AddonEdited == false)
            {
                Log.Write("Editing Addon");
                AddonEdit();
                Log.Write("Editing Addon Complete");
                Thread.Sleep(2000);

            }
            if (AddonEmbeded == false)
            {
                Log.Write("embedingin rangecheck");
                AddonEmbedEdit();
                Log.Write("embedingin RangeCheck Complete");
                Thread.Sleep(2000);
            }
            if (RangeLib == false)
            {
                RangeLibCopy();
                WoW.SendMacro("/reload");
            }
            Thread.Sleep(350);
        }
        private void AddonEdit()
        {
            try
            {
                string addonlua = File.ReadAllText("" + WoW.AddonPath + "\\" + AddonName + "\\" + AddonName + ".lua");
                Log.Write("Addon Editing in progress");
                addonlua = addonlua.Replace("local lastCombat = nil" + Environment.NewLine + "local alphaColor = 1", "local lastCombat = nil" + Environment.NewLine + "local alphaColor = 1" + Environment.NewLine + CustomLua);
                addonlua = addonlua.Replace("InitializeOne()" + Environment.NewLine + "            InitializeTwo()", "InitializeOne()" + Environment.NewLine + "            InitializeTwo()" + Environment.NewLine + "            InitializeThree()");
                File.WriteAllText("" + WoW.AddonPath + "\\" + AddonName + "\\" + AddonName + ".lua", addonlua);
                //WoW.SendMacro("/reload");
                while (WoW.HealthPercent == 0 || WoW.HastePercent == 0)
                {
                    Thread.Sleep(25);
                }
                AddonEdited = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex, "PixelMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        public override Form SettingsForm { get; set; }
        public class DetectKeyPress
        {
            public static int Num1 = 0x31;
            public static int Num2 = 0x32;
            public static int Num3 = 0x33;
            public static int Num4 = 0x34;
            public static int Num5 = 0x35;
            public static int Num6 = 0x36;
            public static int Numpad0 = 0x60;
            public static int Numpad1 = 0x61;
            public static int Numpad2 = 0x62;
            public static int Numpad3 = 0x63;
            public static int Numpad4 = 0x64;
            public static int Numpad5 = 0x65;
            public static int Numpad6 = 0x66;
            public static int Numpad7 = 0x67;
            public static int Numpad8 = 0x68;
            public static int Numpad9 = 0x69;
            public static int NumpadDot = 0x6E;
            public static int NumpadADD = 0x6B;

            public static int Shift = 0x10;
            public static int Ctrl = 0x11;
            public static int Alt = 0x12;

            public static int Z = 0x5A;
            public static int X = 0x58;
            public static int C = 0x43;
            public static int V = 0x56;
            public static int Slash = 0xDC;

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            internal static extern short GetKeyState(int virtualKeyCode);
        }
    }
}

/*
[AddonDetails.db]
AddonAuthor=Sorcerer
AddonName=
WoWVersion=Legion - 70100
[SpellBook.db]
Spell,136,Mend Pet,D8
Spell,982,Revive Pet,D8
Spell,883,Call Pet,D7
Spell,5512,Healthstone,D1
Spell,127834,Potion,D1
Spell,19577,Intimidation,D9
Spell,109248,Binding Shot,D9
Spell,193455,Cobra Shot,NumPad2
Spell,109304,Exhilaration,NumPad7
Spell,120679,Dire Beast,NumPad1
Spell,217200,Dire Frenzy,NumPad1
Spell,34026,Kill Command,NumPad3
Spell,131894,A Murder of Crows,NumPad4
Spell,120360,Barrage,NumPad4
Spell,194386,Voley,NumPad4
Spell,2643,Multi-Shot,NumPad5
Spell,207068,Titan's Thunder,Add
Spell,19574,Bestial Wrath,NumPad9
Spell,55709,Heart of the Phoenix,D8
Spell,144259,Kil'jaeden's Burning Wish,D2
Spell,193530,Aspect of the Wild,Divide
Spell,53209,Chimaera Shot,D0
Spell,201430,Stampede,NumPad6
Aura,120694,Dire Beast
Aura,5384,Feign Death
Aura,19574,Bestial Wrath
Aura,127271,Mount
Aura,186265,Aspect of the Turtle
Aura,118455,BeastCleave
Aura,194386,Voley
Item,5512,Healthstone
Item,127834,Potion
Item,144259,Kil'jaeden's Burning Wish
*/