namespace PKHeX.Core;

/// <summary> Generation 8 <see cref="PKM"/> format. </summary>
public sealed class PB8LUMI : PB8
{
    public override PersonalInfo8BDSP PersonalInfo => PersonalTable.BDSPLUMI.GetFormEntry(Species, Form);
    public override EntityContext Context => EntityContext.Gen8b;

    public PB8LUMI()
    {
        Egg_Location = Met_Location = Locations.Default8bNone;
        AffixedRibbon = -1; // 00 would make it show Kalos Champion :)
    }

    public PB8LUMI(byte[] data) : base(data) { }
    public override PB8LUMI Clone() => new((byte[])Data.Clone());
    public override bool IsNative => BDSPLumi;

    // Maximums
    public override ushort MaxMoveID => Legal.MaxMoveID_8b;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_9;
    public override int MaxAbilityID => Legal.MaxAbilityID_8b;
    public override int MaxItemID => 1836;
    public override int MaxBallID => Legal.MaxBallID_8b;
    public override int MaxGameID => Legal.MaxGameID_HOME;
}
