using ImGuiNET;
using System;
using System.Numerics;

namespace SpeedLib.SpeedLib.Helpers;

public class EzGUI
{
    public static void VerticalText(string text, uint textColor, float scale)
    {

        var drawList = ImGui.GetWindowDrawList();
        var pos = ImGui.GetCursorScreenPos();
        var font = ImGui.GetFont();
        var size = ImGui.CalcTextSize(text);
        pos.X = (float)Math.Round(pos.X);
        pos.Y = (float)Math.Round(pos.Y) + (float)Math.Round(size.X * scale);

        foreach (var c in text)
        {
            var glyph = font.FindGlyph(c);

            drawList.PrimReserve(6, 4);

            drawList.PrimQuadUV(
                pos + new Vector2(glyph.Y0 * scale, -glyph.X0 * scale),
                pos + new Vector2(glyph.Y0 * scale, -glyph.X1 * scale),
                pos + new Vector2(glyph.Y1 * scale, -glyph.X1 * scale),
                pos + new Vector2(glyph.Y1 * scale, -glyph.X0 * scale),

                new Vector2(glyph.U0, glyph.V0),
                new Vector2(glyph.U1, glyph.V0),
                new Vector2(glyph.U1, glyph.V1),
                new Vector2(glyph.U0, glyph.V1),
                textColor);
            pos.Y -= glyph.AdvanceX * scale;
        }

        ImGui.Dummy(new Vector2(size.Y, size.X));
    }

    private static float startTime;
    public static void FlashText(string text, Vector4 colour1, Vector4 colour2, float duration)
    {
        float currentTime = (float)ImGui.GetTime();
        float elapsedTime = currentTime - startTime;

        float t = (float)Math.Sin(elapsedTime / duration * Math.PI * 2) * 0.5f + 0.5f;

        // Interpolate the color difference
        Vector4 interpolatedColor = new(
            colour1.X + t * (colour2.X - colour1.X),
            colour1.Y + t * (colour2.Y - colour1.Y),
            colour1.Z + t * (colour2.Z - colour1.Z),
            1.0f
        );

        ImGui.PushStyleColor(ImGuiCol.Text, interpolatedColor);
        ImGui.Text(text);
        ImGui.PopStyleColor();

        if (elapsedTime >= duration)
        {
            startTime = currentTime;
        }
    }
}
