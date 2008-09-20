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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Microsoft.Win32;
using FreeCL.RTL;

namespace Translate
{
	/// <summary>
	/// Description of ResultBrowser.
	/// </summary>
	public partial class ResultBrowser : UserControl
	{
		public ResultBrowser()
		{
			inResize = true;
			InitializeComponent();

			wBrowser.StatusTextChanged += wBrowser_StatusTextChanged;
			inResize = false;
			RecalcSizes();
		}

		string statusText;
		public string StatusText {
			get { return statusText; }
		}
		
		public event EventHandler StatusTextChanged;
		private void wBrowser_StatusTextChanged(object sender, EventArgs e)
		{
		    statusText = wBrowser.StatusText;
		    if(StatusTextChanged != null)
		    	StatusTextChanged(this, new EventArgs());
		}

		bool isClean;
		bool forceCleaning;
		public void Clear()
		{
			if(isClean)
				return;
				
			if(UpdatesManager.IsNewVersion && !FreeCL.Forms.Application.IsCommandSwitchSet("skipchangelog"))
			{
				UpdatesManager.IsNewVersion = false;
				string url = Constants.ChangeLogPageUrlBase;
				if(FreeCL.RTL.LangPack.CurrentLanguage == "Ukrainian")
				{
					url += "uk.html";
				}
				else if(FreeCL.RTL.LangPack.CurrentLanguage == "Russian")
				{
					url += "ru.html";
				}
				else
				{
					url += "en.html";				
				}
				url+= "?ver=" + FreeCL.RTL.ApplicationInfo.ProductVersion;
				forceCleaning = true;
				wBrowser.Navigate(url);
				return;
			}
			
			if (wBrowser.Document != null && !forceCleaning)
			{ 
				Wait();	
				
				if(wBrowser.Document != null)
				{
					if(!HtmlHelper.ClearTranslations(wBrowser))
					{	//possible disabled javascript or error
						forceCleaning = true;
					}	
					else
						isClean = true;	//avoid double cleaning
				}
				else
					forceCleaning = true;
			}
			
			if(wBrowser.Document == null || forceCleaning)
			{
				forceCleaning = false;
				wBrowser.Navigate(new Uri(WebUI.ResultsWebServer.Uri, "Default.aspx"));
			}
			RecalcSizes();
			isClean = true;	
		}
		
		public void Wait()
		{
			WebBrowserHelper.Wait(wBrowser);
		}
		
		public void Stop()
		{
			wBrowser.Stop();
		}
		

		char[] delimiterChars = { ' ', ',', '.', ':', ';', '\t', '\n', '!', '?', '(', ')', '[', ']', '{', '}', '*', '/', '@', '#', '$', '%', '^', '&', '+', '=', '\\', '|' };
		
		List<string> SplitResultToParts(string data)
		{
			List<string> result = new List<string>();
			char[] dataChars = data.ToCharArray();
			
			List<char> delimiterCharsList = new List<char>(delimiterChars);
			delimiterCharsList.Sort();
			
			StringBuilder sb = new StringBuilder();
			foreach(char ch in dataChars)
			{
				if(delimiterCharsList.BinarySearch(ch) >= 0)	
				{
					if(sb.Length >= 0)
					{
						result.Add(sb.ToString());
						sb = new StringBuilder();
					}
					
					result.Add(ch.ToString());
				}
				else
				{
					sb.Append(ch);
				}
			}
			
			if(sb.Length >= 0)
			{
				result.Add(sb.ToString());
			}
			
			return result;
		}
		
		string GetResultHtml(Result result)
		{
			return GetResultHtml(result, 0);
		}
		
