
Version 1.0.0.24 (SIS - 29.11.2019)
1. Updated driver executable from C3Net to C3Driver
2. Updates to EFTEndMessage .Trim('\0'))) added for customer and merchant length string
3. CurrentIndex value changed from 55 to 512 for ExtendedFields != "1"

Version 1.0.0.23 (SIS - 26.06.2018)
-----------------------------
1. Fixed a bug that appeared in the previous version. 'VISA_RRP_DATA' failed to update.
   Update the c3config 'VISA_RRP_DATA' setting with it's appropriate value whenever UpdateSettings command is executed.

Version 1.0.0.20 (SIS - 20.06.2018)
-----------------------------
2. Update the c3config 'VISA_RRP_DATA' setting with it's appropriate value whenever UpdateSettings command is executed.

Version 1.0.0.18 (ASI - 20.06.2018)
-----------------------------
1. Version increase because the driver version needs to match with the loader version, no new changes were added

Version 1.0.0.4 (SIS - 24.11.2017)
----------------------------------
1. Fix. The COM port was not written correctly in the c3config because the setting type was changed to SerialPortSelection

Version 1.0.0.3 (ASI - 24.11.2017)
----------------------------------
1. Added error code and description to the Pay response back to Core

Version 1.0.0.2 (SIS - 20.05.2017)
----------------------------------
1. Added currency selection.

Version 1.0.0.1 (SIS - 15.05.2017)
----------------------------------
1. Printing failure receipt.