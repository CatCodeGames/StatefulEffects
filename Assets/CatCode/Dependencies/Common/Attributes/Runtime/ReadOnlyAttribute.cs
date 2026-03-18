using System;
using UnityEngine;

namespace CatCode.Common
{
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public ReadOnlyMode Mode { get; private set; }

        public ReadOnlyAttribute(ReadOnlyMode mode = ReadOnlyMode.InRuntime)
        {
            Mode = mode;
        }
    }

    [Flags]
    public enum ReadOnlyMode
    {
        InRuntime = 0 << 1,
        InEditor = 0 << 2,
        All = InRuntime | InEditor
    }
}