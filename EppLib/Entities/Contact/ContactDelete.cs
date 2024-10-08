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
using System.Xml;

namespace EppLib.Entities
{
    public class ContactDelete : ContactBase<ContactDeleteResponse>
    {
        private string m_id;

        public ContactDelete(string mId)
        {
            m_id = mId;
        }

        protected override XmlElement BuildCommandElement(XmlDocument doc, XmlElement commandRootElement)
        {
            var contact_delete = BuildCommandElement(doc, "delete", commandRootElement);

            AddXmlElement(doc, contact_delete, "contact:id", m_id, namespaceUri);

            return contact_delete;
        }

        public override ContactDeleteResponse FromBytes(byte[] bytes)
        {
            return new ContactDeleteResponse(bytes);
        }
    }
}
