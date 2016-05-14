using System;
using Multitenant.Interception.Infrastructure;
using ProStoneIMS2015.Models;

namespace ProStoneIMS2015.Models
{
    /// <summary>
    /// Base class that all entities which support multitenancy should derive from.
    /// </summary>
    [TenantAware("TenantId")]
    public class TenantEntity
    {
        /// <summary>
        /// In this case this is the User Id 
        /// as each user is able to access only his own entities
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// The user the entity belongs to
        /// </summary>
        /// 
        [System.Web.Script.Serialization.ScriptIgnore]
        public Subscriber subscriber { get; set; }
        public string Notes { get; set; }
        public bool Inactive { get; set; }
    }
}