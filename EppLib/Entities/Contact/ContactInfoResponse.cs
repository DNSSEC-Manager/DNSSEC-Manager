﻿// Copyright 2012 Code Maker Inc. (http://codemaker.net)
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Generic;
using System.Xml;

namespace EppLib.Entities
{
    public class ContactInfoResponse : EppResponse
    {
        protected Contact _contact = new Contact();
        public virtual Contact Contact
        {
            get { return _contact; }
        }

        public ContactInfoResponse(string xml) : base(xml) { }

        public ContactInfoResponse(byte[] bytes) : base(bytes) { }

        protected override void ProcessDataNode(XmlDocument doc, XmlNamespaceManager namespaces)
        {
            namespaces.AddNamespace("contact", "urn:ietf:params:xml:ns:contact-1.0");

            var children = doc.SelectSingleNode("//contact:infData", namespaces);

            if (children != null)
            {
                var idNode = children.SelectSingleNode("contact:id", namespaces);

                if (idNode != null)
                {
                    Contact.Id = idNode.InnerText;
                }

                var roidNode = children.SelectSingleNode("contact:roid", namespaces);

                if (roidNode != null)
                {
                    Contact.Roid = roidNode.InnerText;
                }

                var statusNode = children.SelectSingleNode("contact:status", namespaces);

                if (statusNode != null)
                {
                    if (statusNode.Attributes["s"] != null) Contact.Status = statusNode.Attributes["s"].Value;
                }

                var statusNodes = children.SelectNodes("contact:status", namespaces);

                if (statusNodes != null)
                {
                    if (Contact.StatusList == null) Contact.StatusList = new List<string>();
                    foreach (XmlNode status in statusNodes)
                    {
                        if (status.Attributes["s"] != null) Contact.StatusList.Add(status.Attributes["s"].Value);
                    }
                }

                var emailNode = children.SelectSingleNode("contact:email", namespaces);

                if (emailNode != null)
                {
                    Contact.Email = emailNode.InnerText;
                }

                var voiceNode = children.SelectSingleNode("contact:voice", namespaces);

                if (voiceNode != null)
                {
                    Contact.Voice = new Telephone(voiceNode.InnerText, "");
                    if (voiceNode.Attributes["x"] != null)
                    {
                        Contact.Voice.Extension = voiceNode.Attributes["x"].Value;
                    }
                }

                var faxNode = children.SelectSingleNode("contact:fax", namespaces);

                if (faxNode != null)
                {
                    Contact.Fax = new Telephone(faxNode.InnerText, "");
                }

                var clIdNode = children.SelectSingleNode("contact:clID", namespaces);

                if (clIdNode != null)
                {
                    Contact.ClId = clIdNode.InnerText;
                }

                var crIdNode = children.SelectSingleNode("contact:crID", namespaces);

                if (crIdNode != null)
                {
                    Contact.CrId = crIdNode.InnerText;
                }

                var upIdNode = children.SelectSingleNode("contact:upID", namespaces);

                if (upIdNode != null)
                {
                    Contact.UpId = upIdNode.InnerText;
                }

                var crDateNode = children.SelectSingleNode("contact:crDate", namespaces);

                if (crDateNode != null)
                {
                    Contact.CrDate = crDateNode.InnerText;
                }

                var upDateNode = children.SelectSingleNode("contact:upDate", namespaces);

                if (upDateNode != null)
                {
                    Contact.UpDate = upDateNode.InnerText;
                }

                var trDateNode = children.SelectSingleNode("contact:trDate", namespaces);

                if (trDateNode != null)
                {
                    Contact.TrDate = trDateNode.InnerText;
                }

                var authInfoNode = children.SelectSingleNode("contact:authInfo", namespaces);

                if (authInfoNode != null)
                {
                    var pwNode = authInfoNode.SelectSingleNode("contact:pw", namespaces);

                    if (pwNode != null)
                    {
                        Contact.Password = pwNode.InnerText;
                    }
                }

                var discloseNode = children.SelectSingleNode("contact:disclose", namespaces);

                if (discloseNode != null)
                {
                    bool flag;
                    if (!Boolean.TryParse(discloseNode.Attributes["flag"].Value, out flag))
                    {
                        if (discloseNode.Attributes["flag"].Value == "0")
                        {
                            flag = false;
                        }
                        else if (discloseNode.Attributes["flag"].Value == "1")
                        {
                            flag = true;
                        }
                        else
                        {
                            throw new Exception("Unable to parse contact:disclose flag");
                        }
                    }
                    Contact.DiscloseFlag = flag;
                    Contact.DiscloseMask = flag ? Contact.DiscloseFlags.None : Contact.DiscloseFlags.All;

                    foreach (XmlNode changeNode in discloseNode.ChildNodes)
                    {
                        switch (changeNode.LocalName)
                        {
                            case "name":
                                if (changeNode.Attributes["type"].Value == "int")
                                {
                                    Contact.DiscloseMask = flag ?
                                        Contact.DiscloseMask | Contact.DiscloseFlags.NameInt
                                        : Contact.DiscloseMask & ~Contact.DiscloseFlags.NameInt;
                                }
                                else if (changeNode.Attributes["type"].Value == "loc")
                                {
                                    Contact.DiscloseMask = flag ?
                                        Contact.DiscloseMask | Contact.DiscloseFlags.NameLoc
                                        : Contact.DiscloseMask & ~Contact.DiscloseFlags.NameLoc;
                                }
                                break;
                            case "org":
                                if (changeNode.Attributes["type"].Value == "int")
                                {
                                    Contact.DiscloseMask = flag ?
                                        Contact.DiscloseMask | Contact.DiscloseFlags.OrganizationInt
                                        : Contact.DiscloseMask & ~Contact.DiscloseFlags.OrganizationInt;
                                }
                                else if (changeNode.Attributes["type"].Value == "loc")
                                {
                                    Contact.DiscloseMask = flag ?
                                        Contact.DiscloseMask | Contact.DiscloseFlags.OrganizationLoc
                                        : Contact.DiscloseMask & ~Contact.DiscloseFlags.OrganizationLoc;
                                }
                                break;
                            case "addr":
                                if (changeNode.Attributes["type"].Value == "int")
                                {
                                    Contact.DiscloseMask = flag ?
                                        Contact.DiscloseMask | Contact.DiscloseFlags.AddressInt
                                        : Contact.DiscloseMask & ~Contact.DiscloseFlags.AddressInt;
                                }
                                else if (changeNode.Attributes["type"].Value == "loc")
                                {
                                    Contact.DiscloseMask = flag ?
                                        Contact.DiscloseMask | Contact.DiscloseFlags.AddressLoc
                                        : Contact.DiscloseMask & ~Contact.DiscloseFlags.AddressLoc;
                                }
                                break;
                            case "voice":
                                Contact.DiscloseMask = flag ?
                                    Contact.DiscloseMask | Contact.DiscloseFlags.Voice
                                    : Contact.DiscloseMask & ~Contact.DiscloseFlags.Voice;
                                break;
                            case "fax":
                                Contact.DiscloseMask = flag ?
                                    Contact.DiscloseMask | Contact.DiscloseFlags.Fax
                                    : Contact.DiscloseMask & ~Contact.DiscloseFlags.Fax;
                                break;
                            case "email":
                                Contact.DiscloseMask = flag ?
                                    Contact.DiscloseMask | Contact.DiscloseFlags.Email
                                    : Contact.DiscloseMask & ~Contact.DiscloseFlags.Email;
                                break;
                        }
                    }
                }

                var postalInfoNode = children.SelectSingleNode("contact:postalInfo", namespaces);

                if (postalInfoNode != null)
                {
                    Contact.PostalInfo = new PostalInfo();

                    var nameNode = postalInfoNode.SelectSingleNode("contact:name", namespaces);

                    if (nameNode != null)
                    {
                        Contact.PostalInfo.m_name = nameNode.InnerText;
                    }

                    var orgNode = postalInfoNode.SelectSingleNode("contact:org", namespaces);

                    if (orgNode != null)
                    {
                        Contact.PostalInfo.m_org = orgNode.InnerText;
                    }

                    Contact.PostalInfo.m_type = postalInfoNode.Attributes["type"].Value;

                    var addrNode = postalInfoNode.SelectSingleNode("contact:addr", namespaces);

                    if (addrNode != null)
                    {
                        Contact.PostalInfo.m_address = new PostalAddress();

                        var streetNodes = addrNode.SelectNodes("contact:street", namespaces);
                        if (streetNodes != null)
                        {
                            if (streetNodes[0] != null)
                            {
                                Contact.PostalInfo.m_address.Street1 = streetNodes[0].InnerText;
                            }

                            if (streetNodes[1] != null)
                            {
                                Contact.PostalInfo.m_address.Street2 = streetNodes[1].InnerText;
                            }

                            if (streetNodes[2] != null)
                            {
                                Contact.PostalInfo.m_address.Street3 = streetNodes[2].InnerText;
                            }
                        }

                        var cityNode = addrNode.SelectSingleNode("contact:city", namespaces);

                        if (cityNode != null)
                        {
                            Contact.PostalInfo.m_address.City = cityNode.InnerText;
                        }

                        var spNode = addrNode.SelectSingleNode("contact:sp", namespaces);

                        if (spNode != null)
                        {
                            Contact.PostalInfo.m_address.StateProvince = spNode.InnerText;
                        }

                        var pcNode = addrNode.SelectSingleNode("contact:pc", namespaces);

                        if (pcNode != null)
                        {
                            Contact.PostalInfo.m_address.PostalCode = pcNode.InnerText;
                        }

                        var ccNode = addrNode.SelectSingleNode("contact:cc", namespaces);

                        if (ccNode != null)
                        {
                            Contact.PostalInfo.m_address.CountryCode = ccNode.InnerText;
                        }
                    }
                }
            }
        }

