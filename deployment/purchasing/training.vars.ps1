# structure
$purchasing_web_directory = "C:\Applications\PurchasingTraining"
$vendor_web_directory = "C:\Applications\VendorTraining"
$nsb_directory = "C:\Applications\Purchasing.Server"
$etl_jde_directory = "C:\Installs\PurchasingTraining\tools\Purchasing.ETL.JDE"
$etl_jde_po_directory = "C:\Installs\PurchasingTraining\tools\Purchasing.ETL.JDE.PO"
$notifier_directory = "C:\Installs\PurchasingTraining\tools\Purchasing.Notifier"
$keepAliveDirectory = "C:\Installs\PurchasingTraining\tools\KeepVendorPortalActive"
$paymentRequestsSharedDirectory = "\\dwhomes\INVOICEST"

$accounting_api_uri = "https://accountingtrain.davidweekleyhomes.com/api/"

$newRelicVendorName = "Vendor (Training)"
$newRelicPurchasingName = "Purchasing (Training)"

# db settings
$jde_db_server = "DWHOMES"
$jde_db_username = "ASPNETT"
$jde_db_password = "a5pc#n3t"
$jde_db_collection = "TRAINDATA"
$jde_lookup_db_name = "TESTCOMM"

$db_server = "WKSQL3"
$db_name = "PurchasingTraining"
$db_nsb_application_name = "Purchasing.Server.Training"
$db_p_ui_application_name = "Purchasing.UI.Training"
$db_v_ui_application_name = "VendorPortal.UI.Training"

$nsb_service_name = "DWH - Purchasing.Server"
$nsb_enableServiceControlHeartbeat = "true"

$header_image_file_name = "DWH logo_color_233x34_train.png"

$purchasing_identity_uri = "https://purchasingtraining.davidweekleyhomes.com"
$vendor_identity_uri = "https://vendortraining.davidweekleyhomes.com"
$vendor_auth_issuer = "https://vendorauthtrain.davidweekleyhomes.com/issue/wsfed"
$vendor_auth_authority = "https://vendorauthtrain.davidweekleyhomes.com/IdSrv"
$vendor_auth_metadata = "https://vendorauthtrain.davidweekleyhomes.com/FederationMetadata/2007-06/FederationMetadata.xml"
$vendor_auth_base_url = "https://vendorauthtrain.davidweekleyhomes.com"

$sendFeedbackAddresses = "benf@headspring.com, deran@headspring.com, dane.schilling@headspring.com, eduardo@headspring.com, imorfey@dwhomes.com, rhartzog@dwhomes.com, JCrouch@dwhomes.com, mfinocchio@dwhomes.com"
$errorReportingEmailAddresses = "benf@headspring.com, dane.schilling@headspring.com, deran@headspring.com, eduardo@headspring.com"
$errorReportingSubject = "Purchasing/Vendor Exception (training)"

$documentSharePath = "C:\_Uploads\"

$dataBusSharePath = "\\wkcorpappdev1\PurchasingDataBus"
$actionMailerPickupDirectory = "C:\Temp\_Emails"
$smtpDeliveryMethod = "network" #"SpecifiedPickupDirectory"
$helpDeskMessagesEndPoint= "Helpdesk.Server"

$purchaseOrderUrl = "https://vendortraining.davidweekleyhomes.com/PurchaseOrder/Details"
$notifyEmailSubjectPrefix = "Purchasing Training - "