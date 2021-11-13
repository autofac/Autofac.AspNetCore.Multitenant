# Sample ASP.NET Core Multitenant Applications

Each of the 'sandbox' applications shows, in the respective framework version, the functionality of Autofac.AspNetCore.Multitenant in action. You can launch them from Visual Studio or VS Code with the settings in this project; or you can run from the command line with `dotnet run`.

## Try It Out

You can see different dependencies used by querying the `api/values` route:

```sh
# Get the default value - will show `base` since there's no tenant ID.
curl http://localhost:5555/api/values

# Get some tenant-specific values - will show the tenant ID due to overrides.
curl http://localhost:5555/api/values?tenant=a
curl http://localhost:5555/api/values?tenant=b

# Only tenant a and b have overrides. Tenant c will show `base` again.
curl http://localhost:5555/api/values?tenant=c
```

## Points of Interest

### Program.cs

When using the `WebHostBuilder` the `UseAutofacMultitenantRequestServices` extension is used to tie the multitenant container to the request lifetime scope generation process.

**If you comment out the `UseAutofacMultitenantRequestServices` line it'll stop working** and you'll only ever see `base`. This illustrates why this package is required.

## Startup.cs

In `ConfigureServices` we...

- Create a tenant ID strategy.
- Set up the multitenant container.
- Store the multitenant container in a place that the request services middleware can access it later.
- Return an `AutofacServiceProvider` so services can be resolved normally.
