using System;
using System.Collections.Generic;
using System.Text;

namespace Fixit.Notification.Management.Lib.Models
{
    public class CraftsmenScores
    {
        public UserDocument UserDocument { get; }

        public float QualificationScore { get; set; }

        public float AvailibilityScore { get; set; }

        public float ProximityScore { get; set; }

        public float RatingScore { get; set; }

        public CraftsmenScores(UserDocument userDocument)
        {
            UserDocument = userDocument;
            QualificationScore = 0;
            AvailibilityScore = 0;
            ProximityScore = 0;
            RatingScore = 0;
        }

    }
}