		string GetResultHtml(Result result, double indent)
		{

			StringBuilder htmlString = new StringBuilder();
			
			if(result.Error == null || result.ResultNotFound)
			{
				if(!string.IsNullOrEmpty(result.EditArticleUrl))
				{
					string link_f = "<a href=\"{0}\">{1} \"{2}\"</a><br><br>";
					htmlString.AppendFormat(link_f, result.EditArticleUrl,
						result.ResultNotFound ? 
							LangPack.TranslateString("Create article") : 
							LangPack.TranslateString("Open article"),
						result.Phrase);
				}
			}
			
			if(result.Error == null)
			{
				if(result.Childs.Count != 0)
				{ 
					//we has childs list
					if(result.Translations.Count > 0 && !string.IsNullOrEmpty(result.Translations[0]))
							htmlString.AppendFormat("<span style=\"" + HtmlHelper.BoldTextStyle + "\">{0}</span>  ", 
								HttpUtility.HtmlEncode(result.Translations[0]));
								
					if(!string.IsNullOrEmpty(result.Abbreviation))
					{
						htmlString.AppendFormat("<span style=\""+ HtmlHelper.DefaultTextStyle +"\"> {0} </span>", HttpUtility.HtmlEncode(result.Abbreviation));
					}
						
					if(result.Translations.Count > 0 || !string.IsNullOrEmpty(result.Abbreviation))
						htmlString.Append("<br>");
							
					foreach(Result r in result.Childs)
					{
						if(/*r.Phrase != result.Phrase && */!string.IsNullOrEmpty(r.Phrase))
						{
							if(indent != 0)
								htmlString.AppendFormat("<p style=\"margin-top: 0pt; margin-bottom: 0em; margin-left: {0}em;\">", 
									indent.ToString("0.##", CultureInfo.InvariantCulture));
							
							if(!r.Phrase.StartsWith("html!"))
							{
								htmlString.AppendFormat("<span style=\"" + HtmlHelper.BoldTextStyle + "\">{0}</span>", 
									HttpUtility.HtmlEncode(r.Phrase));
							}
							else
							{  //append html directly
								htmlString.Append(r.Phrase.Substring(5));
							}
								
							if(indent != 0)
								htmlString.Append("</p>");
						}
								
						if(!string.IsNullOrEmpty(r.Abbreviation) && r.Childs.Count == 0)
						{
							htmlString.AppendFormat("<span style=\""+ HtmlHelper.DefaultTextStyle +"\"> {0} </span>", HttpUtility.HtmlEncode(r.Abbreviation));
						}
						
						if(/*r.Phrase != result.Phrase && */indent > 0.5 && (!string.IsNullOrEmpty(r.Phrase) || !string.IsNullOrEmpty(r.Abbreviation)))
							htmlString.Append("<br>");

						htmlString.AppendFormat("{0}", 
							GetResultHtml(r, indent + 0.5 ));
					}
					return htmlString.ToString();
				}
				
				if(result.Parent == null)
				{ //abreviations
					if(!string.IsNullOrEmpty(result.Abbreviation))
					{
						htmlString.AppendFormat("<span style=\""+ HtmlHelper.DefaultTextStyle +"\"> {0} </span>", HttpUtility.HtmlEncode(result.Abbreviation));
					}
						
					if(result.Translations.Count > 0 || !string.IsNullOrEmpty(result.Abbreviation))
						htmlString.Append("<br>");
				}
				List<char> delimiterCharsList = new List<char>(delimiterChars);
				delimiterCharsList.Sort();
			
				string topPhrase;
				if(result.Parent != null)
					topPhrase = result.Parent.Phrase;
				else
					topPhrase = result.Phrase;
				topPhrase = topPhrase.ToLowerInvariant();
				
				foreach(string s in result.Translations)
				{
					if(indent > 0)
						htmlString.Append("<li>");
						
					htmlString.AppendFormat("<p style=\"margin-top: 0pt; margin-bottom: 0pt; margin-left: {0}em;\">", 
						indent.ToString("0.##", CultureInfo.InvariantCulture));

					if(!s.StartsWith("html!"))
					{
						List<string> words = SplitResultToParts(s);
						
						foreach(string word in words)
						{
							if(string.IsNullOrEmpty(word))
								continue;
								
							bool IsDelimiter = false;
							bool IsError = word.Length > 1;
							if(!IsError && word.Length == 1)
							{
								IsError = delimiterCharsList.BinarySearch(word[0]) < 0;
								IsDelimiter = true;
							}
							if(IsError)	
								IsError = topPhrase.IndexOf(word.ToLowerInvariant()) >=0;
								
							if(!IsDelimiter)
							{
								if(IsError)
									htmlString.AppendFormat("<span style=\"" + HtmlHelper.ErrorTextStyle + "\">{0}</span>", HttpUtility.HtmlEncode(word));
								else
									htmlString.AppendFormat("<span style=\""+ HtmlHelper.DefaultTextStyle +"\">{0}</span>", HttpUtility.HtmlEncode(word));
							}
							else
								htmlString.Append(HttpUtility.HtmlEncode(word));
						}
					}
					else
					{ //append html directly
						int allowedWidth = (int)((double)wBrowser.Width*.95) - 26;
						if(vScrollBar.Visible)
							allowedWidth += SystemInformation.VerticalScrollBarWidth;

						htmlString.Append(s.Substring(5).Replace("{allowed_width}", allowedWidth.ToString()));
					}
					
					htmlString.Append("</p>");
					if(indent > 0)
						htmlString.Append("</li>");
				}
				
			}
			else
			{
				htmlString.Append(HttpUtility.HtmlEncode(LangPack.TranslateString(result.Error.Message)));
			}
			return htmlString.ToString();
		}
		