        protected override void ProcessExtensionNode(XmlDocument doc, XmlNamespaceManager namespaces)
        {
            namespaces.AddNamespace("cira", "urn:ietf:params:xml:ns:cira-1.0");

            var children = doc.SelectSingleNode("/ns:epp/ns:response/ns:extension/cira:ciraInfo", namespaces);

            if (children != null)
            {
                var crLanguage = children.SelectSingleNode("cira:language", namespaces);

                if (crLanguage != null)
                {
                    Contact.Language = crLanguage.InnerText;
                }

                var crCprCategory = children.SelectSingleNode("cira:cprCategory", namespaces);

                if (crCprCategory != null)
                {
                    Contact.CprCategory = crCprCategory.InnerText;
                }

                var crIndividual = children.SelectSingleNode("cira:individual", namespaces);

                if (crIndividual != null)
                {
                    Contact.Individual = crIndividual.InnerText;
                }

                var crCiraAgreementVersion = children.SelectSingleNode("cira:ciraAgreementVersion", namespaces);

                if (crCiraAgreementVersion != null)
                {
                    Contact.CiraAgreementVersion = crCiraAgreementVersion.InnerText;
                }

                var crAgreementTimestamp = children.SelectSingleNode("cira:agreementTimestamp", namespaces);

                if (crAgreementTimestamp != null)
                {
                    Contact.AgreementTimestamp = crAgreementTimestamp.InnerText;
                }

                var crWhoisDisplaySetting = children.SelectSingleNode("cira:whoisDisplaySetting", namespaces);

                if (crWhoisDisplaySetting != null)
                {
                    Contact.WhoisDisplaySetting = crWhoisDisplaySetting.InnerText;
                }
            }
        }
    }
}