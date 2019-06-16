﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
namespace HL.Xshtd
{
    using System;
    using System.Collections.Generic;
    using ICSharpCode.AvalonEdit.Utils;

    /// <summary>
    /// A &lt;SyntaxDefinition&gt; element.
    /// </summary>
    [Serializable]
	public class XhstdThemeDefinition
	{
		/// <summary>
		/// Creates a new XhstdThemeDefinition object.
		/// </summary>
		public XhstdThemeDefinition()
		{
			this.Elements = new NullSafeCollection<XshtdElement>();
		}
		
		/// <summary>
		/// Gets/sets the definition name
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// Gets the collection of elements.
		/// </summary>
		public IList<XshtdElement> Elements { get; private set; }
		
		/// <summary>
		/// Applies the visitor to all elements.
		/// </summary>
		public void AcceptElements(IXshtdVisitor visitor)
		{
			foreach (XshtdElement element in Elements) {
				element.AcceptVisitor(visitor);
			}
		}
    }
}
