using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Core.Database;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Seeders;

namespace Fixit.Notification.Management.Lib.Models
{
    [DataContract]
    public class RatingListDocument : DocumentBase, IFakeSeederAdapter<RatingListDocument>
    {
        [DataMember]
        public float AverageRating { get; set; }

        [DataMember]
        public int ReviewCount { get; set; }

        #region SeedFakeDtos
        public new IList<RatingListDocument> SeedFakeDtos()
        {
            RatingListDocument firstRatingListDocument = new RatingListDocument
            {
                AverageRating = 5,
                ReviewCount = 15
            };
            RatingListDocument secondRatingListDocument = null;

            return new List<RatingListDocument>
            {
                firstRatingListDocument,
                secondRatingListDocument
            };
        }
        #endregion
    }
}
