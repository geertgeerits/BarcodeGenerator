namespace BarcodeGenerator
{
    internal partial class ClassPayloadGS1Validation
    {
        // Add once in the class (central place for all supported GS1 keys and data attributes)
        private static readonly string[] _knownGs1KeysData =
        [
            // Primary identification key formats (x 16) - index in array = 0 to 15
            "01",     // gtin-code
            "8006",   // itip-code
            "8013",   // gmn-code
            "8010",   // cpid-code
            "414",    // gln-code
            "415",    // payTo-code
            "417",    // partyGln-code
            "8017",   // gsrnp-code
            "8018",   // gsrn-code
            "255",    // gcn-code
            "00",     // sscc-code
            "253",    // gdti-code
            "401",    // ginc-code
            "402",    // gsin-code
            "8003",   // grai-code
            "8004",   // giai-code
            
            // Key qualifier formats ( x 9) - index in array = 16 to 24
            "22",     // cpv-code
            "10",     // lot-code
            "21",     // ser-code
            "8011",   // cpsn-code
            "254",    // glnx-code
            "8020",   // refno-code
            "8019",   // srin-code
            "235",    // tpx-code
            "7040",    // uic-ext-code

            // Data attributes - index in array = 25 to 537

            // netWeightVMTICode - netWeightVMTIValue = 6DIGIT - netWeightVMTIParameter = netWeightVMTICode "=" netWeightVMTIValue 
            "3100","3101","3102","3103","3104","3105",
            "3200","3201","3202","3203","3204","3205",
            "3560","3561","3562","3563","3564","3565",
            "3570","3571","3572","3573","3574","3575",

            // lengthVMTICode - lengthVMTIValue = 6DIGIT - lengthVMTIParameter = lengthVMTICode "=" lengthVMTIValue
            "3110","3111","3112","3113","3114","3115",
            "3210","3211","3212","3213","3214","3215",
            "3220","3221","3222","3223","3224","3225",
            "3230","3231","3232","3233","3234","3235",

            // widthVMTICode - widthVMTIValue = 6DIGIT - widthVMTIParameter = widthVMTICode "=" widthVMTIValue
            "3120","3121","3122","3123","3124","3125",
            "3240","3241","3242","3243","3244","3245",
            "3250","3251","3252","3253","3254","3255",
            "3260","3261","3262","3263","3264","3265",

            // depthVMTICode - depthVMTIValue = 6DIGIT - depthVMTIParameter = depthVMTICode "=" depthVMTIValue
            "3130","3131","3132","3133","3134","3135",
            "3270","3271","3272","3273","3274","3275",
            "3280","3281","3282","3283","3284","3285",
            "3290","3291","3292","3293","3294","3295",

            // areaVMTICode - areaVMTIValue = 6DIGIT - areaVMTIParameter = areaVMTICode "=" areaVMTIValue
            "3140","3141","3142","3143","3144","3145",
            "3500","3501","3502","3503","3504","3505",
            "3510","3511","3512","3513","3514","3515",
            "3520","3521","3522","3523","3524","3525",

            // netVolumeVMTICode - netVolumeVMTIValue = 6DIGIT - netVolumeVMTIParameter = netVolumeVMTICode "=" netVolumeVMTIValue
            "3150","3151","3152","3153","3154","3155",
            "3160","3161","3162","3163","3164","3165",
            "3600","3601","3602","3603","3604","3605",
            "3610","3611","3612","3613","3614","3615",
            "3640","3641","3642","3643","3644","3645",
            "3650","3651","3652","3653","3654","3655",
            "3660","3661","3662","3663","3664","3665",

            // massPerUnitAreaVMTICode - massPerUnitAreaVMTIValue = 6DIGIT - massPerUnitAreaVMTIParameter = massPerUnitAreaVMTICode "=" massPerUnitAreaVMTIValue
            "3370","3371","3372","3373","3374","3375",

            // grossWeightCode - grossWeightValue = 6DIGIT - grossWeightParameter = grossWeightCode "=" grossWeightValue
            "3300","3301","3302","3303","3304","3305",
            "3400","3401","3402","3403","3404","3405",

            // logisticLengthCode - logisticLengthValue = 6DIGIT - logisticLengthParameter = logisticLengthCode "=" logisticLengthValue
            "3310","3311","3312","3313","3314","3315",
            "3410","3411","3412","3413","3414","3415",
            "3420","3421","3422","3423","3424","3425",
            "3430","3431","3432","3433","3434","3435",

            // logisticWidthCode - logisticWidthValue = 6DIGIT - logisticWidthParameter = logisticWidthCode "=" logisticWidthValue
            "3320","3321","3322","3323","3324","3325",
            "3440","3441","3442","3443","3444","3445",
            "3450","3451","3452","3453","3454","3455",
            "3460","3461","3462","3463","3464","3465",

            // logisticDepthCode - logisticDepthValue = 6DIGIT - logisticDepthParameter = logisticDepthCode "=" logisticDepthValue
            "3330","3331","3332","3333","3334","3335",
            "3470","3471","3472","3473","3474","3475",
            "3480","3481","3482","3483","3484","3485",
            "3490","3491","3492","3493","3494","3495",

            // logisticAreaCode - logisticAreaValue = 6DIGIT - logisticAreaParameter = logisticAreaCode "=" logisticAreaValue
            "3340","3341","3342","3343","3344","3345",
            "3530","3531","3532","3533","3534","3535",
            "3540","3541","3542","3543","3544","3545",
            "3550","3551","3552","3553","3554","3555",

            // logisticVolumeCode - logisticVolumeValue = 6DIGIT - logisticVolumeParameter = logisticVolumeCode "=" logisticVolumeValue
            "3350","3351","3352","3353","3354","3355",
            "3360","3361","3362","3363","3364","3365",
            "3620","3621","3622","3623","3624","3625",
            "3630","3631","3632","3633","3634","3635",
            "3670","3671","3672","3673","3674","3675",
            "3680","3681","3682","3683","3684","3685",
            "3690","3691","3692","3693","3694","3695",

            // processorCode - processorValue = 3DIGIT 1*27XCHAR - processorParameter = processorCode "=" processorValue
            "7030","7031","7032","7033","7034","7035","7036","7037","7038","7039",

            "02",       // contentParameter         = "02=" 14DIGIT
            "11",       // prodDateParameter        = "11=" 6DIGIT
            "12",       // dueDateParameter         = "12=" 6DIGIT
            "13",       // packDateParameter        = "13=" 6DIGIT
            "15",       // bestBeforeDateParameter  = "15=" 6DIGIT
            "16",       // sellByDateParameter      = "16=" 6DIGIT
            "7006",     // firstFreezeDateParameter = "7006=" 6DIGIT
            "7007",     // harvestDateParameter     = "7007=" 6DIGIT [6DIGIT]
            "7011",     // testByDateParameter      = "7011=" 6DIGIT [4DIGIT]
            "8005",     // pricePerUnitParameter    = "8005=" 6DIGIT
            "20",       // variantParameter         = "20=" 2DIGIT
            "30",       // varCountParameter        = "30=" 1*8DIGIT
            "37",       // countParameter           = "37=" 1*8DIGIT
            "90",       // mutualParameter          = "90=" 1*30XCHAR
            "240",      // additionalIdParameter    = "240=" 1*30XCHAR
            "241",      // custPartNoParameter      = "241=" 1*30XCHAR
            "242",      // mtoVariantParameter      = "242=" 1*6DIGIT
            "243",      // pcnParameter             = "243=" 1*20XCHAR
            "250",      // secondarySerialParameter = "250=" 1*30XCHAR
            "251",      // refToSourceParameter     = "251=" 1*30XCHAR

            // amountCode - amountValue = 1*15DIGIT - amountParameter = amountCode "=" amountValue
            "3900","3901","3902","3903","3904","3905","3906","3907","3908","3909",

            // amountISOCode - 3DIGIT 1*15DIGIT - amountISOParameter = amountISOCode "=" amountISOValue
            "3910","3911","3912","3913","3914","3915","3916","3917","3918","3919",

            // priceCode - priceValue = 1*15DIGIT - priceParameter = priceCode "=" priceValue
            "3920","3921","3922","3923","3924","3925","3926","3927","3928","3929",

            // priceISOCode - 3DIGIT 1*15DIGIT - priceISOParameter = priceISOCode "=" priceISOValue
            "3930","3931","3932","3933","3934","3935","3936","3937","3938","3939",

            // percentOffCode - percentOffValue = 4DIGIT - percentOffParameter = percentOffCode "=" percentOffValue
            "3940","3941","3942","3943",

            "400",      // orderNumberParameter             = "400=" 1*30XCHAR
            "403",      // routeParameter                   = "403=" 1*30XCHAR
            "410",      // shipToLocParameter               = "410=" 13DIGIT
            "411",      // billToParameter                  = "411=" 13DIGIT
            "412",      // purchaseFromParameter            = "412=" 13DIGIT
            "413",      // shipForLocParameter              = "413=" 13DIGIT
            "416",      // prodServLocParameter             = "416=" 13DIGIT
            "420",      // shipToPostParameter              = "420=" 1*20XCHAR
            "421",      // shipToPostISOParameter           = "421=" 3DIGIT 1*9XCHAR
            "422",      // originParameter                  = "422=" 3DIGIT
            "424",      // countryProcessParameter          = "424=" 3DIGIT
            "426",      // countryFullProcessParameter      = "426=" 3DIGIT
            "423",      // countryInitialProcessParameter   = "423=" 3DIGIT 1*12DIGIT
            "425",      // countryDisassemblyParameter      = "425=" 3DIGIT 1*12DIGIT
            "427",      // originSubdivisionParameter       = "427=" 1*3XCHAR
            "710",      // nhrnPZNParameter                 = "710=" 1*20XCHAR
            "711",      // nhrnCIPParameter                 = "711=" 1*20XCHAR
            "712",      // nhrnCNParameter                  = "712=" 1*20XCHAR
            "713",      // nhrnDRNParameter                 = "713=" 1*20XCHAR
            "714",      // nhrnAIMParameter                 = "714=" 1*20XCHAR
            "715",      // nhrnUS-FDAParameter              = "715=" 1*20XCHAR
            "716",      // nhrnAICParameter                 = "716=" 1*20XCHAR
            "717",      // nhrnSRNParameter                 = "717=" 1*20XCHAR
            "7001",     // nsnParameter                     = "7001=" 13DIGIT
            "7002",     // meatCutParameter                 = "7002=" 1*30XCHAR
            "7004",     // activePotencyParameter           = "7004=" 1*4DIGIT
            "7005",     // catchAreaParameter               = "7005=" 1*12XCHAR
            "7008",     // aquaticSpeciesParameter          = "7008=" 1*3XCHAR
            "7009",     // fishingGearTypeParameter         = "7009=" 1*10XCHAR
            "7010",     // prodMethodParameter              = "7010=" 1*2XCHAR
            "7020",     // refurbLotParameter               = "7020=" 1*20XCHAR
            "7021",     // funcStatParameter                = "7021=" 1*20XCHAR
            "7022",     // revStatParameter                 = "7022=" 1*20XCHAR
            "7023",     // giaiAssemblyParameter            = "7023=" 1*30XCHAR
            "7041",     // ufrgtUnitTypeParameter           = "7041=" 1*4XCHAR

            // certificationRefCode - certificationRefValue = 2XCHAR 1*28XCHAR - certificationRefParameter = certificationRefCode "=" certificationRefValue
            "7230","7231","7232","7233","7234","7235","7236","7237","7238","7239",

            "7240",     // protocolParameter            = "7240=" 1*20XCHAR
            "7241",     // aidcMediaTypeParameter       = "7241=" 2DIGIT
            "7242",     // vcnParameter                 = "7242=" 1*25XCHAR
            "7250",     // dobParameter                 = "7250=" 8DIGIT
            "7251",     // dobTimeParameter             = "7251=" 12DIGIT
            "7252",     // bioSexParameter              = "7252=" 1DIGIT
            "7253",     // familyNameParameter          = "7253=" 1*40XCHAR
            "7254",     // givenNameParameter           = "7254=" 1*40XCHAR
            "7255",     // suffixParameter              = "7255=" 1*10XCHAR
            "7256",     // fullNameParameter            = "7256=" 1*90XCHAR
            "7257",     // personAddrParameter          = "7257=" 1*70XCHAR
            "7258",     // birthSequenceParameter       = "7258=" 1DIGIT "/" 1DIGIT
            "7259",     // babyParameter                = "7259=" 1*40XCHAR
            "8001",     // dimensionsParameter          = "8001=" 14DIGIT
            "8002",     // cmtNoParameter               = "8002=" 1*20XCHAR
            "8007",     // ibanParameter                = "8007=" 1*34XCHAR
            "8008",     // prodTimeParameter            = "8008=" 8DIGIT [2DIGIT] [2DIGIT]
            "8009",     // opticalSensorParameter       = "8009=" 1*50XCHAR
            "8012",     // versionParameter             = "8012=" 1*20XCHAR
            "8026",     // itipContentParameter         = "8026=" 14DIGIT 2DIGIT 2DIGIT
            "8030",     // digSigParameter              = "8030=" 1*90ZCHAR
            "8040",     // imeiParameter                = "8040=" 15DIGIT
            "8041",     // imei2Parameter               = "8041=" 15DIGIT
            "8042",     // eSimParameter                = "8042=" 32DIGIT
            "8043",     // pSimParameter                = "8043=" 18DIGIT [1*2DIGIT]
            "8110",     // couponIDNAParameter          = "8110=" 1*70XCHAR
            "8111",     // pointsParameter              = "8111=" 4DIGIT
            "8112",     // paperlessCouponIDNAParameter = "8112=" 1*70XCHAR
            "4300",     // shipToCompParameter          = "4300=" 1*35XCHAR
            "4301",     // shipToNameParameter          = "4301=" 1*35XCHAR
            "4302",     // shipToAdd1Parameter          = "4302=" 1*70XCHAR
            "4303",     // shipToAdd2Parameter          = "4303=" 1*70XCHAR
            "4304",     // shipToSubParameter           = "4304=" 1*70XCHAR
            "4305",     // shipToLocalityParameter      = "4305=" 1*70XCHAR
            "4306",     // shipToRegParameter           = "4306=" 1*70XCHAR
            "4307",     // shipToCountryParameter       = "4307=" 2XCHAR
            "4308",     // shipToPhoneParameter         = "4308=" 1*30XCHAR
            "4309",     // shipToGeoParameter           = "4309=" 20DIGIT
            "4310",     // rtnToCompParameter           = "4310=" 1*35XCHAR
            "4311",     // rtnToNameParameter           = "4311=" 1*35XCHAR
            "4312",     // rtnToAdd1Parameter           = "4312=" 1*70XCHAR
            "4313",     // rtnToAdd2Parameter           = "4313=" 1*70XCHAR
            "4314",     // rtnToSubParameter            = "4314=" 1*70XCHAR
            "4315",     // rtnToLocParameter            = "4315=" 1*70XCHAR
            "4316",     // rtnToRegParameter            = "4316=" 1*70XCHAR
            "4317",     // rtnToCountryParameter        = "4317=" 2XCHAR
            "4318",     // rtnToPostParameter           = "4318=" 1*20XCHAR
            "4319",     // rtnToPhoneParameter          = "4319=" 1*30XCHAR
            "4320",     // srvDescriptionParameter      = "4320=" 1*35XCHAR
            "4321",     // dangerousGoodsParameter      = "4321=" BOOLEAN
            "4322",     // authToLeaveParameter         = "4322=" BOOLEAN
            "4323",     // sigRequiredParameter         = "4323=" BOOLEAN
            "4324",     // notBeforeDelDateParameter    = "4324=" 10DIGIT
            "4325",     // notAfterDelDateParameter     = "4325=" 10DIGIT
            "4326",     // releaseDateParameter         = "4326=" 6DIGIT
            "4330",     // maxTempFParameter            = "4330=" 6DIGIT ["-"]
            "4331",     // maxTempCParameter            = "4331=" 6DIGIT ["-"]
            "4332",     // minTempFParameter            = "4332=" 6DIGIT ["-"]
            "4333",     // minTempCParameter            = "4333=" 6DIGIT ["-"]

            // amountPayPerUnitCode - amountPayPerUnitValue = 6DIGIT - amountPayPerUnitParameter = amountPayPerUnitCode "=" amountPayPerUnitValue
            "3950","3951","3952","3953","3954","3955",

            // gtinParameter        = gtin-code "=" gtin-value
            // itipParameter        = itip-code "=" itip-value
            // gmnParameter         = gmn-code "=" gmn-value
            // cpidParameter        = cpid-code "=" cpid-value
            // glnParameter         = gln-code "=" gln-value
            // payToParameter       = payTo-code "=" payTo-value
            // partyGlnParameter    = partyGln-code "=" partyGln-value
            // gsrnpParameter       = gsrnp-code "=" gsrnp-value
            // gsrnParameter        = gsrn-code "=" gsrn-value
            // gcnParameter         = gcn-code "=" gcn-value
            // ssccParameter        = sscc-code "=" sscc-value
            // gdtiParameter        = gdti-code "=" gdti-value
            // gincParameter        = ginc-code "=" ginc-value
            // gsinParameter        = gsin-code "=" gsin-value
            // graiParameter        = grai-code "=" "0" grai-value
            // giaiParameter        = giai-code "=" giai-value
            
            // internalCode - internalValue = 1*90XCHAR - internalParameter = internalCode "=" internalValue
            "91","92","93","94","95","96","97","98","99",
            
            // lotParameter         = lot-code "=" lot-value

            "17",       // expiryDateParameter  = "17=" 6DIGIT
            "7003",     // expiryTimeParameter  = "7003=" 10DIGIT
        ];

        // 39-character subset of ISO/IEC 646 (for GS1 identifiers) - YCHAR = DIGIT / UPPERALPHA / YSYMBOL 
        private static readonly string _YCHAR_39CharacterSubset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-#/";

        // 64-character subset of ISO/IEC 646 (for GS1 identifiers) - ZCHAR = DIGIT / UPPERALPHA / LOWERALPHA / ZSYMBOL
        private static readonly string _ZCHAR_64CharacterSubset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_=";

        // 82-character subset of ISO/IEC 646 (for GS1 identifiers) - XCHAR = DIGIT / UPPERALPHA / LOWERALPHA / XSYMBOL
        private static readonly string _XCHAR_82CharacterSubset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz\"-._!%&+,/*[]';:<>=?";

        // Dictionary to hold GS1 keys and their corresponding values
        private static readonly Dictionary<string, string> gs1 = [];

        // Error message for GS1 identifier validation
        private static string cErrorMessage = string.Empty;


        /// <summary>
        /// Splits the GS1 identifier into key-value pairs and adds them to the dictionary
        /// Keys are surrounded by slashes (/) or start with a question mark (?) or start with an ampersand (&)
        /// and values are separated by equal signs (=) for keys started with question mark or ampersand.
        /// Example: /00/195201234567891232?02=09520123456788&37=25&10=ABC123
        ///          /01/09520123456788?8004=9520614141234567
        /// </summary>
        /// <param name="gs1Identifier"></param>
        private static void SplitKeyValuePairs(string gs1Identifier)
        {
            gs1.Clear();

            if (string.IsNullOrWhiteSpace(gs1Identifier))
            {
                return;
            }

            // Split into path part (/01/09520123456788/...) and query part (?10=ABC123&37=25)
            int queryStart = gs1Identifier.IndexOf('?');
            string pathPart = queryStart >= 0 ? gs1Identifier[..queryStart] : gs1Identifier;
            string queryPart = queryStart >= 0 ? gs1Identifier[(queryStart + 1)..] : string.Empty;

            // Parse slash-based key/value pairs: /01/09520123456788/21/ABC123
            string[] pathSegments = pathPart.Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i + 1 < pathSegments.Length; i += 2)
            {
                string key = pathSegments[i];
                string value = pathSegments[i + 1];
                gs1[key] = value;
            }

            // Parse query-based key/value pairs: 10=ABC123&37=25
            if (!string.IsNullOrWhiteSpace(queryPart))
            {
                string[] queryPairs = queryPart.Split('&', StringSplitOptions.RemoveEmptyEntries);
                foreach (string pair in queryPairs)
                {
                    int equalsIndex = pair.IndexOf('=');
                    if (equalsIndex > 0)
                    {
                        string key = pair[..equalsIndex];
                        string value = pair[(equalsIndex + 1)..];
                        gs1[key] = value;
                    }
                }
            }

            // Delete duplicate keys in the dictionary (if any) - keep the first occurrence
            //Dictionary<string, string> dict = gs1
            //    .GroupBy(x => x.Key)
            //    .ToDictionary(g => g.Key, g => g.First().Value);
        }

        /// <summary>
        /// Determines whether the specified string is a valid GS1 identifier
        /// </summary>
        /// <param name="editor"></param>
        /// <returns></returns>
        public static async Task<bool> IsValidGS1Identifier(Editor editor)
        {
            bool bIsValid = true;

            SplitKeyValuePairs(editor.Text);

            Debug.WriteLine($"Length array _knownGs1KeysData: {_knownGs1KeysData.Length}");

            foreach (var kvp in gs1)
            {
                Debug.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}, Value length: {kvp.Value.Length}");
                bIsValid &= await IsValidGS1IdentifierPart2(editor, kvp.Key, kvp.Value);
            }

            return bIsValid;
        }

        /// <summary>
        /// Determines whether the specified string is a valid GS1 identifier
        /// </summary>
        /// <param name="editor">The Editor containing the GS1 identifier string to validate.</param>
        /// <param name="cKey">The GS1 key to validate.</param>
        /// <param name="cValue">The GS1 value to validate.</param>
        /// <returns>true if the specified string is a valid GS1 identifier; otherwise, false.</returns>
        private static async Task<bool> IsValidGS1IdentifierPart2(Editor editor, string cKey, string cValue)
        {
            // GS1 Digital Link Test Suite: https://ref.gs1.org/test-suites/resolver/
            // GS1 Digital Link Validator: https://qrbolt.com/en/validate-gs1
            // FeatherQR Playground: https://guitarrapc.github.io/FeatherQR/

            string gs1Identifier = editor.Text;
            bool bIsValid = true;
            cErrorMessage = string.Empty;

            // Check if the key in the GS1 identifier exists in the array of valid keys
            bIsValid &= !ContainsInvalidKeyOrDataAttribute(gs1Identifier);
            if (!bIsValid)
            {
                cErrorMessage = CodeLang.ErrorGS1KeyInvalid_Text;
            }

            // Primary identification key formats (x 16)

            // gtin-code 01 - Global Trade Item Number (GTIN) = 14DIGIT ; GTIN-8, GTIN-12 and GTIN-13 SHALL be expressed as 14 digits, with leading zeroes serving as filler digits
            if (bIsValid && cKey == "01")
                {
                // If the GS1 identifier length is less than 14 characters, add zeroes to the left to make it 14 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 14)
                {
                    cValue = cValue.PadLeft(14, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetKey("01", gs1Identifier), gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The GTIN must have a valid check digit (/01/09520123456788 is valid, /01/09520123456789 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 14))
                {
                    bIsValid = false;
                }

                // Check the path element order for /22/, /10/, and /21/ if they exist in the GS1 identifier
                if (bIsValid)
                {
                    int nStartPos22 = GetKeyStartPosition("/22/", gs1Identifier);
                    int nStartPos10 = GetKeyStartPosition("/10/", gs1Identifier);
                    int nStartPos21 = GetKeyStartPosition("/21/", gs1Identifier);
                    Debug.WriteLine($"Start Position 22: {nStartPos22}");
                    Debug.WriteLine($"Start Position 10: {nStartPos10}");
                    Debug.WriteLine($"Start Position 21: {nStartPos21}");

                    // nStartPos22 must be less than nStartPos10 and nStartPos10 must be less than nStartPos21 - /22/, /10/, and /21/
                    if (nStartPos22 != -1 && nStartPos10 != -1 && nStartPos21 != -1)
                    {
                        if (!(nStartPos22 < nStartPos10 && nStartPos10 < nStartPos21))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only two of the three keys exist, check their order - /22/ and /10/
                    else if (nStartPos22 != -1 && nStartPos10 != -1 && nStartPos21 == -1)
                    {
                        if (!(nStartPos22 < nStartPos10))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only two of the three keys exist, check their order - /22/ and /21/
                    else if (nStartPos22 != -1 && nStartPos10 == -1 && nStartPos21 != -1)
                    {
                        if (!(nStartPos22 < nStartPos21))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only two of the three keys exist, check their order - /10/ and /21/
                    else if (nStartPos22 == -1 && nStartPos10 != -1 && nStartPos21 != -1)
                    {
                        if (!(nStartPos10 < nStartPos21))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only one of the three keys exist, no order check is needed

                    // If the order of the keys is invalid, set the error message
                    if (!bIsValid)
                    {
                        cErrorMessage = CodeLang.ErrorGS1KeyOrderInvalid_Text;
                    }
                }
            }

            // itip-code 8006 - International Ticketing Information Protocol (ITIP) = 14DIGIT 2DIGIT 2DIGIT ; 14 digits then 2 digits then 2 digits
            else if (bIsValid && cKey == "8006")
                {
                // If the GS1 identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 18)
                {
                    cValue = cValue.PadLeft(18, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetKey("8006", gs1Identifier), gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The ITIP must have a valid check digit (/8006/095201234567881122 is valid, /8006/095201234567891122 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue[..14], 14))
                {
                    bIsValid = false;
                }

                // Check the path element order for /22/, /10/, and /21/ if they exist in the GS1 identifier
                if (bIsValid)
                {
                    int nStartPos22 = GetKeyStartPosition("/22/", gs1Identifier);
                    int nStartPos10 = GetKeyStartPosition("/10/", gs1Identifier);
                    int nStartPos21 = GetKeyStartPosition("/21/", gs1Identifier);

                    // nStartPos22 must be less than nStartPos10 and nStartPos10 must be less than nStartPos21 - /22/, /10/, and /21/
                    if (nStartPos22 != -1 && nStartPos10 != -1 && nStartPos21 != -1)
                    {
                        if (!(nStartPos22 < nStartPos10 && nStartPos10 < nStartPos21))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only two of the three keys exist, check their order - /22/ and /10/
                    else if (nStartPos22 != -1 && nStartPos10 != -1 && nStartPos21 == -1)
                    {
                        if (!(nStartPos22 < nStartPos10))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only two of the three keys exist, check their order - /22/ and /21/
                    else if (nStartPos22 != -1 && nStartPos10 == -1 && nStartPos21 != -1)
                    {
                        if (!(nStartPos22 < nStartPos21))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only two of the three keys exist, check their order - /10/ and /21/
                    else if (nStartPos22 == -1 && nStartPos10 != -1 && nStartPos21 != -1)
                    {
                        if (!(nStartPos10 < nStartPos21))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only one of the three keys exist, no order check is needed

                    // If the order of the keys is invalid, set the error message
                    if (!bIsValid)
                    {
                        cErrorMessage = CodeLang.ErrorGS1KeyOrderInvalid_Text;
                    }
                }
            }

            // gmn-code 8013 - Global Model Number (GMN) = 1*25XCHAR ; 1-25 characters from 82-character subset
            else if (bIsValid && cKey == "8013")
                {
                // If the GS1 identifier does not have a length of 1-25 characters, return false
                if (cValue.Length is < 1 or > 25)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsXCHAR82CharacterSubset(cValue);
                }
            }

            // cpid-code 8010 - Component / Part Identifier (CPID) = 1*30YCHAR ; 1-30 characters from 39-character subset
            else if (bIsValid && cKey == "8010")
            {
                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 39-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsYCHAR39CharacterSubset(cValue);
                }
            }

            // gln-code 414 - Physical / Global Location Number (GLN) = 13DIGIT ; exactly 13 digits
            else if (bIsValid && cKey == "414")
            {
                // If the GS1 identifier length is less than 13 characters, add zeroes to the left to make it 13 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 13)
                {
                    cValue = cValue.PadLeft(13, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetKey("414", gs1Identifier), gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The GLN must have a valid check digit (/414/9520123456788 is valid, /414/9520123456780 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 13))
                {
                    bIsValid = false;
                }
            }

            // payTo-code 415 - GLN of invoicing party = 13DIGIT ; exactly 13 digits
            else if (bIsValid && cKey == "415")
            {
                // If the GS1 identifier does not have a length of 13 characters, return false
                if (cValue.Length != 13)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // partyGln-code 417 - Party GLN = 13DIGIT ; exactly 13 digits
            else if (bIsValid && cKey == "417")
            {
                // If the GS1 identifier does not have a length of 13 characters, return false
                if (cValue.Length != 13)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // gsrnp-code 8017 -  GSRN of the Provider = 18DIGIT ; exactly 18 digits
            else if (bIsValid && cKey == "8017")
            {
                // If the GS1 identifier does not have a length of 18 characters, return false
                if (cValue.Length != 18)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // gsrn-code 8018 - GSRN of the Recipient = 18DIGIT ; exactly 18 digits
            else if (bIsValid && cKey == "8018")
            {
                // If the GS1 identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 18)
                {
                    cValue = cValue.PadLeft(18, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetKey("8018", gs1Identifier), gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // If the GS1 identifier does not have a length of 18 characters, return false
                if (cValue.Length != 18)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The GSRN must have a valid check digit (/8018/195201234567891232 is valid, /8018/195201234567891230 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 18))
                {
                    bIsValid = false;
                }
            }

            // gcn-code 255 - Global Coupon Number (GCN) =  13DIGIT [1*12DIGIT] ; 13 digits then optional 1 - 12 digits
            else if (bIsValid && cKey == "255")
            {
                // If the GS1 identifier does not have a length of 13-25 characters, return false
                if (cValue.Length is < 13 or > 25)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // If the GCN identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 13)
                {
                    cValue = cValue.PadLeft(13, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetKey("255", gs1Identifier), gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The GCN must have a valid check digit (/255/0952012345678 is valid, /255/0952012345679 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue[..13], 13))
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // sscc-code 00 - Serial Shipping Container Code (SSCC) = 18DIGIT ; exactly 18 digits 
            else if (bIsValid && cKey == "00")
            {
                // If the GS1 identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 18)
                {
                    cValue = cValue.PadLeft(18, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetKey("00", gs1Identifier), gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The GTIN must have a valid check digit (/00/195201234567891232 is valid, /00/195201234567891230 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 18))
                {
                    bIsValid = false;
                }
            }

            // gdti-code 253 - Global Document Type Identifier (GDTI) = 13DIGIT [1*17XCHAR] ; 13 digits then optional 1 - 17 characters ; from the 82-character subset
            else if (bIsValid && cKey == "253")
            {
                // If the GS1 identifier does not have a length of 13-30 characters, return false
                if (cValue.Length is < 13 or > 30)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The first 13characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue[..13]);
                }

                // The characters of the GS1 identifier must be from the 82-character subset (from character 14 to the end)
                else if (bIsValid && cValue.Length > 13)
                {
                    bIsValid = ValueIsXCHAR82CharacterSubset(cValue[13..]);
                }
            }

            // ginc-code 401 - Global Identification Number for Consignment (GINC) = 1*30XCHAR ; 1 - 30 characters from the 82-character subset
            else if (bIsValid && cKey == "401")
            {
                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsXCHAR82CharacterSubset(cValue);
                }
            }

            // gsin-code 402 - Global Shipment Identification Number (GSIN) = 17DIGIT ; exactly 17 digits
            else if (bIsValid && cKey == "402")
            {
                // If the GS1 identifier length is less than 14 characters, add zeroes to the left to make it 14 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 17)
                {
                    cValue = cValue.PadLeft(17, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetKey("402", gs1Identifier), gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // If the GS1 identifier does not have a length of 17 characters, return false
                if (cValue.Length != 17)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The GSIN must have a valid check digit (/402/9520123456788 is valid, /402/09520123456789000 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 17))
                {
                    bIsValid = false;
                }
            }

            // grai-code 8003 - Global Returnable Asset Identifier (GRAI) = 13DIGIT [1*16XCHAR] ; 13 digits then optional 1 - 16 characters ; from the 82-character subset of ISO / IEC 646
            else if (bIsValid && cKey == "8003")
            {
                // If the GS1 identifier does not have a length of 13-29 characters, return false
                if (cValue.Length is < 13 or > 29)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset (from character 14 to the end)
                else if (bIsValid && cValue.Length > 13)
                {
                    bIsValid = ValueIsXCHAR82CharacterSubset(cValue[13..]);
                }

                // If the GS1 identifier length is less than 13 characters, add zeroes to the left to make it 13 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 13)
                {
                    cValue = cValue.PadLeft(13, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetKey("8003", gs1Identifier), gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The GRAI must have a valid check digit (/8003/9520123456788 is valid, /8003/9520123456789 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 13))
                {
                    bIsValid = false;
                }
            }

            // giai-code 8004 - Global Individual Asset Identifier (GIAI) = 1*30XCHAR  ; 1-30 characters from 82-character subset
            else if (bIsValid && cKey == "8004")
            {
                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsXCHAR82CharacterSubset(cValue);
                }
            }

            // Key qualifier formats ( x 9)

            // cpv-code 22 - Consumer Product Variant (CPV) = 1*20XCHAR ; 1-20 characters from 82-character subset
            else if (bIsValid && cKey == "22")
            {
                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsXCHAR82CharacterSubset(cValue);
                }
            }

            // lot-code 10 - Batch/Lot identifier = 1*20XCHAR ; 1-20 characters from 82-character subset
            else if (bIsValid && cKey == "10")
            {
                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsXCHAR82CharacterSubset(cValue);
                }
            }

            // ser-code 21 - GTIN Serial Number = 1*20XCHAR ; 1-20 characters from 82-character subset
            else if (bIsValid && cKey == "21")
            {
                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsXCHAR82CharacterSubset(cValue);
                }
            }

            // cpsn-code 8011 - CPID Serial Number = 1*12DIGIT ; 1-12 digits
            else if (bIsValid && cKey == "8011")
            {
                // If the GS1 identifier does not have a length of 1-12 digits, return false
                if (cValue.Length is < 1 or > 12)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // glnx-code 254 - GLN extension = 1*20XCHAR ; 1-20 characters from 82-character subset
            else if (bIsValid && cKey == "254")
            {
                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsXCHAR82CharacterSubset(cValue);
                }
            }

            // refno-code 8020 - Payment Reference Number = 1*25XCHAR ; 1-25 characters from 82-character subset
            else if (bIsValid && cKey == "8020")
            {
                // If the GS1 identifier does not have a length of 1-25 characters, return false
                if (cValue.Length is < 1 or > 25)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsXCHAR82CharacterSubset(cValue);
                }
            }

            // srin-code 8019 - Service Relation Instance Number = 1*10DIGIT ; 1-10 digits
            else if (bIsValid && cKey == "8019")
            {
                // If the GS1 identifier does not have a length of 1-10 digits, return false
                if (cValue.Length is < 1 or > 10)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // tpx-code 235 - third-party controlled serialised extension to GTIN = 1*28XCHAR ; 1-28 characters from 82-character subset
            else if (bIsValid && cKey == "235")
            {
                // If the GS1 identifier does not have a length of 1-28 characters, return false
                if (cValue.Length is < 1 or > 28)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsXCHAR82CharacterSubset(cValue);
                }
            }

            // uic-ext-code 7040 - GS1 UIC with Extension 1 and Importer Index = 1DIGIT 3XCHAR ; 1 digit then 3 characters from 82-character subset
            else if (bIsValid && cKey == "7040")
            {
                // If the GS1 identifier does not have a length of 4 characters, return false
                if (cValue.Length != 4)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The first character of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue[..1]);
                }

                // The characters of the GS1 identifier must be from the 82-character subset (from character 2 to the end)
                else if (bIsValid && cValue.Length > 1)
                {
                    bIsValid = ValueIsXCHAR82CharacterSubset(cValue[1..]);
                }
            }

            // Validate the GS1 identifier using the ValidateDataAttribute method, which checks the overall structure and format of the GS1 identifier
            else if (bIsValid)
            {
                bIsValid = ValidateDataAttribute(cKey, cValue);
            }

            // If the GS1 identifier is not valid, display an error message and return false to indicate that the validation failed
            if (!bIsValid)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{cKey}\n{cErrorMessage}", CodeLang.ButtonClose_Text);
                _ = editor.Focus();
                return false;
            }

            return bIsValid;
        }

        /// <summary>
        /// Validation method for GS1 identifier data attributes
        /// </summary>
        /// <param name="dataAttribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool ValidateDataAttribute(string dataAttribute, string value)
        {
            cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;

            Debug.WriteLine($"Validating data attribute: {dataAttribute} - Value: {value}");

            // Boolean

            // BOOLEAN
            if (dataAttribute is "4321" or "4322" or "4323")
            {
                if (value.Length != 1 || value != "0" || value != "1" || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // Characters

            // 1 * 2 XCHAR
            else if (dataAttribute is "7010")
            {
                if (value.Length is < 1 or > 2 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 1 * 3 XCHAR
            else if (dataAttribute is "427" or "7008")
            {
                if (value.Length is < 1 or > 3 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            //  1 * 4 XCHAR
            else if (dataAttribute is "7041")
            {
                if (value.Length is < 1 or > 4 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 1 * 10 XCHAR
            else if (dataAttribute is "7009" or "7255")
            {
                if (value.Length is < 1 or > 10 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 1 * 12 XCHAR
            else if (dataAttribute is "7005")
            {
                if (value.Length is < 1 or > 12 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 1 * 20 XCHAR
            else if (dataAttribute is "243" or "420" or "710" or "711" or "712" or "713" or "714" or "715" or "716" or "717"
                or "7020" or "7021" or "7022" or "7240" or "8002" or "8012" or "4318")
            {
                if (value.Length is < 1 or > 20 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 1 * 25 XCHAR
            else if (dataAttribute is "7242")
            {
                if (value.Length is < 1 or > 25 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 1 * 30 XCHAR
            else if (dataAttribute is "90" or "240" or "241" or "250" or "251"
                or "400" or "403" or "7002" or "7023" or "4308" or "4319")
            {
                if (value.Length is < 1 or > 30 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 1 * 34 XCHAR
            else if (dataAttribute is "8007")
            {
                if (value.Length is < 1 or > 34 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 1 * 35 XCHAR
            else if (dataAttribute is "4300" or "4301" or "4310" or "4311" or "4320")
            {
                if (value.Length is < 1 or > 35 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 1 * 40 XCHAR
            else if (dataAttribute is "7253" or "7254" or "7259")
            {
                if (value.Length is < 1 or > 40 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 1 * 50 XCHAR
            else if (dataAttribute is "8009")
            {
                if (value.Length is < 1 or > 50 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 1 * 70 XCHAR
            else if (dataAttribute is "7257" or "8110" or "8112" or "4302" or "4303" or "4304" or "4305" or "4306"
                or "4312" or "4313" or "4314" or "4315" or "4316")
            {
                if (value.Length is < 1 or > 70 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 1 * 90 XCHAR
            else if (dataAttribute is "7256" or "91" or "92" or "93" or "94" or "95" or "96" or "97" or "98" or "99")
            {
                if (value.Length is < 1 or > 90 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 1 * 90 ZCHAR
            else if (dataAttribute is "8030")
            {
                if (value.Length is < 1 or > 90 || !ValueIsZCHAR64CharacterSubset(value))
                {
                    return false;
                }
            }

            // 2 XCHAR
            else if (dataAttribute is "4307" or "4317")
            {
                if (value.Length != 2 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // 2 XCHAR 1 * 28 XCHAR
            else if (dataAttribute is "7230" or "7231" or "7232" or "7233" or "7234" or "7235" or "7236" or "7237" or "7238" or "7239")
            {
                if (value.Length is < 3 or > 30 || !ValueIsXCHAR82CharacterSubset(value))
                {
                    return false;
                }
            }

            // Digits

            // 1 * 4 DIGIT
            else if (dataAttribute is "7004")
            {
                if (value.Length is < 1 or > 4 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 * 6 DIGIT
            else if (dataAttribute is "242")
            {
                if (value.Length is < 1 or > 6 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 * 8 DIGIT
            else if (dataAttribute is "30" or "37")
            {
                if (value.Length is < 1 or > 8 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 * 15 DIGIT
            else if (dataAttribute is "3900" or "3901" or "3902" or "3903" or "3904" or "3905" or "3906" or "3907" or "3908" or "3909"
                or "3920" or "3921" or "3922" or "3923" or "3924" or "3925" or "3926" or "3927" or "3928" or "3929")
            {
                if (value.Length is < 1 or > 15 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 DIGIT
            else if (dataAttribute is "7252")
            {
                if (value.Length != 1 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1DIGIT "/" 1DIGIT
            else if (dataAttribute is "7258")
            {
                if (value.Length != 3 || !ValueIsDigit(value[..1]) || value[1] != '/' || !ValueIsDigit(value[2..]))
                {
                    return false;
                }
            }

            // 2 DIGIT
            else if (dataAttribute is "20" or "7241")
            {
                if (value.Length != 2 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 3 DIGIT
            else if (dataAttribute is "422" or "424" or "426")
            {
                if (value.Length != 3 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 3 DIGIT 1 * 12 DIGIT
            else if (dataAttribute is "423" or "425")
            {
                if (value.Length is < 4 or > 15 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 3 DIGIT 1 * 15 DIGIT
            else if (dataAttribute is "3910" or "3911" or "3912" or "3913" or "3914" or "3915" or "3916" or "3917" or "3918" or "3919"
                or "3930" or "3931" or "3932" or "3933" or "3934" or "3935" or "3936" or "3937" or "3938" or "3939")
            {
                if (value.Length is < 4 or > 18 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 4 DIGIT
            else if (dataAttribute is "3940" or "3941" or "3942" or "3943" or "8111")
            {
                if (value.Length != 4 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 6 DIGIT [4 DIGIT]
            else if (dataAttribute is "7011")
            {
                if (value.Length != 6 || value.Length != 10 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 6 DIGIT ["-"]
            else if (dataAttribute is "4330" or "4331" or "4332" or "4333")
            {
                if (value.Length is < 6 or > 7 || !ValueIsDigit(value[..6]))
                {
                    return false;
                }
                
                if (value.Length == 7 && !value.EndsWith('-'))
                {
                    return false;
                }
            }

            // 8 DIGIT
            else if (dataAttribute is "7250")
            {
                if (value.Length != 8 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 13 DIGIT
            else if (dataAttribute is "410" or "411" or "412" or "413" or "416" or "7001")
            {
                if (value.Length != 13 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 14 DIGIT
            else if (dataAttribute is "02" or "8001")
            {
                if (value.Length != 14 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 14 DIGIT 2 DIGIT 2 DIGIT
            else if (dataAttribute is "8026")
            {
                if (value.Length != 18 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 15 DIGIT
            else if (dataAttribute is "8040" or "8041")
            {
                if (value.Length != 15 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 18 DIGIT [1 * 2 DIGIT]
            else if (dataAttribute is "8043")
            {
                if (value.Length != 18 || value.Length != 19 || value.Length != 20 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 20 DIGIT
            else if (dataAttribute is "4309")
            {
                if (value.Length != 20 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 32 DIGIT
            else if (dataAttribute is "8042")
            {
                if (value.Length != 32 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // Mixed/others: digits and characters

            // 3 DIGIT 1 * 9 XCHAR
            else if (dataAttribute is "421")
            {
                if (value.Length is < 4 or > 12 || !ValueIsDigit(value[..3]))
                {
                    return false;
                }
            }

            // 3 DIGIT 1 * 27 XCHAR
            else if (dataAttribute is "7030" or "7031" or "7032" or "7033" or "7034" or "7035" or "7036" or "7037" or "7038" or "7039")
            {
                if (value.Length is < 4 or > 30 || !ValueIsDigit(value[..3]))
                {
                    return false;
                }
            }

            // Dates

            // 6 DIGIT (= date)
            else if (dataAttribute is "11" or "12" or "13" or "15" or "16" or "17" or "4326" or "7006")
            {
                if (value.Length != 6 || !ValueIsDigit(value) || !IsDateYYMMDD(value))
                {
                    return false;
                }
            }

            // 6 DIGIT [4 DIGIT] (= date)
            else if (dataAttribute is "7011")
            {
                if (value.Length != 6 || value.Length != 10 || !ValueIsDigit(value) || !IsDateYYMMDD(value[..6]))
                {
                    return false;
                }
            }

            // 6 DIGIT [6 DIGIT] (= date)
            else if (dataAttribute is "7007")
            {
                if (value.Length != 6 || value.Length != 12 || !ValueIsDigit(value) || !IsDateYYMMDD(value[..6]))
                {
                    return false;
                }
            }

            // 10 DIGIT (= date/time)
            else if (dataAttribute is "4324" or "4325")
            {
                if (value.Length != 10 || !ValueIsDigit(value) || !IsDateYYMMDD(value[..6]))
                {
                    return false;
                }
            }

            // Times

            // 8 DIGIT [2 DIGIT] [2 DIGIT] (= time)
            else if (dataAttribute is "8008")
            {
                if (value.Length != 8 || value.Length != 10 || value.Length != 12 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 10 DIGIT (= date/time)
            else if (dataAttribute is "7003")
            {
                if (value.Length != 10 || !ValueIsDigit(value) || !IsDateYYMMDD(value[..6]) || !IsTimeHHMM(value.Substring(6, 4)))
                {
                    return false;
                }
            }

            // 12 DIGIT (= time)
            else if (dataAttribute is "7251")
            {
                if (value.Length != 12 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // Default 6 digits - Except for the following keys, which have specific validation rules and are handled separately above.

            // 6 DIGIT
            else if (dataAttribute is not "01" and not "8006" and not "8013" and not "8010" and not "414" and not "415" and not "417"
                and not "8017" and not "8018" and not "255" and not "00" and not "253" and not "401" and not "402" and not "8003" and not "8004"
                and not "22" and not "10" and not "21" and not "8011" and not "254" and not "8020" and not "8019" and not "235" and not "7040")
            {
                if (value.Length != 6 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the Key or data attribute of the GS1 identifier
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cGs1Identifier"></param>
        /// <returns></returns>
        private static string GetKey(string key, string cGs1Identifier)
        {
            if (cGs1Identifier.Contains($"/{key}/"))
            {
                return $"/{key}/";
            }
            else if (cGs1Identifier.Contains($"?{key}="))
            {
                return $"?{key}=";
            }
            else if (cGs1Identifier.Contains($"&{key}="))
            {
                return $"&{key}=";
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the value of the Gs1 identifier following a certain key or data attribute. The value is extracted from the
        /// GS1 identifier string based on the position of the key and the next delimiter (forward slash, question mark, or ampersand) or the end of the string.
        /// </summary>
        /// <param name="cKey"></param>
        /// <param name="cGs1Identifier"></param>
        /// <returns></returns>
        private static string GetValue(string cKey, string cGs1Identifier)
        {
            int index = cGs1Identifier.IndexOf(cKey);

            if (index >= 0)
            {
                // Find the start position of the value after the key
                int nStartPosition = index + cKey.Length;

                // Find the end position of the value, which is either the next forward slash or question mark or ampersand or the end of the string
                int nEndPosition = cGs1Identifier.IndexOfAny(['/', '?', '&'], nStartPosition);

                // If no forward slash or question mark or ampersand is found, set the end position to the end of the string
                if (nEndPosition == -1)
                {
                    nEndPosition = cGs1Identifier.Length;
                }

                Debug.WriteLine(string.Concat("cGs1IdentifierValue: ", cGs1Identifier.AsSpan(nStartPosition, nEndPosition - nStartPosition)));

                return cGs1Identifier[nStartPosition..nEndPosition];
            }

            // If no forward slash is found, return an empty string
            return string.Empty;
        }

        /// <summary>
        /// Finds the start index of a key segment within a GS1 identifier string
        /// </summary>
        /// <param name="cKey"></param>
        /// <param name="cGs1Identifier"></param>
        /// <returns></returns>
        private static int GetKeyStartPosition(string cKey, string cGs1Identifier)
        {
            int index = cGs1Identifier.IndexOf(cKey);
            if (index >= 0)
            {
                // Find the start position of the key
                return index;
            }
            
            // If no forward slash is found, return -1
            return -1;
        }

        ///// <summary>
        ///// Finds the start and end indexes of a key segment within a GS1 identifier string.
        ///// </summary>
        ///// <param name="cKey">Key segment to locate.</param>
        ///// <param name="cGs1Identifier">GS1 identifier string to search.</param>
        ///// <returns>A tuple containing the zero-based start index and end index of the located key segment; returns (-1, -1)
        ///// when the key segment is not found. The end index is the position of the next '/' or '?' delimiter after the
        ///// start index, or the string length when no such delimiter exists.</returns>
        ///// <remarks>Use: (int nStartPos22, int nEndPos22) = GetKeyPosition("/22/", gs1Identifier);</remarks>
        //private static (int start, int end) GetKeyPosition(string cKey, string cGs1Identifier)
        //{
        //    int index = cGs1Identifier.IndexOf(cKey);

        //    if (index >= 0)
        //    {
        //        // Find the start position of the key
        //        int nStartPosition = index;

        //        // Find the end position of the key, which is either the next forward slash or question mark or the end of the string
        //        int nEndPosition = cGs1Identifier.IndexOfAny(['/', '?'], nStartPosition + 1);

        //        // If no forward slash or question mark is found, set the end position to the end of the string
        //        if (nEndPosition == -1)
        //        {
        //            nEndPosition = cGs1Identifier.Length;
        //        }

        //        return (nStartPosition, nEndPosition);
        //    }

        //    // If no forward slash is found, return -1 for both start and end positions
        //    return (-1, -1);
        //}

        /// <summary>
        /// Determines whether a string consists only of digit characters - DIGIT
        /// </summary>
        /// <remarks>An empty string returns <see langword="true"/>. This method expects a non-null value.</remarks>
        /// <param name="value">The string to evaluate.</param>
        /// <returns><see langword="true"/> if all characters in <paramref name="value"/> are digits; otherwise, <see langword="false"/>.</returns>
        private static bool ValueIsDigit(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsDigit(c))
                {
                    cErrorMessage = CodeLang.ErrorGS1NumberOnlyDigits_Text;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Validates whether a string represents a date in the `yyMMdd` format using the invariant culture.
        /// </summary>
        /// <remarks>Parsing uses `DateTime.TryParseExact` with `DateTimeStyles.None`. When validation
        /// fails, `cErrorMessage` is set to `CodeLang.ErrorGS1DateInvalid_Text`.</remarks>
        /// <param name="input">Text to validate as a date in `yyMMdd` format.</param>
        /// <returns>`true` if `input` is a valid date in `yyMMdd` format; otherwise, `false`.</returns>
        static bool IsDateYYMMDD(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            // Try parsing with exact format "yyMMdd"
            bool bIsValid = DateTime.TryParseExact(
                input,
                "yyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _ // discard parsed DateTime
            );

            if (!bIsValid)
            {
                cErrorMessage = CodeLang.ErrorGS1DateInvalid_Text;
            }

            return bIsValid;
        }

        /// <summary>
        /// Validate if a string is a time in HHMM format
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static bool IsTimeHHMM(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            // Try parsing with exact format "HHmm"
            bool bIsValid = DateTime.TryParseExact(
                input,
                "HHmm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _ // discard parsed DateTime
            );

            if (!bIsValid)
            {
                cErrorMessage = CodeLang.ErrorGS1TimeInvalid_Text;
            }

            return bIsValid;
        }

        /// <summary>
        /// Check if all characters in the input are in the allowed 39-character subset - YCHAR
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ValueIsYCHAR39CharacterSubset(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                cErrorMessage = CodeLang.ErrorGS1ValueContainsCharacterInvalid_Text;
                return false;
            }

            // Check if all characters in the input are in the allowed set
            bool bIsValid = value.All(c => _YCHAR_39CharacterSubset.Contains(c));
            if (!bIsValid)
            {
                cErrorMessage = CodeLang.ErrorGS1ValueContainsCharacterInvalid_Text;
            }

            return bIsValid;
        }

        // Check if all characters in the input are in the allowed 64-character subset - ZCHAR
        public static bool ValueIsZCHAR64CharacterSubset(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                cErrorMessage = CodeLang.ErrorGS1ValueContainsCharacterInvalid_Text;
                return false;
            }

            // Check if all characters in the input are in the allowed set
            bool bIsValid = value.All(c => _ZCHAR_64CharacterSubset.Contains(c));
            if (!bIsValid)
            {
                cErrorMessage = CodeLang.ErrorGS1ValueContainsCharacterInvalid_Text;
            }

            return bIsValid;
        }

        /// <summary>
        /// Check if all characters in the input are in the allowed 82-character subset - XCHAR
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ValueIsXCHAR82CharacterSubset(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                cErrorMessage = CodeLang.ErrorGS1ValueContainsCharacterInvalid_Text;
                return false;
            }

            // Check if all characters in the input are in the allowed set
            bool bIsValid = value.All(c => _XCHAR_82CharacterSubset.Contains(c));
            if (!bIsValid)
            {
                cErrorMessage = CodeLang.ErrorGS1ValueContainsCharacterInvalid_Text;
            }

            return bIsValid;
        }

        /// <summary>
        /// Calculate the gtin check digit (Modulo10) and validate the value against the check digit
        /// </summary>
        /// <param name="value">The GTIN value to validate.</param>
        /// <param name="length">The expected length of the GTIN value.</param>
        /// <returns>`true` if the GTIN value is valid; otherwise, `false`.</returns>
        public static bool ValidateValueCheckDigitModulo10(string value, int length)
        {
            // Validate input: the value must be a non-empty string containing only digits
            if (string.IsNullOrWhiteSpace(value) || !value.All(char.IsDigit))
            {
                cErrorMessage = CodeLang.ErrorGS1NumberOnlyDigits_Text;
                return false;
            }

            // Store the original value for later comparison
            string valueOriginal = value;

            // If the value is less than length digits return false
            if (value.Length != length)
            {
                cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                return false;
            }

            // If the value is already length digits, we assume the last digit is the check digit and remove it for calculation
            else
            {
                value = value[..^1];
            }

            int sum = 0;
            int weight = 3;

            // Starting from the right, multiply digits by alternating weights of 3 and 1
            for (int i = value.Length - 1; i >= 0; i--)
            {
                sum += (value[i] - '0') * weight;
                weight = weight == 3 ? 1 : 3;
            }

            int checkDigit = (10 - (sum % 10)) % 10;

            // If the last digit of the valueOriginal is equal to the calculated check digit, the value is valid
            if (valueOriginal[^1] != checkDigit.ToString()[0])
            {
                cErrorMessage = CodeLang.ErrorGS1CheckDigitInvalid_Text;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks whether the GS1 identifier contains one or more invalid keys or data attributes.
        /// Returns true when an invalid key or data attribute is found, or when no GS1 key or data attribute is found.
        /// </summary>
        /// <param name="gs1Identifier"></param>
        /// <returns></returns>
        private static bool ContainsInvalidKeyOrDataAttribute(string gs1Identifier)
        {
            if (string.IsNullOrWhiteSpace(gs1Identifier))
            {
                return true;
            }

            bool hasAtLeastOneKey = false;

            // Split once: path + optional query
            string[] uriParts = gs1Identifier.Split('?', 2, StringSplitOptions.None);
            string pathPart = uriParts[0];

            // Parse path keys as alternating key/value segments once GS1 path starts
            string[] segments = pathPart.Split('/', StringSplitOptions.RemoveEmptyEntries);

            bool gs1PathStarted = false;
            int index = 0;

            while (index < segments.Length)
            {
                if (!gs1PathStarted)
                {
                    if (!IsPotentialGs1KeyToken(segments[index]))
                    {
                        index++;
                        continue;
                    }

                    gs1PathStarted = true;
                }

                // Expected key segment
                string key = segments[index];
                hasAtLeastOneKey = true;

                if (!IsPotentialGs1KeyToken(key) || Array.IndexOf(_knownGs1KeysData, key) < 0)
                {
                    return true;
                }

                // Move to value segment
                index++;

                // Key without value -> invalid structure
                if (index >= segments.Length)
                {
                    return true;
                }

                // Skip value segment, next should be next key
                index++;
            }

            // Parse query keys: ?01=...&10=...
            if (uriParts.Length == 2 && !string.IsNullOrWhiteSpace(uriParts[1]))
            {
                string queryPart = uriParts[1];

                foreach (string parameter in queryPart.Split('&', StringSplitOptions.RemoveEmptyEntries))
                {
                    string key = parameter.Split('=', 2, StringSplitOptions.None)[0];

                    if (!IsPotentialGs1KeyToken(key))
                    {
                        continue;
                    }

                    hasAtLeastOneKey = true;

                    if (Array.IndexOf(_knownGs1KeysData, key) < 0)
                    {
                        return true;
                    }
                }
            }

            // No GS1 key found at all -> treat as invalid
            return !hasAtLeastOneKey;
        }

        /// <summary>
        /// Check if a key is a potential GS1 key
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool IsPotentialGs1KeyToken(string token)
        {
            if (string.IsNullOrEmpty(token) || token.Length is < 2 or > 4)
            {
                return false;
            }

            foreach (char c in token)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
