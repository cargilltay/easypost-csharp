using System.Collections.Generic;
using EasyPost.BetaFeatures.Parameters;
using EasyPost.Models.Shared;
using Newtonsoft.Json;

namespace EasyPost.Models.API
{
    public class ReferralCustomer : BaseUser, IReferralCustomerParameter
    {
        internal ReferralCustomer()
        {
        }
    }

    public class ReferralCustomerCollection : Collection
    {
        #region JSON Properties

        [JsonProperty("referral_customers")]
        public List<ReferralCustomer>? ReferralCustomers { get; set; }

        #endregion

        internal ReferralCustomerCollection()
        {
        }

        protected internal override TParameters BuildNextPageParameters<TEntries, TParameters>(IEnumerable<TEntries> entries, int? pageSize = null) => throw new System.NotImplementedException();
    }
}