		public void SetResult(Result result, LanguagePair languagePair)
		{
			if(result == null)
				throw new ArgumentNullException("result");
				
			if(result.ResultNotFound && 
				string.IsNullOrEmpty(result.EditArticleUrl) &&
				TranslateOptions.Instance.ResultWindowOptions.HideWithoutResult)
				return; //skip
				
			string htmlString = "";
			if(TranslateOptions.Instance.ResultWindowOptions.ShowServiceName)
			{
				htmlString+= string.Format(CultureInfo.InvariantCulture, 
					HtmlHelper.ServiceNameFormat, 
					result.ServiceItem.Service.Url, 
					HttpUtility.HtmlEncode(LangPack.TranslateString(result.ServiceItem.Service.FullName)));
					
				htmlString+= ", ";			
				htmlString+= LangPack.TranslateString("Type") + " : " + ServiceSettingsContainer.GetServiceItemType(result.ServiceItem);
			}

			if(result.Subject != SubjectConstants.Common)
			{
				if(htmlString.Length > 0)
					htmlString+= ", ";			
				htmlString+= LangPack.TranslateString("Subject") + " : " + LangPack.TranslateString(result.Subject);
			}
			
			if(languagePair.From == Language.Any || languagePair.To == Language.Any ||  TranslateOptions.Instance.ResultWindowOptions.ShowTranslationDirection)
			{
				if(htmlString.Length > 0)
					htmlString+= ", ";			
				
				if(result.ServiceItem is MonolingualDictionary)
				{
					htmlString+= LangPack.TranslateLanguage(result.LanguagePair.From);
				}
				else
				{
					htmlString+= LangPack.TranslateLanguage(result.LanguagePair.From) +
							"->" + 
							LangPack.TranslateLanguage(result.LanguagePair.To);
				}
			}
			
			if(htmlString.Length > 0)
				htmlString+= "<hr style=\"width: 100%; height: 1px;\">";
			
			
			htmlString += GetResultHtml(result);
			
			if(result.QueryTicks != 0 && TranslateOptions.Instance.ResultWindowOptions.ShowQueryStatistics)
			{
				htmlString+= "<hr style=\"width: 100%; height: 1px;\">";
				htmlString += "<span style=\"" + HtmlHelper.InfoTextStyle+ "\">";
				htmlString += string.Format(CultureInfo.InvariantCulture, "Query time : {0} s", new DateTime(result.QueryTicks).ToString("ss.fffffff", CultureInfo.InvariantCulture) );
				htmlString += ", Retry count : " + result.RetryCount; 
				htmlString += ", Bytes sent : " + result.BytesSent; 
				htmlString += ", Bytes received : " + result.BytesReceived; 
				htmlString += "</span>";
			}
						
			if(!TranslateOptions.Instance.ResultWindowOptions.ShowAccents)
			{
				htmlString = htmlString.Replace("́","");
			}
			
			Wait();
			HtmlHelper.AddTranslationCell(wBrowser, isClean, htmlString, result.ServiceItem);
			isClean = false;	
			RecalcSizes();
		}
		
