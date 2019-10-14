using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerboKatz
{
    using UnityEngine;

    public class ScreenMessageLogHandler : ILogHandler
    {
        public void Log(params object[] debugStrings)
        {
            Log(LogMode.Log, debugStrings);
        }

        public void Log(LogMode mode, params object[] debugStrings)
        {
            var debugStringBuilder = new StringBuilder();
            foreach (var debugString in debugStrings)
            {
                debugStringBuilder.Append(debugString.ToString());
            }

            switch (mode)
            {
                case LogMode.Log:
                    ScreenMessages.PostScreenMessage(debugStringBuilder.ToString(), 1f, ScreenMessageStyle.UPPER_LEFT, Color.green);
                    break;
                case LogMode.Warning:
                    ScreenMessages.PostScreenMessage(debugStringBuilder.ToString(), 1f, ScreenMessageStyle.UPPER_LEFT, Color.yellow);
                    break;
                case LogMode.Error:
                case LogMode.Exception:
                    ScreenMessages.PostScreenMessage(debugStringBuilder.ToString(), 1f, ScreenMessageStyle.UPPER_LEFT, Color.red);
                    break;
            }
        }
    }
}
