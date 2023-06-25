namespace PKHeX.Core;

/// <summary>
/// Utility logic for checking encounter state.
/// </summary>
public static class EncounterStateUtil
{
    /// <summary>
    /// Checks if the input <see cref="pk"/> could have been a wild encounter.
    /// </summary>
    /// <param name="pk">Pokémon to check.</param>
    /// <returns>True if the <see cref="pk"/> could have been a wild encounter, false otherwise.</returns>
    public static bool CanBeWildEncounter(PKM pk)
    {
        if (pk.IsEgg)
            return false;
        if (IsMetAsEgg(pk))
            return false;
        return true;
    }

    /// <summary>
    /// Checks if the input <see cref="pk"/> was met as an egg.
    /// </summary>
    /// <param name="pk">Pokémon to check.</param>
    /// <returns>True if the <see cref="pk"/> was met as an egg, false otherwise.</returns>
    /// <remarks>Only applicable for Generation 4 origins and above.</remarks>
    public static bool IsMetAsEgg(PKM pk) => pk.Egg_Day != 0;
}
