# structure
$purchasing_web_directory = "C:\Applications\PurchasingTest"
$vendor_web_directory = "C:\Applications\VendorTest"
$nsb_directory = "C:\Applications\Purchasing.Server"
$etl_jde_directory = "C:\Installs\PurchasingTest\tools\Purchasing.ETL.JDE"
$etl_jde_po_directory = "C:\Installs\PurchasingTest\tools\Purchasing.ETL.JDE.PO"
$notifier_directory = "C:\Installs\PurchasingTest\tools\Purchasing.Notifier"
$keepAliveDirectory = "C:\Installs\PurchasingTest\tools\KeepVendorPortalActive"
$paymentRequestsSharedDirectory = "\\dwhomes\INVOICEST"

$accounting_api_uri = "https://accountingtest.davidweekleyhomes.com/api/"

$newRelicVendorName = "Vendor (Test)"
$newRelicPurchasingName = "Purchasing (Test)"

# db settings
$jde_db_server = "DWHOMES"
$jde_db_username = "ASPNETST"
$jde_db_password = "a5pc#n3t"
$jde_db_collection = "TESTDATA"
$jde_lookup_db_name = "TESTCOMM"

$db_server = "WKSQL3"
$db_name = "PurchasingTest"
$db_nsb_application_name = "Purchasing.Server.Dev"
$db_p_ui_application_name = "Purchasing.UI.Dev"
$db_v_ui_application_name = "VendorPortal.UI.Dev"

$nsb_service_name = "DWH - Purchasing.Server"
$nsb_enableServiceControlHeartbeat = "true"


$header_image_file_name = "DWH logo_color_233x34_dev.png"

$purchasing_identity_uri = "https://purchasingtest.davidweekleyhomes.com"
$vendor_identity_uri = "https://vendortest.davidweekleyhomes.com"
$vendor_auth_issuer = "https://vendorauthtest.davidweekleyhomes.com/issue/wsfed"
$vendor_auth_authority = "https://vendorauthtest.davidweekleyhomes.com/IdSrv"
$vendor_auth_metadata = "https://vendorauthtest.davidweekleyhomes.com/FederationMetadata/2007-06/FederationMetadata.xml"
$vendor_auth_base_url = "https://vendorauthtest.davidweekleyhomes.com"

$sendFeedbackAddresses = "benf@headspring.com, dane.schilling@headspring.com, deran@headspring.com, eduardo@headspring.com, imorfey@dwhomes.com"
$errorReportingEmailAddresses = "benf@headspring.com, dane.schilling@headspring.com, deran@headspring.com, eduardo@headspring.com"
$errorReportingSubject = "Purchasing/Vendor Exception (test)"

$documentSharePath = "C:\_Uploads\"

$dataBusSharePath = "\\wkcorpappdev1\LLDataBus"
$actionMailerPickupDirectory = "C:\Temp\_Emails"
$smtpDeliveryMethod = "network" #"SpecifiedPickupDirectory"
$helpDeskMessagesEndPoint= "Helpdesk.Server"

$purchaseOrderUrl = "https://vendortest.davidweekleyhomes.com/PurchaseOrder/Details"
$notifyEmailSubjectPrefix = "Purchasing Test - "