using Dalamud;
using ImGuiNET;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SpeedLib.SpeedLib.Helpers;

internal static class PlayerAttributes
{
    private static unsafe void SetMoveControlData(float speed) =>
        SafeMemory.Write(((delegate* unmanaged[Stdcall]<byte, nint>)Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 ?? ?? 74 ?? 83 ?? ?? 75 ?? 0F ?? ?? ?? 66"))(1) + 8, speed);

    public static void SetSpeed(float speedBase)
    {
        Svc.SigScanner.TryScanText("f3 ?? ?? ?? ?? ?? ?? ?? e8 ?? ?? ?? ?? 48 ?? ?? ?? ?? ?? ?? 0f ?? ?? e8 ?? ?? ?? ?? f3 ?? ?? ?? ?? ?? ?? ?? f3 ?? ?? ?? ?? ?? ?? ?? f3 ?? ?? ?? f3", out var address);
        address = address + 4 + Marshal.ReadInt32(address + 4) + 4;
        SafeMemory.Write(address + 20, speedBase);
        SetMoveControlData(speedBase);
    }

    private static nint SetPosFunPtr => Svc.SigScanner.TryScanText("E8 ?? ?? ?? ?? 83 4B 70 01", out var ptr) ? ptr : nint.Zero;

    public static unsafe void SetPos(float x, float y, float z)
    {
        if (SetPosFunPtr != nint.Zero && Svc.ClientState.LocalPlayer != null)
        {
            ((delegate* unmanaged[Stdcall]<long, float, float, float, long>)SetPosFunPtr)(Svc.ClientState.LocalPlayer.Address, x, z, y);
        }
    }

    public static void SetPos(Vector3 pos) => SetPos(pos.X, pos.Z, pos.Y);

    public static void SetPosToMouse()
    {
        if (Svc.ClientState.LocalPlayer == null) return;

        var mousePos = ImGui.GetIO().MousePos;
        Svc.GameGui.ScreenToWorld(mousePos, out var pos, 100000f);
        Svc.Log.Info($"Moving from {pos.X}, {pos.Z}, {pos.Y}");
        if (pos != Vector3.Zero)
            SetPos(pos);
    }
}
