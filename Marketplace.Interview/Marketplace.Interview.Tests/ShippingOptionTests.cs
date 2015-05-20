using System.Collections.Generic;
using NUnit.Framework;
using Marketplace.Interview.Business.Basket;
using Marketplace.Interview.Business.Shipping;

namespace Marketplace.Interview.Tests
{
    [TestFixture]
    public class ShippingOptionTests
    {
        [Test]
        public void FlatRateShippingOptionTest()
        {
            var flatRateShippingOption = new FlatRateShipping {FlatRate = 1.5m};
            var shippingAmount = flatRateShippingOption.GetAmount(new LineItem(), new Basket());
            Assert.That(shippingAmount, Is.EqualTo(1.5m), "Flat rate shipping not correct.");
        }

        [Test]
        public void PerRegionShippingOptionTest()
        {
            var perRegionShippingOption = new PerRegionShipping()
            {
                PerRegionCosts = new[]
                                    {
                                        new RegionShippingCost()
                                            {
                                                DestinationRegion =
                                                    RegionShippingCost.Regions.UK,
                                                Amount = .75m
                                            },
                                        new RegionShippingCost()
                                            {
                                                DestinationRegion =
                                                    RegionShippingCost.Regions.Europe,
                                                Amount = 1.5m
                                            }
                                    },
            };

            var shippingAmount = perRegionShippingOption.GetAmount(new LineItem() {DeliveryRegion = RegionShippingCost.Regions.Europe}, new Basket());
            Assert.That(shippingAmount, Is.EqualTo(1.5m));

            shippingAmount = perRegionShippingOption.GetAmount(new LineItem() { DeliveryRegion = RegionShippingCost.Regions.UK}, new Basket());
            Assert.That(shippingAmount, Is.EqualTo(.75m));
        }


        [Test]
        public void FexExShippingOptionTest()
        {
            var perRegionShippingOption = new FexExShipping()
            {
                PerRegionCosts = new[]
                                    {
                                        new RegionShippingCost()
                                            {
                                                DestinationRegion =
                                                    RegionShippingCost.Regions.UK,
                                                Amount = .75m
                                            },
                                        new RegionShippingCost()
                                            {
                                                DestinationRegion =
                                                    RegionShippingCost.Regions.Europe,
                                                Amount = 1.5m
                                            }
                                    },
            };

            var shippingAmount = perRegionShippingOption.GetAmount(new LineItem() { DeliveryRegion = RegionShippingCost.Regions.Europe }, new Basket());
            Assert.That(shippingAmount, Is.EqualTo(1.5m));

            shippingAmount = perRegionShippingOption.GetAmount(new LineItem() { DeliveryRegion = RegionShippingCost.Regions.UK }, new Basket());
            Assert.That(shippingAmount, Is.EqualTo(.75m));
        }

        [Test]
        public void ShippingDiscount()
        {
            var FexExShippingOption = new FexExShipping()
            {
                PerRegionCosts = new[]
                        {
                            new RegionShippingCost()
                                {
                                    DestinationRegion =
                                        RegionShippingCost.Regions.UK,
                                    Amount = .7m
                                },
                            new RegionShippingCost()
                                {
                                    DestinationRegion =
                                        RegionShippingCost.Regions.Europe,
                                    Amount = 1.2m
                                }
                        },
            };
            var basket = new Basket()
            {
                LineItems = new List<LineItem>
                                                 {
                                                     new LineItem()
                                                         {
                                                             Id = 1,
                                                             DeliveryRegion = RegionShippingCost.Regions.UK,
                                                             SupplierId = 1,
                                                             Shipping = FexExShippingOption
                                                         },
                                                     new LineItem()
                                                         {
                                                             Id = 2,
                                                             DeliveryRegion = RegionShippingCost.Regions.UK,
                                                             SupplierId = 1,
                                                             Shipping = FexExShippingOption
                                                         },
                                                 }
            };

            var calculator = new ShippingCalculator();
            decimal basketDiscount = calculator.CalculateDiscount(basket);
            Assert.That(basketDiscount, Is.EqualTo(.5m));
        }

        [Test]
        public void BasketShippingTotalTest()
        {
            var perRegionShippingOption = new PerRegionShipping()
            {
                PerRegionCosts = new[]
                        {
                            new RegionShippingCost()
                                {
                                    DestinationRegion =
                                        RegionShippingCost.Regions.UK,
                                    Amount = .75m
                                },
                            new RegionShippingCost()
                                {
                                    DestinationRegion =
                                        RegionShippingCost.Regions.Europe,
                                    Amount = 1.5m
                                }
                        },
            };

            var FexExShippingOption = new FexExShipping()
            {
                PerRegionCosts = new[]
                        {
                            new RegionShippingCost()
                                {
                                    DestinationRegion =
                                        RegionShippingCost.Regions.UK,
                                    Amount = .7m
                                },
                            new RegionShippingCost()
                                {
                                    DestinationRegion =
                                        RegionShippingCost.Regions.Europe,
                                    Amount = 1.2m
                                }
                        },
            };

            var flatRateShippingOption = new FlatRateShipping {FlatRate = 1.1m};

            var basket = new Basket()
                             {
                                 LineItems = new List<LineItem>
                                                 {
                                                     new LineItem()
                                                         {
                                                             DeliveryRegion = RegionShippingCost.Regions.UK,
                                                             Shipping = perRegionShippingOption
                                                         },
                                                     new LineItem()
                                                         {
                                                             DeliveryRegion = RegionShippingCost.Regions.Europe,
                                                             Shipping = perRegionShippingOption
                                                         },
                                                     new LineItem()
                                                         {
                                                             Id = 1,
                                                             DeliveryRegion = RegionShippingCost.Regions.UK,
                                                             SupplierId = 1,
                                                             Shipping = FexExShippingOption
                                                         },
                                                     new LineItem()
                                                         {
                                                             Id = 2,
                                                             DeliveryRegion = RegionShippingCost.Regions.UK,
                                                             SupplierId = 1,
                                                             Shipping = FexExShippingOption
                                                         },
                                                     new LineItem() {Shipping = flatRateShippingOption},
                                                 }
                             };

            var calculator = new ShippingCalculator();

            decimal basketShipping = calculator.CalculateShipping(basket);
            decimal basketDiscount= calculator.CalculateDiscount(basket);
            decimal totalShipping = basketShipping - basketDiscount;
            Assert.That(basketDiscount, Is.EqualTo(.5m));
            Assert.That(totalShipping, Is.EqualTo(4.25m));
        }
    }
}