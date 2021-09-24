using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using iText.Kernel.Geom;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Properties;
using iText.Layout.Element;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Font;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationSearchUtility
{
    class Fortinet
    {
        bool editAdminFlag;
        bool adminMaintainerFlag;
        bool unsetAllowaccessFlag;
        bool tlsVersionFlag;
        bool httpLoginFlag;
        bool sPortFlag;
        bool sshPortFlag;
        bool adminTimeoutFlag;
        bool trustedHostFlag;
        bool graceTimeFlag;
        bool adminLockoutThreshFlag;
        bool adminLockoutDurationFlag;
        bool preLoginFlag;
        bool postLoginFlag;
        bool strongCryptoFlag;
        bool staticKeysFlag;
        bool DHparamsFlag;
        bool autoInstallUSBFlag;
        bool autoInstallImageFlag;
        bool ntpServerFlag;
        bool passwordPolicyFlag;
        bool passwordStatusEnableFlag;
        bool passwordLowerCaseFlag;
        bool passwordUpperCaseFlag;
        bool passwordNonAlphaFlag;
        bool passwordMinNumberFlag;
        bool passwordExpireStatusFlag;
        bool localInPolicyFlag;
        bool intfWan1Flag;
        bool srcAddrFlag;
        bool dstAddrFlag;
        bool actionDenyFlag;
        bool serviceBGPflag;
        bool scheduleAlwaysFlag;
        bool alertemailFlag;
        bool configAVHeuristicFlag;
        bool configAVProfileFlag;
        bool httpScanFlag;
        bool httpAVmonitorFlag;
        bool httpQuarantineFlag;
        bool httpOutbreakPreventionDisabledFlag;
        bool httpOutbreakPreventionFilesFlag;
        bool httpOutbreakPreventionFullArchiveFlag;
        bool httpContentDisarmFlag;
        bool smtpScanFlag;
        bool smtpAVmonitorFlag;
        bool smtpQuarantineFlag;
        bool smtpOutbreakPreventionDisabledFlag;
        bool smtpOutbreakPreventionFilesFlag;
        bool smtpOutbreakPreventionFullArchiveFlag;
        bool smtpContentDisarmFlag;
        bool nac_quarFlag;
        bool nac_quarInfectedFlag;
        bool blockBotnetFlag;
        bool filterRefererLogFlag;
        bool deepInspectionFlag;


        int sPortNumber;
        int sshPortNumber;
        int adminTimeoutNumber;
        int graceTimeNumber;
        int adminLockoutThreshNumber;
        int adminLockoutDurationNumber;

        int missingCommands = 0;
        int totalCommands = 56;
        int tempTotal = 5;

        string fileContent = "";
        string sPortString = null;
        string sshPortString = null;
        string adminTimeoutString = null;
        string graceTimeString = null;
        string adminLockoutThreshString = null;
        string adminLockoutDurationString = null;
        List<string> editArray = new List<string>();
        Regex httpRX = new Regex(@"(http\b)");


        public void analyzeFortiConfig(string filepath)
        {
            using (var streamReader = File.OpenText(filepath))
            {
                fileContent = streamReader.ReadToEnd();

                //1. Default Admin Command
                if (fileContent.Contains("edit \"admin\""))
                {
                    editAdminFlag = true;
                }
                else
                {
                    editAdminFlag = false;
                    ++missingCommands;
                }

                //2. Admin maintainer Command
                if (fileContent.Contains("set admin-maintainer disable"))
                {
                    adminMaintainerFlag = true;
                }
                else
                {
                    adminMaintainerFlag = false;
                    ++missingCommands;
                }

                //3. Disable Access on external interfaces(s)
                if (fileContent.Contains("unset allowaccess"))
                {
                    unsetAllowaccessFlag = true;
                }
                else
                {
                    unsetAllowaccessFlag = false;
                    ++missingCommands;
                }

                //4. HTTP is Allowed
                var lines = File.ReadLines(filepath)
                                .SkipWhile(line => !line.Contains("config system interface"))
                                .TakeWhile(line => !line.Contains("end"));

                foreach (var line in lines)
                {
                    string text = null;
                    line.Trim();
                    text = line;

                    Match matches = httpRX.Match(text);
                    if (matches.Value != "")
                    {
                        //++missingCommands;
                        editArray.Add(text);
                    }
                }

                //5. TLS Version 
                if (fileContent.Contains("set admin-https-ssl-versions tlsv1-2"))
                {
                    tlsVersionFlag = true;
                }
                else
                {
                    ++missingCommands;
                    tlsVersionFlag = false;
                }

                
                //6. HTTP Login Attemps 
                if (fileContent.Contains("set admin-https-redirect enable"))
                {
                    httpLoginFlag = true;
                }
                else
                {
                    ++missingCommands;
                    httpLoginFlag = false;
                }

                //7. Default Port for HTTPS 
                if (fileContent.Contains("set admin-sport"))
                {
                    sPortFlag = true;
                    foreach (var line in File.ReadLines(filepath))
                    {
                        if (line.Contains("set admin-sport") && sPortString == null)
                        {
                            sPortString = line;
                            sPortNumber = Convert.ToInt32(Regex.Match(sPortString, @"\d+").Value);
                            if (sPortNumber == 443)
                            {
                                sPortString = "set admin-sport is default port (443)";
                            }
                        }
                    }
                }
                else
                {
                    ++missingCommands;
                    sPortFlag = false;
                    sPortString = "set admin-sport command NOT found.";
                }

                //7. Default Port for SSH 
                if (fileContent.Contains("set admin-ssh-port"))
                {
                    sshPortFlag = true;
                    foreach (var line in File.ReadLines(filepath))
                    {
                        if (line.Contains("set admin-sport") && sshPortString == null)
                        {
                            sshPortString = line;
                            sshPortNumber = Convert.ToInt32(Regex.Match(sPortString, @"\d+").Value);
                            if (sPortNumber == 22)
                            {
                                sshPortString = "set admin-ssh-sport is default port (22)";
                            }
                        }
                    }
                }
                else
                {
                    ++missingCommands;
                    sshPortFlag = false;
                    sshPortString = "set admin-ssh-port command NOT found.";
                }

                //8. Admin Timeout Setting
                if (fileContent.Contains("set admintimeout"))
                {
                    adminTimeoutFlag = true;
                    foreach (var line in File.ReadLines(filepath))
                    {
                        if (line.Contains("set admintimeout") && adminTimeoutString == null)
                        {
                            adminTimeoutString = line;
                            adminTimeoutNumber = Convert.ToInt32(Regex.Match(adminTimeoutString, @"\d+").Value);
                            if (adminTimeoutNumber > 5)
                            {
                                adminTimeoutString = "Admin timeout is set to " + adminTimeoutNumber + " minutes, which is too long.";
                                break;
                            }
                            if (adminTimeoutNumber < 0)
                            {
                                adminTimeoutString = "Admin timeout is set to " + adminTimeoutNumber + " minutes, which is too short.";
                                break;
                            }
                        }
                    }
                }
                else
                {
                    ++missingCommands;
                    adminTimeoutFlag = false;
                    adminTimeoutString = "set admintimeout command not found. Admintimeout is set to default (10 minutes)";
                }

                //9. Grace time to authenticate
                if (fileContent.Contains("set admin-ssh-grace-time"))
                {
                    graceTimeFlag = true;
                    foreach (var line in File.ReadLines(filepath))
                    {
                        if (line.Contains("set admin-ssh-grace-time") && graceTimeString == null)
                        {
                            graceTimeString = line;
                            graceTimeNumber = Convert.ToInt32(Regex.Match(graceTimeString, @"\d+").Value);
                            if (graceTimeNumber > 30)
                            {
                                graceTimeString = "Grace time to authenticate is set to " + graceTimeNumber;
                            }
                        }
                    }
                }
                else
                {
                    ++missingCommands;
                    graceTimeFlag = false;
                    graceTimeString = "set admin-ssh-grace-time command NOT found";
                }

                //10. Trusted Hosts not defined
                if (fileContent.Contains("set trustedhost"))
                {
                    trustedHostFlag = true;
                }
                else
                {
                    ++missingCommands;
                    trustedHostFlag = false;
                }

                //11. Admin Lockout Settings
                if (fileContent.Contains("set admin-lockout-threshold"))
                {
                    adminLockoutThreshFlag = true;
                    foreach (var line in File.ReadLines(filepath))
                    {
                        if (line.Contains("set admin-lockout-threshold") && adminLockoutThreshString == null)
                        {
                            adminLockoutThreshString = line;
                            adminLockoutThreshNumber = Convert.ToInt32(Regex.Match(sPortString, @"\d+").Value);
                            if (adminLockoutThreshNumber > 3)
                            {
                                adminLockoutThreshString = "Admin lockout threshold is set to " + adminLockoutThreshNumber;
                            }
                        }
                    }
                }
                else
                {
                    ++missingCommands;
                    adminLockoutThreshFlag = false;
                    adminLockoutThreshString = "set admin-lockout-threshold command NOT found.";
                }
                if (fileContent.Contains("set admin-lockout-duration"))
                {
                    adminLockoutDurationFlag = true;
                    foreach (var line in File.ReadLines(filepath))
                    {
                        if (line.Contains("set admin-lockout-threshold") && adminLockoutDurationString == null)
                        {
                            adminLockoutDurationString = line;
                            adminLockoutDurationNumber = Convert.ToInt32(Regex.Match(sPortString, @"\d+").Value);
                            if (adminLockoutDurationNumber > 300)
                            {
                                adminLockoutDurationString = "Admin lockout duration is set to " + adminLockoutDurationNumber;
                            }
                        }
                    }
                }
                else
                {
                    ++missingCommands;
                    adminLockoutDurationFlag = false;
                }

                //12. Disclaimer Banners
                if (fileContent.Contains("set pre-login-banner enable"))
                {
                    preLoginFlag = true;
                }
                else
                {
                    ++missingCommands;
                    preLoginFlag = false;
                }
                if (fileContent.Contains("set post-login-banner enable"))
                {
                    postLoginFlag = true;
                }
                else
                {
                    ++missingCommands;
                    postLoginFlag = false;
                }

                //13. Strong Encryption not enabled 
                if (fileContent.Contains("set strong-crypto enable"))
                {
                    strongCryptoFlag = true;
                }
                else
                {
                    ++missingCommands;
                    strongCryptoFlag = false;
                }

                //14. Static Keys for TLS Session
                if (fileContent.Contains("set ssl-static-key-ciphers disable"))
                {
                    staticKeysFlag = true;
                }
                else
                {
                    ++missingCommands;
                    staticKeysFlag = false;
                }

                //15. Diffie-Hellman (DH) Exchanges
                if (fileContent.Contains("set dh-params 8192"))
                {
                    DHparamsFlag = true;
                }
                else
                {
                    ++missingCommands;
                    DHparamsFlag = false;
                }

                //17. Auto USB Installation
                if (fileContent.Contains("set auto-install-config disable"))
                {
                    autoInstallUSBFlag = true;
                }
                else
                {
                    ++missingCommands;
                    autoInstallUSBFlag = false;
                }
                if (fileContent.Contains("set auto-install-image disable"))
                {
                    autoInstallImageFlag = false;
                }
                else
                {
                    ++missingCommands;
                    autoInstallImageFlag = false;
                }

                //18. NTP Server not defined
                if (fileContent.Contains("config ntpserver"))
                {
                    var ntpServerDefLines = File.ReadLines(filepath)
                                .SkipWhile(line => line.Contains("config ntpserver"))
                                .TakeWhile(line => line.Contains("end"));
                    if (ntpServerDefLines.Contains("set server"))
                    {
                        ntpServerFlag = true;
                    }
                }
                else
                {
                    ++missingCommands;
                    ntpServerFlag = false;
                }

                //19. Password Policy not defined
                if (fileContent.Contains("config system password-policy"))
                {
                    passwordPolicyFlag = true;
                    var passwordDefLines = File.ReadLines(filepath)
                                .SkipWhile(line => line != "config system password-policy")
                                .TakeWhile(line => line != "end");
                    if (passwordDefLines.Contains("set status enable"))
                    {
                        passwordStatusEnableFlag = true;
                    }
                    else
                    {
                        ++missingCommands;
                        passwordStatusEnableFlag = false;
                    }
                    if (passwordDefLines.Contains("set min-lower-case-letter"))
                    {
                        passwordLowerCaseFlag = true;
                    }
                    else
                    {
                        ++missingCommands;
                        passwordLowerCaseFlag = false;
                    }
                    if (passwordDefLines.Contains("set min-upper-case-letter"))
                    {
                        passwordUpperCaseFlag = true;
                    }
                    else
                    {
                        ++missingCommands;
                        passwordUpperCaseFlag = false;
                    }
                    if (passwordDefLines.Contains("set min-non-alphanumeric"))
                    {
                        passwordNonAlphaFlag = true;
                    }
                    else
                    {
                        ++missingCommands;
                        passwordNonAlphaFlag = false;
                    }
                    if (passwordDefLines.Contains("set min-number"))
                    {
                        passwordMinNumberFlag = true;
                    }
                    else
                    {
                        ++missingCommands;
                        passwordMinNumberFlag = false;
                    }
                    if (passwordDefLines.Contains("set expire-status enable"))
                    {
                        passwordExpireStatusFlag = true;
                    }
                    else
                    {
                        ++missingCommands;
                        passwordExpireStatusFlag = false;
                    }
                }
                else
                {
                    ++missingCommands;
                    passwordPolicyFlag = false;
                }

                //20. Local-in Policies not defined to close ports
                if (fileContent.Contains("config firewall local-in-policy"))
                {
                    localInPolicyFlag = true;
                    var localInPolicyLines = File.ReadLines(filepath)
                                .SkipWhile(line => line != "config firewall local-in-policy")
                                .TakeWhile(line => line != "end");
                    if (localInPolicyLines.Contains("set intf wan1"))
                    {
                        intfWan1Flag = true;
                    }
                    else
                    {
                        ++missingCommands;
                        intfWan1Flag = false;
                    }
                    if (localInPolicyLines.Contains("set srcaddr all"))
                    {
                        srcAddrFlag = true;
                    }
                    else
                    {
                        ++missingCommands;
                        srcAddrFlag = false;
                    }
                    if (localInPolicyLines.Contains("set dstaddr all"))
                    {
                        dstAddrFlag = true;
                    }
                    else
                    {
                        ++missingCommands;
                        dstAddrFlag = false;
                    }
                    if (localInPolicyLines.Contains("set action deny"))
                    {
                        actionDenyFlag = true;
                    }
                    else
                    {
                        ++missingCommands;
                        actionDenyFlag = false;
                    }
                    if (localInPolicyLines.Contains("set service BGP"))
                    {
                        serviceBGPflag = true;
                    }
                    else
                    {
                        ++missingCommands;
                        serviceBGPflag = false;
                    }
                    if (localInPolicyLines.Contains("set schedule always"))
                    {
                        scheduleAlwaysFlag = true;
                    }
                    else
                    {
                        ++missingCommands;
                        scheduleAlwaysFlag = false;
                    }
                }
                else
                {
                    ++missingCommands;
                    localInPolicyFlag = false;
                }

                //21. Alertemail Setting
                if (fileContent.Contains("alertemail setting"))
                {
                    alertemailFlag = true;
                }
                else
                {
                    ++missingCommands;
                    alertemailFlag = false;
                }

                //Config Antivirus Heuristic
                if (fileContent.Contains("config antivirus heuristic"))
                {
                    configAVHeuristicFlag = true;
                }
                else
                {
                    ++missingCommands;
                    configAVHeuristicFlag = false;
                }

                //22. Config Antivirus Profile
                if (fileContent.Contains("config antivirus profile"))
                {
                    configAVProfileFlag = true;
                }
                else
                {
                    ++missingCommands;
                    configAVProfileFlag = false;
                }

                //23. HTTP Options
                if (fileContent.Contains("config http"))
                {
                    var configHttpLines = File.ReadLines(filepath)
                            .SkipWhile(line => !line.Contains("config http"))
                            .TakeWhile(line => !line.Contains("end"));
                    string[] httpArray = configHttpLines.ToArray();

                    foreach (string s in httpArray)
                    {
                        if (s.Contains("set options scan"))
                        {
                            httpScanFlag = true;
                        }
                        else
                        {
                            //++missingCommands;
                            httpScanFlag = false;
                        }
                        if (s.Contains("set options avmonitor"))
                        {
                            httpAVmonitorFlag = true;
                        }
                        else
                        {
                            //++missingCommands;
                            httpAVmonitorFlag = false;
                        }
                        if (s.Contains("set options quarantine"))
                        {
                            httpQuarantineFlag = true;
                        }
                        else
                        {
                            //++missingCommands;
                            httpQuarantineFlag = false;
                        }
                        if (s.Contains("set outbreak-prevention disabled"))
                        {
                            httpOutbreakPreventionDisabledFlag = true;
                        }
                        else
                        {
                            //++missingCommands;
                            httpOutbreakPreventionDisabledFlag = false;
                        }
                        if (s.Contains("set outbreak-prevention files"))
                        {
                            httpOutbreakPreventionFilesFlag = true;
                        }
                        else
                        {
                            //++missingCommands;
                            httpOutbreakPreventionFilesFlag = false;
                        }
                        if (s.Contains("set outbreak-prevention full-archive"))
                        {
                            httpOutbreakPreventionFullArchiveFlag = true;
                        }
                        else
                        {
                            //++missingCommands;
                            httpOutbreakPreventionFullArchiveFlag = false;
                        }
                        if (s.Contains("set content-disarm"))
                        {
                            httpContentDisarmFlag = true;
                        }
                        else
                        {
                            //++missingCommands;
                            httpContentDisarmFlag = false;
                        }
                    }
                }

                //24. SMTP Options
                if (fileContent.Contains("config smtp"))
                {
                    var configSmtpLines = File.ReadLines(filepath)
                            .SkipWhile(line => !line.Contains("config smtp"))
                            .TakeWhile(line => !line.Contains("end"));
                    string[] smtpArray = configSmtpLines.ToArray();

                    foreach (string s in smtpArray)
                    {
                        if (s.Contains("set options scan"))
                        {
                            smtpScanFlag = true;
                        }
                        else
                        {
                            //++missingCommands;
                            smtpScanFlag = false;
                        }
                        if (s.Contains("set options avmonitor"))
                        {
                            smtpAVmonitorFlag = true;
                        }
                        else
                        {
                            //++missingCommands;
                            smtpAVmonitorFlag = false;
                        }
                        if (s.Contains("set options quarantine"))
                        {
                            smtpQuarantineFlag = true;
                        }
                        else
                        {
                            //++missingCommands;
                            smtpQuarantineFlag = false;
                        }
                        if (s.Contains("set outbreak-prevention disabled"))
                        {
                            smtpOutbreakPreventionDisabledFlag = true;
                        }
                        else
                        {
                            //++missingCommands;
                            smtpOutbreakPreventionDisabledFlag = false;
                        }
                        if (s.Contains("set outbreak-prevention files"))
                        {
                            smtpOutbreakPreventionFilesFlag = true;
                        }
                        else
                        {
                            //++missingCommands;
                            smtpOutbreakPreventionFilesFlag = false;
                        }
                        if (s.Contains("set outbreak-prevention full-archive"))
                        {
                            smtpOutbreakPreventionFullArchiveFlag = true;
                        }
                        else
                        {
                            //++missingCommands;
                            smtpOutbreakPreventionFullArchiveFlag = false;
                        }
                        if (s.Contains("set content-disarm"))
                        {
                            smtpContentDisarmFlag = true;
                        }
                        else
                        {
                            smtpContentDisarmFlag = false;
                        }
                    }
                }

                //25. Quarantining of infected hosts to banned User list check
                if (fileContent.Contains("config nac-quar"))
                {
                    nac_quarFlag = true;
                    var nacQuarLines = File.ReadLines(filepath)
                                                    .SkipWhile(line => line != "config system interface")
                                                    .TakeWhile(line => line != "end");

                    if (nacQuarLines.Contains("set infected quar-src-ip"))
                    {
                        nac_quarInfectedFlag = true;
                    }
                }
                else
                {
                    ++missingCommands;
                    nac_quarFlag = false;
                    nac_quarInfectedFlag = false;
                }

                //26. Requests to known command and control (C2) servers are not blocked.
                if (fileContent.Contains("set block-botnet enable"))
                {
                    blockBotnetFlag = true;
                }
                else
                {
                    ++missingCommands;
                    blockBotnetFlag = false;
                }

                //27. Referer field is not logged
                if (fileContent.Contains("web-filter-referer-log"))
                {
                    filterRefererLogFlag = true;
                }
                else
                {
                    ++missingCommands;
                    filterRefererLogFlag = false;
                }

                //28. Deep SSL inspection is not enabled
                if (fileContent.Contains("set inspect-all deep-inspection"))
                {
                    deepInspectionFlag = true;
                }
                else
                {
                    ++missingCommands;
                    deepInspectionFlag = false;
                }
            }
        }


        public static readonly string IMG = @"C:\Users\LENOVO\Desktop\Test\pdfTests\newLogo.png";

        public void createFortiReport(String dest, string orgName, string deviceName, string deviceType)
        {
            PdfFont helveticaBoldFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
            PdfFont helveticaRegularFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

            PdfDocument pdfDOC = new PdfDocument(new PdfWriter(dest, new WriterProperties().AddUAXmpMetadata().SetPdfVersion(PdfVersion.PDF_1_7)));
            Document document = new Document(pdfDOC, PageSize.A4.Rotate());

            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);

            float leftMargin = 20;
            float topMargin = 20;
            int headingCounter = 0;


            Paragraph header = new Paragraph("NetHarden\nBy\nDigital Arrays");
            header.SetMarginLeft(leftMargin);
            header.SetFont(helveticaBoldFont);
            header.SetFontSize(25);
            document.Add(header);
            document.Add(ls);

            header = new Paragraph("\nConfiguration Vulnerabilities Analysis Report").SetTextAlignment(TextAlignment.CENTER);
            header.SetFont(helveticaRegularFont);
            header.SetFontSize(16);
            document.Add(header);
            document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));


            Paragraph organizationNamePara = new Paragraph("Organization: " + orgName).SetTextAlignment(TextAlignment.CENTER);
            organizationNamePara.SetMarginLeft(leftMargin);
            organizationNamePara.SetFont(helveticaBoldFont);
            organizationNamePara.SetFontSize(18);
            document.SetTopMargin(topMargin);
            document.Add(ls);
            document.Add(organizationNamePara);
            //document.Add(ls);
            Paragraph deviceNamePara = new Paragraph("Device Name: " + deviceName).SetTextAlignment(TextAlignment.CENTER);
            deviceNamePara.SetFont(helveticaBoldFont);
            deviceNamePara.SetFontSize(18);
            //document.Add(ls);
            document.Add(deviceNamePara);
            //document.Add(ls);
            Paragraph deviceTypePara = new Paragraph("Device Type: " + deviceType).SetTextAlignment(TextAlignment.CENTER);
            deviceTypePara.SetFont(helveticaBoldFont);
            deviceTypePara.SetFontSize(18);
            //document.Add(ls);
            document.Add(deviceTypePara);
            document.Add(ls);
            document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            Paragraph summary = new Paragraph("Summary Analysis").SetTextAlignment(TextAlignment.CENTER);
            summary.SetFont(helveticaBoldFont);
            summary.SetFontSize(18);
            summary.SetMarginTop(topMargin);
            document.Add(summary);
            document.Add(ls);
            Paragraph summaryText = new Paragraph("\n\nAnalyzed " + totalCommands + " commands in total and " + missingCommands + " vulnerabilities alerts have been raised.\nDetails for each alert mentioned as follows.")
                    .SetTextAlignment(TextAlignment.CENTER);
            summaryText.SetFont(helveticaRegularFont);
            summary.SetFontSize(16);
            document.Add(summaryText);
            document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));

            Paragraph paragraphHeading;
            Paragraph descriptionHeading;
            Paragraph descriptionText;
            Paragraph command;
            Paragraph risk;

            //Default Admin Command
            if (editAdminFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Admin Account").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Default “admin” account name has not been renamed.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0edit admin command NOT found.\n");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Admin maintainer Command
            if (adminMaintainerFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Maintainer account is not disabled").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0The maintainer account is not disabled. Administrators with physical access to a FortiGate appliance can use a console cable " +
                    "and a special administrator account called maintainer to log into the CLI. Logging in with the maintainer account requires rebooting the FortiGate.").SetTextAlignment(TextAlignment.JUSTIFIED);
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                descriptionText = new Paragraph("\u00a0\u00a0If you disable maintainer and lose your administrator passwords you will no longer be able to log into your FortiGate.").SetTextAlignment(TextAlignment.JUSTIFIED);
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0set admin-maintainer disable command NOT found.\n");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Unset Allowaccess Command
            if (unsetAllowaccessFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Disable Access on External Interfaces(s)").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Admin access to external interface (internet-facing) is not disabled");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0If unset allowaccess is NOT found.\n");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //HTTP is allowed
            if (editArray.Count != 0)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". HTTP is Allowed").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0On “dmz”, “mgmt”, “lan”,  interface HTTP is allowed in addition to HTTPS");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                /*command = new Paragraph("\u00a0\u00a0Http found on these following commands\n");
                foreach (string httpCommand in httpFound)
                {
                    command.Add("\u00a0\u00a0" + httpCommand + "\n");
                }
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);*/
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //TLS Version Command
            if (tlsVersionFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". TLS v1.2").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0TLS 1.2 is not implemented for HTTPS-admin access\n\n");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0If set admin-https-ssl-versions tlsv1-2 NOT found.\n");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //HTTP Login Attempts
            if (httpLoginFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". HTTP login Attempts ").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                //document.Add(ls);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Attempted HTTP login connections are NOT redirected to HTTPS.\n\n");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0If set admin-https-redirect enable is NOT found\n");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Default Ports for HTTPS
            if (sPortFlag == true)
            {
                if (sPortNumber == 443)
                {
                    headingCounter++;
                    paragraphHeading = new Paragraph(headingCounter + ". Default Ports For HTTPS").SetTextAlignment(TextAlignment.CENTER);
                    paragraphHeading.SetMarginTop(topMargin);
                    paragraphHeading.SetFont(helveticaBoldFont);
                    paragraphHeading.SetFontSize(18);
                    document.Add(paragraphHeading);
                    descriptionHeading = new Paragraph("Description");
                    descriptionHeading.SetMarginLeft(leftMargin);
                    descriptionHeading.SetFont(helveticaBoldFont);
                    descriptionHeading.SetFontSize(17);
                    document.Add(descriptionHeading);
                    document.Add(ls);
                    descriptionText = new Paragraph("\n\u00a0\u00a0You can change the default port configurations for HTTPS and SSH administrative access for added security");
                    descriptionText.SetMarginLeft(leftMargin);
                    descriptionText.SetFont(helveticaRegularFont);
                    descriptionText.SetFontSize(14);
                    document.Add(descriptionText);
                    descriptionText = new Paragraph("\n\u00a0\u00a0If you change the HTTPS port numbers, make sure your changes do not conflict with ports used for other services");
                    descriptionText.SetMarginLeft(leftMargin);
                    descriptionText.SetFont(helveticaRegularFont);
                    descriptionText.SetFontSize(14);
                    document.Add(descriptionText);
                    command = new Paragraph("\u00a0\u00a0sport" + sPortString + "\n");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                    risk = new Paragraph("Risk Assessmnet");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaBoldFont);
                    risk.SetFontSize(17);
                    document.Add(risk);
                    document.Add(ls);
                    risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaRegularFont);
                    risk.SetFontSize(14);
                    document.Add(risk);
                    Paragraph recommendation = new Paragraph("Recommendation");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaBoldFont);
                    recommendation.SetFontSize(17);
                    document.Add(recommendation);
                    document.Add(ls);
                    recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaRegularFont);
                    recommendation.SetFontSize(14);
                    document.Add(risk);
                    document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
                }
            }
            else
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Default Ports For HTTPS").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0You can change the default port configurations for HTTPS and SSH administrative access for added security");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                descriptionText = new Paragraph("\n\u00a0\u00a0If you change the HTTPS port numbers, make sure your changes do not conflict with ports used for other services");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0" + "set admin-sport not found" + "\n");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);
                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Default Ports for SSH
            if (sshPortFlag == true)
            {
                if (sshPortNumber == 22)
                {
                    headingCounter++;
                    paragraphHeading = new Paragraph(headingCounter + ". Default Ports For SSH").SetTextAlignment(TextAlignment.CENTER);
                    paragraphHeading.SetMarginTop(topMargin);
                    paragraphHeading.SetFont(helveticaBoldFont);
                    paragraphHeading.SetFontSize(18);
                    document.Add(paragraphHeading);
                    descriptionHeading = new Paragraph("Description");
                    descriptionHeading.SetMarginLeft(leftMargin);
                    descriptionHeading.SetFont(helveticaBoldFont);
                    descriptionHeading.SetFontSize(17);
                    document.Add(descriptionHeading);
                    document.Add(ls);
                    descriptionText = new Paragraph("\n\u00a0\u00a0You can change the default port configurations for SSH administrative access for added security\n\n");
                    descriptionText.SetMarginLeft(leftMargin);
                    descriptionText.SetFont(helveticaRegularFont);
                    descriptionText.SetFontSize(14);
                    document.Add(descriptionText);
                    descriptionText = new Paragraph("\n\u00a0\u00a0If you change the SSH port numbers, make sure your changes do not conflict with ports used for other services");
                    descriptionText.SetMarginLeft(leftMargin);
                    descriptionText.SetFont(helveticaRegularFont);
                    descriptionText.SetFontSize(14);
                    document.Add(descriptionText);
                    command = new Paragraph("\u00a0\u00a0" + sshPortString + "\n");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                    risk = new Paragraph("Risk Assessmnet");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaBoldFont);
                    risk.SetFontSize(17);
                    document.Add(risk);
                    document.Add(ls);
                    risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaRegularFont);
                    risk.SetFontSize(14);
                    document.Add(risk);
                    Paragraph recommendation = new Paragraph("Recommendation");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaBoldFont);
                    recommendation.SetFontSize(17);
                    document.Add(recommendation);
                    document.Add(ls);
                    recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaRegularFont);
                    recommendation.SetFontSize(14);
                    document.Add(risk);
                    document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
                }
            }
            else
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Default Ports For SSH").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0You can change the default port configurations for SSH administrative access for added security");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                descriptionText = new Paragraph("\n\u00a0\u00a0If you change the SSH port numbers, make sure your changes do not conflict with ports used for other services");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0" + "set admin-ssh-port not found" + "\n");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);
                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Admin Timeout Settings
            if (adminTimeoutFlag == true)
            {
                if (adminTimeoutNumber > 5 || adminTimeoutNumber < 0)
                {
                    headingCounter++;
                    paragraphHeading = new Paragraph(headingCounter + ". Admin Timeout Settings").SetTextAlignment(TextAlignment.CENTER);
                    paragraphHeading.SetMarginTop(topMargin);
                    paragraphHeading.SetFont(helveticaBoldFont);
                    paragraphHeading.SetFontSize(18);
                    document.Add(paragraphHeading);
                    descriptionHeading = new Paragraph("Description");
                    descriptionHeading.SetMarginLeft(leftMargin);
                    descriptionHeading.SetFont(helveticaBoldFont);
                    descriptionHeading.SetFontSize(17);
                    document.Add(descriptionHeading);
                    document.Add(ls);
                    descriptionText = new Paragraph("\n\u00a0\u00a0Admin timeout is set to " + adminTimeoutNumber + " minutes, which is too long");
                    descriptionText.SetMarginLeft(leftMargin);
                    descriptionText.SetFont(helveticaRegularFont);
                    descriptionText.SetFontSize(14);
                    document.Add(descriptionText);
                    command = new Paragraph("\u00a0\u00a0Find set admintimeout <number> if number not <5 and >0" + "\n");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                    risk = new Paragraph("Risk Assessmnet");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaBoldFont);
                    risk.SetFontSize(17);
                    document.Add(risk);
                    document.Add(ls);
                    risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaRegularFont);
                    risk.SetFontSize(14);
                    document.Add(risk);
                    Paragraph recommendation = new Paragraph("Recommendation");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaBoldFont);
                    recommendation.SetFontSize(17);
                    document.Add(recommendation);
                    document.Add(ls);
                    recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaRegularFont);
                    recommendation.SetFontSize(14);
                    document.Add(risk);
                    document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));

                }
            }
            else
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Admin Timeout Settings").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Admin timeout is set to " + adminTimeoutNumber + " minutes, which is too short.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0If set admintimeout not found, then report <default 10>" + "\n\u00a0\u00a0find set admintimeout <number> if number not <5 and >0");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);
                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Grace Time to Authenticate
            if (graceTimeFlag == true)
            {
                if (graceTimeNumber > 30)
                {
                    headingCounter++;
                    paragraphHeading = new Paragraph(headingCounter + ". Grace Time to Authenticate").SetTextAlignment(TextAlignment.CENTER);
                    paragraphHeading.SetMarginTop(topMargin);
                    paragraphHeading.SetFont(helveticaBoldFont);
                    paragraphHeading.SetFontSize(18);
                    document.Add(paragraphHeading);
                    descriptionHeading = new Paragraph("Description");
                    descriptionHeading.SetMarginLeft(leftMargin);
                    descriptionHeading.SetFont(helveticaBoldFont);
                    descriptionHeading.SetFontSize(17);
                    document.Add(descriptionHeading);
                    document.Add(ls);
                    descriptionText = new Paragraph("\n\u00a0\u00a0Grace time permitted between making an SSH connection and authenticating is not defined. The range can be between 10 and 3600 seconds, the default is 120 seconds (minutes)\n");
                    descriptionText.SetMarginLeft(leftMargin);
                    descriptionText.SetFont(helveticaRegularFont);
                    descriptionText.SetFontSize(14);
                    document.Add(descriptionText);
                    descriptionText = new Paragraph("\n\u00a0\u00a0By shortening this time, you can decrease the chances of a brute force attack from being successful. For example, you could set the time to 30 seconds.");
                    descriptionText.SetMarginLeft(leftMargin);
                    descriptionText.SetFont(helveticaRegularFont);
                    descriptionText.SetFontSize(14);
                    document.Add(descriptionText);
                    command = new Paragraph("\u00a0\u00a0If set admin-ssh-grace-time <number> and number > 30");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                    risk = new Paragraph("Risk Assessmnet");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaBoldFont);
                    risk.SetFontSize(17);
                    document.Add(risk);
                    document.Add(ls);
                    risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaRegularFont);
                    risk.SetFontSize(14);
                    document.Add(risk);
                    Paragraph recommendation = new Paragraph("Recommendation");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaBoldFont);
                    recommendation.SetFontSize(17);
                    document.Add(recommendation);
                    document.Add(ls);
                    recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaRegularFont);
                    recommendation.SetFontSize(14);
                    document.Add(risk);
                    document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
                }
            }
            else
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Grace Time to Authenticate").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Grace time permitted between making an SSH connection and authenticating is not defined. The range can be between 10 and 3600 seconds, the default is 120 seconds (minutes)");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                descriptionText = new Paragraph("\n\u00a0\u00a0By shortening this time, you can decrease the chances of a brute force attack from being successful. For example, you could set the time to 30 seconds.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0If set admin-ssh-grace-time NOT found" + "\n\u00a0\u00a0If set admin-ssh-grace-time <number> and number > 30" + "\u00a0\u00a0");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);
                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Trusted Hosts not defined
            if (trustedHostFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Trusted Host(s) Not Defined").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0No trusted hosts are defined to limit the admin login from only defined hosts.\n");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0If set trustedhost NOT found");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);
                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Admin Lockout Settings
            if (adminLockoutThreshFlag == true)
            {
                if (adminLockoutThreshNumber > 3)
                {
                    headingCounter++;
                    paragraphHeading = new Paragraph(headingCounter + ". Admin Lockout Settings").SetTextAlignment(TextAlignment.CENTER);
                    paragraphHeading.SetMarginTop(topMargin);
                    paragraphHeading.SetFont(helveticaBoldFont);
                    paragraphHeading.SetFontSize(18);
                    document.Add(paragraphHeading);
                    descriptionHeading = new Paragraph("Description");
                    descriptionHeading.SetMarginLeft(leftMargin);
                    descriptionHeading.SetFont(helveticaBoldFont);
                    descriptionHeading.SetFontSize(17);
                    document.Add(descriptionHeading);
                    document.Add(ls);
                    descriptionText = new Paragraph("\n\u00a0\u00a0Admin lockout threshold (default is 3 attempts).\n");
                    descriptionText.SetMarginLeft(leftMargin);
                    descriptionText.SetFont(helveticaRegularFont);
                    descriptionText.SetFontSize(14);
                    document.Add(descriptionText);
                    command = new Paragraph("\u00a0\u00a0Found set admin-lockout-threshold " + adminLockoutThreshNumber);
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                    risk = new Paragraph("Risk Assessmnet");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaBoldFont);
                    risk.SetFontSize(17);
                    document.Add(risk);
                    document.Add(ls);
                    risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaRegularFont);
                    risk.SetFontSize(14);
                    document.Add(risk);
                    Paragraph recommendation = new Paragraph("Recommendation");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaBoldFont);
                    recommendation.SetFontSize(17);
                    document.Add(recommendation);
                    document.Add(ls);
                    recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaRegularFont);
                    recommendation.SetFontSize(14);
                    document.Add(risk);
                    document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
                }
            }
            else
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Admin Lockout Settings").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Admin lockout threshold (default is 3 attempts) has not been defined.\n");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0set admin-lockout-threshold <number> NOT found.");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }
            if (adminLockoutDurationFlag == true)
            {
                if (adminLockoutDurationNumber > 300)
                {
                    headingCounter++;
                    paragraphHeading = new Paragraph(headingCounter + ". Admin Lockout Settings").SetTextAlignment(TextAlignment.CENTER);
                    paragraphHeading.SetMarginTop(topMargin);
                    paragraphHeading.SetFont(helveticaBoldFont);
                    paragraphHeading.SetFontSize(18);
                    document.Add(paragraphHeading);
                    descriptionHeading = new Paragraph("Description");
                    descriptionHeading.SetMarginLeft(leftMargin);
                    descriptionHeading.SetFont(helveticaBoldFont);
                    descriptionHeading.SetFontSize(17);
                    document.Add(descriptionHeading);
                    document.Add(ls);
                    descriptionText = new Paragraph("\n\u00a0\u00a0Admin lockout duration (default is 60 seconds) has been defined.\n");
                    descriptionText.SetMarginLeft(leftMargin);
                    descriptionText.SetFont(helveticaRegularFont);
                    descriptionText.SetFontSize(14);
                    document.Add(descriptionText);
                    command = new Paragraph("\u00a0\u00a0Found set admin-lockout-duration " + adminLockoutDurationNumber);
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                    risk = new Paragraph("Risk Assessmnet");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaBoldFont);
                    risk.SetFontSize(17);
                    document.Add(risk);
                    document.Add(ls);
                    risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaRegularFont);
                    risk.SetFontSize(14);
                    document.Add(risk);
                    Paragraph recommendation = new Paragraph("Recommendation");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaBoldFont);
                    recommendation.SetFontSize(17);
                    document.Add(recommendation);
                    document.Add(ls);
                    recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaRegularFont);
                    recommendation.SetFontSize(14);
                    document.Add(risk);

                    document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
                }
            }
            else
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Admin Lockout Settings").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Admin lockout duration (default is 60 seconds) has not been defined.\n");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0set admin-lockout-duration <number> NOT found");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Disclaimer Banners
            if (preLoginFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Disclaimer Banners").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Pre-login disclaimer banners are not defined for GUI or CLI.\n");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0If set pre-login-banner enable NOT found");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }
            if (postLoginFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Disclaimer Banners").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Post-login disclaimer banners are not defined for GUI or CLI.\n");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0If set post-login-banner enable NOT found");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Stronger Encryption Not enabled
            if (strongCryptoFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Stronger Encryption Not Enabled").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Stronger encryption has not been enabled for FortiOS to use only strong ciphers.\n");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0If set strong-crypto enable NOT found");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Static Keys for TLS Sessions
            if (staticKeysFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Static Keys for TLS Session").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Use of static keys for TLS session has not been disabled\n");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0If set ssl-static-key-ciphers disable NOT found");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Diffie-Hellman Excchanges
            if (DHparamsFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Diffie-Hellman Exchanges").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Larger values for Diffie-Hellman (DH) exchanges are not defined to result in stronger encryption.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                descriptionText = new Paragraph("\n\u00a0\u00a0However, make sure the DH bit value setting that you choose is compatible with the systems that your FortiGate will be communicating with.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);

                command = new Paragraph("\u00a0\u00a0If set dh-params 8192 NOT found");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }


            //Auto USB installation
            if (autoInstallUSBFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Auto USB Installation").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0USB installation is enabled");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                descriptionText = new Paragraph("\n•\u00a0\u00a0An attacker with physical access to a FortiGate could load a new configuration or firmware on the FortiGate using the USB port.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0If set auto-install-config disable is NOT found.");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);
                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }
            if (autoInstallImageFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Auto USB Installation").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0USB installation is enabled\n");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                descriptionText = new Paragraph("\n\u00a0\u00a0An attacker with physical access to a FortiGate could load a new configuration or firmware on the FortiGate using the USB port.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0If set auto-install-image disable NOT found");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }


            //NTP Server not defined
            if (ntpServerFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". NTP Server Not Defined").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0NTPSync is enabled, but NTP Server is NOT defined from where to synchronise time.\n");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0If following NOT found: \n\u00a0\u00a0config ntpserver\n\u00a0\u00a0\u00a0edit 1\n\u00a0\u00a0\u00a0\u00a0set server < ntp - server - ip >");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Password Policy not defined
            if (passwordPolicyFlag == false || passwordStatusEnableFlag == false || passwordLowerCaseFlag == false || passwordUpperCaseFlag == false || passwordNonAlphaFlag == false || passwordMinNumberFlag == false || passwordExpireStatusFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Password Policy NOT Defined").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Password policy is not enforced / defined.\n\u00a0\u00a0\u00a0• required length of the password," +
                    "\n\u00a0\u00a0\u00a0• what it must contain (numbers, upper and lower case, and so on),\n\u00a0\u00a0\u00a0• an expiry time.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                descriptionText = new Paragraph("\n\u00a0\u00a0Use the password policy feature to make sure all admins use strong passwords as per organization's password policy.\n");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                if (passwordPolicyFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0\u00a0• config system password-policy NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (passwordStatusEnableFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0\u00a0• set status enable NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (passwordLowerCaseFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0\u00a0• set min-lower-case-letter NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (passwordUpperCaseFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0\u00a0• set min-upper-case-letter NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (passwordNonAlphaFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0\u00a0• set min-non-alphanumeric NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (passwordMinNumberFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0\u00a0• set min-number NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (passwordExpireStatusFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0\u00a0• set expire-status enable NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //local-in Policies not defined to close ports
            if (localInPolicyFlag == false || intfWan1Flag == false || srcAddrFlag == false || dstAddrFlag == false || actionDenyFlag == false || serviceBGPflag == false || scheduleAlwaysFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Local-in Policies not defined to close ports").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\n\u00a0\u00a0Local-in policies to close open ports or otherwise restrict access to FortiOS are not defined. " +
                    "Following example blocks traffic that matches the BGP firewall service. ");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                if (localInPolicyFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0\u00a0• config firewall local-in-policy");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (intfWan1Flag == false)
                {
                    command = new Paragraph("\u00a0\u00a0\u00a0• set intf wan1");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (srcAddrFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0\u00a0• set srcaddr all NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (dstAddrFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0\u00a0• set action deny NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (serviceBGPflag == false)
                {
                    command = new Paragraph("\u00a0\u00a0\u00a0• set service BGP NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (scheduleAlwaysFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0\u00a0• set schedule always NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }

                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }


            //Alertemail setting
            if (alertemailFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Alertemail Setting").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\u00a0\u00a0FortiGate unit has not been configured to send an alert email. " +
                    "Alert emails can be sent to up to three recipients for various events e.g. when disk usage exceeds a threshold, or when configuration is " +
                    "changed etc.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0alertemail setting NOT found.");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Config Antivirus Heuristics
            if (configAVHeuristicFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Antivirus Heuristic").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\u00a0\u00a0Global heuristic options has not been configured for antivirus scanning in binary files. " +
                    "Three modes are available; 1) Pass; 2) block; 3) disable (default).");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0config antivirus heuristic NOT found.");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Config Antivirus Profile
            if (configAVProfileFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Antivirus Profile").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\u00a0\u00a0profiles have not been configured that can be applied to firewall policies. " +
                    "Antivirus profiles configure how virus scanning is applied to sessions accepted by a firewall policy that includes the antivirus profile.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0config antivirus profile NOT found.");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //HTTP Config Options
            if (httpScanFlag == false || httpAVmonitorFlag == false || httpQuarantineFlag == false || httpOutbreakPreventionDisabledFlag == false || httpOutbreakPreventionFilesFlag == false || httpOutbreakPreventionFullArchiveFlag == false || httpContentDisarmFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Antivirus Not Enabled for HTTP Protocol").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\u00a0\u00a0profiles have not been configured that can be applied to firewall policies. " +
                    "Antivirus profiles configure how virus scanning is applied to sessions accepted by a firewall policy that includes the antivirus profile.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0set options scan within Config http NOT found.");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                if (httpAVmonitorFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0set options avmonitor within Config http NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (httpQuarantineFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0set options quarantine within Config http NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (httpOutbreakPreventionDisabledFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0set outbreak-prevention disabled within Config http NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (httpOutbreakPreventionFilesFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0set outbreak-prevention files within Config http NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (httpOutbreakPreventionFullArchiveFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0set outbreak-prevention full-archive within Config http NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (httpContentDisarmFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0set content-disarm within Config http NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);
            }

            //SMTP Config Options
            if (smtpScanFlag == false || smtpAVmonitorFlag == false || smtpQuarantineFlag == false || smtpOutbreakPreventionDisabledFlag == false || smtpOutbreakPreventionFilesFlag == false || smtpOutbreakPreventionFullArchiveFlag == false || smtpContentDisarmFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Antivirus Not Enabled for SMTP Protocol").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\u00a0\u00a0profiles have not been configured that can be applied to firewall policies. " +
                    "Antivirus profiles configure how virus scanning is applied to sessions accepted by a firewall policy that includes the antivirus profile.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0set options scan within Config smtp NOT found.");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                if (smtpAVmonitorFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0set options avmonitor within Config smtp NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (smtpQuarantineFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0set options quarantine within Config smtp NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (smtpOutbreakPreventionDisabledFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0set outbreak-prevention disabled within Config smtp NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (smtpOutbreakPreventionFilesFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0set outbreak-prevention files within Config smtp NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (smtpOutbreakPreventionFullArchiveFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0set outbreak-prevention full-archive within Config smtp NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                if (smtpContentDisarmFlag == false)
                {
                    command = new Paragraph("\u00a0\u00a0set content-disarm within Config smtp NOT found.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                }
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);
            }

            //Quarantining of infected hosts
            if (nac_quarFlag == true)
            {
                if (nac_quarInfectedFlag == false)
                {
                    headingCounter++;
                    paragraphHeading = new Paragraph(headingCounter + ". Quarantining of Infected Hosts").SetTextAlignment(TextAlignment.CENTER);
                    paragraphHeading.SetMarginTop(topMargin);
                    paragraphHeading.SetFont(helveticaBoldFont);
                    paragraphHeading.SetFontSize(18);
                    document.Add(paragraphHeading);
                    descriptionHeading = new Paragraph("Description");
                    descriptionHeading.SetMarginLeft(leftMargin);
                    descriptionHeading.SetFont(helveticaBoldFont);
                    descriptionHeading.SetFontSize(17);
                    document.Add(descriptionHeading);
                    document.Add(ls);
                    descriptionText = new Paragraph("\u00a0\u00a0Quarantining of infected hosts to the banned user list has not been enabled. " +
                        "It will Quarantine all traffic from the infected hosts source IP.");
                    descriptionText.SetMarginLeft(leftMargin);
                    descriptionText.SetFont(helveticaRegularFont);
                    descriptionText.SetFontSize(14);
                    document.Add(descriptionText);
                    command = new Paragraph("\u00a0\u00a0set infected quar-src-ip NOT found within config nac-quar.");
                    command.SetMarginLeft(leftMargin);
                    command.SetFont(helveticaRegularFont);
                    command.SetFontSize(14);
                    document.Add(command);
                    risk = new Paragraph("Risk Assessmnet");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaBoldFont);
                    risk.SetFontSize(17);
                    document.Add(risk);
                    document.Add(ls);
                    risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                    risk.SetMarginLeft(leftMargin);
                    risk.SetFont(helveticaRegularFont);
                    risk.SetFontSize(14);
                    document.Add(risk);
                    Paragraph recommendation = new Paragraph("Recommendation");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaBoldFont);
                    recommendation.SetFontSize(17);
                    document.Add(recommendation);
                    document.Add(ls);
                    recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                    recommendation.SetMarginLeft(leftMargin);
                    recommendation.SetFont(helveticaRegularFont);
                    recommendation.SetFontSize(14);
                    document.Add(risk);

                    document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
                }
            }
            else
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Quarantining of Infected Hosts").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\u00a0\u00a0Quarantining of infected hosts to the banned user list has not been enabled. " +
                    "It will Quarantine all traffic from the infected hosts source IP.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0config nac-quar NOT found.");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Block-botnet Enable
            if (blockBotnetFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Request to Known Command & Control (C2) servers not blocked").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\u00a0\u00a0FortiGuard maintains a database containing a list of known botnet command and control (C&C) addresses. " +
                    "This database is updated dynamically and stored on the FortiGate and requires a valid FortiGuard AntiVirus subscription.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0set block-botnet enable NOT found.");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Web-filter-referer-Log
            if (filterRefererLogFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Referer Field Not Logged").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\u00a0\u00a0Referer field is not logged which helps to investigating an incident. " +
                    "With modern web pages serving content from Content Delivery Networks and other services, the referrer URL plays a very important part " +
                    "in showing the ‘real’ website a person was visiting.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0web-filter-referer-log NOT found.");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);

                document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }

            //Deep SSL inspection
            if (deepInspectionFlag == false)
            {
                headingCounter++;
                paragraphHeading = new Paragraph(headingCounter + ". Deep SSL inspection is not Enabled").SetTextAlignment(TextAlignment.CENTER);
                paragraphHeading.SetMarginTop(topMargin);
                paragraphHeading.SetFont(helveticaBoldFont);
                paragraphHeading.SetFontSize(18);
                document.Add(paragraphHeading);
                descriptionHeading = new Paragraph("Description");
                descriptionHeading.SetMarginLeft(leftMargin);
                descriptionHeading.SetFont(helveticaBoldFont);
                descriptionHeading.SetFontSize(17);
                document.Add(descriptionHeading);
                document.Add(ls);
                descriptionText = new Paragraph("\u00a0\u00a0Without Deep SSL Inspection enabled, referrer URLs for HTTPS websites will not be logged, " +
                    "and only the domain portion of a URL will be logged (e.g. www.google.com, but not www.google.com/search?q=my+search+term).");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                descriptionText = new Paragraph("\u00a0\u00a0With the availability of free SSL certificates, it is now easier than ever for a bad actor to " +
                    "spin up a HTTPS website, host malware on it and direct traffic to the site with a variety of methods such as Phishing, Malvertising, " +
                    "URL injections, malicious redirects etc. Without Deep SSL Inspection, that malware will sail straight through your firewall.");
                descriptionText.SetMarginLeft(leftMargin);
                descriptionText.SetFont(helveticaRegularFont);
                descriptionText.SetFontSize(14);
                document.Add(descriptionText);
                command = new Paragraph("\u00a0\u00a0set inspect-all deep-inspection NOT found.");
                command.SetMarginLeft(leftMargin);
                command.SetFont(helveticaRegularFont);
                command.SetFontSize(14);
                document.Add(command);
                risk = new Paragraph("Risk Assessmnet");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaBoldFont);
                risk.SetFontSize(17);
                document.Add(risk);
                document.Add(ls);
                risk = new Paragraph("\u00a0\u00a0Placeholder Text here\n\n");
                risk.SetMarginLeft(leftMargin);
                risk.SetFont(helveticaRegularFont);
                risk.SetFontSize(14);
                document.Add(risk);
                Paragraph recommendation = new Paragraph("Recommendation");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaBoldFont);
                recommendation.SetFontSize(17);
                document.Add(recommendation);
                document.Add(ls);
                recommendation = new Paragraph("\u00a0\u00a0Placeholder Text here");
                recommendation.SetMarginLeft(leftMargin);
                recommendation.SetFont(helveticaRegularFont);
                recommendation.SetFontSize(14);
                document.Add(risk);
            }

            missingCommands = 0;
            document.Close();
            //MessageBox.Show("Report Successfully Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
