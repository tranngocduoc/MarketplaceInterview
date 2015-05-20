using System.Linq;
using Marketplace.Interview.Business.Shipping;

namespace Marketplace.Interview.Business.Basket
{
    public interface IShippingCalculator
    {
        decimal CalculateShipping(Basket basket);

        decimal CalculateDiscount(Basket basket);
    }

    public class ShippingCalculator : IShippingCalculator
    {
        public decimal CalculateShipping(Basket basket)
        {
            foreach (var lineItem in basket.LineItems)
            {
                lineItem.ShippingAmount = lineItem.Shipping.GetAmount(lineItem, basket);
                lineItem.ShippingDescription = lineItem.Shipping.GetDescription(lineItem, basket);
            }
            return basket.LineItems.Sum(li => li.ShippingAmount);
        }

        public decimal CalculateDiscount(Basket basket)
        {
            int total = 0;
            foreach (var lineItem in basket.LineItems)
            {
                if (basket.LineItems.Any(
                        item => item.Id < lineItem.Id
                        && item.Shipping.GetType() == lineItem.Shipping.GetType()
                        && item.SupplierId == lineItem.SupplierId
                        && item.DeliveryRegion == lineItem.DeliveryRegion
                        && item.Shipping.GetType() == typeof(FexExShipping)
                    ))
                {
                    total = total + 1;
                }
            }
            if (total > 0)
            {
                return 0.5m;
            }
            else
            {
                return 0;
            }
        }

    }
}