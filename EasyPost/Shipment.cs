﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace EasyPost
{
    public class Shipment : Resource
    {
        [JsonProperty("batch_id")]
        public string batch_id { get; set; }
        [JsonProperty("batch_message")]
        public string batch_message { get; set; }
        [JsonProperty("batch_status")]
        public string batch_status { get; set; }
        [JsonProperty("buyer_address")]
        public Address buyer_address { get; set; }
        [JsonProperty("carrier_accounts")]
        public List<CarrierAccount> carrier_accounts { get; set; }
        [JsonProperty("created_at")]
        public DateTime? created_at { get; set; }
        [JsonProperty("customs_info")]
        public CustomsInfo customs_info { get; set; }
        [JsonProperty("fees")]
        public List<Fee> fees { get; set; }
        [JsonProperty("forms")]
        public List<Form> forms { get; set; }
        [JsonProperty("from_address")]
        public Address from_address { get; set; }
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("insurance")]
        public string insurance { get; set; }
        [JsonProperty("is_return")]
        public bool? is_return { get; set; }
        [JsonProperty("messages")]
        public List<Message> messages { get; set; }
        [JsonProperty("mode")]
        public string mode { get; set; }
        [JsonProperty("options")]
        public Options options { get; set; }
        [JsonProperty("order_id")]
        public string order_id { get; set; }
        [JsonProperty("parcel")]
        public Parcel parcel { get; set; }
        [JsonProperty("postage_label")]
        public PostageLabel postage_label { get; set; }
        [JsonProperty("rates")]
        public List<Rate> rates { get; set; }
        [JsonProperty("reference")]
        public string reference { get; set; }
        [JsonProperty("refund_status")]
        public string refund_status { get; set; }
        [JsonProperty("return_address")]
        public Address return_address { get; set; }
        [JsonProperty("scan_form")]
        public ScanForm scan_form { get; set; }
        [JsonProperty("selected_rate")]
        public Rate selected_rate { get; set; }
        [JsonProperty("service")]
        public string service { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("tax_identifiers")]
        public List<TaxIdentifier> tax_identifiers { get; set; }
        [JsonProperty("to_address")]
        public Address to_address { get; set; }
        [JsonProperty("tracker")]
        public Tracker tracker { get; set; }
        [JsonProperty("tracking_code")]
        public string tracking_code { get; set; }
        [JsonProperty("updated_at")]
        public DateTime? updated_at { get; set; }
        [JsonProperty("usps_zone")]
        public string usps_zone { get; set; }

        /// <summary>
        ///     Purchase a label for this shipment with the given rate.
        /// </summary>
        /// <param name="rateId">The id of the rate to purchase the shipment with.</param>
        /// <param name="insuranceValue">The value to insure the shipment for.</param>
        public async Task Buy(string rateId, string insuranceValue = null)
        {
            if (id == null)
            {
                throw new PropertyMissing("id");
            }

            Request request = new Request("shipments/{id}/buy", Method.Post);
            request.AddUrlSegment("id", id);

            Dictionary<string, object> body =
                new Dictionary<string, object>
                {
                    {
                        "rate", new Dictionary<string, object>
                        {
                            {
                                "id", rateId
                            }
                        }
                    }
                };

            if (insuranceValue != null)
            {
                body["insurance"] = insuranceValue;
            }

            request.AddBody(body);

            Shipment result = await request.Execute<Shipment>();

            insurance = result.insurance;
            postage_label = result.postage_label;
            tracking_code = result.tracking_code;
            tracker = result.tracker;
            selected_rate = result.selected_rate;
            forms = result.forms;
            messages = result.messages;
            fees = result.fees;
        }

        /// <summary>
        ///     Purchase a label for this shipment with the given rate.
        /// </summary>
        /// <param name="rate">EasyPost.Rate object instance to purchase the shipment with.</param>
        /// <param name="insuranceValue">The value to insure the shipment for.</param>
        public async Task Buy(Rate rate, string insuranceValue = null) => await Buy(rate.id, insuranceValue);

        /// <summary>
        ///     Generate a postage label for this shipment.
        /// </summary>
        /// <param name="fileFormat">Format to generate the label in. Valid formats: "pdf", "zpl" and "epl2".</param>
        public async Task GenerateLabel(string fileFormat)
        {
            if (id == null)
            {
                throw new PropertyMissing("id");
            }

            Request request = new Request("shipments/{id}/label");
            request.AddUrlSegment("id", id);
            // This is a GET, but uses the request body, so use ParameterType.GetOrPost instead.
            request.AddParameter("file_format", fileFormat, ParameterType.GetOrPost);

            Merge(await request.Execute<Shipment>());
        }

        /// <summary>
        ///     Get the Smartrates for this shipment.
        /// </summary>
        /// <returns>A list of EasyPost.Smartrate instances.</returns>
        public async Task<List<Smartrate>> GetSmartrates()
        {
            if (id == null)
            {
                throw new PropertyMissing("id");
            }

            Request request = new Request("shipments/{id}/smartrate");
            request.AddUrlSegment("id", id);
            request.RootElement = "result";
            List<Smartrate> smartrates = await request.Execute<List<Smartrate>>();
            return smartrates;
        }

        /// <summary>
        ///     Insure shipment for the given amount.
        /// </summary>
        /// <param name="amount">The amount to insure the shipment for. Currency is provided when creating a shipment.</param>
        public async Task Insure(double amount)
        {
            if (id == null)
            {
                throw new PropertyMissing("id");
            }

            Request request = new Request("shipments/{id}/insure", Method.Post);
            request.AddUrlSegment("id", id);
            request.AddQueryString(new Dictionary<string, object>
            {
                {
                    "amount", amount
                }
            });

            Merge(await request.Execute<Shipment>());
        }

        /// <summary>
        ///     Get the lowest rate for the shipment. Optionally whitelist/blacklist carriers and services from the search.
        /// </summary>
        /// <param name="includeCarriers">Carriers whitelist.</param>
        /// <param name="includeServices">Services whitelist.</param>
        /// <param name="excludeCarriers">Carriers blacklist.</param>
        /// <param name="excludeServices">Services blacklist.</param>
        /// <returns>EasyPost.Rate instance or null if no rate was found.</returns>
        public Rate LowestRate(IEnumerable<string> includeCarriers = null, IEnumerable<string> includeServices = null,
            IEnumerable<string> excludeCarriers = null, IEnumerable<string> excludeServices = null)
        {
            if (rates == null)
            {
                return null;
            }

            List<Rate> result = new List<Rate>(rates);

            if (includeCarriers != null)
            {
                FilterRates(ref result, rate => includeCarriers.Contains(rate.carrier));
            }

            if (includeServices != null)
            {
                FilterRates(ref result, rate => includeServices.Contains(rate.service));
            }

            if (excludeCarriers != null)
            {
                FilterRates(ref result, rate => !excludeCarriers.Contains(rate.carrier));
            }

            if (excludeServices != null)
            {
                FilterRates(ref result, rate => !excludeServices.Contains(rate.service));
            }

            return result.OrderBy(rate => double.Parse(rate.rate)).FirstOrDefault();
        }

        /// <summary>
        ///     Refresh the rates for this Shipment.
        /// </summary>
        /// <param name="parameters">Optional dictionary of parameters for the API request.</param>
        public async Task RegenerateRates(Dictionary<string, object> parameters = null)
        {
            if (id == null)
            {
                throw new PropertyMissing("id");
            }

            Request request = new Request("shipments/{id}/rerate", Method.Post);
            request.AddUrlSegment("id", id);
            if (parameters != null)
            {
                request.AddBody(parameters);
            }

            rates = (await request.Execute<Shipment>()).rates;
        }

        /// <summary>
        ///     Send a refund request to the carrier the shipment was purchased from.
        /// </summary>
        public async Task Refund()
        {
            if (id == null)
            {
                throw new PropertyMissing("id");
            }

            Request request = new Request("shipments/{id}/refund");
            request.AddUrlSegment("id", id);

            Merge(await request.Execute<Shipment>());
        }

        /// <summary>
        ///     Create a Shipment.
        /// </summary>
        /// <param name="parameters">
        ///     Optional dictionary containing parameters to create the shipment with. Valid pairs:
        ///     * {"from_address", Dictionary&lt;string, object&gt;} See Address.Create for a list of valid keys.
        ///     * {"to_address", Dictionary&lt;string, object&gt;} See Address.Create for a list of valid keys.
        ///     * {"buyer_address", Dictionary&lt;string, object&gt;} See Address.Create for a list of valid keys.
        ///     * {"return_address", Dictionary&lt;string, object&gt;} See Address.Create for a list of valid keys.
        ///     * {"parcel", Dictionary&lt;string, object&gt;} See Parcel.Create for list of valid keys.
        ///     * {"customs_info", Dictionary&lt;string, object&gt;} See CustomsInfo.Create for lsit of valid keys.
        ///     * {"options", Dictionary&lt;string, object&gt;} See https://www.easypost.com/docs/api#shipments for list of
        ///     options.
        ///     * {"is_return", bool}
        ///     * {"currency", string} Defaults to "USD".
        ///     * {"reference", string}
        ///     * {"carrier_accounts", List&lt;string&gt;} List of CarrierAccount.id to limit rating.
        ///     All invalid keys will be ignored.
        /// </param>
        /// <returns>EasyPost.Batch instance.</returns>
        public static async Task<Shipment> Create(Dictionary<string, object> parameters = null) => await SendCreate(parameters ?? new Dictionary<string, object>());


        /// <summary>
        ///     Get a paginated list of shipments.
        /// </summary>
        /// <param name="parameters">
        ///     Optional dictionary containing parameters to filter the list with. Valid pairs:
        ///     * {"before_id", string} String representing a Shipment. Starts with "shp_". Only retrieve shipments created before
        ///     this id. Takes precedence over after_id.
        ///     * {"after_id", string} String representing a Shipment. Starts with "shp_". Only retrieve shipments created after
        ///     this id.
        ///     * {"start_datetime", DateTime} Starting time for the search.
        ///     * {"end_datetime", DateTime} Ending time for the search.
        ///     * {"page_size", int} Size of page. Default to 20.
        ///     * {"purchased", bool} If true only display purchased shipments.
        ///     All invalid keys will be ignored.
        /// </param>
        /// <returns>An EasyPost.ShipmentCollection instance.</returns>
        public static async Task<ShipmentCollection> All(Dictionary<string, object> parameters = null)
        {
            Request request = new Request("shipments");
            request.AddQueryString(parameters ?? new Dictionary<string, object>());

            ShipmentCollection shipmentCollection = await request.Execute<ShipmentCollection>();
            shipmentCollection.filters = parameters;
            return shipmentCollection;
        }

        /// <summary>
        ///     Retrieve a Shipment from its id.
        /// </summary>
        /// <param name="id">String representing a Shipment. Starts with "shp_".</param>
        /// <returns>EasyPost.Shipment instance.</returns>
        public static async Task<Shipment> Retrieve(string id)
        {
            Request request = new Request("shipments/{id}");
            request.AddUrlSegment("id", id);

            return await request.Execute<Shipment>();
        }

        private static void FilterRates(ref List<Rate> rates, Func<Rate, bool> filter) => rates = rates.Where(filter).ToList();

        private static async Task<Shipment> SendCreate(Dictionary<string, object> parameters)
        {
            Request request = new Request("shipments", Method.Post);
            request.AddBody(new Dictionary<string, object>
            {
                {
                    "shipment", parameters
                }
            });

            return await request.Execute<Shipment>();
        }
    }
}
