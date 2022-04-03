using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fixit.Notification.Management.Lib.Models.Classification
{
    public class GoogleDistanceMatrixResponse
    {
        public IEnumerable<string> Destination_addresses { get; set; }

        public IEnumerable<string> Origin_addresses { get; set; }

        public IEnumerable<DistanceMatrix> Rows { get; set; }

        public string Status { get; set; }

        public Distance Distance
        {
            get { return Rows.FirstOrDefault().Elements.FirstOrDefault().Distance; }
        }

        public Duration Duration
        {
            get { return Rows.FirstOrDefault().Elements.FirstOrDefault().Duration; }
        }
    }

    public class DistanceMatrix
    {
        public IEnumerable<Elements> Elements { get; set; }
    }

    public class Elements
    {
        public Distance Distance { get; set; }
        public Duration Duration { get; set; }
        public string Status { get; set; }
    }

    public class Distance
    {
        public string Text { get; set; }
        public string Value { get; set; }

    }

    public class Duration
    {
        public string Text { get; set; }
        public string Value { get; set; }

    }

}
