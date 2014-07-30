# structure
$purchasing_web_directory = "C:\Applications\Purchasing"
$vendor_web_directory = "C:\Applications\Vendor"
$nsb_directory = "C:\Applications\Purchasing.Server"
$etl_jde_directory = "C:\Installs\Purchasing\tools\Purchasing.ETL.JDE"
$etl_jde_po_directory = "C:\Installs\Purchasing\tools\Purchasing.ETL.JDE.PO"
$notifier_directory = "C:\Installs\Purchasing\tools\Purchasing.Notifier"
$keepAliveDirectory = "C:\Installs\Purchasing\tools\KeepVendorPortalActive"
$paymentRequestsSharedDirectory = "\\dwhomes\INVOICES"

$accounting_api_uri = "https://accounting.davidweekleyhomes.com/api/"

$newRelicVendorName = "Vendor (Prod)"
$newRelicPurchasingName = "Purchasing (Prod)"

# db settings
$jde_db_server = "DWHOMES"
$jde_db_username = "ASPNETP"
$jde_db_password = "a5pc#n3t"
$jde_db_collection = "DWHPROD"
$jde_lookup_db_name = "DWHCOMM"

$db_server = "WKSQL1"
$db_name = "Purchasing"
$db_nsb_application_name = "Purchasing.Server.Prod"
$db_p_ui_application_name = "Purchasing.UI.Prod"
$db_v_ui_application_name = "VendorPortal.UI.Prod"

$nsb_service_name = "DWH - Purchasing.Server"
$nsb_enableServiceControlHeartbeat = "false"

$header_image_file_name = "DWH logo_color_233x34_prod.png"

$purchasing_identity_uri = "https://purchasing.davidweekleyhomes.com"
$vendor_identity_uri = "https://vendor.davidweekleyhomes.com"
$vendor_auth_issuer = "http://vendorauth.davidweekleyhomes.com/issue/wsfed"
$vendor_auth_authority = "https://vendorauth.davidweekleyhomes.com/IdSrv"
$vendor_auth_metadata = "https://vendorauth.davidweekleyhomes.com/FederationMetadata/2007-06/FederationMetadata.xml"
$vendor_auth_base_url = "https://vendorauth.davidweekleyhomes.com"

$sendFeedbackAddresses = "app-notify@dwhomes.com, benf@headspring.com, dane.schilling@headspring.com, deran@headspring.com, eduardo@headspring.com, imorfey@dwhomes.com, rhartzog@dwhomes.com, JCrouch@dwhomes.com, mfinocchio@dwhomes.com"
$errorReportingEmailAddresses = "app-notify@dwhomes.com, deran@headspring.com, dane.schilling@headspring.com, benf@headspring.com, eduardo@headspring.com"
$errorReportingSubject = "Purchasing/Vendor Exception (prod)"

$documentSharePath = "\\wkhoupfs\PurchasingShare\"

$dataBusSharePath = "\\wkhoupfs\LLShare\LLDataBus"
$actionMailerPickupDirectory = ""		#Blank means send emails via SMTP
$smtpDeliveryMethod = "network"
$helpDeskMessagesEndPoint= "Helpdesk.Server"

$purchaseOrderUrl = "https://vendor.davidweekleyhomes.com/PurchaseOrder/Details"
$notifyEmailSubjectPrefix = ""
