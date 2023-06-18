namespace Mango.Web.Utility
{
    public class SD
    {
        //is a string where we set the base url from configuration
        public static string CouponAPIBase { get; set; }
        public enum ApiType
        {
            GET,POST, PUT, DELETE
        }
    }
}
