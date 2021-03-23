using Fixit.Notification.Management.Lib.Models;
using static Fixit.Notification.Management.Lib.Builders.Utils.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Fixit.Notification.Management.Lib.Builders
{
    public class FixClassificationBuilder : IFixClassificationBuilder
    {
        private readonly List<CraftsmenScores> _craftsmenScores = new List<CraftsmenScores>();

        public FixClassificationBuilder(List<UserDocument> craftsmenList)
        {
            if(craftsmenList == null)
            {
                throw new ArgumentNullException($"{nameof(FixClassificationBuilder)} expects the {nameof(craftsmenList)} to have a list user craftsmen");
            }
            craftsmenList.ForEach(craftsmen =>
            {
                _craftsmenScores.Add(new CraftsmenScores(craftsmen));
            });
        }

        public FixClassificationBuilder ClassifyCraftsmenSkill()
        {
            throw new NotImplementedException();
        }

        public FixClassificationBuilder ClassifyCraftsmenLocation()
        {
            throw new NotImplementedException();
        }

        public FixClassificationBuilder ClassifyCraftsmenAvailability()
        {
            throw new NotImplementedException();
        }

        public FixClassificationBuilder ClassifyCraftsmenRating(List<RatingListDocument> ratingList, float? P = 0.5f, float? M = 15f, float ratingScoreWeight= 0.2f)
        {
            if (ratingList.Any())
            {
                ratingList.ForEach(rating =>
                { 
                    var ratingScore = CalculateRatingScore(rating.AverageRating, rating.ReviewCount, P, M);
                    var index = _craftsmenScores.FindIndex(craftsmen => craftsmen.UserDocument.id.Equals(rating.EntityId));
                    if (index != -1)
                    {
                        _craftsmenScores[index].RatingScore = ratingScore * ratingScoreWeight;
                    }
                });
            }
            return this;
        }

        public List<CraftsmenScores> GetCraftsmenScores()
        {
            return _craftsmenScores;
        }

    }
}
