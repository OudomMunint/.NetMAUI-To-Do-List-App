using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListApp.Helpers;
public static class UiHelpers
{
    public static void SetSwitchColors(IEnumerable<Switch> switches)
    {
        foreach (var s in switches)
        {
            s.ThumbColor = Colors.White;

            s.Toggled += (sender, e) =>
            {
                s.ThumbColor = Colors.White;
                s.OnColor = Colors.LimeGreen;
            };
        }
    }
}