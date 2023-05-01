using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using EasyPost.Utilities.Internal.Attributes;
using EasyPost.Utilities.Internal.Extensions;

namespace EasyPost.BetaFeatures.Parameters.EndShippers
{
    /// <summary>
    ///     Parameters for <see cref="EasyPost.Services.EndShipperService.All(All)"/> API calls.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class All : BaseAllParameters
    {
        #region Request Parameters

        /// <summary>
        ///     Only records created after the given ID will be included.
        /// </summary>
        [TopLevelRequestParameter(Necessity.Optional, "after_id")]
        public string? AfterId { get; set; }

        /// <summary>
        ///     The number of records to return on each page. The maximum value is 100, and default is 20.
        /// </summary>
        [TopLevelRequestParameter(Necessity.Optional, "page_size")]
        public int? PageSize { get; set; }

        #endregion

        protected override TParameters FromDictionaryProtected<TParameters>(Dictionary<string, object> dictionary)
        {
            var parameters = new All
            {
                PageSize = dictionary.GetOrNullInt("page_size"),
                AfterId = dictionary.GetOrNull<string>("after_id"),
            };

            return (parameters as TParameters)!;
        }
    }
}
