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

namespace EppLib.Entities
{
    public class CiraCprCategory
    {
        public string CprCode { get; set; }
        public string CprDescription { get; set; }

        public CiraCprCategory(string cprCode, string cprDescription)
        {
            CprCode = cprCode;
            CprDescription = cprDescription;
        }

        public override bool Equals(object obj)
        {
            if ((CiraCprCategory)obj == null) return false;

            return CprCode.Equals(((CiraCprCategory)obj).CprCode);
        }

        public override int GetHashCode()
        {
            if (CprCode == null) return 0;

            return CprCode.GetHashCode();
        }
    }
}
