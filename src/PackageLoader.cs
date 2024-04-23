using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace dotnet.nuget.tree
{
    internal class PackageLoader
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ConcurrentDictionary<string, Task<ExpandoObject>> _cache = new ConcurrentDictionary<string, Task<ExpandoObject>>();
        internal int _cacheHits = 0;
        private readonly bool _disableCache = false;

        internal PackageLoader(bool disableCache)
        {
            _disableCache = disableCache;
        }

        internal async IAsyncEnumerable<ProjectPackage> GetDependenciesForProject(Project project, string name, string version)
        {
            dynamic packageJson = await GetWithCache($"https://api.nuget.org/v3/registration5-semver1/{name.ToLower()}/{version.ToLower()}.json");
            if (packageJson == null) yield break;

            dynamic packageDefinition = await GetWithCache(packageJson.catalogEntry);
            if (!IsPropertyExist(packageDefinition, "dependencyGroups")) yield break;

            foreach (dynamic dependency in packageDefinition.dependencyGroups)
            {
                if (!IsPropertyExist(dependency, "targetFramework")) continue;
                if (!IsPropertyExist(dependency, "dependencies")) continue;
                if (
                    project.TargetFrameworks.Any(targetFramework => dependency.targetFramework.ToLower().Contains(targetFramework)) ||
                    dependency.targetFramework.ToLower().Contains("netstandard")
                )
                {
                    foreach (dynamic packageDependency in dependency.dependencies)
                    {
                        yield return new ProjectPackage
                        {
                            Name = packageDependency.id,
                            Version = Regex.Replace(packageDependency.range, @"[\[\],\)\s]", string.Empty)
                        };
                    }
                    continue;
                }
            }
        }

        internal async Task<ExpandoObject> GetWithCache(string uri)
        {
            if (_disableCache)
            {
                return await _loadData(uri);
            }

            var cacheHit = true;
            var result = _cache.GetOrAdd(uri, _loadData);

            if (cacheHit)
            {
                Interlocked.Increment(ref _cacheHits);
            }

            return await result;

            Task<ExpandoObject> _loadData(string _uri)
            {
                cacheHit = false;
                return _httpClient.GetAsync(uri).ContinueWith((response) =>
                {
                    if (!response.Result.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    return response.Result.Content.ReadAsAsync<ExpandoObject>();
                }).ContinueWith((doubleWrapped) => doubleWrapped?.Result?.Result);
            }
        }

        private static bool IsPropertyExist(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>)settings).ContainsKey(name);

            return settings.GetType().GetProperty(name) != null;
        }
    }
}
