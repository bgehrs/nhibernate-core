﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.GH1667
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		private INHibernateLoggerFactory _defaultLogger;

		private static readonly FieldInfo _loggerFactoryField =
			typeof(NHibernateLogger).GetField("_loggerFactory", BindingFlags.NonPublic | BindingFlags.Static);

		protected override void OnSetUp()
		{
			_defaultLogger = (INHibernateLoggerFactory)_loggerFactoryField.GetValue(null);
			NHibernateLogger.SetLoggersFactory(new EnumeratingLoggerFactory());

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new EntityChild { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally", Children = new HashSet<EntityChild> { e1 } };
				session.Save(e2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			if (_defaultLogger != null)
				NHibernateLogger.SetLoggersFactory(_defaultLogger);

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from EntityChild").ExecuteUpdate();
				session.CreateQuery("delete from Entity").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public async Task LoggingDoesNotWreckCollectionLoadingAsync()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var parent = await (session.Query<Entity>().FirstAsync());
				Assert.That(parent.Children, Has.Count.EqualTo(1));
			}
		}
	}
}
