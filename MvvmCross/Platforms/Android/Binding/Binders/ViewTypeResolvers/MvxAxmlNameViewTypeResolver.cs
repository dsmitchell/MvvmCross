// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Android.Views;
using Microsoft.Extensions.Logging;
using MvvmCross.Binding;
using MvvmCross.IoC;

namespace MvvmCross.Platforms.Android.Binding.Binders.ViewTypeResolvers
{
    public class MvxAxmlNameViewTypeResolver : MvxLongLowerCaseViewTypeResolver, IMvxAxmlNameViewTypeResolver
    {
        public MvxAxmlNameViewTypeResolver(IMvxTypeCache<View> typeCache)
            : base(typeCache)
        {
            ViewNamespaceAbbreviations = new Dictionary<string, string>();
        }

        public IDictionary<string, string> ViewNamespaceAbbreviations { get; }

        public override Type Resolve(string tagName)
        {
            var unabbreviatedTagName = UnabbreviateTagName(tagName);
            var longLowerCaseName = GetLookupName(unabbreviatedTagName);
            return ResolveLowerCaseTypeName(longLowerCaseName);
        }

        private string UnabbreviateTagName(string tagName)
        {
            var filteredTagName = tagName;
            if (ViewNamespaceAbbreviations != null)
            {
                var split = tagName.Split(new[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 2)
                {
                    var abbreviate = split[0];
                    string fullName;
                    if (ViewNamespaceAbbreviations.TryGetValue(abbreviate, out fullName))
                    {
                        filteredTagName = fullName + "." + split[1];
                    }
                    else
                    {
                        MvxBindingLog.Instance?.LogTrace("Abbreviation not found {Abbreviation}", abbreviate);
                    }
                }
            }
            return filteredTagName;
        }

        protected string GetLookupName(string tagName)
        {
            var nameBuilder = new StringBuilder();

            switch (tagName)
            {
                case "View":
                case "ViewGroup":
                    nameBuilder.Append("android.view.");
                    break;

                default:
                    if (!IsFullyQualified(tagName))
                        nameBuilder.Append("android.widget.");
                    break;
            }

            nameBuilder.Append(tagName);
            return nameBuilder.ToString().ToLowerInvariant();
        }
    }
}
