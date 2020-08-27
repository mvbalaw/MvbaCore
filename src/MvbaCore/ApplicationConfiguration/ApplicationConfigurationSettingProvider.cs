//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System;
using System.Configuration;
using JetBrains.Annotations;

namespace MvbaCore.ApplicationConfiguration
{
	public interface IApplicationConfigurationSettingProvider
	{
		[Pure]
		[CanBeNull]
		string GetSetting([NotNull] string key);

		[Pure]
		bool GetSettingAsBool([NotNull] string key);
	}

	public class ApplicationConfigurationSettingProvider : IApplicationConfigurationSettingProvider
	{
		public string GetSetting(string key)
		{
			return ConfigurationManager.AppSettings[key];
		}

		public bool GetSettingAsBool(string key)
		{
			return Convert.ToBoolean(GetSetting(key));
		}
	}
}