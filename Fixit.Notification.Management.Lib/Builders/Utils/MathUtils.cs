using Fixit.Core.DataContracts.Users.Availabilities;
using Fixit.Notification.Management.Lib.Builders.Extensions;
using Fixit.Notification.Management.Lib.Models;
using System;
using System.Collections.Generic;

namespace Fixit.Notification.Management.Lib.Builders.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// Calculates the rating score of the craftsman based on 
        /// their average rating and the quantity of ratings.
        /// </summary>
        /// <param name="averageRating">Average rating of user</param>
        /// <param name="reviewCount">Quantity of ratings of user</param>
        /// <param name="maxScore">The maximum score given by the classifier</param>
        /// <param name="P">Weight on quantity and quality. Default 50/50 ratio. Increase P to put more importance on the quality</param>
        /// <param name="M">Number that is considered to be a moderate quantity of reviews.</param>
        /// <returns>Returns the rating score of a craftsman user.</returns>
        public static float CalculateRatingScore(float averageRating, float reviewCount, float maxScore = 10, float? P = 0.5f, float? M = 15f)
        {
            // Q represents the importance of the quantity of ratings
            float Q = (float)(-M / Math.Log(1f / 2f));
            float ratingScore = (float)(P * averageRating + maxScore * (1 - P) * (1 - Math.Exp(-reviewCount / Q)));

            return ratingScore;
        }


        /// <summary>
        /// Calculates the proximity score of the craftsman based on 
        /// the distance needed to travel to the fix request.
        /// </summary>
        /// <param name="distance">Distance needed to travel to the fix request</param>
        /// <param name="maxScore">The maximum score given by the classifier</param>
        /// <param name="n">Determines how 'flat' the curve is at the top</param>
        /// <param name="maximumDistance">Maximum radius allowed for the classifier to consider a craftsman</param>
        /// <returns>Returns the proximity score of a craftsman user.</returns>
        public static float CalculateProximityScore(double distance, float maxScore=10, float n=1.5f, double maximumDistance = 200000)
        {
            float proximityScore = (float)(maxScore - (maxScore * Math.Pow(distance/maximumDistance, 1/n)));
            proximityScore = proximityScore < 0 ? 0 : proximityScore; 
            return proximityScore;
        }


        /// <summary>
        /// Calculates the availability score of the craftsman based on
        /// how much time the craftsman has to do the fix request.
        /// </summary>
        /// <param name="fixRequestTime">Distance needed to travel to the fix request</param>
        /// <param name="businessHours">Business hours of the craftsmam</param>
        /// <param name="maxScore">The maximum score given by the classifier</param>
        /// <returns>Returns the availability score of a craftsman user.</returns>
        public static float CalculateAvailabilityScore(DateTime fixRequestTime, List<BusinessHoursRangeDto> businessHours, float maxScore=10)
        {
            double availabilityScore = 0;

            foreach (var schedule in businessHours)
            {
                var openingHour = new DateTime(schedule.StartTimestampUtc);
                var closingHour = new DateTime(schedule.EndTimestampUtc);
                if (!DateTimeExtensions.IsEarlierThan(fixRequestTime, openingHour))
                {
                    var totalWorkHourPerday = closingHour- openingHour;
                    var multiplier = maxScore / totalWorkHourPerday.TotalMinutes;

                    TimeSpan timeInterval = closingHour - fixRequestTime;
                    availabilityScore += Math.Max(0, timeInterval.TotalMinutes) * multiplier;
                }
            }

            return (float)availabilityScore;
        }


        /// <summary>
        /// Calculates the qualification score of the craftsman based on 
        /// the number of skills related to the work category of the fix request.
        /// </summary>
        /// <param name="qualifiedSkillCount">The number of skills the craftsman has of the work category</param>
        /// <param name="skillCount">The number of skills the category has under</param>
        /// <param name="n">Determines how 'fast' the curve increases.  Curve increase faster as n increases.</param>
        /// <returns>Returns the qualification score of a craftsman user.</returns>
        public static float CalculateQualifificationScore(int qualifiedSkillCount, int skillCount, float maxScore=10, double n=1)
        {
            float qualificationScore = (float)(maxScore * Math.Pow((double)qualifiedSkillCount/skillCount, n));

            return qualificationScore;
        }

        public static float CalculateTotalScore(CraftsmenScores craftsmenScores)
        {
            var totalScore = craftsmenScores.QualificationScore + craftsmenScores.ProximityScore + craftsmenScores.AvailibilityScore + craftsmenScores.RatingScore;

            return totalScore;
        }

        public static float CalculateAverageScore(List<CraftsmenScores> craftsmenScores)
        {
            var sum = 0f;
            foreach(var craftsman in craftsmenScores)
            {
                sum += craftsman.TotalScore;
            }
            var average = sum / craftsmenScores.Count;

            return average;
        }

    }
}
