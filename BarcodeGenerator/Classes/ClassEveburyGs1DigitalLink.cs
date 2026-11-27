using System;
using System.Collections.Generic;
using System.Text;
using Evebury.Gs1.DigitalLink;

namespace BarcodeGenerator
{
    internal class ClassEveburyGs1DigitalLink
    {
        //Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

        public static void GenerateGS1DigitalLink()
        {
            //construct the builder
            DigitalLinkBuilder builder = new();

            //set the primary key this is required!
            builder.SetPrimaryKey(PrimaryKeyType.GTIN, "00074562000525");

            //optionally set your custom domain
            builder.SetCustomDomainUri("https://www.evebury.com");

            //add a date
            builder.AddDate(DateType.PRODUCTION_DATE, DateTime.Now);

            //add a list value
            builder.AddString(StringType.INTERNAL_COMPANY, "value1/with slash");
            builder.AddString(StringType.INTERNAL_COMPANY, "value2");

            //add net weight value (decimal precision inferred)
            builder.AddNetWeight(new Weight(290.3, WeightUnit.POUNDS));

            //add price value set to the decimal precision
            builder.AddPrice(PriceType.PRICE_TRADE_ITEM, new Price(88, 2, CurrencyCode.EUR));

            //uncomment to raise validation error
            builder.AddKey(KeyType.GTIN, "00074562000526");

            DigitalLink link = builder.Build();
            if (!link.IsValid)
            {
                Debug.WriteLine("Errors:");
                List<ValidationError> errors = link.GetValidationErrors();
                foreach (ValidationError error in errors)
                {
                    //Value '00074562000526' does not have a valid check digit. Segment: GTIN - Global Trade Item Number (GTIN).
                    Debug.WriteLine(error);
                }
            }
            else
            {
                Debug.Write("Payload: ");
                //https://www.evebury.com/01/00074562000525?11=260318&3201=002903&3932=9788800&91=value1%2fwith+slash&92=value2
                Debug.WriteLine(link.Uri);
            }
        }
    }
}
