## First Time Setup

### ADFS Configuration for localwarranty.davidweekleyhomes.com

#### Prepare IIS Express
From an administrative command prompt, run:

* `netsh http add urlacl url=http://localwarranty.davidweekleyhomes.com:80/ user=everyone`
* `netsh http add urlacl url=https://localwarranty.davidweekleyhomes.com:443/ user=everyone`

#### Add host file entry

    127.0.0.1               localwarranty.davidweekleyhomes.com

#### Update applicationhost.config in

    C:\Users\<you!>\My Documents\IIS Express\config

The important parts are the `<binding>` tags:

    <site name="Warranty.UI" id="2099">
        <application path="/" applicationPool="Clr4IntegratedAppPool">      
            <virtualDirectory path="/" physicalPath="C:\projects\warranty\src\Warranty.UI" />
        </application>
        <bindings>
            <binding protocol="http" bindingInformation="*:80:localwarranty.davidweekleyhomes.com" />
            <binding protocol="https" bindingInformation="*:443:localwarranty.davidweekleyhomes.com" />
        </bindings>
    </site>

#### Create Certificate

    makecert -r -pe -n "CN=localwarranty.davidweekleyhomes.com" -b 01/01/2000 -e 01/01/2036 -eku 1.3.6.1.5.5.7.3.1 -ss my -sr localMachine -sky exchange -sp "Microsoft RSA SChannel Cryptographic Provider" -sy 12

Run MMC.exe -> File -> Add/Remove Snap In -> select Certificates

- Computer Account -> Finish -> Find your certificate in Certificates\Personal\Certificates
- Double Click your Certificate (the date should be 2036)
- Go to Details and scroll down to Thumbprint
- Copy the Thumbprint hash, remove all spaces
- In Admin Command Prompt run:

    <pre>netsh http add sslcert ipport=0.0.0.0:443 appid={214124cd-d05b-4309-9af9-9caa44b2b74a} certhash=YOURCERTHASHHERE  
    !!!!!!!!!!!!FAILED!!!!!!!!!!
    (Appid doesn't matter)</pre>

- In MMC, copy your certificate into Trusted Root Certificates

**Note:** If you get "parameter is incorrect" and have double checked the cert hash. You may want to generate a new guid (sql server has a function new_id() that makes it pretty quick) and replace the appid with the new guid

#### Install the Identity and Access Tool for VS2012
http://visualstudiogallery.msdn.microsoft.com/e21bf653-dfe1-4d81-b3d3-795cb104066e

#### Save & build
At this point, you can authenticate to ADFS