		public void SetStatistics(long translateTicks)
		{
			if(!TranslateOptions.Instance.ResultWindowOptions.ShowQueryStatistics)
			  return;
			  
			string htmlString = string.Format(CultureInfo.InvariantCulture, "Full time : {0} s", new DateTime(translateTicks).ToString("ss.fffffff", CultureInfo.InvariantCulture) );;
			htmlString = "<span style=\""+ HtmlHelper.InfoTextStyle+"\">" + htmlString + "</span>";
			
			Wait();
			HtmlHelper.AddTranslationCell(wBrowser, isClean, htmlString);

			isClean = false;	
			RecalcSizes();
		}
		
		private static int CompareStringsByLength(string x, string y)
		{
			if (x == null)
			{
				if (y == null)
					return 0;
				else
					return -1;
			}
			else
			{
				if (y == null)
					return 1;
				else
				{
					return y.Length - x.Length; //bigges length is smalles
				}
			}
		}

		
		static int ExtractAdWords(List<string> data, int count)
		{
			List<string> copy = new List<string> (data);
			data.Clear();			
			copy.Sort(CompareStringsByLength);
			int length = 0;
			for(int i = 0; i < copy.Count; i++)
			{
				if(copy[i].Length > 2 && length < count)
				{
					if(!data.Contains(copy[i]))
					{
						length += copy[i].Length;
						data.Add(copy[i]);
					}
				}
				
				if(length >= count)
				{
					break;
				}
			}
			return length;
		}
		
		string GetResultString(Result r)
		{
			string resultwords = "";
			
			if(!r.ResultNotFound && r.Error == null)
			{
				foreach(string s in r.Translations)
				{
					if(!s.StartsWith("html!"))
						resultwords += s + " ";
				}	
					
				foreach(Result child in r.Childs)
				{
					resultwords += GetResultString(child);
				}
			}
			
			return resultwords;
		}
		
		string currentQuery = "";
		string BuildQuery(string phrase, ReadOnlyServiceSettingCollection settings, ReadOnlyResultCollection results)
		{
			if(phrase == null || settings == null)
				return "";
			else
			{
				string result;
				
				if(wBrowser.Width > 740)
					result = "w=728&h=90";
				else if(wBrowser.Width > 480)
					result = "w=468&h=60";
				else if(wBrowser.Width > 246)
					result = "w=234&h=60";
				else if(wBrowser.Width > 137)
					result = "w=125&h=125";
				else
					return null;
					
				//selecting words from query and result
				List<string> phrasewords = SplitResultToParts(phrase.Replace("́",""));
				int count = ExtractAdWords(phrasewords, 25);
				
				string resultstr = "";
				foreach(Result r in results)
				{
					resultstr += GetResultString(r);
				}
				
				List<string> resultwords = SplitResultToParts(resultstr.Replace("́",""));
				count = ExtractAdWords(resultwords, 60 - count);
				
				if(phrasewords.Count + resultwords.Count == 0 )
					return null;
				
				string phraseStr = "";
				foreach(string s in resultwords)
					phraseStr += s + " ";

				foreach(string s in phrasewords)
				{
					if(!resultwords.Contains(s))
						phraseStr += s + " ";
				}
					
				phraseStr = phraseStr.Substring(0, phraseStr.Length - 1);
					
				result += "&s=" + HttpUtility.UrlEncode(phraseStr);
				
				List<string> subjects = new List<string>();
				foreach(ServiceItemSetting ss in settings)
				{
					if(ss.Subject != SubjectConstants.Common && !subjects.Contains(ss.Subject))
						subjects.Add(ss.Subject);
				}

				int idx = 0;				
				foreach(string sub in subjects)
				{
					result += "&sub" + idx.ToString(CultureInfo.InvariantCulture) + "=" + HttpUtility.UrlEncode(sub);
					idx++;
				}
				return result;
			}
		}
		
