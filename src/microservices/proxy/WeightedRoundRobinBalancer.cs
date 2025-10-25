using Ocelot.LoadBalancer.LoadBalancers;
using Ocelot.Responses;
using Ocelot.Values;

namespace proxy;

public class WeightedRoundRobinBalancer : ILoadBalancer
{
    private readonly Func<Task<List<Service>>> _services;
    private static readonly object _lock = new();
    private int _counter;
    private readonly int[] _weights;
    private readonly int _totalWeight;
    private readonly bool _gradualMigration;
    public WeightedRoundRobinBalancer(
        Func<Task<List<Service>>> services,
        int[] weights,
        bool gradualMigration)
    {
        _services = services;
        _weights = weights;
        _totalWeight = weights.Sum();
        _gradualMigration = gradualMigration;
    }

    public string Type => nameof(WeightedRoundRobinBalancer);

    public void Release(ServiceHostAndPort hostAndPort) { }

    

    public async Task<Response<ServiceHostAndPort>> Lease(HttpContext httpContext)
    {
        var services = await _services();

        if (!_gradualMigration)
        {
            return new OkResponse<ServiceHostAndPort>(services[0].HostAndPort);
        }

        lock (_lock)
        {
            var position = (int)(_counter++ % _totalWeight);

            int cumulativeWeight = 0;
            for (int i = 0; i < services.Count; i++)
            {
                cumulativeWeight += _weights[i];
                if (position < cumulativeWeight)
                {
                    return new OkResponse<ServiceHostAndPort>(services[i].HostAndPort);
                }
            }

            return new OkResponse<ServiceHostAndPort>(services[0].HostAndPort);
        }
    }
}
