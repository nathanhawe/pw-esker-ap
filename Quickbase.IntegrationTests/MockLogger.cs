﻿using Microsoft.Extensions.Logging;
using System;

namespace Quickbase.IntegrationTests
{
	internal class MockLogger<T> : ILogger<T>
	{
		public IDisposable BeginScope<TState>(TState state)
		{
			throw new NotImplementedException();
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			throw new NotImplementedException();
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{

		}
	}
}
