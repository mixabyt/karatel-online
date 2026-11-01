using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GA_Request
{
	public enum RequestType
	{
		GA_GetHeatmapGameInfo,
		GA_GetHeatmapData
	}

	public delegate void SubmitSuccessHandler(RequestType requestType, Hashtable returnParam, SubmitErrorHandler errorEvent);

	public delegate void SubmitErrorHandler(string message);

	public Dictionary<RequestType, string> Requests = new Dictionary<RequestType, string>
	{
		{
			RequestType.GA_GetHeatmapGameInfo,
			"game"
		},
		{
			RequestType.GA_GetHeatmapData,
			"heatmap"
		}
	};

	private string _baseURL = "http://data-api.gameanalytics.com";

	public WWW RequestGameInfo(SubmitSuccessHandler successEvent, SubmitErrorHandler errorEvent)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected Obj, but got Unknown
		if (string.IsNullOrEmpty(GA.SettingsGA.GameKey))
		{
			GA.LogWarning("Game key not set - please setup your Game key in GA_Settings, under the Basic tab");
			return null;
		}
		if (string.IsNullOrEmpty(GA.SettingsGA.ApiKey))
		{
			GA.LogWarning("API key not set - please setup your API key in GA_Settings, under the Advanced tab");
			return null;
		}
		string gameKey = GA.SettingsGA.GameKey;
		string text = "game_key=" + gameKey + "&keys=area%7Cevent_id%7Cbuild";
		text = text.Replace(" ", "%20");
		string uRL = GetURL(Requests[RequestType.GA_GetHeatmapGameInfo]);
		uRL = uRL + "/?" + text;
		WWW www = null;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Authorization", GA_Submit.CreateMD5Hash(text + GA.SettingsGA.ApiKey));
		www = new WWW(uRL, new byte[1], dictionary);
		GA.RunCoroutine(Request(www, RequestType.GA_GetHeatmapGameInfo, successEvent, errorEvent), () => www.isDone);
		return www;
	}

	public WWW RequestHeatmapData(List<string> events, string area, string build, SubmitSuccessHandler successEvent, SubmitErrorHandler errorEvent)
	{
		return RequestHeatmapData(events, area, build, null, null, successEvent, errorEvent);
	}

	public WWW RequestHeatmapData(List<string> events, string area, string build, DateTime? startDate, DateTime? endDate, SubmitSuccessHandler successEvent, SubmitErrorHandler errorEvent)
	{
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Expected Obj, but got Unknown
		string gameKey = GA.SettingsGA.GameKey;
		string text = string.Empty;
		for (int i = 0; i < events.Count; i++)
		{
			text = ((i != events.Count - 1) ? (text + events[i] + "|") : (text + events[i]));
		}
		string text2 = "game_key=" + gameKey + "&event_ids=" + text + "&area=" + area;
		if (!build.Equals(string.Empty))
		{
			text2 = text2 + "&build=" + build;
		}
		text2 = text2.Replace(" ", "%20");
		if (startDate.HasValue && endDate.HasValue)
		{
			DateTime dateTime = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
			DateTime dateTime2 = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 0, 0, 0);
			string text3 = text2;
			text2 = text3 + "&start_ts=" + DateTimeToUnixTimestamp(dateTime) + "&end_ts=" + DateTimeToUnixTimestamp(dateTime2);
		}
		string uRL = GetURL(Requests[RequestType.GA_GetHeatmapData]);
		uRL = uRL + "/?" + text2;
		WWW www = null;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Authorization", GA_Submit.CreateMD5Hash(text2 + GA.SettingsGA.ApiKey));
		www = new WWW(uRL, new byte[1], dictionary);
		GA.RunCoroutine(Request(www, RequestType.GA_GetHeatmapData, successEvent, errorEvent), () => www.isDone);
		return www;
	}

	public static double DateTimeToUnixTimestamp(DateTime dateTime)
	{
		return (dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
	}

	private IEnumerator Request(WWW www, RequestType requestType, SubmitSuccessHandler successEvent, SubmitErrorHandler errorEvent)
	{
		yield return www;
		GA.Log("GameAnalytics: URL " + www.url);
		try
		{
			if (!string.IsNullOrEmpty(www.error))
			{
				throw new Exception(www.error);
			}
			string text = www.text;
			text = text.Replace("null", "0");
			Hashtable returnParam = (Hashtable)GA_MiniJSON.JsonDecode(text);
			if (returnParam != null)
			{
				GA.Log("GameAnalytics: Result: " + text);
				successEvent?.Invoke(requestType, returnParam, errorEvent);
				yield break;
			}
			throw new Exception(text);
		}
		catch (Exception ex)
		{
			Exception e = ex;
			errorEvent?.Invoke(e.Message);
		}
	}

	private string GetURL(string category)
	{
		return _baseURL + "/" + category;
	}
}
