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
using System.Diagnostics.CodeAnalysis;
using System.Web; 
using System.Text; 
using System.Globalization;


namespace Translate
{
	/// <summary>
	/// Description of WordNetDictionary.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	public class WordNetDictionary : MonolingualDictionary
	{
		public WordNetDictionary()
		{
			CharsLimit = 50;
			LinesLimit = 1;
			Name = "_dictionary";
		
			AddSupportedTranslation(new LanguagePair(Language.English, Language.English));
			AddSupportedTranslation(new LanguagePair(Language.English_US, Language.English_US));
			
			AddSupportedSubject(SubjectConstants.Common);
		}
		
		protected override void DoTranslate(string phrase, LanguagePair languagesPair, string subject, Result result, NetworkSetting networkSetting)
		{
			Encoding encoding = Encoding.GetEncoding("iso-8859-1");
			string query = "http://wordnetweb.princeton.edu/perl/webwn?s=" + HttpUtility.UrlEncode(phrase, encoding);
			WebRequestHelper helper = 
				new WebRequestHelper(result, new Uri(query), 
					networkSetting, 
					WebRequestContentType.UrlEncodedGet, encoding);
			
			string responseFromServer = helper.GetResponse();
			
			if(responseFromServer.Contains("<h3>Your search did not return any results.</h3>"))
			{
				result.ResultNotFound = true;
				throw new TranslationException("Nothing found");
			}
			
			
			if(responseFromServer.Contains("<h3>Sorry(?) your search can only contain letters(?)"))
			{
				throw new TranslationException("Query contains extraneous symbols");
			}
			
			result.ArticleUrl = query;
			result.ArticleUrlCaption = phrase;
			
			string[] nodes =  StringParser.ParseItemsList("<h3>", "</ul>", responseFromServer);
			
			bool first = true;
			string nodename;
			
			Result child = result;
			string[] subnodes;
			string translation;
			
			foreach(string node in nodes)
			{
				nodename = StringParser.ExtractLeft("</h3>", node);
				if(first && nodes.Length == 1)
				{
					child.Abbreviation = nodename;
				}
				else
				{
					child = new Result(result.ServiceItem, nodename, result.LanguagePair, result.Subject);
					result.Childs.Add(child);
				}
				
				first = false;
				
				subnodes = StringParser.ParseItemsList("<li>", "</li>", node);
				foreach(string subnode in subnodes)
				{
					translation = StringParser.RemoveAll("<", ">", subnode);
					translation = StringParser.ExtractRight(")", translation);
					child.Translations.Add(translation);
				}
			}
		
		}
		
	}
}
