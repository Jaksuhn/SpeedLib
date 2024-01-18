using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;

namespace SpeedLib.SpeedLib.Helpers;

public class Inventory
{
    private static unsafe (InventoryType inv, int slot)? FindItemInInventory(uint itemId, IEnumerable<InventoryType> inventories)
    {
        foreach (var inv in inventories)
        {
            var cont = InventoryManager.Instance()->GetInventoryContainer(inv);
            for (var i = 0; i < cont->Size; ++i)
            {
                if (cont->GetInventorySlot(i)->ItemID == itemId)
                {
                    return (inv, i);
                }
            }
        }
        return null;
    }
}
