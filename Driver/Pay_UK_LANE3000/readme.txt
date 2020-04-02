Version 1.0.0.20 - UNRELEASED VERSION ASI (04.07.2019)
-----------------------------
1. Documentation update with correct path to WindowsDriver

version 1.0.0.19 (SIS 12.06.2019)
--------------------------------
2. Changed the default value of the c3config 'VISA_RRP_DATA' property.

version 1.0.0.18 (SIS 23.11.2017)
--------------------------------
Modified two properties in the sale command that didn't correspond to the certification.
1. I changed the subfunction from ‘00’ to ‘0 ’.
2. Changed ‘CCustomerPresent’ from ‘0’ to ‘1’.

version 1.0.0.17 (ASI 23.11.2017)
--------------------------------
1. Added error code and description to the Pay response back to Core

version 1.0.0.16 (SIS 02.05.2017)
--------------------------------
1. Added currency selection.
2. Added failure ticket saving.

version 1.0.0.10 (SIS 02.05.2017)
--------------------------------
1. Fixed a bug.
   When the payment was executed the Thread was aborted after a period of time because the Payment method was not on a separate thread.

version 1.0.0.6 (SIS 18.04.2017)
--------------------------------
First version.