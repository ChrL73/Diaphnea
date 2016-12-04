using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    interface IAttachment
    {
        List<GeoPoint> AttachmentLine { get; }
        DatabaseMapItem SmoothedAttachmentLine { get; }
    }
}
