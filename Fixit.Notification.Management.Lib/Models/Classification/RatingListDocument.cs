using System.Runtime.Serialization;
using Fixit.Core.Database;

namespace Fixit.Notification.Management.Lib.Models
{
    [DataContract]
    public class RatingListDocument : DocumentBase
    {
        [DataMember]
        public float AverageRating { get; set; }

        [DataMember]
        public int ReviewCount { get; set; }

    }
}
