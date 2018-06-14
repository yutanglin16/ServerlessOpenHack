using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SoftServerless
{
    [DataContract]
    public class RatingDto
    {
        [DataMember(Name ="id")]
        public Guid Id { get; set; }

        [DataMember(Name = "userId")]
        public Guid UserId { get; set; }

        [DataMember(Name = "productId")]
        public Guid ProductId { get; set; }

        [DataMember(Name = "timestamp")]
        public DateTime Timestamp { get; set; }

        [DataMember(Name = "locationName")]
        public string LocationName { get; set; }

        [DataMember(Name = "rating")]
        public int Rating { get; set; }

        [DataMember(Name = "userNotes")]
        public string UserNotes { get; set; }

        [DataMember(Name = "sentimentScore")]
        public double? SentimentScore { get; set; }
    }
}