		public void AddAdvertisement(AsyncTranslateState translateState)
		{
			bool failed = true;
			foreach(Result r in translateState.Results)
			{
				if(!r.ResultNotFound && r.Error == null)
				{
					failed = false;
					break;
				}
			}
			
			if(failed)
				return;

			string query = BuildQuery(translateState.Phrase, translateState.TranslatorsSettings, translateState.Results);
			
			if(query == currentQuery || string.IsNullOrEmpty(query))
				return;
				
			currentQuery = query;
			SetExplorerSound(false);
			advertLoaded = false;
			wAdvertBrowser.Navigate(new Uri(Constants.StatsPageUrl + "?" + currentQuery));
		}
		
		
		public void ShowSource()
		{
			Wait();
			string clean = wBrowser.DocumentText;
			int bodyidx = clean.IndexOf("<body>");
			clean = clean.Substring(0, bodyidx);
			HtmlSourceViewForm form = new HtmlSourceViewForm();
			form.Source = clean + wBrowser.Document.Body.OuterHtml + "\r\n</html>";
			form.Text = wBrowser.Url.ToString();
			form.Show();
		}
		
		public override ContextMenuStrip ContextMenuStrip {
			get { return wBrowser.ContextMenuStrip; }
			set { wBrowser.ContextMenuStrip = value; }
		} 
		
		bool advertLoaded;
		bool inResize;
		
		bool needRecalcSize;
		void RecalcSizes()
		{
			needRecalcSize = true;
		}
		void RealRecalcSizes()
		{
			
			if(inResize)
				return;
				
			inResize = true;
			bool isHeightChanged = false;
			try
			{
				int allowedWidth = ClientSize.Width;
				if(vScrollBar.Visible)
					allowedWidth -= SystemInformation.VerticalScrollBarWidth;
					
		
				
				if(wBrowser.Width != allowedWidth)
				{
					wBrowser.Width = allowedWidth;
				}
					
				if(wAdvertBrowser.Width != allowedWidth)
					wAdvertBrowser.Width = allowedWidth;
	
				
				if(!advertLoaded)
				{
					if(wAdvertBrowser.Height != 0)
					{
						wAdvertBrowser.Height = 0;
						isHeightChanged = true;
					}
				}
				else if(wAdvertBrowser.Document != null && wAdvertBrowser.Document.Body != null && wAdvertBrowser.Document.Body.ScrollRectangle.Height != 0)
				{
					if(wAdvertBrowser.Height != wAdvertBrowser.Document.Body.ScrollRectangle.Height + 2)
					{
						wAdvertBrowser.Height = wAdvertBrowser.Document.Body.ScrollRectangle.Height + 2;
						isHeightChanged = true;
					}
				}
	
				int allowedHeight = ClientSize.Height;
				
				if((isClean && !advertLoaded) || (wBrowser.Document == null || wBrowser.Document.Body == null))
				{
					if(wBrowser.Height != allowedHeight)
					{
						wBrowser.Height = allowedHeight;
						isHeightChanged = true;
					}
				}	
				else if(isClean)
				{
					if(wBrowser.Height != allowedHeight - wAdvertBrowser.Height)
						wBrowser.Height = allowedHeight - wAdvertBrowser.Height;
				}
				else if(wBrowser.Document != null && wBrowser.Document.Body != null && wBrowser.Document.Body.ScrollRectangle.Height != 0)
				{
					int height = wBrowser.Document.Body.ScrollRectangle.Height + 2;
					if(wAdvertBrowser.Height + height < Height)
						height = Height - wAdvertBrowser.Height;
						
					if(wBrowser.Height != height)
					{
						wBrowser.Height = height;
						isHeightChanged = true;
					}
				}
				
				if(wAdvertBrowser.Top != wBrowser.Bottom)
				{
					wAdvertBrowser.Top = wBrowser.Bottom;
					isHeightChanged = true;
				}
				
				if(wAdvertBrowser.Bottom < Height)
				{
					wBrowser.Top += Height-wAdvertBrowser.Bottom;
					wAdvertBrowser.Top = wBrowser.Bottom;
					isHeightChanged = true;
				}
				
				int FullHeight = wBrowser.Height + wAdvertBrowser.Height;
				if(FullHeight > Height)
				{
					if(!vScrollBar.Visible)
					{
						allowedWidth -= SystemInformation.VerticalScrollBarWidth;
						wBrowser.Width = allowedWidth;
						wAdvertBrowser.Width = allowedWidth;
						vScrollBar.LargeChange = Height;
						vScrollBar.Maximum = FullHeight - Height + vScrollBar.LargeChange - 1;
						vScrollBar.Value = 0;
						wBrowser.Top = 0;
						wAdvertBrowser.Top = wBrowser.Height;
						vScrollBar.Visible = true;
						vScrollBar.Enabled = true;
						isHeightChanged = true;
					}
					else if(vScrollBar.Maximum != FullHeight - Height + vScrollBar.LargeChange - 1)
					{
						vScrollBar.Maximum = FullHeight - Height + vScrollBar.LargeChange - 1;
					}
				}
				else
				{
					if(vScrollBar.Visible)
					{
						vScrollBar.Visible = false;
						vScrollBar.Enabled = false;
						wBrowser.Top = 0;
						wAdvertBrowser.Top = wBrowser.Height;
						wBrowser.Width = Width;
						wAdvertBrowser.Width = Width;
						isHeightChanged = true;
					}
				}
			}
			finally
			{
				inResize = false;
			}
			if(isHeightChanged && IsHandleCreated)
			{
				needRecalcSize = true;
			}
			else
				needRecalcSize = false;
		}
		
