namespace BarcodeGenerator
{
    internal partial class ClassPayloadGS1Validation
    {
        // Add once in the class (central place for all supported GS1 keys)
        private static readonly string[] _knownGs1Keys =
        [
            // Primary identification key formats (x 16)
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
            
            // Key qualifier formats ( x 9)
            "22",     // cpv-code
            "10",     // lot-code
            "21",     // ser-code
            "8011",   // cpsn-code
            "254",    // glnx-code
            "8020",   // refno-code
            "8019",   // srin-code
            "235",    // tpx-code
            "7040"    // uic-ext-code
        ];

        private static readonly string[] _dataAttributes =
        [
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

            // contentParameter = "02=" 14DIGIT
            "02",

            // prodDateParameter = "11=" 6DIGIT
            "11",

            // dueDateParameter = "12=" 6DIGIT
            "12",

            // packDateParameter = "13=" 6DIGIT
            "13",

            // bestBeforeDateParameter = "15=" 6DIGIT
            "15",

            // sellByDateParameter = "16=" 6DIGIT
            "16",

            // firstFreezeDateParameter = "7006=" 6DIGIT
            "7006",

            // harvestDateParameter = "7007=" 6DIGIT [6DIGIT]
            "7007",

            // testByDateParameter = "7011=" 6DIGIT [4DIGIT]
            "7011",

            // pricePerUnitParameter = "8005=" 6DIGIT
            "8005",

            // variantParameter = "20=" 2DIGIT
            "20",

            // varCountParameter = "30=" 1*8DIGIT
            "30",

            // countParameter = "37=" 1*8DIGIT
            "37",

            // mutualParameter = "90=" 1*30XCHAR
            "90",

            // additionalIdParameter = "240=" 1*30XCHAR
            "240",

            // custPartNoParameter = "241=" 1*30XCHAR
            "241",

            // mtoVariantParameter = "242=" 1*6DIGIT
            "242",

            // pcnParameter = "243=" 1*20XCHAR
            "243",

            // secondarySerialParameter = "250=" 1*30XCHAR
            "250",

            // refToSourceParameter = "251=" 1*30XCHAR
            "251",

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

            // orderNumberParameter = "400=" 1*30XCHAR
            "400",







            ];

        //  82-character subset of ISO/IEC 646 (for GS1 identifiers)
        private static readonly string _82CharacterSubset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz\"-._!%&+,/*[]';:<>=?";

        // 39-character subset of ISO/IEC 646 (for GS1 identifiers)
        private static readonly string _39CharacterSubset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-#/";

        // Error message for GS1 identifier validation
        private static string cErrorMessage = string.Empty;

