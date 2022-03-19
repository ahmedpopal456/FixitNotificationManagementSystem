using Fixit.Notification.Management.Lib.Models;
using System.Collections.Generic;

namespace Fixit.Notification.Management.Lib.Builders
{
  public interface IFixClassificationBuilder
  {
    /// <summary>
    /// Classify craftsmen location address 
    /// </summary>
    /// <param>None</param>
    /// <returns>Returns the ProximityScore of each craftsman</returns>
    FixClassificationBuilder ClassifyCraftsmenLocation(string distanceMatrixUri, string googleApiKey, int maximumDistance = 200000, int maximumDuration = 7200, float maxScore = 10, float n = 1.5f, float proximityScoreWeight = 0.25f);

    /// <summary>
    /// Classify craftsmen availability/schedule
    /// </summary>
    /// <param name="maxScore">Maximum score given by the the classifier. Default set to 10.</param>
    /// <param name="availabilityScoreWeight">The weight of the RatingScore towards the final score in the classification</param>
    /// <returns>Returns the AvailabilityScore of each craftsman</returns>
    FixClassificationBuilder ClassifyCraftsmenAvailability(float maxScore = 10, float availabilityScoreWeight = 0.25f);

    /// <summary>
    /// Classify craftsmen ratings 
    /// </summary>
    /// <param name="ratingList">A list of ratings of all craftsmen</param>
    /// <param name="P">Weight on quantity and quality. Default 50/50 ratio</param>
    /// <param name="M">Moderate number of reviews.</param>
    /// <param name="maxScore">Maximum score given by the the classifier. Default set to 10.</param>
    /// <param name="ratingScoreWeight">The weight of the RatingScore towards the final score in the classification</param>
    /// <returns>Returns the RatingScore of each craftsman, if applicaple</returns>
    FixClassificationBuilder ClassifyCraftsmenRating(List<RatingListDocument> ratingList, float? P = 0.5f, float? M = 15f, float maxScore = 10f, float ratingScoreWeight = 0.2f);
  }
}