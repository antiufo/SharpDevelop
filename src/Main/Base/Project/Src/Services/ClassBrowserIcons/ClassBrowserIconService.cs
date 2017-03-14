// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.SharpDevelop
{
	
	public static class ClassBrowserIconService
	{
		#region GetIImage
		static ConcurrentDictionary<ImageSource, IImage> imageCache = new ConcurrentDictionary<ImageSource, IImage>();
		
		internal static IImage GetIImage(ImageSource imageSource)
		{
			return imageCache.GetOrAdd(imageSource, _ => new ImageSourceImage(_));
		}
		#endregion
		
		#region Entity Images
		public static readonly IImage Class = GetIImage(CompletionImage.Class.BaseImage);
		public static readonly IImage Struct = GetIImage(CompletionImage.Struct.BaseImage);
		public static readonly IImage Interface = GetIImage(CompletionImage.Interface.BaseImage);
		public static readonly IImage Enum = GetIImage(CompletionImage.Enum.BaseImage);
		public static readonly IImage Method = GetIImage(CompletionImage.Method.BaseImage);
		public static readonly IImage Property = GetIImage(CompletionImage.Property.BaseImage);
		public static readonly IImage Field = GetIImage(CompletionImage.Field.BaseImage);
		
		#region Entity Images
		static readonly ClassBrowserImage[] entityImages = {
			AddImage("Icons.16x16.Class"),
			AddImage("Icons.16x16.InternalClass"),
			AddImage("Icons.16x16.ProtectedClass"),
			AddImage("Icons.16x16.PrivateClass"),
			
			AddImage("Icons.16x16.Struct"),
			AddImage("Icons.16x16.InternalStruct"),
			AddImage("Icons.16x16.ProtectedStruct"),
			AddImage("Icons.16x16.PrivateStruct"),
			
			AddImage("Icons.16x16.Interface"),
			AddImage("Icons.16x16.InternalInterface"),
			AddImage("Icons.16x16.ProtectedInterface"),
			AddImage("Icons.16x16.PrivateInterface"),
			
			AddImage("Icons.16x16.Enum"),
			AddImage("Icons.16x16.InternalEnum"),
			AddImage("Icons.16x16.ProtectedEnum"),
			AddImage("Icons.16x16.PrivateEnum"),
			
			AddImage("Icons.16x16.Method"),
			AddImage("Icons.16x16.InternalMethod"),
			AddImage("Icons.16x16.ProtectedMethod"),
			AddImage("Icons.16x16.PrivateMethod"),
			
			AddImage("Icons.16x16.Property"),
			AddImage("Icons.16x16.InternalProperty"),
			AddImage("Icons.16x16.ProtectedProperty"),
			AddImage("Icons.16x16.PrivateProperty"),
			
			AddImage("Icons.16x16.Field"),
			AddImage("Icons.16x16.InternalField"),
			AddImage("Icons.16x16.ProtectedField"),
			AddImage("Icons.16x16.PrivateField"),
			
			AddImage("Icons.16x16.Delegate"),
			AddImage("Icons.16x16.InternalDelegate"),
			AddImage("Icons.16x16.ProtectedDelegate"),
			AddImage("Icons.16x16.PrivateDelegate"),
			
			AddImage("Icons.16x16.Event"),
			AddImage("Icons.16x16.InternalEvent"),
			AddImage("Icons.16x16.ProtectedEvent"),
			AddImage("Icons.16x16.PrivateEvent"),
			
			AddImage("Icons.16x16.Property"),
			AddImage("Icons.16x16.InternalProperty"),
			AddImage("Icons.16x16.ProtectedProperty"),
			AddImage("Icons.16x16.PrivateProperty"),
			
			AddImage("Icons.16x16.ExtensionMethod"),
			AddImage("Icons.16x16.ExtensionMethod"),
			AddImage("Icons.16x16.ExtensionMethod"),
			AddImage("Icons.16x16.ExtensionMethod")
		};
		
		public static IImage GetIcon(IType t)
		{
			ITypeDefinition def = t.GetDefinition();
			if (def != null)
				return GetIcon(def);
			else
				return null;
		}
		
		public static IImage GetIcon(IUnresolvedEntity entity)
		{
			return GetIImage(CompletionImage.GetImage(entity));
		}
		
		public static IImage GetIcon(IVariable v)
		{
			if (v is IField) {
				return GetIcon((IEntity)v);
			} else if (v.IsConst) {
				return Const;
			} else if (v is IParameter) {
				return Parameter;
			} else {
				return LocalVariable;
			}
		}
		
		public static IImage GetIcon(ITypeDefinition t)
		{
			return GetIImage(CompletionImage.GetImage(t));
		}
		#endregion
		
		public static IImage Namespace { get { return GetIImage(CompletionImage.NamespaceImage); } }
		public static IImage Solution { get { return SD.ResourceService.GetImage("Icons.16x16.CombineIcon"); } }
		public static IImage Const { get { return GetIImage(CompletionImage.Literal.BaseImage); } }
		public static IImage GotoArrow { get { return SD.ResourceService.GetImage("Icons.16x16.SelectionArrow"); } }
		
		public static IImage LocalVariable { get { return SD.ResourceService.GetImage("Icons.16x16.Local"); } }
		public static IImage Parameter { get { return SD.ResourceService.GetImage("Icons.16x16.Parameter"); } }
		public static IImage Keyword { get { return SD.ResourceService.GetImage("Icons.16x16.Keyword"); } }
		public static IImage Operator { get { return SD.ResourceService.GetImage("Icons.16x16.Operator"); } }
		public static readonly IImage Delegate = GetIImage(CompletionImage.Delegate.BaseImage);
		public static IImage CodeTemplate { get { return SD.ResourceService.GetImage("Icons.16x16.TextFileIcon"); } }
		public static readonly IImage Event = GetIImage(CompletionImage.Event.BaseImage);
		public static readonly IImage Indexer = GetIImage(CompletionImage.Indexer.BaseImage);
		#endregion
		
		#region Get Methods for Entity Images
		
		public static IImage GetIcon(IEntity entity)
		{
			if (field.IsConst) {
				if (field.DeclaringType.ClassType == ClassType.Enum)
					return EnumValue;
				else
					return Const;
			} else if (field.IsParameter) {
				return Parameter;
			} else if (field.IsLocalVariable) {
				return LocalVariable;
			} else {
				return entityImages[FieldIndex + GetModifierOffset(field.Modifiers)];
			}
		
		// This overload exists to avoid the ambiguity between IEntity and IVariable
		public static IImage GetIcon(IField v)
		{
			return GetIcon((IEntity)v);
		}
		
		public static ClassBrowserImage GetIcon(FieldInfo fieldinfo)
		{
			if (fieldinfo.IsLiteral) {
				if (fieldinfo.DeclaringType.IsEnum) return EnumValue;
				else return Const;
			}
			
			if (fieldinfo.IsAssembly) {
				return entityImages[FieldIndex + internalModifierOffset];
			}
			
			if (fieldinfo.IsPrivate) {
				return entityImages[FieldIndex + privateModifierOffset];
			}
			
			if (!(fieldinfo.IsPrivate || fieldinfo.IsPublic)) {
				return entityImages[FieldIndex + protectedModifierOffset];
			}
			
			return entityImages[FieldIndex];
		}
		public static readonly ClassBrowserImage EnumValue = AddImage("Icons.16x16.EnumValue");
	}
}
