//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MvbaCore.Interfaces
{
    public interface IPropertyMetaData
    {
        IList<string> DependsOnProperty { get; }

        bool IsRequired { get; }

        int? MaxLength { get; }

        int? MaxValue { get; }

        int? MinLength { get; }

        int? MinValue { get; }

        string Name { get; }

        PropertyInfo PropertyInfo { get; }

        Type ReturnType { get; }

        string ValidationType { get; }

        void Combine(IPropertyMetaData parentMetaData);
    }
}