        /// <summary>
        /// Determines whether the specified string is a valid GS1 identifier
        /// </summary>
        /// <param name="editor">The Editor containing the GS1 identifier string to validate.</param>
        /// <returns>true if the specified string is a valid GS1 identifier; otherwise, false.</returns>
        public static async Task<bool> IsValidGS1Identifier(Editor editor)
        {
            // GS1 Digital Link Test Suite: https://ref.gs1.org/test-suites/resolver/
            // GS1 Digital Link Validator: https://qrbolt.com/en/validate-gs1
            // FeatherQR Playground: https://guitarrapc.github.io/FeatherQR/

            string gs1Identifier = editor.Text;
            bool bIsValid = true;
            string cValue;
            string cKey = string.Empty;
            cErrorMessage = string.Empty;

            // Check if there are keys in the GS1 identifier that are not in the list of valid keys
            bIsValid &= !ContainsInvalidKey(gs1Identifier);
            if (!bIsValid)
            {
                cErrorMessage = CodeLang.ErrorGS1KeyInvalid_Text;
            }

            // Primary identification key formats (x 16)

            // gtin-code 01 - Global Trade Item Number (GTIN) = 14DIGIT ; GTIN-8, GTIN-12 and GTIN-13 SHALL be expressed as 14 digits, with leading zeroes serving as filler digits
            if (bIsValid && (gs1Identifier.Contains("/01/") || gs1Identifier.Contains("?01=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("01", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier length is less than 14 characters, add zeroes to the left to make it 14 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 14)
                {
                    cValue = cValue.PadLeft(14, '0');
                    editor.Text = gs1Identifier.Replace(GetKeyValue(cKey, gs1Identifier), cValue);
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
            if (bIsValid && (gs1Identifier.Contains("/8006/") || gs1Identifier.Contains("?8006=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("8006", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 18)
                {
                    cValue = cValue.PadLeft(18, '0');
                    editor.Text = gs1Identifier.Replace(GetKeyValue(cKey, gs1Identifier), cValue);
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
            if (bIsValid && (gs1Identifier.Contains("/8013/") || gs1Identifier.Contains("?8013=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("8013", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-25 characters, return false
                if (cValue.Length is < 1 or > 25)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // cpid-code 8010 - Component / Part Identifier (CPID) = 1*30YCHAR ; 1-30 characters from 39-character subset
            if (bIsValid && (gs1Identifier.Contains("/8010/") || gs1Identifier.Contains("?8010=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("8010", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 39-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs39CharacterSubset(cValue);
                }
            }

            // gln-code 414 - Physical / Global Location Number (GLN) = 13DIGIT ; exactly 13 digits
            if (bIsValid && (gs1Identifier.Contains("/414/") || gs1Identifier.Contains("?414=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("414", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier length is less than 13 characters, add zeroes to the left to make it 13 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 13)
                {
                    cValue = cValue.PadLeft(13, '0');
                    editor.Text = gs1Identifier.Replace(GetKeyValue(cKey, gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The GLN must have a valid check digit (/414/9520123456788 is valid, /414/9520123456780 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 13))
                {
                    bIsValid = false;
                }
            }

            // payTo-code 415 - GLN of invoicing party = 13DIGIT ; exactly 13 digits
            if (bIsValid && (gs1Identifier.Contains("/415/") || gs1Identifier.Contains("?415=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("415", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

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
            if (bIsValid && (gs1Identifier.Contains("/417/") || gs1Identifier.Contains("?417=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("417", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

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
            if (bIsValid && (gs1Identifier.Contains("/8017/") || gs1Identifier.Contains("?8017=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("8017", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

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
            if (bIsValid && (gs1Identifier.Contains("/8018/") || gs1Identifier.Contains("?8018=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("8018", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 18)
                {
                    cValue = cValue.PadLeft(18, '0');
                    editor.Text = gs1Identifier.Replace(GetKeyValue(cKey, gs1Identifier), cValue);
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
            if (bIsValid && (gs1Identifier.Contains("/255/") || gs1Identifier.Contains("?255=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("255", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

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
                    editor.Text = gs1Identifier.Replace(GetKeyValue(cKey, gs1Identifier), cValue);
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
            if (bIsValid && (gs1Identifier.Contains("/00/") || gs1Identifier.Contains("?00=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("00", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 18)
                {
                    cValue = cValue.PadLeft(18, '0');
                    editor.Text = gs1Identifier.Replace(GetKeyValue(cKey, gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The GTIN must have a valid check digit (/00/195201234567891232 is valid, /00/195201234567891230 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 18))
                {
                    bIsValid = false;
                }
            }

            // gdti-code 253 - Global Document Type Identifier (GDTI) = 13DIGIT [1*17XCHAR] ; 13 digits then optional 1 - 17 characters ; from the 82-character subset
            if (bIsValid && (gs1Identifier.Contains("/253/") || gs1Identifier.Contains("?253=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("253", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

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
                    bIsValid = ValueIs82CharacterSubset(cValue[13..]);
                }
            }

            // ginc-code 401 - Global Identification Number for Consignment (GINC) = 1*30XCHAR ; 1 - 30 characters from the 82-character subset
            if (bIsValid && (gs1Identifier.Contains("/401/") || gs1Identifier.Contains("?401=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("401", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // gsin-code 402 - Global Shipment Identification Number (GSIN) = 17DIGIT ; exactly 17 digits
            if (bIsValid && (gs1Identifier.Contains("/402/") || gs1Identifier.Contains("?402=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("402", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier length is less than 14 characters, add zeroes to the left to make it 14 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 17)
                {
                    cValue = cValue.PadLeft(17, '0');
                    editor.Text = gs1Identifier.Replace(GetKeyValue(cKey, gs1Identifier), cValue);
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
            if (bIsValid && (gs1Identifier.Contains("/8003/") || gs1Identifier.Contains("?8003=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("8003", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 13-29 characters, return false
                if (cValue.Length is < 13 or > 29)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset (from character 14 to the end)
                else if (bIsValid && cValue.Length > 13)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue[13..]);
                }

                // If the GS1 identifier length is less than 13 characters, add zeroes to the left to make it 13 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 13)
                {
                    cValue = cValue.PadLeft(13, '0');
                    editor.Text = gs1Identifier.Replace(GetKeyValue(cKey, gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The GRAI must have a valid check digit (/8003/9520123456788 is valid, /8003/9520123456789 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 13))
                {
                    bIsValid = false;
                }
            }

            // giai-code 8004 - Global Individual Asset Identifier (GIAI) = 1*30XCHAR  ; 1-30 characters from 82-character subset
            if (bIsValid && (gs1Identifier.Contains("/8004/") || gs1Identifier.Contains("?8004=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("8004", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // Key qualifier formats ( x 9)

            // cpv-code 22 - Consumer Product Variant (CPV) = 1*20XCHAR ; 1-20 characters from 82-character subset
            if (bIsValid && (gs1Identifier.Contains("/22/") || gs1Identifier.Contains("?22=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("22", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // lot-code 10 - Batch/Lot identifier = 1*20XCHAR ; 1-20 characters from 82-character subset
            if (bIsValid && (gs1Identifier.Contains("/10/") || gs1Identifier.Contains("?10=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("10", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // ser-code 21 - GTIN Serial Number = 1*20XCHAR ; 1-20 characters from 82-character subset
            if (bIsValid && (gs1Identifier.Contains("/21/") || gs1Identifier.Contains("?21=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("21", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // cpsn-code 8011 - CPID Serial Number = 1*12DIGIT ; 1-12 digits
            if (bIsValid && (gs1Identifier.Contains("/8011/") || gs1Identifier.Contains("?8011=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("8011", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

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
            if (bIsValid && (gs1Identifier.Contains("/254/") || gs1Identifier.Contains("?254=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("254", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // refno-code 8020 - Payment Reference Number = 1*25XCHAR ; 1-25 characters from 82-character subset
            if (bIsValid && (gs1Identifier.Contains("/8020/") || gs1Identifier.Contains("?8020=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("8020", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-25 characters, return false
                if (cValue.Length is < 1 or > 25)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // srin-code 8019 - Service Relation Instance Number = 1*10DIGIT ; 1-10 digits
            if (bIsValid && (gs1Identifier.Contains("/8019/") || gs1Identifier.Contains("?8019=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("8019", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

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
            if (bIsValid && (gs1Identifier.Contains("/235/") || gs1Identifier.Contains("?235=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("235", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-28 characters, return false
                if (cValue.Length is < 1 or > 28)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // uic-ext-code 7040 - GS1 UIC with Extension 1 and Importer Index = 1DIGIT 3XCHAR ; 1 digit then 3 characters from 82-character subset
            if (bIsValid && (gs1Identifier.Contains("/7040/") || gs1Identifier.Contains("?7040=")))
            {
                // Get the key and value of the GS1 identifier following the key
                cKey = GetKey("7040", gs1Identifier);
                cValue = GetKeyValue(cKey, gs1Identifier);

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
                    bIsValid = ValueIs82CharacterSubset(cValue[1..]);
                }
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
        /// Get the Key of the GS1 identifier
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

            return string.Empty;
        }

        /// <summary>
        /// Get the value of the Gs1 identifier following a certain key
        /// </summary>
        /// <param name="cKey"></param>
        /// <param name="cGs1Identifier"></param>
        /// <returns></returns>
        private static string GetKeyValue(string cKey, string cGs1Identifier)
        {
            int index = cGs1Identifier.IndexOf(cKey);

            if (index >= 0)
            {
                // Find the start position of the value after the key
                int nStartPosition = index + cKey.Length;

                // Find the end position of the value, which is either the next forward slash or question mark or the end of the string
                int nEndPosition = cGs1Identifier.IndexOfAny(['/', '?'], nStartPosition);

                // If no forward slash or question mark is found, set the end position to the end of the string
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
        /// Determines whether a string consists only of digit characters.
        /// </summary>
        /// <remarks>An empty string returns <see langword="true"/>. This method expects a non-null value.</remarks>
        /// <param name="cValue">The string to evaluate.</param>
        /// <returns><see langword="true"/> if all characters in <paramref name="cValue"/> are digits; otherwise, <see langword="false"/>.</returns>
        private static bool ValueIsDigit(string cValue)
        {
            foreach (char c in cValue)
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
        /// Check if all characters in the input are in the allowed 39-character subset
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ValueIs39CharacterSubset(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                cErrorMessage = CodeLang.ErrorGS1ValueContainsCharacterInvalid_Text;
                return false;
            }

            // Check if all characters in the input are in the allowed set
            bool bIsValid = value.All(c => _39CharacterSubset.Contains(c));
            if (!bIsValid)
            {
                cErrorMessage = CodeLang.ErrorGS1ValueContainsCharacterInvalid_Text;
            }

            return bIsValid;
        }

        /// <summary>
        /// Check if all characters in the input are in the allowed 82-character subset
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ValueIs82CharacterSubset(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                cErrorMessage = CodeLang.ErrorGS1ValueContainsCharacterInvalid_Text;
                return false;
            }

            // Check if all characters in the input are in the allowed set
            bool bIsValid = value.All(c => _82CharacterSubset.Contains(c));
            if (!bIsValid)
            {
                cErrorMessage = CodeLang.ErrorGS1ValueContainsCharacterInvalid_Text;
            }

            return bIsValid;
        }

        /// <summary>
        /// Calculate the gtin check digit
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
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
        /// Checks whether the GS1 identifier contains one or more invalid keys.
        /// Returns true when an invalid key is found, or when no GS1 key is found.
        /// </summary>
        /// <param name="gs1Identifier"></param>
        /// <returns></returns>
        private static bool ContainsInvalidKey(string gs1Identifier)
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

                if (!IsPotentialGs1KeyToken(key) || Array.IndexOf(_knownGs1Keys, key) < 0)
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

                    if (Array.IndexOf(_knownGs1Keys, key) < 0)
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
