﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyPost.Models.API;
using EasyPost.Utilities.Annotations;
using Xunit;

namespace EasyPost.Tests
{
    public class OrderTest : UnitTest
    {
        public OrderTest() : base("order")
        {
        }

        #region CRUD Operations

        [Fact]
        [CrudOperations.Create]
        public async Task TestCreate()
        {
            UseVCR("create");

            Order order = await CreateBasicOrder();

            Assert.IsType<Order>(order);
            Assert.StartsWith("order_", order.Id);
            Assert.NotNull(order.Rates);
        }

        [Fact]
        [CrudOperations.Read]
        public async Task TestGetRates()
        {
            UseVCR("get_rates");

            Order order = await CreateBasicOrder();

            await order.GetRates();

            List<Rate> rates = order.Rates;

            Assert.NotNull(rates);
            foreach (Rate rate in rates)
            {
                Assert.IsType<Rate>(rate);
            }
        }

        [Fact]
        [CrudOperations.Read] // not really a Read operation, but most logical attribute to maintain CRUD placement
        public async Task TestLowestRate()
        {
            UseVCR("lowest_rate");

            Order order = await CreateBasicOrder();

            // test lowest rate with no filters
            Rate lowestRate = order.LowestRate();
            Assert.Equal("First", lowestRate.Service);
            Assert.Equal("5.49", lowestRate.Price);
            Assert.Equal("USPS", lowestRate.Carrier);

            // test lowest rate with service filter (this rate is higher than the lowest but should filter)
            List<string> services = new List<string> { "Priority" };
            lowestRate = order.LowestRate(null, services);
            Assert.Equal("Priority", lowestRate.Service);
            Assert.Equal("7.37", lowestRate.Price);
            Assert.Equal("USPS", lowestRate.Carrier);

            // test lowest rate with carrier filter (should error due to bad carrier)
            List<string> carriers = new List<string> { "BAD_CARRIER" };
            Assert.Throws<Exception>(() => order.LowestRate(carriers));
        }

        [Fact]
        [CrudOperations.Read]
        public async Task TestRetrieve()
        {
            UseVCR("retrieve");

            Order order = await CreateBasicOrder();

            Order retrievedOrder = await Client.Order.Retrieve(order.Id);

            Assert.IsType<Order>(retrievedOrder);
            // Must compare IDs since other elements of objects may be different
            Assert.Equal(order.Id, retrievedOrder.Id);
        }

        [Fact]
        [CrudOperations.Update]
        public async Task TestBuy()
        {
            UseVCR("buy");

            Order order = await CreateBasicOrder();

            order = await order.Buy(Fixture.Usps, Fixture.UspsService);

            List<Shipment> shipments = order.Shipments;

            foreach (Shipment shipment in shipments)
            {
                Assert.IsType<Shipment>(shipment);
                Assert.NotNull(shipment.PostageLabel);
            }
        }

        #endregion

        private async Task<Order> CreateBasicOrder() => await Client.Order.Create(Fixture.BasicOrder);
    }
}
