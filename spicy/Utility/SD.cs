using spicy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.Utility
{
    public static class SD
    {
        public const String ManagerUser = "Manager";
        public const String KitchenUser = "Kitchen";
        public const String FrontDeskUser = " FrontDesk";
        public const String CustomerEndUser = "Customer";

        public const String ShoppingCartCount = "ShoppingCartCount";

        public const String ssCouponCode = "CouponCode";

        public const String StatusSubmited = "Submited";
        public const String StatusIsProcess = "Begin Prepared";
        public const String StatusReady = "Ready for PickUp";
        public const String StatusCompleted = "Completed";
        public const String StatusCanceled = "Canceled";

        public const String PaymentStatusPending = "Pending";
        public const String PaymentStatusApproved = "Approved";
        public const String PaymentStatusRejected = "Rejected";




        public static double DiscountPrice(Coupon coupon,double OrderTotalOrginal)
        {
            if (coupon == null)
            {
                return Math.Round(OrderTotalOrginal, 2);
            }
            else
            {
                if (coupon.MinimumAmount > OrderTotalOrginal)
                {
                    return Math.Round(OrderTotalOrginal, 2);
                }
                else
                {
                    if(int.Parse(coupon.CouponType)==(int)Coupon.EcouponType.Doller)
                    {
                        return Math.Round(OrderTotalOrginal - coupon.Discount, 2);
                    }
                    else
                    {
                        return Math.Round(OrderTotalOrginal - (OrderTotalOrginal * (coupon.Discount/100)), 2);
                    }
                }
            }
        }




        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }


    }
}
