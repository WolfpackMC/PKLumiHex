using System;
using System.Diagnostics.Metrics;
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
        if ((uint)species > (uint)Legal.MaxSpeciesID_9)
            throw new ArgumentOutOfRangeException(nameof(species));
        return OFS_STATE + (species / 2);
    }

    private int GetBooleanStructOffset(int index, int baseOffset)
    {
        if ((uint)index > (uint)Legal.MaxSpeciesID_9)
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
        else if ((uint)species > Legal.MaxSpeciesID_8b)
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
        else if ((uint)species > Legal.MaxSpeciesID_8b)
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
        else if (species > Legal.MaxSpeciesID_8b)
            return;

        var index = species - 1;
        var offset = OFS_LANGUAGE + (sizeof(int) * index);
        WriteInt32LittleEndian(SAV.Data.AsSpan(PokeDex + offset), value);
    }

    public override void CaughtAll(bool shinyToo = false)
    {
        var pt = Personal;
        for (ushort species = 1; species <= Legal.MaxSpeciesID_9; species++)
        {
            SetState(species, ZukanState8b.Caught);
            var pi = pt[species];
            var m = !pi.OnlyFemale;
            var f = !pi.OnlyMale;
            SetGenderFlags(species, m, f, m && shinyToo, f && shinyToo);
            SetLanguageFlag(species, SAV.Language, true);
        }
    }

    public override void SetAllSeen(bool value = true, bool shinyToo = false)
    {
        var pt = Personal;
        for (ushort species = 1; species <= Legal.MaxSpeciesID_9; species++)
        {
            if (value)
            {
                if (!GetSeen(species))
                    SetState(species, ZukanState8b.Seen);
                var pi = pt[species];
                var m = !pi.OnlyFemale;
                var f = !pi.OnlyMale;
                SetGenderFlags(species, m, f, m && shinyToo, f && shinyToo);
            }
            else
            {
                ClearDexEntryAll(species);
            }
        }
    }

    public override void SetDexEntryAll(ushort species, bool shinyToo = false)
    {
        SetState(species, ZukanState8b.Caught);

        var pt = Personal;
        var pi = pt[species];
        var m = !pi.OnlyFemale;
        var f = !pi.OnlyMale;
        SetGenderFlags(species, m, f, m && shinyToo, f && shinyToo);

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

    public override void ClearDexEntryAll(ushort species)
    {
        SetState(species, ZukanState8b.None);
        SetGenderFlags(species, false, false, false, false);

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
}
