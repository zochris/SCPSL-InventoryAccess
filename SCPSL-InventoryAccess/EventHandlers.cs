using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Interactables.Interobjects.DoorUtils;

namespace SCPSL_InventoryAccess
{
    public class EventHandlers
    {
        public void OnPlayerDoorInteract(InteractingDoorEventArgs ev)
        {
            if (ev.Player.Side == Side.Scp) return;
            if (ev.IsAllowed) return;
            ev.IsAllowed = HasPermission(ev.Player, ev.Door.RequiredPermissions);
        }

        public void OnPlayerLockerInteract(InteractingLockerEventArgs ev)
        {
            if (ev.Player.Side == Side.Scp) return;
            if (ev.IsAllowed) return;

            ev.IsAllowed = HasPermission(ev.Player, ev.Chamber.accessToken);
        }

        public void OnGeneratorAccess(UnlockingGeneratorEventArgs ev)
        {
            if (ev.Player.Side == Side.Scp) return;
            if (ev.IsAllowed) return;
            ev.IsAllowed = HasPermission(ev.Player, "ARMORY_LVL_2");
        }

        public void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
        {
            ev.IsAllowed = false; // Due to workaround, reset allowed

            if (ev.Player.IsBypassModeEnabled)
            {
                ev.IsAllowed = true;
                return;
            }

            if (ev.Player.Side == Side.Scp) return;

            ev.IsAllowed = HasPermission(ev.Player, "CONT_LVL_3");
        }

        private bool HasPermission(Player player, DoorPermissions permissions)
        {
            return player.Inventory.items.Any(item => permissions.CheckPermissions(item.id, player.ReferenceHub));
        }
        
        private bool HasPermission(Player player, string requested)
        {
            if (requested == "")
            {
                return true;
            }

            foreach (var item in player.Inventory.items)
            {
                foreach (var permission in player.Inventory.GetItemByID(item.id).permissions)
                {
                    if (requested.Contains(permission))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
