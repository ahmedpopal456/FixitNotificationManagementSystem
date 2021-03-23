using System;
using System.Collections.Generic;
using System.Text;

namespace Fixit.Notification.Management.Lib.Builders.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// Calculates the rating score of the craftsman based on their average rating and the quantity of ratings
        /// </summary>
        /// <param name="averageRating">Average rating of user</param>
        /// <param name="reviewCount">Quantity of ratings of user</param>
        /// <param name="P">Weight on quantity and quality. Default 50/50 ratio</param>
        /// <param name="M">Moderate number of reviews.</param>
        /// <returns>Returns RatingScore of a craftsman user.</returns>
        public static float CalculateRatingScore(float averageRating, float reviewCount, float? P = 0.5f, float? M = 15f)
        {
            // Q represents the importance of the quantity of ratings
            float Q = (float)(-M / Math.Log(1f / 2f));
            float ratingScore = (float)(P * averageRating + 10 * (1 - P) * (1 - Math.Exp(-reviewCount / Q)));

            return ratingScore;
        }

    }
}
