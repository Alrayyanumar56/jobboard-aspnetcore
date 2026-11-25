using System;
using System.Collections.Generic;
using System.Text;

namespace JobBoard.Domain.Enums
{
    public enum Location
    {
        OnSite,
        Remote,
        // Added last so OnSite (0) and Remote (1) keep their stored numbers.
        Hybrid,
    }
}
