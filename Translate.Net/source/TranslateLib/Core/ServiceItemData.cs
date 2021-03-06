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
 * Portions created by the Initial Developer are Copyright (C) 2008
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
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace Translate
{
	/// <summary>
	/// Description of ServiceItemData.
	/// </summary>
	[Serializable()]
	public class ServiceItemData : ServiceItemSortData
	{
		public ServiceItemData()
		{
		}
		
		public ServiceItemData(string name, LanguagePair languagePair, string subject):base(name, subject)
		{
			this.languagePair = languagePair;
		}
		
		public ServiceItemData(ServiceItem serviceItem, LanguagePair languagePair, string subject):base(serviceItem.FullName, subject)
		{
			this.languagePair = languagePair;
			this.ServiceItem = serviceItem;
		}
		
		[NonSerialized]
		ServiceItem serviceItem;
		[ XmlIgnore()]
		public ServiceItem ServiceItem {
			get { return serviceItem; }
			set 
			{ 
				serviceItem = value; 
				if(serviceItem != null)
					Name = serviceItem.FullName;
			}
		}
		
		public void AttachToServiceItem()
		{
			if(serviceItem == null)
				serviceItem = Manager.FindServiceItem(Name, languagePair, Subject);
		}
		
		
		LanguagePair languagePair;
		public LanguagePair LanguagePair {
			get { return languagePair; }
			set { languagePair = value; }
		}
		
		public override bool Equals(Object obj)
		{
			ServiceItemData arg = obj as ServiceItemData;
			if(arg == null) return false;
			return Name.Equals(arg.Name) && Subject.Equals(arg.Subject) && languagePair.Equals(arg.languagePair);
		}
			
		public static int GetHashCode(string serviceItemName, LanguagePair languagePair, string subject)	
		{
			return unchecked(serviceItemName.GetHashCode() * 1000 + languagePair.GetHashCode() + subject.GetHashCode()*10000);
		}
		
		public override int GetHashCode() 
		{
      		return GetHashCode(Name, languagePair, Subject);
   		}	
			
		public static bool operator ==(ServiceItemData a, ServiceItemData b)
		{
			bool anull, bnull;
			anull = Object.ReferenceEquals(a,null); 
			bnull = Object.ReferenceEquals(b,null);
			if (anull && bnull) return true;
			if (anull || bnull) return false;
			return a.Equals(b);
		}
	
		public static bool operator !=(ServiceItemData a, ServiceItemData b)
		{
			return !(a == b);
		}
		
		
	}
	
	[Serializable()]
	public class ServiceItemsDataCollection: Collection<ServiceItemData>
	{
		public ServiceItemsDataCollection()
		{
		
		}
	}
	
}
