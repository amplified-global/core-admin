using DotNetEd.CoreAdmin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetEd.CoreAdmin.ViewComponents
{
	public class CoreAdminMenuViewComponent : ViewComponent
	{
		private readonly CoreAdminTree coreAdminTree;
		private readonly IServiceProvider serviceProvider;

		public CoreAdminMenuViewComponent(CoreAdminTree coreAdminTree, IServiceProvider serviceProvider)
		{
			this.coreAdminTree = coreAdminTree;
			this.serviceProvider = serviceProvider;
		}

		public IViewComponentResult Invoke()
		{
			var viewModel = new MenuViewModel();

			var db2Tables = new Dictionary<string, List<string>>(coreAdminTree.Db2Tables);

			// Dynamically add tenant databases from Context2ConnectionStrings
			var options = serviceProvider.GetServices<CoreAdminOptions>().FirstOrDefault();
			if (options?.Context2ConnectionStrings != null)
			{
				var discoveredDbSets = serviceProvider.GetServices<DiscoveredDbSetEntityType>();
				var knownDbContextTypes = discoveredDbSets.Select(d => d.DbContextType).Distinct().ToList();

				foreach (var dbContextType in knownDbContextTypes)
				{
					if (options.Context2ConnectionStrings.TryGetValue(dbContextType.Name, out List<Func<string>> connectionStrings))
					{
						foreach (var connectionStringFunc in connectionStrings)
						{
							var connectionString = connectionStringFunc();
							var dbName = new Npgsql.NpgsqlConnectionStringBuilder(connectionString).Database;

							var dbSetProperties = dbContextType.GetProperties()
								.Where(p => p.PropertyType.IsGenericType && p.PropertyType.Name.StartsWith("DbSet") && (options.IgnoreEntityTypes == null || !options.IgnoreEntityTypes.Contains(p.PropertyType.GenericTypeArguments.First())))
								.Select(p => p.Name)
								.ToList();

							if (dbSetProperties.Any() && !db2Tables.ContainsKey(dbName))
							{
								db2Tables[dbName] = dbSetProperties;
							}
						}
					}
				}
			}

			viewModel.Db2Tables = db2Tables;

			return View(viewModel);
		}
	}
}
