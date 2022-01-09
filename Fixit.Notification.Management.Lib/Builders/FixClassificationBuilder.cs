using System;
using System.Linq;
using System.Collections.Generic;
using Fixit.Notification.Management.Lib.Models;
using Fixit.Notification.Management.Lib.Builders.Extensions;
using Fixit.Notification.Management.Lib.Builders.Rules;
using static Fixit.Notification.Management.Lib.Builders.Utils.MathUtils;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Notification.Management.Lib.Models.Classification;
using Fixit.Core.DataContracts.Users.Skill;

namespace Fixit.Notification.Management.Lib.Builders
{
    public class FixClassificationBuilder : IFixClassificationBuilder
    {
        private readonly List<CraftsmenScores> _craftsmenScores = new List<CraftsmenScores>();
        private readonly FixDocument _fixRequest;

        public FixClassificationBuilder(FixDocument fixRequest, List<UserDocument> craftsmenList)
        {
            if(craftsmenList == null)
            {
                throw new ArgumentNullException($"{nameof(FixClassificationBuilder)} expects the {nameof(craftsmenList)} to have a list user craftsmen");
            }
            if(fixRequest == null)
            {
                throw new ArgumentNullException($"{nameof(FixClassificationBuilder)} expects the {nameof(fixRequest)} to have fix request");
            }
            craftsmenList.ForEach(craftsmen => { _craftsmenScores.Add(new CraftsmenScores(craftsmen)); });
            _fixRequest = fixRequest;
        }

        public FixClassificationBuilder ClassifyCraftsmenSkill(IEnumerable<SkillDto> skills, float maxScore = 10, float n = 1.5f, float qualificationScoreWeight = 0.30f)
        {
            if (skills != null && skills.Any())
            {
                _craftsmenScores.ToList().ForEach(craftsmanScore =>
                {
                    if (craftsmanScore.UserDocument.Skills.Any() && MinimumRequirementValidators.HasMinimumSkillRequirement(skills, craftsmanScore, out int qualifiedSkillCount))
                    {                
                        var qualificationScore = CalculateQualifificationScore(qualifiedSkillCount, skills.ToList().Count, maxScore);
                        craftsmanScore.QualificationScore = qualificationScore * qualificationScoreWeight;
                    }
                    else
                    {
                         _craftsmenScores.Remove(craftsmanScore); 
                    }
                });
            }

            return this;
        }

        public FixClassificationBuilder ClassifyCraftsmenLocation(string distanceMatrixUri, string googleApiKey, int maximumDistance=200000, int maximumDuration=7200, float maxScore=10, float n = 1.5f, float proximityScoreWeight = 0.25f)
        {
            _craftsmenScores.ToList().ForEach(craftsmanScore =>
            {
                var craftsmanLocation = craftsmanScore.UserDocument.SavedAddresses;
                if(craftsmanLocation != null)
                {
                    var responseFromServer = GoogleDistanceMatrixApi.GetLocationDistanceAndDuration(distanceMatrixUri, craftsmanLocation.First(address => address.IsCurrentAddress).Address.FormattedAddress, _fixRequest.Location.FormattedAddress, googleApiKey);
                
                    if (MinimumRequirementValidators.HasMinimumLocationRequirement(maximumDistance, maximumDuration, responseFromServer, out double distance, out double duration))
                    {
                        var proximityScore = CalculateProximityScore(distance, maxScore, n, maximumDistance);
                        craftsmanScore.ProximityScore = proximityScore * proximityScoreWeight;
                    }
                    else
                    {
                        _craftsmenScores.Remove(craftsmanScore);
                    }
                }
                else
                {
                    _craftsmenScores.Remove(craftsmanScore);
                }
            });  

            return this;
        }

        public FixClassificationBuilder ClassifyCraftsmenAvailability(float maxScore=10, float availabilityScoreWeight = 0.25f)
        {
            if(_fixRequest.Details.Type == WorkType.QuickFix)
            {
                var fixStartSchedule = DateTimeExtensions.ConvertUtcTimeStampToDateTime(_fixRequest.Schedule.ToList()[0].StartTimestampUtc);
                DateTime fixStartTime = DateTimeExtensions.AdjustDateTime(fixStartSchedule);
            
                _craftsmenScores.ToList().ForEach(craftsmanScore =>
                {
                    var availability = craftsmanScore.UserDocument.Availability;
                    if(availability != null)
                    {
                        var index = (int)fixStartSchedule.DayOfWeek;
                        var todaysBusinessHours = availability.Schedule.ToList()[index].BusinessHours.ToList();
                        float availabilityScore = maxScore;

                        if (!availability.Type.Equals(AvailabilityType.Always))
                        {
                            availabilityScore = CalculateAvailabilityScore(fixStartTime, todaysBusinessHours, maxScore);
                        }

                        craftsmanScore.AvailibilityScore = availabilityScore * availabilityScoreWeight;
                        if(craftsmanScore.AvailibilityScore == 0) 
                        {
                            _craftsmenScores.Remove(craftsmanScore);
                        }
                    }
                });
            }

            return this;
        }

        public FixClassificationBuilder ClassifyCraftsmenRating(List<RatingListDocument> ratingList, float? P = 0.5f, float? M = 15f, float maxScore=10f, float ratingScoreWeight= 0.2f)
        {
            if (ratingList.Any())
            {
                ratingList.ForEach(rating =>
                { 
                    var ratingScore = CalculateRatingScore(rating.AverageRating, rating.ReviewCount, maxScore, P, M);
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

        public FixClassificationBuilder GetCrafstmenTotalScore()
        {
            _craftsmenScores.ToList().ForEach(craftsmanScore => 
            { 
                craftsmanScore.TotalScore = CalculateTotalScore(craftsmanScore);
            });

            return this;
        }

        public FixClassificationBuilder GetQualifiedCraftsmenByAverage()
        {
            GetCrafstmenTotalScore();
            var average = CalculateAverageScore(_craftsmenScores);

            _craftsmenScores.ToList().ForEach(craftsmanScore => 
            {
                if(craftsmanScore.TotalScore < average)
                {
                    _craftsmenScores.Remove(craftsmanScore);
                }
            });

            return this;
        }

    }
}
