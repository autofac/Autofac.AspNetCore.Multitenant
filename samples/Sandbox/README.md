# Sandbox

This demo shows the multitenant ASP.NET Core support in action.

## Program.cs
When using the `WebHostBuilder` the `UseAutofacMultitenantRequestServices` extension is used to tie the multitenant container to the request lifetime scope generation process.

## Startup.cs
In `ConfigureServices` we...

- Create a tenant ID strategy.
- Set up the multitenant container.
- Store the multitenant container in a place that the request services middleware can access it later.
- Return an `AutofacServiceProvider` so services can be resolved normally.

## Trying It Out

Build and launch the site. There is only one route in the site: `/api/values`

When you hit `/api/values` you should get `base` to indicate the base dependency is getting used.

Pass in a tenant ID on the query string to change tenant.

- `/api/values?tenant=a` should yield `a`
- `/api/values?tenant=b` should yield `b`
- Any other value for the tenant ID should yield `base` because only tenant `a` and `b` are configured with specific overrides.

**If you comment out the `UseAutofacMultitenantRequestServices` line it'll stop working** and you'll only ever see `base`. This illustrates why this package is required.