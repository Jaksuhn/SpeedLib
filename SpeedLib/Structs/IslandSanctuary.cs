using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpeedLib.Structs;

public static class IslandSanctuary
{
    [StructLayout(LayoutKind.Explicit, Size = 0x40)]
    public unsafe partial struct AgentMJICraftSchedule
    {
        [StructLayout(LayoutKind.Explicit, Size = 0x98)]
        public unsafe partial struct ItemData
        {
            [FieldOffset(0x10)] public fixed ushort Materials[4];
            [FieldOffset(0x20)] public ushort ObjectId;
        }

        [StructLayout(LayoutKind.Explicit, Size = 0xC)]
        public unsafe partial struct EntryData
        {
            [FieldOffset(0x0)] public ushort CraftObjectId;
            [FieldOffset(0x2)] public ushort u2;
            [FieldOffset(0x4)] public uint u4;
            [FieldOffset(0x8)] public byte StartingSlot;
            [FieldOffset(0x9)] public byte Duration;
            [FieldOffset(0xA)] public byte Started;
            [FieldOffset(0xB)] public byte Efficient;
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x54)]
        public unsafe partial struct WorkshopData
        {
            [FieldOffset(0x00)] public byte NumScheduleEntries;
            [FieldOffset(0x08)] public fixed byte EntryData[6 * 0xC];
            [FieldOffset(0x50)] public uint UsedTimeSlots;

            public Span<EntryData> Entries => new(Unsafe.AsPointer(ref EntryData[0]), 6);
        }

        [StructLayout(LayoutKind.Explicit, Size = 0xB60)]
        public unsafe partial struct AgentData
        {
            [FieldOffset(0x000)] public int InitState;
            [FieldOffset(0x004)] public int SettingAddonId;
            [FieldOffset(0x0D0)] public StdVector<ItemData> Items;
            [FieldOffset(0x400)] public fixed byte WorkshopData[4 * 0x54];
            [FieldOffset(0x5A8)] public uint CurScheduleSettingObjectIndex;
            [FieldOffset(0x5AC)] public int CurScheduleSettingWorkshop;
            [FieldOffset(0x5B0)] public int CurScheduleSettingStartingSlot;
            [FieldOffset(0x7E8)] public byte CurScheduleSettingNumMaterials;
            [FieldOffset(0x810)] public uint RestCycles;
            [FieldOffset(0x814)] public uint NewRestCycles;
            [FieldOffset(0xB58)] public byte CurrentCycle; // currently viewed
            [FieldOffset(0xB59)] public byte CycleInProgress;
            [FieldOffset(0xB5A)] public byte CurrentIslandRank; // incorrect!

            public Span<WorkshopData> Workshops => new(Unsafe.AsPointer(ref WorkshopData[0]), 4);
        }

        [FieldOffset(0)] public AgentInterface AgentInterface;
        [FieldOffset(0x28)] public AgentData* Data;
    }

    public enum ScheduleListEntryType : int
    {
        NormalEntry = 0,
        LastEntry = 1,
        Category = 2,
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x8)]
    public unsafe struct ScheduleListEntry
    {
        [FieldOffset(0x0)] public ScheduleListEntryType Type;
        [FieldOffset(0x4)] public uint Value; // for Category - category id (time/etc), otherwise - MJICraftworksObject row index - 1
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct MJICraftScheduleSettingData
    {
        [FieldOffset(0x1A8)] public StdVector<Pointer<Pointer<ScheduleListEntry>>> Entries;
        [FieldOffset(0x1E4)] public int NumEntries;

        public int FindEntryIndex(uint rowId)
        {
            for (var i = 0; i < NumEntries; ++i)
            {
                var p = Entries.Span[i].Value->Value;
                if (p->Type != ScheduleListEntryType.Category && p->Value == rowId - 1)
                    return i;
            }
            return -1;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct AddonMJICraftScheduleSetting
    {
        [FieldOffset(0x000)] public AtkUnitBase AtkUnitBase;
        [FieldOffset(0x220)] public MJICraftScheduleSettingData* Data;
    }
}
