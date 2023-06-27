using System;
using System.Diagnostics.Metrics;
using System.Reflection;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used for Brilliant Diamond &amp; Shining Pearl.
/// </summary>
/// <remarks>size: 0x30B8, struct_name: ZUKAN_WORK</remarks>
public sealed class Zukan8bLumi : Zukan8b
{
    private const int OFS_STATE = 0;

    private static PersonalTable8BDSP Personal => PersonalTable.BDSPLUMI;

    public Zukan8bLumi(SAV8BSLuminescent sav, int dex) : base(sav, dex) { }

    private int GetStateStructOffset(int species)
    {
        if (species > Legal.MaxSpeciesID_9)
            throw new ArgumentOutOfRangeException(nameof(species));
        return OFS_STATE + (species / 2);
    }

    private int GetBooleanStructOffset(int index, int baseOffset)
    {
        if (index > Legal.MaxSpeciesID_9 - 1)
            throw new ArgumentOutOfRangeException(nameof(index));
        return baseOffset + (index / 8);
    }

    private void SetNibble(ref byte bitFlag, byte bitIndex, byte nibbleValue)
    {
        bitFlag = (byte)(bitFlag & ~(0xF << bitIndex) | (nibbleValue << bitIndex));
    }

    private void SetBit(ref byte bitFlag, byte bitIndex, bool bitValue)
    {
        bitFlag = (byte)(bitFlag & ~(0xF << bitIndex) | ((bitValue ? 1 : 0) << bitIndex));
    }

    public override ZukanState8b GetState(ushort species) => (ZukanState8b)(SAV.Data[PokeDex + GetStateStructOffset(species)] >> ((species & 1) * 4) & 0xF);

    public override void SetState(ushort species, ZukanState8b state) => SetNibble(ref SAV.Data[PokeDex + GetStateStructOffset(species)], (byte)((species & 1) * 4), (byte)state);

    public override bool GetBoolean(int index, int baseOffset) => (SAV.Data[PokeDex + GetBooleanStructOffset(index, baseOffset)] >> (index % 8) & 1) == 1;

    public override void SetBoolean(int index, int baseOffset, bool value) => SetBit(ref SAV.Data[PokeDex + GetBooleanStructOffset(index, baseOffset)], (byte)(index % 8), value);

    public override bool GetLanguageFlag(ushort species, int language)
    {
        if (species > Legal.MaxSpeciesID_9)
            throw new ArgumentOutOfRangeException(nameof(species));
        // Lang flags in 1.3.0 Lumi Revision 1 Save hasn't been changed to bitfields
        if (species > Legal.MaxSpeciesID_8b)
            return false;

        var languageBit = GetLanguageBit(language);
        if (languageBit == -1)
            return false;

        var index = species - 1;
        var offset = OFS_LANGUAGE + (sizeof(int) * index);
        var current = ReadInt32LittleEndian(SAV.Data.AsSpan(PokeDex + offset));
        return (current & (1 << languageBit)) != 0;
    }

    public override void SetLanguageFlag(ushort species, int language, bool value)
    {
        if (species > Legal.MaxSpeciesID_9)
            throw new ArgumentOutOfRangeException(nameof(species));
        // Lang flags in 1.3.0 Lumi Revision 1 Save hasn't been changed to bitfields
        if (species > Legal.MaxSpeciesID_8b)
            return;

        var languageBit = GetLanguageBit(language);
        if (languageBit == -1)
            return;

        var index = species - 1;
        var offset = OFS_LANGUAGE + (sizeof(int) * index);
        var current = ReadInt32LittleEndian(SAV.Data.AsSpan(PokeDex + offset));
        var mask = (1 << languageBit);
        var update = value ? current | mask : current & ~(mask);
        WriteInt32LittleEndian(SAV.Data.AsSpan(PokeDex + offset), update);
    }

    public override void SetLanguageFlags(ushort species, int value)
    {
        if (species > Legal.MaxSpeciesID_9)
            throw new ArgumentOutOfRangeException(nameof(species));
        // Lang flags in 1.3.0 Lumi Revision 1 Save hasn't been changed to bitfields
        if (species > Legal.MaxSpeciesID_8b)
            return;

        var index = species - 1;
        var offset = OFS_LANGUAGE + (sizeof(int) * index);
        WriteInt32LittleEndian(SAV.Data.AsSpan(PokeDex + offset), value);
    }

    public override void SetDex(PKM pk)
    {
        ushort species = pk.Species;

        if (species is 0 or > Legal.MaxSpeciesID_9) return;
        if (pk.IsEgg) return;

        var originalState = GetState(species);
        bool shiny = pk.IsShiny;

        SetState(species, ZukanState8b.Caught);
        SetGenderFlag(species, pk.Gender, shiny);

        if (species > Legal.MaxSpeciesID_8b) return;

        SetLanguageFlag(species, pk.Language, true);
        SetHasFormFlag(species, pk.Form, shiny, true);

        if (species is (int)Species.Spinda)
            SAV.ZukanExtra.SetDex(originalState, pk.EncryptionConstant, pk.Gender, shiny);
    }

    public override void CaughtAll(bool shinyToo = false)
    {
        for (ushort species = 1; species <= Legal.MaxSpeciesID_9; species++)
        {
            SetState(species, ZukanState8b.Caught);

            var m = !OnlyFemale(species);
            var f = !OnlyMale(species);
            SetGenderFlags(species, m, f, m && shinyToo, f && shinyToo);

            if (species > Legal.MaxSpeciesID_8b) return;
            SetLanguageFlag(species, SAV.Language, true);
        }
    }

    public override void SetAllSeen(bool value = true, bool shinyToo = false)
    {
        for (ushort species = 1; species <= Legal.MaxSpeciesID_9; species++)
        {
            if (value)
            {
                if (!GetSeen(species))
                    SetState(species, ZukanState8b.Seen);

                var m = !OnlyFemale(species);
                var f = !OnlyMale(species);
                SetGenderFlags(species, m, f, m && shinyToo, f && shinyToo);
            }
            else
            {
                ClearDexEntryAll(species);
            }
        }
    }

    public void SetDexEntryAll(bool shinyToo = false)
    {
        for (ushort species = 1; species <= Legal.MaxSpeciesID_9; species++)
        {
            SetState(species, ZukanState8b.Caught);

            var m = !OnlyFemale(species);
            var f = !OnlyMale(species);
            SetGenderFlags(species, m, f, m && shinyToo, f && shinyToo);

            if (species > 493) return;

            var formCount = GetFormCount(species);
            if (formCount is not 0)
            {
                for (byte form = 0; form < formCount; form++)
                {
                    SetHasFormFlag(species, form, false, true);
                    if (shinyToo)
                        SetHasFormFlag(species, form, true, true);
                }
            }

            SetLanguageFlags(species, LANGUAGE_ALL);
        }
    }

    public override void ClearDexEntryAll(ushort species)
    {
        SetState(species, ZukanState8b.None);
        SetGenderFlags(species, false, false, false, false);

        if (species > Legal.MaxSpeciesID_8b) return;

        var formCount = GetFormCount(species);
        if (formCount is not 0)
        {
            for (byte form = 0; form < formCount; form++)
            {
                SetHasFormFlag(species, form, false, false);
                SetHasFormFlag(species, form, true, false);
            }
        }

        SetLanguageFlags(species, LANGUAGE_NONE);
    }

    public bool OnlyFemale(ushort species) => Personal[species].OnlyFemale;
    public bool OnlyMale(ushort species) => Personal[species].OnlyMale;
}
