using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;

namespace SpeedLib.SpeedLib.Helpers;

internal unsafe class IslandSanctuary
{
    public Structs.IslandSanctuary.AgentMJICraftSchedule* Agent = (Structs.IslandSanctuary.AgentMJICraftSchedule*)AgentModule.Instance()->GetAgentByInternalId(AgentId.MJICraftSchedule);
    public Structs.IslandSanctuary.AgentMJICraftSchedule.AgentData* AgentData => Agent != null ? Agent->Data : null;
    private unsafe delegate nint ReceiveEventDelegate(AtkEventListener* eventListener, AtkEventType eventType, uint eventParam, void* eventData, void* inputData);

    public static unsafe bool IsWorkshopUnlocked(int w, out int maxWorkshops)
    {
        maxWorkshops = 0;
        try
        {
            var currentRank = MJIManager.Instance()->IslandState.CurrentRank;
            switch (w)
            {
                case 1 when currentRank < 3:
                    maxWorkshops = 0;
                    break;
                case 2 when currentRank < 6:
                    maxWorkshops = 1;
                    break;
                case 3 when currentRank < 8:
                    maxWorkshops = 2;
                    break;
                case 4 when currentRank < 14:
                    maxWorkshops = 3;
                    break;
                default:
                    return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            ex.Log();
            return false;
        }
    }

    public static unsafe bool IsCraftworkObjectCraftable(MJICraftworksObject item) => MJIManager.Instance()->IslandState.CurrentRank >= item.LevelReq;
    public static unsafe bool IsWorkshopOpen() => Addons.TryGetAddonByName<AtkUnitBase>("MJICraftSchedule", out var addon) && addon->IsVisible;
    public static unsafe bool IsCraftSelectOpen() => Addons.TryGetAddonByName<AtkUnitBase>("MJICraftScheduleSetting", out var addon) && addon->IsVisible;
    public static unsafe int? GetOpenCycle() =>
        Addons.TryGetAddonByName<AtkUnitBase>("MJICraftSchedule", out var addon) && addon->IsVisible && addon->AtkValues[0].Type != 0
        ? (int)addon->AtkValues[0].UInt : null;

    public static List<int> GetCurrentRestDays()
    {
        var restDays1 = MJIManager.Instance()->CraftworksRestDays[0];
        var restDays2 = MJIManager.Instance()->CraftworksRestDays[1];
        var restDays3 = MJIManager.Instance()->CraftworksRestDays[2];
        var restDays4 = MJIManager.Instance()->CraftworksRestDays[3];

        return [restDays1, restDays2, restDays3, restDays4];
    }

    public void SetRestCycles(uint mask)
    {
        Svc.Log.Info($"Setting rest: {mask:X}");
        AgentData->NewRestCycles = mask;
        SynthesizeEvent(5, new AtkValue[] { new() { Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int, Int = 0 } });
    }

    private void SynthesizeEvent(ulong eventKind, Span<AtkValue> args)
    {
        var eventData = stackalloc int[] { 0, 0, 0 };
        Agent->AgentInterface.ReceiveEvent(eventData, args.GetPointer(0), (uint)args.Length, eventKind);
    }
}
