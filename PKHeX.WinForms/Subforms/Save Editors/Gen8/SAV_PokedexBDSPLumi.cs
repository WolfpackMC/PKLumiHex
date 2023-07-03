using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;
public partial class SAV_PokedexBDSPLumi : Form
{
    private readonly SaveFile Origin;
    private readonly SAV8BSLuminescent SAV;
    private readonly Zukan8bLumi Zukan;

    public SAV_PokedexBDSPLumi(SaveFile sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV8BSLuminescent)(Origin = sav).Clone();
        Zukan = SAV.Zukan;

        editing = true;
        // Clear Listbox and ComboBox
        LB_Species.Items.Clear();
        CB_Species.Items.Clear();

        // Fill List
        CB_Species.InitializeBinding();
        CB_Species.DataSource = new BindingSource(GameInfo.FilteredSources.Species.Skip(1).ToList(), null);

        for (int i = 1; i <= SAV.MaxSpeciesID; i++)
            LB_Species.Items.Add($"{i:000} - {GameInfo.Strings.specieslist[i]}");

        editing = false;
        LB_Species.SelectedIndex = 0;
        CB_Species.KeyDown += WinFormsUtil.RemoveDropCB;
        CHK_National.Checked = Zukan.HasNationalDex;
    }

    private bool editing;
    private ushort species = ushort.MaxValue;

    private void ChangeCBSpecies(object sender, EventArgs e)
    {
        if (editing) return;
        SetEntry();

        editing = true;
        species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        LB_Species.SelectedIndex = species - 1;
        LB_Species.TopIndex = LB_Species.SelectedIndex;
        GetEntry();
        B_ModifyForms.Enabled = (Zukan8b.GetFormCount(species) != 0);
        mnuMFAllLang.Enabled = !(species > 493);
        editing = false;
    }

    private void ChangeLBSpecies(object sender, EventArgs e)
    {
        if (editing) return;
        SetEntry();

        editing = true;
        species = (ushort)(LB_Species.SelectedIndex + 1);
        CB_Species.SelectedValue = species;
        GetEntry();
        B_ModifyForms.Enabled = (Zukan8b.GetFormCount(species) != 0);
        mnuMFAllLang.Enabled = !(species > 493);
        editing = false;
    }

    private void GetEntry()
    {
        if (species > 1010) return;

        CB_State.SelectedIndex = (int)Zukan.GetState(species);

        // Lang flags in 1.3.0 Lumi Revision 1 Save hasn't been changed to bitfields
        GB_Language.Visible = (uint)species <= 493;

        Zukan.GetGenderFlags(species, out var m, out var f, out var ms, out var fs);
        CHK_M.Checked = m;
        CHK_F.Checked = f;
        CHK_MS.Checked = ms;
        CHK_FS.Checked = fs;

        if (species > 493) return;

        CHK_LangJPN.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.Japanese);
        CHK_LangENG.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.English);
        CHK_LangFRE.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.French);
        CHK_LangITA.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.Italian);
        CHK_LangGER.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.German);
        CHK_LangSPA.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.Spanish);
        CHK_LangKOR.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.Korean);
        CHK_LangCHS.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.ChineseS);
        CHK_LangCHT.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.ChineseT);

        var f1 = CLB_FormRegular;
        var f2 = CLB_FormShiny;
        f1.Items.Clear();
        f2.Items.Clear();
        var fc = Zukan8b.GetFormCount(species);
        if (fc <= 0)
            return;

        var forms = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Context).Take(fc).ToArray();
        f1.Items.AddRange(forms);
        f2.Items.AddRange(forms);
        for (byte i = 0; i < f1.Items.Count; i++)
        {
            f1.SetItemChecked(i, Zukan.GetHasFormFlag(species, i, false));
            f2.SetItemChecked(i, Zukan.GetHasFormFlag(species, i, true));
        }
    }

    private void SetEntry()
    {
        if (species > 1010) return;

        Zukan.SetState(species, (ZukanState8b)CB_State.SelectedIndex);

        Zukan.SetGenderFlags(species, CHK_M.Checked, CHK_F.Checked, CHK_MS.Checked, CHK_FS.Checked);

        if (species > 493) return;

        Zukan.SetLanguageFlag(species, (int)LanguageID.Japanese, CHK_LangJPN.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.English, CHK_LangENG.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.French, CHK_LangFRE.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.Italian, CHK_LangITA.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.German, CHK_LangGER.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.Spanish, CHK_LangSPA.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.Korean, CHK_LangKOR.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.ChineseS, CHK_LangCHS.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.ChineseT, CHK_LangCHT.Checked);

        for (byte i = 0; i < CLB_FormRegular.Items.Count; i++)
        {
            Zukan.SetHasFormFlag(species, i, false, CLB_FormRegular.GetItemChecked(i));
            Zukan.SetHasFormFlag(species, i, true, CLB_FormShiny.GetItemChecked(i));
        }
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SetEntry();
        Zukan.HasNationalDex = CHK_National.Checked;
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Modify_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
    }

    private void B_ModifyForms_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        modifyMenuForms.Show(btn.PointToScreen(new Point(0, btn.Height)));
    }

    private void B_ModifyEntry_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        modifyMenuEntry.Show(btn.PointToScreen(new Point(0, btn.Height)));
    }

    private void ModifyAll(object sender, EventArgs e)
    {
        SetEntry();

        if (sender == mnuSeenNone)
            Zukan.SetAllSeen(false);
        else if (sender == mnuSeenAll)
            Zukan.SetAllSeen(shinyToo: ModifierKeys == Keys.Control);
        else if (sender == mnuCaughtNone)
            Zukan.CaughtNone();
        else if (sender == mnuCaughtAll)
            Zukan.CaughtAll(ModifierKeys == Keys.Control);
        else if (sender == mnuComplete)
            Zukan.SetDexEntryAll(ModifierKeys == Keys.Control);

        GetEntry();
    }

    private void ModifyAllForms(object sender, EventArgs e)
    {
        var f1 = CLB_FormRegular;
        var f2 = CLB_FormShiny;
        if (sender == mnuFormAllRegular)
        {
            for (int i = 0; i < f1.Items.Count; i++)
            {
                f1.SetItemChecked(i, true);
                f2.SetItemChecked(i, ModifierKeys == Keys.Control);
            }
        }
        else
        {
            for (int i = 0; i < f1.Items.Count; i++)
            {
                f1.SetItemChecked(i, false);
                f2.SetItemChecked(i, false);
            }
        }
    }

    private void ModifyEntry(object sender, EventArgs e)
    {
        if (sender == mnuMFCurLang)
        {
            CHK_M.Checked = !Zukan.OnlyFemale(species);
            CHK_F.Checked = !Zukan.OnlyMale(species);
            CHK_MS.Checked = (!Zukan.OnlyFemale(species) && (ModifierKeys == Keys.Control));
            CHK_FS.Checked = (!Zukan.OnlyMale(species) && (ModifierKeys == Keys.Control));

            CB_State.SelectedIndex = (int)ZukanState8b.Caught;

            if (species > 493) return;

            CHK_LangJPN.Checked = ((int)LanguageID.Japanese == SAV.Language);
            CHK_LangENG.Checked = ((int)LanguageID.English == SAV.Language);
            CHK_LangFRE.Checked = ((int)LanguageID.French == SAV.Language);
            CHK_LangITA.Checked = ((int)LanguageID.Italian == SAV.Language);
            CHK_LangGER.Checked = ((int)LanguageID.German == SAV.Language);
            CHK_LangSPA.Checked = ((int)LanguageID.Spanish == SAV.Language);
            CHK_LangKOR.Checked = ((int)LanguageID.Korean == SAV.Language);
            CHK_LangCHS.Checked = ((int)LanguageID.ChineseS == SAV.Language);
            CHK_LangCHT.Checked = ((int)LanguageID.ChineseT == SAV.Language);
        }

        if (sender == mnuMFAllLang)
        {
            CB_State.SelectedIndex = (int)ZukanState8b.Caught;

            CHK_M.Checked = !Zukan.OnlyFemale(species);
            CHK_F.Checked = !Zukan.OnlyMale(species);
            CHK_MS.Checked = (!Zukan.OnlyFemale(species) && (ModifierKeys == Keys.Control));
            CHK_FS.Checked = (!Zukan.OnlyMale(species) && (ModifierKeys == Keys.Control));

            CHK_LangJPN.Checked = CHK_LangENG.Checked = CHK_LangFRE.Checked = CHK_LangGER.Checked = CHK_LangITA.Checked = true;
            CHK_LangSPA.Checked = CHK_LangKOR.Checked = CHK_LangCHS.Checked = CHK_LangCHT.Checked = true;
        }

        if (sender == mnuClearEntry)
        {
            CHK_M.Checked = CHK_F.Checked = CHK_MS.Checked = CHK_FS.Checked = false;

            CB_State.SelectedIndex = (int)ZukanState8b.None;

            if (species > 493) return;

            for (int i = 0; i < CLB_FormRegular.Items.Count; i++)
            {
                CLB_FormRegular.SetItemChecked(i, false);
                CLB_FormShiny.SetItemChecked(i, false);
            }

            CHK_LangJPN.Checked = CHK_LangENG.Checked = CHK_LangFRE.Checked = CHK_LangGER.Checked = CHK_LangITA.Checked = false;
            CHK_LangSPA.Checked = CHK_LangKOR.Checked = CHK_LangCHS.Checked = CHK_LangCHT.Checked = false;
        }
    }

    private void dexTips_Popup(object sender, PopupEventArgs e) { }
}
