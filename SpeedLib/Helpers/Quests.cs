using Lumina.Excel.GeneratedSheets;
using Lumina.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpeedLib.SpeedLib.Helpers;

public class Quests
{
    private static readonly Dictionary<uint, Quest> QuestSheet = Svc.Data?.GetExcelSheet<Quest>()?.Where(x => x.Id.RawString.Length > 0).ToDictionary(i => i.RowId, i => i);
    private readonly List<SeString> questNames = Svc.Data.GetExcelSheet<Quest>(Svc.ClientState.ClientLanguage).Select(x => x.Name).ToList();

    public static string GetQuestNameByID(ushort id)
    {
        if (id > 0)
        {
            var digits = id.ToString().Length;
            if (QuestSheet!.Any(x => Convert.ToInt16(x.Value.Id.RawString.GetLast(digits)) == id))
            {
                return QuestSheet!.First(x => Convert.ToInt16(x.Value.Id.RawString.GetLast(digits)) == id).Value.Name.RawString.Replace("", "").Trim();
            }
        }
        return "";
    }

    public Quest GetQuestByName(string input)
    {
        var matchingRows = questNames.Select((n, i) => (n, i)).Where(t => !string.IsNullOrEmpty(t.n) && IsMatch(input, t.n)).ToList();
        if (matchingRows.Count > 1)
        {
            matchingRows = matchingRows.OrderByDescending(t => MatchingScore(t.n, input)).ToList();
        }
        return matchingRows.Count > 0 ? Svc.Data.GetExcelSheet<Quest>(Svc.ClientState.ClientLanguage).GetRow((uint)matchingRows.First().i) : null;
    }

    private static bool IsMatch(string x, string y) => Regex.IsMatch(x, $@"\b{Regex.Escape(y)}\b");
    private static object MatchingScore(string item, string line)
    {
        var score = 0;

        // primitive matching based on how long the string matches. Enough for now but could need expanding later
        if (line.Contains(item))
            score += item.Length;

        return score;
    }
}
