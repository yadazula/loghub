﻿using System.Collections.Generic;

namespace LogHub.Web.ViewModels
{
	public class PagedResult<T>
	{
		public ushort Page { get; set; }
		public int Total { get; set; }
		public IEnumerable<T> Models { get; set; }
	}
}