/* 
 * Title Direct Search Services
 *
 * Title Direct Search Services
 *
 * OpenAPI spec version: 4.0.1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Pims.Ltsa.Models
{
    /// <summary>
    /// DescriptionOfLand
    /// </summary>
    [DataContract]
    public partial class DescriptionOfLand
    {
        /// <summary>
        /// Parcel Status
        /// </summary>
        /// <value>Parcel Status</value>
        [JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: 0)]
        [JsonConverter(typeof(JsonStringEnumMemberConverter))]
        public enum ParcelStatusEnum
        {
            Unknown = 0,
            /// <summary>
            /// Enum AActive for value: A - Active
            /// </summary>
            [EnumMember(Value = "A")]
            AActive = 1,
            /// <summary>
            /// Enum IInactive for value: I - Inactive
            /// </summary>
            [EnumMember(Value = "I")]
            IInactive = 2
        }
        /// <summary>
        /// Parcel Status
        /// </summary>
        /// <value>Parcel Status</value>
        [DataMember(Name = "parcelStatus", EmitDefaultValue = false)]
        public ParcelStatusEnum ParcelStatus { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionOfLand" /> class.
        /// </summary>
        /// <param name="parcelIdentifier">Parcel Identifier (PID) of Parcel to which Title is held. (required).</param>
        /// <param name="fullLegalDescription">The full legal description of the parcel. (required).</param>
        /// <param name="parcelStatus">Parcel Status (required).</param>
        public DescriptionOfLand(string parcelIdentifier = default, string fullLegalDescription = default, ParcelStatusEnum parcelStatus = default)
        {
            // to ensure "parcelIdentifier" is required (not null)
            if (parcelIdentifier == null)
            {
                throw new InvalidDataException("parcelIdentifier is a required property for DescriptionOfLand and cannot be null");
            }
            else
            {
                this.ParcelIdentifier = parcelIdentifier;
            }
            // to ensure "fullLegalDescription" is required (not null)
            if (fullLegalDescription == null)
            {
                throw new InvalidDataException("fullLegalDescription is a required property for DescriptionOfLand and cannot be null");
            }
            else
            {
                this.FullLegalDescription = fullLegalDescription;
            }
            this.ParcelStatus = parcelStatus;
        }

        /// <summary>
        /// Parcel Identifier (PID) of Parcel to which Title is held.
        /// </summary>
        /// <value>Parcel Identifier (PID) of Parcel to which Title is held.</value>
        [DataMember(Name = "parcelIdentifier", EmitDefaultValue = false)]
        public string ParcelIdentifier { get; set; }

        /// <summary>
        /// The full legal description of the parcel.
        /// </summary>
        /// <value>The full legal description of the parcel.</value>
        [DataMember(Name = "fullLegalDescription", EmitDefaultValue = false)]
        public string FullLegalDescription { get; set; }
    }
}
