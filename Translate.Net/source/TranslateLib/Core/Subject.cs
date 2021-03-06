#region License block : MPL 1.1/GPL 2.0/LGPL 2.1
/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is the FreeCL.Net library.
 *
 * The Initial Developer of the Original Code is 
 *  Oleksii Prudkyi (Oleksii.Prudkyi@gmail.com).
 * Portions created by the Initial Developer are Copyright (C) 2007-2008
 * the Initial Developer. All Rights Reserved.
 *
 * Contributor(s):
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Translate
{
	public static class SubjectConstants
	{
		public const string Any = "Any";
		public const string Common = "Common";
		public const string Aviation = "Aviation";
		public const string Auto = "Auto";
		public const string Anatomy = "Anatomy";
		public const string Bank = "Bank";
		public const string Bible = "Bible";
		public const string Biology = "Biology";
		public const string Business = "Business";
		public const string Military = "Military";
		public const string Law = "Law";
		public const string Informatics = "Informatics";
		public const string Internet = "Internet";
		public const string Art = "Art";
		public const string Space = "Space";
		public const string Medicine = "Medicine";
		public const string Music = "Music";
		public const string Sex = "Sex";
		public const string Sport = "Sport";
		public const string Tech = "Tech";
		public const string Philosophy = "Philosophy";
		public const string Chemistry = "Chemistry";
		public const string Economy = "Economy";
		public const string Electronics = "Electronics";
		public const string Logistics = "Logistics";
		public const string Travel = "Travel";
		public const string Football = "Football";
		public const string Phrasebook = "Phrasebook";
		public const string Perfumery = "Perfumery";
		public const string Places = "Places";
		public const string Humour = "Humour";
		public const string History = "History";
		public const string Geology = "Geology";
	}
	
	public class SubjectCollection : List<String>
	{
	
	}
	
	public class ReadOnlySubjectCollection: ReadOnlyCollection<String>
	{
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public ReadOnlySubjectCollection(List<String> list):base(list)
		{
		
		}
	}
	
	[Serializable()]
	public class StringsDictionary : Dictionary<string, string>
	{
		public StringsDictionary ()
		{
		
		}
	
		public StringsDictionary (int capacity):base(capacity)
		{
		
		}
		
		protected StringsDictionary(SerializationInfo info, StreamingContext context):base(info, context)
		{
		}
	}	
	
	
}