		void ResultBrowserResize(object sender, EventArgs e)
		{
			RealRecalcSizes();
		}
		
		void ResultBrowserClientSizeChanged(object sender, EventArgs e)
		{
			RecalcSizes();
		}
		
		void WAdvertBrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			advertLoaded = !e.Url.AbsoluteUri.Contains("shdoclc.dll") && 
				!wAdvertBrowser.DocumentText.Contains("shdocvw.dll") &&
				!wAdvertBrowser.DocumentText.Contains("shdoclc.dll");
				
			SetExplorerSound(true);
			RecalcSizes();
		}

		void ResultBrowserSizeChanged(object sender, EventArgs e)
		{
			RecalcSizes();				
		}
		
		void GEventsIdle(object sender, EventArgs e)
		{
			try
			{
				if(needRecalcSize)
					RealRecalcSizes();
			}
			catch(ObjectDisposedException)
			{	
				//raised in sharpdevloper formdesigner
				//do nothing
			}
		}
		
		
		
		[EditorBrowsableAttribute()]
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if(vScrollBar.Visible)
			{
				int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
				int numberOfPixelsToMove = -numberOfTextLinesToMove * 8;
				int newvalue = vScrollBar.Value + numberOfPixelsToMove;
				if(newvalue > vScrollBar.Maximum - vScrollBar.LargeChange + 1)
					newvalue = vScrollBar.Maximum - vScrollBar.LargeChange + 1;
				if(newvalue > vScrollBar.Maximum)
					newvalue = vScrollBar.Maximum;
				if(newvalue < vScrollBar.Minimum)
					newvalue = vScrollBar.Minimum;
				vScrollBar.Value = newvalue;
			}
		
