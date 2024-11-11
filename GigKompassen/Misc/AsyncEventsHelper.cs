using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigKompassen.Misc
{
  public static class AsyncEventsHelper
  {
    public delegate Task AsyncEventHandler<TEventArgs>(object? sender, TEventArgs e);

    public static async Task InvokeAsync<TEventArgs>(this AsyncEventHandler<TEventArgs>? eventHandler, object? sender, TEventArgs e)
    {
      if (eventHandler == null)
        return;

      var tasks = eventHandler.GetInvocationList().Cast<AsyncEventHandler<TEventArgs>>().Select(p => p.Invoke(sender, e));
      try
      {
        await Task.WhenAll(tasks);
      }
      catch (Exception ex)
      {
        throw new AggregateException("An error occurred while invoking an event", ex);
      }
    }
  }
}
