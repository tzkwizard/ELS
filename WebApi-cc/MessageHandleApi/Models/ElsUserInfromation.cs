// Code generated by Microsoft (R) AutoRest Code Generator 0.9.6.0
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace WebApi.Models
{
    public partial class ElsUserInfromation
    {
        private int? _districtCode;
        
        /// <summary>
        /// Optional.
        /// </summary>
        public int? DistrictCode
        {
            get { return this._districtCode; }
            set { this._districtCode = value; }
        }
        
        private string _userId;
        
        /// <summary>
        /// Optional.
        /// </summary>
        public string UserId
        {
            get { return this._userId; }
            set { this._userId = value; }
        }
        
        /// <summary>
        /// Initializes a new instance of the ElsUserInfromation class.
        /// </summary>
        public ElsUserInfromation()
        {
        }
        
        /// <summary>
        /// Serialize the object
        /// </summary>
        /// <returns>
        /// Returns the json model for the type ElsUserInfromation
        /// </returns>
        public virtual JToken SerializeJson(JToken outputObject)
        {
            if (outputObject == null)
            {
                outputObject = new JObject();
            }
            if (this.DistrictCode != null)
            {
                outputObject["DistrictCode"] = this.DistrictCode.Value;
            }
            if (this.UserId != null)
            {
                outputObject["UserId"] = this.UserId;
            }
            return outputObject;
        }
    }
}
