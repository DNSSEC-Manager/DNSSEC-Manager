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
using System.Xml;

namespace EppLib.Entities
{
    public class DomainDelete : DomainBase<DomainDeleteResponse>
    {
        private readonly string DomainName;

        public DomainDelete(string domainName)
        {
            DomainName = domainName;
        }

        protected override XmlElement BuildCommandElement(XmlDocument doc, XmlElement commandRootElement)
        {
            var domainDelete = BuildCommandElement(doc, "delete", commandRootElement);

            AddXmlElement(doc, domainDelete, "domain:name", DomainName, namespaceUri);

            return domainDelete;
        }

        public override DomainDeleteResponse FromBytes(byte[] bytes)
        {
            return new DomainDeleteResponse(bytes);
        }
    }
}
