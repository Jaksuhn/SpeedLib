using Dalamud.Memory;
using Lumina.Text;
using System.Text.RegularExpressions;

namespace SpeedLib.SpeedLib.Helpers;

public static class Text
{
    public static string ParseSeStringLumina(SeString luminaString)
        => luminaString == null ? string.Empty : Dalamud.Game.Text.SeStringHandling.SeString.Parse(luminaString.RawData).TextValue;

    public static string FilterNonAlphanumeric(string input) => Regex.Replace(input, "[^\\p{L}\\p{N}]", string.Empty);

    public static unsafe string AtkValueStringToString(byte* atkString) => MemoryHelper.ReadSeStringNullTerminated(new nint(atkString)).ToString();
}
