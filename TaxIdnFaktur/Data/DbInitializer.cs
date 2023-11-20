using TaxIdnFaktur.Models;

namespace TaxIdnFaktur.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TaxIdnContext context)
        {
            if (context.TaxFakturContainer.Any())
            {
                return;
            }


        }
    }
}