			base.OnMouseWheel(e);
		}
		
		
		void VScrollBarValueChanged(object sender, EventArgs e)
		{
			int top = -vScrollBar.Value;
			wBrowser.Top = top;
			wAdvertBrowser.Top = wBrowser.Bottom;
		}
		
		public void DoScroll(Keys key)
		{
			if(!vScrollBar.Visible)
				return;

			int newvalue = vScrollBar.Value;
			if(key == Keys.Down)
			 	newvalue += vScrollBar.SmallChange;
			else if(key == Keys.Up)
				newvalue -= vScrollBar.SmallChange;
			else if(key == Keys.PageDown)
				newvalue += vScrollBar.LargeChange;
			else if(key == Keys.PageUp)
				newvalue -= vScrollBar.LargeChange;
			
			if(newvalue > vScrollBar.Maximum - vScrollBar.LargeChange + 1)
				newvalue = vScrollBar.Maximum - vScrollBar.LargeChange + 1;
			if(newvalue < vScrollBar.Minimum)
				newvalue = vScrollBar.Minimum;
			vScrollBar.Value = newvalue;
		}
		

		string keyName = "HKEY_CURRENT_USER\\AppEvents\\Schemes\\Apps\\Explorer\\Navigating\\.Current";
		
		void SetExplorerSound(bool restore)
		{
			string current = null;
				
			try
			{
				current = (string)Registry.GetValue(keyName, "", "Not set");
			}
			catch
			{
			
			}
		
			if(!restore)
			{
				if(!string.IsNullOrEmpty(current) && current != "Not set" && !current.EndsWith("1"))
				{
					try
					{
						Registry.SetValue(keyName, "", current + "1");
					}
					catch
					{
					
					}
				}
			}
			else
			{
				if(!string.IsNullOrEmpty(current) && current.EndsWith("1"))
				{
					try
					{
						Registry.SetValue(keyName, "", current.Substring(0, current.Length-1));
					}
					catch
					{
					
					}
				}
			}
		}
		
		void WBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			//if(e.Url.AbsoluteUri == "about:blank")
			//	return;
			if(e.Url.Host == "127.0.0.1" && e.Url.Port == WebUI.ResultsWebServer.Port)
				return;
			if(e.Url.AbsoluteUri.StartsWith(Constants.StatsPageUrl))
				return;
			if(e.Url.AbsoluteUri.StartsWith(Constants.ChangeLogPageUrlBase))
				return;
				
			if(e.Url.AbsoluteUri.StartsWith("http://pagead2.googlesyndication.com/pagead/ads?"))
				return;
				
			if(e.Url.AbsoluteUri.StartsWith("http://ads.adbrite.com/adserver/display_iab_ads.php"))
				return;

			if(e.Url.AbsoluteUri.StartsWith("http://syndication.exoclick.com/ads-iframe-display.php"))
				return;

			if(e.Url.AbsoluteUri.StartsWith("http://ad2.adecn.com/here.spot"))
				return;

			if(e.Url.AbsoluteUri.StartsWith("http://eb.adbureau.net/hserver"))
				return;

			if(e.Url.AbsoluteUri.StartsWith("javascript:"))
				return;

			if(e.Url.AbsoluteUri.Contains("ieframe.dll"))
				return;

			if(e.Url.AbsoluteUri.Contains("shdoclc.dll"))
				return;
				
			

			if(e.Url.AbsoluteUri.Contains("mailto:translate.net@gmail.com"))
			{
				MailTo.Send(ApplicationInfo.SupportEmail, 
					LangPack.TranslateString("Feedback for :") + " " + ApplicationInfo.ProductName + " " + ApplicationInfo.ProductVersion,
					LangPack.TranslateString("<< Enter your feedback or bug report here (English, Ukrainian, Russian). >>"));
				return;
			}	
				

			HtmlHelper.OpenUrl(e.Url);
			e.Cancel = true;
		}
		
		
		void WBrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			if(e.Url.Host == "127.0.0.1" && e.Url.Port == WebUI.ResultsWebServer.Port)
			{
				HtmlHelper.InitDocument(wBrowser);
			}
			else
			{
				wBrowser.Document.Body.Style = HtmlHelper.BodyStyle + "font-size: 8pt; font-family: Tahoma;";
				HtmlHelper.RemoveElement(wBrowser, "big_header");
			}
			RecalcSizes();			
		}
		
		public string GetSelection()
		{
			Wait();			
			return HtmlHelper.GetSelection(wBrowser);
		}
	}
}