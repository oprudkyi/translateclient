﻿#region License block : MPL 1.1/GPL 2.0/LGPL 2.1
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
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Translate
{
	/// <summary>
	/// Description of TranslateProfile.
	/// </summary>
	
	[Serializable()]
	public class TranslateProfile: ICloneable
	{
		public TranslateProfile()
		{
			serviceSettingComparer = new ServiceSettingComparerClass(this);
		}
		
		public virtual object Clone()
		{
			TranslateProfile result = new TranslateProfile();
			result.Name = Name;
			result.Position = Position;
			result.Subjects.AddRange(Subjects);
			result.History.AddRange(History);
			result.SelectedLanguagePair = SelectedLanguagePair;
			foreach(ServiceItemsSortDataCollection d in SortData)
				result.SortData.Add(d);
			foreach(ServiceItemData d in DisabledServiceItems)
				result.DisabledServiceItems.Add(d); 
			return result;	
		}
		
		
		string name;
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		int position;
		public int Position {
			get { return position; }
			set { position = value; }
		}
		
		[NonSerialized]
		SubjectCollection subjects = new SubjectCollection();
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public SubjectCollection Subjects {
			get { return subjects; }
		}
		
		[NonSerialized]
		LanguagePairCollection history = new LanguagePairCollection();
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public LanguagePairCollection History {
			get { return history; }
		}
		
		
		
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public virtual SubjectCollection GetSupportedSubjects()
		{
			return null;
		}
		
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public virtual ReadOnlyLanguagePairCollection GetLanguagePairs()
		{
			return null;
		}
		
		public virtual ReadOnlyServiceSettingCollection GetServiceSettings(string phrase, LanguagePair languagePair)
		{
			return null;		
		}
		
		
		[NonSerialized]
		ServiceItemsDataCollection disabledServiceItems = new ServiceItemsDataCollection();
		
		public ServiceItemsDataCollection DisabledServiceItems {
			get { return disabledServiceItems; }
		}

		public virtual void EnableService(string name, LanguagePair languagePair, string subject, bool enable)
		{
			ServiceItemData sid = new ServiceItemData(name, languagePair, subject);
			if(enable)
			{
				disabledServiceItems.Remove(sid);
			}
			else
			{
				if(!disabledServiceItems.Contains(sid))
					disabledServiceItems.Add(sid);
			}
		}
		
		public virtual bool IsServiceEnabled(string name, LanguagePair languagePair, string subject)
		{
			ServiceItemData sid = new ServiceItemData(name, languagePair, subject);
			return !disabledServiceItems.Contains(sid);
		}
		
		class ServiceSettingComparerClass : IComparer
		{
			
			public ServiceSettingComparerClass(TranslateProfile owner)
			{
				this.owner = owner;
			}
			
			TranslateProfile owner;
			
	        public int Compare(object x, object y)
	        {
	        	ServiceItemSetting xss = ((ServiceSettingsContainer)(((ListViewItem)x).Tag)).Setting;
	        	ServiceItemSetting yss = ((ServiceSettingsContainer)(((ListViewItem)y).Tag)).Setting;
	            return owner.CompareServiceSettings(xss, yss);
	        }
		}
		
		[NonSerialized]
		ServiceSettingComparerClass serviceSettingComparer;
		public IComparer ServiceSettingComparer
		{
			get
			{
				return serviceSettingComparer;
			}
		}

		[NonSerialized]
		LanguagePair languagePair = new LanguagePair();
		public LanguagePair SelectedLanguagePair {
			get { return languagePair; }
			set { languagePair = value; }
		}
		
		public int CompareServiceSettings(ServiceItemSetting x, ServiceItemSetting y)
		{
			ServiceItemsSortDataCollection itemsSortData;
			if(!sortData.TryGetValue(languagePair, out itemsSortData))
			{
				itemsSortData = new ServiceItemsSortDataCollection();
				sortData.Add(languagePair, itemsSortData);
			}
			
			int idxx = itemsSortData.IndexOf(x);
			int idxy = itemsSortData.IndexOf(y);
			int result = idxx - idxy;
			if (result == 0) 
			{
				result = string.Compare(x.ServiceItem.FullName, y.ServiceItem.FullName);
			}
			return result;
		}
		
		SortDataDictionary sortData = new SortDataDictionary();
		public SortDataDictionary SortData {
			get { return sortData; }
			set { sortData = value; }
		}
		
		public void BeforeSave()
		{
			for(int i = 0; i < sortData.Count; i++)
			{
				ServiceItemsSortDataCollection d = sortData[i];
				if(!d.Modified || d.Items.Count <= 1 )
				{
					sortData.RemoveAt(i);
					i--;
				}	
			}
		}
		
		public void MoveBefore(ServiceItemSetting serviceSettingBefore, ServiceItemSetting serviceSetting)
		{
			ServiceItemsSortDataCollection itemsSortData;
			if(sortData.TryGetValue(languagePair, out itemsSortData))
			{
				itemsSortData.MoveBefore(serviceSettingBefore, serviceSetting);
			}
			
		}
		
		public void MoveAfter(ServiceItemSetting serviceSettingAfter, ServiceItemSetting serviceSetting)
		{
			ServiceItemsSortDataCollection itemsSortData;
			if(sortData.TryGetValue(languagePair, out itemsSortData))
			{
				itemsSortData.MoveAfter(serviceSettingAfter, serviceSetting);
			}
		}
		
	}
	
	public class TranslateProfilesCollection :  List<TranslateProfile>
	{
		public TranslateProfile GetByName(string profileName)
		{
			foreach(TranslateProfile pf in this)
			{
				if(pf.Name == profileName)
					return pf;
			}
			return null;
		}
	}
}
